"""Writes the actor-auth known-answer vectors in the parent directory.

Standard library only, and deterministic: signatures use RFC 6979 nonces, so a second run writes
the same bytes. The keys are derived from labels and are test keys, never to be used for anything
else. `csharp-produced.json` is not written here; see ../README.md.

    python spec/vectors/tools/generate.py
"""
import base64
import hashlib
import hmac
import json
import os

P = 0xFFFFFFFF00000001000000000000000000000000FFFFFFFFFFFFFFFFFFFFFFFF
A = P - 3
N = 0xFFFFFFFF00000000FFFFFFFFFFFFFFFFBCE6FAADA7179E84F3B9CAC2FC632551
G = (0x6B17D1F2E12C4247F8BCE6E563A440F277037D812DEB33A0F4A13945D898C296,
     0x4FE342E2FE1A7F9B8EE7EB4A7C0F9E162BCE33576B315ECECBB6406837BF51F5)

# SubjectPublicKeyInfo prefixes for P-256; the public point follows each.
SPKI_NAMED = bytes.fromhex('3059301306072a8648ce3d020106082a8648ce3d030107034200')
SPKI_COMPRESSED = bytes.fromhex('3039301306072a8648ce3d020106082a8648ce3d030107032200')
# The explicit-parameter form, as `openssl ec -param_enc explicit` writes it (cofactor and seed included).
SPKI_EXPLICIT = base64.b64decode(
    'MIIBSzCCAQMGByqGSM49AgEwgfcCAQEwLAYHKoZIzj0BAQIhAP////8AAAABAAAA'
    'AAAAAAAAAAAA////////////////MFsEIP////8AAAABAAAAAAAAAAAAAAAA////'
    '///////////8BCBaxjXYqjqT57PrvVV2mIa8ZR0GsMxTsPY7zjw+J9JgSwMVAMSd'
    'NgiG5wSTamZ44ROdJreBn36QBEEEaxfR8uEsQkf4vOblY6RA8ncDfYEt6zOg9KE5'
    'RdiYwpZP40Li/hp/m47n60p8D54WK84zV2sxXs7LtkBoN79R9QIhAP////8AAAAA'
    '//////////+85vqtpxeehPO5ysL8YyVRAgEBA0IA')

OUT = os.path.normpath(os.path.join(os.path.dirname(os.path.abspath(__file__)), '..'))


def add(p1, p2):
    if p1 is None:
        return p2
    if p2 is None:
        return p1
    x1, y1 = p1
    x2, y2 = p2
    if x1 == x2 and (y1 + y2) % P == 0:
        return None
    if p1 == p2:
        slope = (3 * x1 * x1 + A) * pow(2 * y1, -1, P) % P
    else:
        slope = (y2 - y1) * pow(x2 - x1, -1, P) % P
    x3 = (slope * slope - x1 - x2) % P
    return x3, (slope * (x1 - x3) - y1) % P


def multiply(k, point):
    result = None
    while k:
        if k & 1:
            result = add(result, point)
        point = add(point, point)
        k >>= 1
    return result


def b32(value):
    return value.to_bytes(32, 'big')


def rfc6979(d, digest):
    h1 = b32(int.from_bytes(digest, 'big') % N)
    x = b32(d)
    v = b'\x01' * 32
    k = b'\x00' * 32
    k = hmac.new(k, v + b'\x00' + x + h1, hashlib.sha256).digest()
    v = hmac.new(k, v, hashlib.sha256).digest()
    k = hmac.new(k, v + b'\x01' + x + h1, hashlib.sha256).digest()
    v = hmac.new(k, v, hashlib.sha256).digest()
    while True:
        v = hmac.new(k, v, hashlib.sha256).digest()
        candidate = int.from_bytes(v, 'big')
        if 1 <= candidate < N:
            return candidate
        k = hmac.new(k, v + b'\x00', hashlib.sha256).digest()
        v = hmac.new(k, v, hashlib.sha256).digest()


def sign(d, message):
    """ECDSA over SHA-256 of message; returns (r, s)."""
    digest = hashlib.sha256(message).digest()
    z = int.from_bytes(digest, 'big')
    k = rfc6979(d, digest)
    r = multiply(k, G)[0] % N
    s = pow(k, -1, N) * (z + r * d) % N
    assert r and s
    return r, s


def der_integer(value):
    raw = value.to_bytes((value.bit_length() + 7) // 8 or 1, 'big')
    if raw[0] & 0x80:
        raw = b'\x00' + raw
    return b'\x02' + bytes([len(raw)]) + raw


def der(r, s):
    body = der_integer(r) + der_integer(s)
    assert len(body) < 128
    return b'\x30' + bytes([len(body)]) + body


def raw(r, s):
    return b32(r) + b32(s)


def b64(data):
    return base64.b64encode(data).decode('ascii')


def b64url(data):
    return base64.urlsafe_b64encode(data).decode('ascii').rstrip('=')


class Key:
    def __init__(self, name, note):
        self.name = name
        self.note = note
        self.d = int.from_bytes(hashlib.sha256(('bam-wire test key: ' + name).encode()).digest(), 'big') % (N - 1) + 1
        self.x, self.y = multiply(self.d, G)
        self.point = b'\x04' + b32(self.x) + b32(self.y)
        self.spki = SPKI_NAMED + self.point

    def fingerprint(self):
        return hashlib.sha256(self.spki).hexdigest()

    def describe(self):
        return {
            'name': self.name,
            'note': self.note,
            'privateScalarHex': b32(self.d).hex(),
            'publicPointHex': self.point.hex(),
            'spkiNamedCurveBase64': b64(self.spki),
            'spkiExplicitParametersBase64': b64(SPKI_EXPLICIT + self.point),
            'spkiCompressedPointBase64': b64(SPKI_COMPRESSED + bytes([2 + (self.y & 1)]) + b32(self.x)),
            'fingerprint': self.fingerprint(),
        }


ACTOR = Key('actor', 'The enrolled actor. Signs client tokens and request proofs.')
SERVER = Key('server', 'The host. Signs server tokens.')
OTHER = Key('other', 'A key that is not registered. Its signatures must be refused.')
KEYS = {key.name: key for key in (ACTOR, SERVER, OTHER)}


def write(name, document):
    path = os.path.join(OUT, name)
    with open(path, 'w', encoding='utf-8', newline='\n') as handle:
        json.dump(document, handle, indent=2, ensure_ascii=True)
        handle.write('\n')
    print('wrote', name)


def keys():
    fixture_point = base64.b64decode(
        'MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAE3z51Z1uB07fK6X10tqf6L1DiwyjP'
        'rwTKEK9qFwc3nP19jhVyDCzid58trbYgiI25hKv1zMkw1STmzULy0s7tGg==')[len(SPKI_NAMED):]
    x = int.from_bytes(fixture_point[1:33], 'big')
    y = int.from_bytes(fixture_point[33:], 'big')
    write('keys.json', {
        'description': 'P-256 test keys. Each private scalar is SHA-256("bam-wire test key: <name>") reduced into [1, n-1]. '
                       'The fingerprint is the lowercase hex SHA-256 of the named-curve SubjectPublicKeyInfo with an '
                       'uncompressed point; every encoding of one key has that one fingerprint.',
        'keys': [key.describe() for key in KEYS.values()],
        'publicOnly': [{
            'name': 'openssl-fixture',
            'note': 'A key made by openssl, with no private half here. Its fingerprint is what '
                    '`openssl ec -pubin -param_enc named_curve -conv_form uncompressed -outform DER | sha256sum` prints.',
            'publicPointHex': fixture_point.hex(),
            'spkiNamedCurveBase64': b64(SPKI_NAMED + fixture_point),
            'spkiExplicitParametersBase64': b64(SPKI_EXPLICIT + fixture_point),
            'spkiCompressedPointBase64': b64(SPKI_COMPRESSED + bytes([2 + (y & 1)]) + b32(x)),
            'fingerprint': hashlib.sha256(SPKI_NAMED + fixture_point).hexdigest(),
        }],
    })


def split(target):
    question = target.find('?')
    path = target if question < 0 else target[:question]
    query = '' if question < 0 else target[question:]
    return path or '/', query


def compose(method, target, timestamp, nonce, body):
    path, query = split(target)
    return '\n'.join(['BAM-REQUEST-PROOF-1', method.upper(), path, query, str(timestamp), nonce,
                      hashlib.sha256(body).hexdigest()])


CANONICAL_CASES = [
    ('the documented example', 'post', '/a?b=1', 1760529600, '0123456789abcdef', b''),
    ('no query', 'GET', '/', 1790000000, 'AAAAAAAAAAAAAAAA', b''),
    ('a JSON body', 'POST', '/invoke', 1790000000, 'nonce-with_dash-0001',
     b'{"OperationIdentifier":"Sample.Greeter+Greet, Sample","Arguments":{"name":"Ada"}}'),
    ('percent-encoding is signed as sent', 'PUT', '/a%20b/caf%C3%A9?x=%2F&y=a+b', 1790000000, '00000000000000000000000000000000', b'x'),
    ('an empty query keeps its question mark', 'DELETE', '/a?', 1790000000, 'abcdefghijklmnop', b''),
    ('only the first question mark splits', 'GET', '/a?b=1?c=2', 1790000000, 'abcdefghijklmnop', b''),
    ('dot segments are not resolved', 'GET', '/a/../b//c', 1790000000, 'abcdefghijklmnop', b''),
    ('binary body', 'POST', '/upload', 1790000000, 'abcdefghijklmnop', bytes(range(256))),
    ('the longest nonce', 'POST', '/a', 1, 'z' * 128, b'body'),
]


def canonical_request():
    cases = []
    for name, method, target, timestamp, nonce, body in CANONICAL_CASES:
        path, query = split(target)
        text = compose(method, target, timestamp, nonce, body)
        cases.append({
            'name': name, 'method': method, 'target': target, 'timestamp': timestamp, 'nonce': nonce,
            'bodyHex': body.hex(), 'path': path, 'query': query,
            'bodySha256Hex': hashlib.sha256(body).hexdigest(),
            'canonical': text, 'canonicalUtf8Hex': text.encode('utf-8').hex(),
        })
    write('canonical-request.json', {
        'description': 'The text a request proof signs: seven fields joined by a line feed, with no trailing line feed. '
                       'The target is the request target exactly as sent on the request line.',
        'cases': cases,
        'refusedTargets': [
            {'name': 'empty', 'target': ''},
            {'name': 'absolute form', 'target': 'http://host/a'},
            {'name': 'no leading slash', 'target': 'a/b'},
            {'name': 'asterisk form', 'target': '*'},
            {'name': 'a fragment', 'target': '/a#top'},
            {'name': 'a fragment after the query', 'target': '/a?b=1#top'},
        ],
        'wellFormedNonces': ['0123456789abcdef', 'ABCDEFGHIJKLMNOP', 'a-b_c-d_e-f_g-h_', 'z' * 128],
        'malformedNonces': [
            {'name': 'empty', 'nonce': ''},
            {'name': 'fifteen characters', 'nonce': '0123456789abcde'},
            {'name': 'one hundred and twenty-nine characters', 'nonce': 'z' * 129},
            {'name': 'a plus sign', 'nonce': '0123456789abcde+'},
            {'name': 'a slash', 'nonce': '0123456789abcde/'},
            {'name': 'padding', 'nonce': '0123456789abcde='},
            {'name': 'a space', 'nonce': '0123456789 abcdef'},
            {'name': 'a non-ASCII letter', 'nonce': '0123456789abcdéf'},
        ],
    })


def signatures():
    text = compose('POST', '/invoke', 1790000000, 'nonce-with_dash-0001', b'{"a":1}')
    r, s = sign(ACTOR.d, text.encode())
    other_r, other_s = sign(OTHER.d, text.encode())
    plain_r, plain_s = sign(ACTOR.d, b'hello')

    def case(name, key, signed, signature, algorithm, verifies):
        return {'name': name, 'key': key, 'text': signed, 'algorithm': algorithm,
                'signatureBase64': b64(signature), 'verifies': verifies}

    write('signatures.json', {
        'description': 'ECDSA P-256 signatures over the UTF-8 bytes of the text, hashed with SHA-256. A signature is the '
                       'ASN.1 DER SEQUENCE of r and s, in standard base64. The raw 64-byte r||s form is refused. '
                       'A null algorithm means the header was not sent.',
        'cases': [
            case('a request proof', 'actor', text, der(r, s), 'SHA256WITHECDSA', True),
            case('no algorithm named', 'actor', text, der(r, s), None, True),
            case('the algorithm name in lower case', 'actor', text, der(r, s), 'sha256withecdsa', True),
            case('plain text', 'actor', 'hello', der(plain_r, plain_s), 'SHA256WITHECDSA', True),
            case('one character of the text changed', 'actor', text + ' ', der(r, s), 'SHA256WITHECDSA', False),
            case('signed by another key', 'actor', text, der(other_r, other_s), 'SHA256WITHECDSA', False),
            case('the raw r||s form', 'actor', text, raw(r, s), 'SHA256WITHECDSA', False),
            case('one bit of s changed', 'actor', text, der(r, s ^ 1), 'SHA256WITHECDSA', False),
            case('an algorithm outside the allow-list', 'actor', text, der(r, s), 'SHA1WITHECDSA', False),
            case('no hash', 'actor', text, der(r, s), 'NONEwithECDSA', False),
            case('an empty signature', 'actor', text, b'', 'SHA256WITHECDSA', False),
        ],
    })


def der_raw():
    def pair(name, r, s):
        return {'name': name, 'rHex': b32(r).hex(), 'sHex': b32(s).hex(), 'derHex': der(r, s).hex(), 'rawHex': raw(r, s).hex()}

    r, s = sign(ACTOR.d, b'hello')
    write('der-raw.json', {
        'description': 'One signature in its two encodings. The wire carries DER; WebCrypto and JOSE libraries produce and '
                       'expect raw r||s (each 32 bytes, big-endian, zero-padded), so a client converts both ways. A DER '
                       'INTEGER is minimal: no leading zero bytes, except one when the high bit is set.',
        'cases': [
            pair('a real signature', r, s),
            pair('both high bits set', 1 << 255, (1 << 255) + 1),
            pair('both high bits clear', (1 << 254) + 5, (1 << 254) + 7),
            pair('r is one byte', 1, (1 << 255) + 3),
            pair('s is thirty-one bytes', (1 << 255) + 9, (1 << 247) + 1),
            pair('r has a zero byte inside', 0x7F00FF << 200, 0x100 << 240),
        ],
    })


def system_text_json(value):
    """A JSON string as System.Text.Json writes it by default, for the characters used here."""
    return json.dumps(value).replace('+', '\\u002B')


def token_parts(claims):
    header = '{"alg":"ES256","typ":"JWT"}'
    payload = '{' + ','.join(
        system_text_json(name) + ':' + (str(value) if isinstance(value, int) else system_text_json(value))
        for name, value in claims.items()) + '}'
    return header, payload, b64url(header.encode()) + '.' + b64url(payload.encode())


def tokens():
    fingerprint = ACTOR.fingerprint()
    sets = [
        ('a client token', 'actor', {'sub': 'ada', 'sid': '0123456789abcdef0123456789abcdef', 'iss': 'client',
                                     'iat': 1790000000, 'exp': 1790000120, 'aud': 'https://api.example.test'}),
        ('a client token without an audience', 'actor', {'sub': 'ada', 'sid': 'fedcba9876543210fedcba9876543210',
                                                         'iss': 'client', 'iat': 1790000000, 'exp': 1790000300}),
        ('a server token', 'server', {'sub': 'ada', 'sid': '00112233445566778899aabbccddeeff', 'iss': 'bam',
                                      'iat': 1790000000, 'exp': 1790002700, 'kfp': fingerprint}),
        ('a handle with a plus sign', 'actor', {'sub': 'ada+test@example.test', 'sid': '0123456789abcdef0123456789abcdef',
                                                'iss': 'client', 'iat': 1790000000, 'exp': 1790000120}),
    ]
    cases = []
    for name, key, claims in sets:
        header, payload, signing_input = token_parts(claims)
        r, s = sign(KEYS[key].d, signing_input.encode())
        cases.append({
            'name': name, 'key': key, 'claims': claims, 'headerJson': header, 'payloadJson': payload,
            'signingInput': signing_input, 'signatureDerHex': der(r, s).hex(),
            'token': signing_input + '.' + b64url(der(r, s)),
        })
    header, payload, signing_input = token_parts(sets[0][2])
    r, s = sign(ACTOR.d, signing_input.encode())
    other_r, other_s = sign(OTHER.d, signing_input.encode())
    tampered = token_parts(dict(sets[0][2], sub='eve'))[2]
    write('tokens.json', {
        'description': 'Tokens are three base64url segments without padding: header, payload, signature. The signature is '
                       'ECDSA P-256 with SHA-256 over the ASCII bytes of "header.payload", as ASN.1 DER. The header says '
                       'ES256, but a JOSE ES256 signature is raw r||s and is refused. headerJson and payloadJson are the '
                       'bytes the C# implementation writes for these claims; a verifier accepts any JSON layout.',
        'cases': cases,
        'refused': [
            {'name': 'signed by another key', 'key': 'actor', 'token': signing_input + '.' + b64url(der(other_r, other_s))},
            {'name': 'a JOSE raw signature', 'key': 'actor', 'token': signing_input + '.' + b64url(raw(r, s))},
            {'name': 'the payload changed after signing', 'key': 'actor', 'token': tampered + '.' + b64url(der(r, s))},
            {'name': 'two segments', 'key': 'actor', 'token': signing_input},
            {'name': 'an empty signature', 'key': 'actor', 'token': signing_input + '.'},
        ],
    })


def request_proofs():
    now = 1790000000
    body = b'{"OperationIdentifier":"Sample.Greeter+Greet, Sample","Arguments":{"name":"Ada"}}'

    def proof(name, verified, method='POST', target='/invoke?trace=1', sent_body=body, timestamp=now,
              nonce='proof-nonce-0001', key=ACTOR, signed=None, headers=None, why=None):
        text = compose(*(signed or (method, target, timestamp, nonce, sent_body)))
        r, s = sign(key.d, text.encode())
        sent = {
            'X-Bam-Body-Signature': b64(der(r, s)),
            'X-Bam-Body-Signature-Algorithm': 'SHA256WITHECDSA',
            'X-Bam-Timestamp': str(timestamp),
            'X-Bam-Proof-Nonce': nonce,
        }
        for header, value in (headers or {}).items():
            if value is None:
                del sent[header]
            else:
                sent[header] = value
        entry = {'name': name, 'method': method, 'target': target, 'bodyHex': sent_body.hex(), 'headers': sent, 'verified': verified}
        if why:
            entry['why'] = why
        return entry

    write('request-proof.json', {
        'description': 'Whole requests as a host with default settings judges them. The caller is the actor key, the clock '
                       'reads nowUnixSeconds, and a proof may be maxAgeSeconds old or ahead. Each case is judged by a host '
                       'that has seen no nonce before, except replays, which are judged twice.',
        'nowUnixSeconds': now,
        'maxAgeSeconds': 300,
        'signingKey': 'actor',
        'cases': [
            proof('a signed POST', True),
            proof('a GET with no body', True, method='GET', target='/things', sent_body=b''),
            proof('no algorithm header', True, headers={'X-Bam-Body-Signature-Algorithm': None}),
            proof('exactly as old as allowed', True, timestamp=now - 300),
            proof('exactly as far ahead as allowed', True, timestamp=now + 300),
            proof('one second too old', False, timestamp=now - 301, why='timestamp outside the window'),
            proof('one second too far ahead', False, timestamp=now + 301, why='timestamp outside the window'),
            proof('the body changed after signing', False, sent_body=body + b' ',
                  signed=('POST', '/invoke?trace=1', now, 'proof-nonce-0001', body), why='the body digest differs'),
            proof('the query changed after signing', False, target='/invoke?trace=2',
                  signed=('POST', '/invoke?trace=1', now, 'proof-nonce-0001', body), why='the query is signed'),
            proof('the method changed after signing', False, method='PUT',
                  signed=('POST', '/invoke?trace=1', now, 'proof-nonce-0001', body), why='the method is signed'),
            proof('signed by a key that is not the caller\'s', False, key=OTHER, why='wrong key'),
            proof('no signature header', False, headers={'X-Bam-Body-Signature': None}, why='missing header'),
            proof('no timestamp header', False, headers={'X-Bam-Timestamp': None}, why='missing header'),
            proof('no nonce header', False, headers={'X-Bam-Proof-Nonce': None}, why='missing header'),
            proof('a timestamp with a sign', False, headers={'X-Bam-Timestamp': '+' + str(now)}, why='digits only'),
            proof('a timestamp in milliseconds', False, timestamp=now * 1000, why='timestamp outside the window'),
            proof('a short nonce', False, nonce='short', signed=('POST', '/invoke?trace=1', now, 'proof-nonce-0001', body),
                  why='malformed nonce'),
            proof('an algorithm outside the allow-list', False, headers={'X-Bam-Body-Signature-Algorithm': 'SHA1WITHECDSA'},
                  why='algorithm not allowed'),
            proof('a fragment in the target', False, target='/invoke#top', why='not an origin-form target'),
        ],
        'replays': [
            proof('the same proof sent twice', True, nonce='proof-nonce-replay'),
        ],
    })


def messages():
    import datetime
    expires = 1790002700
    instant = datetime.datetime.fromtimestamp(expires, datetime.timezone.utc).isoformat()
    pem = 'PEM text of the public key'
    write('messages.json', {
        'description': 'JSON bodies as a host with default settings reads and writes them. Request property names are '
                       'matched without regard to case; every listed property must be present. Responses are written '
                       'in camelCase by default, and a host can change that, so a client reads names without regard to case.',
        'requests': [
            {'name': 'register', 'kind': 'register', 'accepted': True,
             'json': json.dumps({'KeySetHandle': 'ada', 'PublicRsaKeyPem': '', 'PublicEccKeyPem': pem})},
            {'name': 'register in camelCase', 'kind': 'register', 'accepted': True,
             'json': json.dumps({'keySetHandle': 'ada', 'publicRsaKeyPem': '', 'publicEccKeyPem': pem})},
            {'name': 'register without the RSA property', 'kind': 'register', 'accepted': False,
             'json': json.dumps({'KeySetHandle': 'ada', 'PublicEccKeyPem': pem})},
            {'name': 'token', 'kind': 'token', 'accepted': True, 'json': json.dumps({'ClientToken': 'a.b.c'})},
            {'name': 'token without its property', 'kind': 'token', 'accepted': False, 'json': '{}'},
        ],
        'responses': [
            {'name': 'registered', 'kind': 'register', 'keySetHandle': 'ada', 'json': '{"keySetHandle":"ada"}'},
            {'name': 'token issued', 'kind': 'token', 'token': 'a.b.c', 'expiresUnixSeconds': expires,
             'keyFingerprint': ACTOR.fingerprint(),
             'json': '{"token":"a.b.c","expiresUtc":"%s","keyFingerprint":"%s"}' % (instant, ACTOR.fingerprint())},
            {'name': 'refused', 'kind': 'failure', 'message': 'Authentication failed.',
             'json': '{"messages":["Authentication failed."]}'},
        ],
    })


if __name__ == '__main__':
    assert multiply(N, G) is None
    assert len(SPKI_EXPLICIT) == 0x14F - 65
    keys()
    canonical_request()
    signatures()
    der_raw()
    tokens()
    request_proofs()
    messages()
