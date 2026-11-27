"""Writes the sealed-frames known-answer vectors: sealed-session.json and sealed-frames.json.

Run through generate.py, which calls write_all(). Standard library only and deterministic, like the
rest: the session keys are derived from labels, and the transcript signatures use RFC 6979.

AES, GCM, HKDF and the key agreement here are for writing test vectors only. They are slow, not
constant-time, and take no care with their secrets. Do not copy them into a client.
"""
import hashlib
import hmac
import json

import generate as g

TRANSCRIPT_TAG = 'BAM-SESSION-TRANSCRIPT-1'
FRAME_TAG = 'BAM-SEALED-FRAME-1'
AUDIENCE = 'https://api.example.test'
EXPIRES = 1790003600

# --- AES-256, one block -------------------------------------------------------------------------

SBOX = []


def _build_sbox():
    p = q = 1
    box = [0] * 256
    while True:
        p = p ^ ((p << 1) & 0xFF) ^ (0x1B if p & 0x80 else 0)
        q ^= q << 1
        q ^= q << 2
        q ^= q << 4
        q &= 0xFF
        if q & 0x80:
            q ^= 0x09
        x = q ^ ((q << 1) | (q >> 7)) ^ ((q << 2) | (q >> 6)) ^ ((q << 3) | (q >> 5)) ^ ((q << 4) | (q >> 4))
        box[p] = (x ^ 0x63) & 0xFF
        if p == 1:
            break
    box[0] = 0x63
    return box


def _xtime(value):
    return ((value << 1) ^ 0x1B) & 0xFF if value & 0x80 else value << 1


def _expand(key):
    assert len(key) == 32
    words = [list(key[i:i + 4]) for i in range(0, 32, 4)]
    rcon = 1
    for i in range(8, 60):
        word = list(words[i - 1])
        if i % 8 == 0:
            word = [SBOX[b] for b in word[1:] + word[:1]]
            word[0] ^= rcon
            rcon = _xtime(rcon)
        elif i % 8 == 4:
            word = [SBOX[b] for b in word]
        words.append([a ^ b for a, b in zip(words[i - 8], word)])
    return [sum(words[r * 4:r * 4 + 4], []) for r in range(15)]


def aes_block(round_keys, block):
    state = [b ^ k for b, k in zip(block, round_keys[0])]
    for round_number in range(1, 15):
        state = [SBOX[b] for b in state]
        state = [state[(i + 4 * (i % 4)) % 16] for i in range(16)]
        if round_number != 14:
            mixed = []
            for c in range(4):
                a = state[c * 4:c * 4 + 4]
                t = a[0] ^ a[1] ^ a[2] ^ a[3]
                mixed += [a[i] ^ t ^ _xtime(a[i] ^ a[(i + 1) % 4]) for i in range(4)]
            state = mixed
        state = [b ^ k for b, k in zip(state, round_keys[round_number])]
    return bytes(state)


# --- GCM ----------------------------------------------------------------------------------------

def _ghash_multiply(x, y):
    z = 0
    v = y
    for bit in range(127, -1, -1):
        if (x >> bit) & 1:
            z ^= v
        v = (v >> 1) ^ (0xE1 << 120) if v & 1 else v >> 1
    return z


def _ghash(h, aad, ciphertext):
    def blocks(data):
        return data + b'\x00' * (-len(data) % 16)

    data = blocks(aad) + blocks(ciphertext) + (len(aad) * 8).to_bytes(8, 'big') + (len(ciphertext) * 8).to_bytes(8, 'big')
    y = 0
    for i in range(0, len(data), 16):
        y = _ghash_multiply(y ^ int.from_bytes(data[i:i + 16], 'big'), h)
    return y.to_bytes(16, 'big')


def _ctr(round_keys, counter_block, data):
    out = bytearray()
    counter = int.from_bytes(counter_block, 'big')
    for i in range(0, len(data), 16):
        stream = aes_block(round_keys, counter.to_bytes(16, 'big'))
        out += bytes(a ^ b for a, b in zip(data[i:i + 16], stream))
        counter = (counter & ~0xFFFFFFFF) | ((counter + 1) & 0xFFFFFFFF)
    return bytes(out)


def gcm_seal(key, nonce, plaintext, aad):
    """AES-256-GCM with a 12-byte nonce. Returns the ciphertext with the 16-byte tag appended."""
    assert len(nonce) == 12
    round_keys = _expand(key)
    h = int.from_bytes(aes_block(round_keys, bytes(16)), 'big')
    j0 = nonce + b'\x00\x00\x00\x01'
    ciphertext = _ctr(round_keys, (int.from_bytes(j0, 'big') + 1).to_bytes(16, 'big'), plaintext)
    tag = bytes(a ^ b for a, b in zip(_ghash(h, aad, ciphertext), aes_block(round_keys, j0)))
    return ciphertext + tag


def gcm_open(key, nonce, sealed, aad):
    """The plaintext, or None when the tag does not verify or the input is too short to hold a tag."""
    if len(sealed) < 16:
        return None
    round_keys = _expand(key)
    h = int.from_bytes(aes_block(round_keys, bytes(16)), 'big')
    j0 = nonce + b'\x00\x00\x00\x01'
    ciphertext, tag = sealed[:-16], sealed[-16:]
    expected = bytes(a ^ b for a, b in zip(_ghash(h, aad, ciphertext), aes_block(round_keys, j0)))
    if not hmac.compare_digest(tag, expected):
        return None
    return _ctr(round_keys, (int.from_bytes(j0, 'big') + 1).to_bytes(16, 'big'), ciphertext)


# --- HKDF and key agreement ---------------------------------------------------------------------

def hkdf(ikm, salt, info, length):
    prk = hmac.new(salt, ikm, hashlib.sha256).digest()
    okm = block = b''
    counter = 1
    while len(okm) < length:
        block = hmac.new(prk, block + info + bytes([counter]), hashlib.sha256).digest()
        okm += block
        counter += 1
    return okm[:length]


def ecdh(private_scalar, peer):
    """The X coordinate of the shared point, as exactly 32 bytes."""
    x, _ = g.multiply(private_scalar, (peer.x, peer.y))
    return g.b32(x)


def self_test():
    """Known answers from FIPS 197, the GCM specification and RFC 5869, so a slip here cannot write wrong vectors."""
    key = bytes(range(32))
    assert aes_block(_expand(key), bytes.fromhex('00112233445566778899aabbccddeeff')).hex() == '8ea2b7ca516745bfeafc49904b496089'
    zero = bytes(32)
    assert gcm_seal(zero, bytes(12), b'', b'').hex() == '530f8afbc74536b9a963b4f1c4cb738b'
    assert gcm_seal(zero, bytes(12), bytes(16), b'').hex() == 'cea7403d4d606b6e074ec5d3baf39d18' + 'd0d1c8a799996bf0265b98b5d48ab919'
    key16 = bytes.fromhex('feffe9928665731c6d6a8f9467308308feffe9928665731c6d6a8f9467308308')
    nonce16 = bytes.fromhex('cafebabefacedbaddecaf888')
    plain16 = bytes.fromhex('d9313225f88406e5a55909c5aff5269a86a7a9531534f7da2e4c303d8a318a72'
                            '1c3c0c95956809532fcf0e2449a6b525b16aedf5aa0de657ba637b39')
    aad16 = bytes.fromhex('feedfacedeadbeeffeedfacedeadbeefabaddad2')
    sealed16 = gcm_seal(key16, nonce16, plain16, aad16)
    assert sealed16.hex() == ('522dc1f099567d07f47f37a32a84427d643a8cdcbfe5c0c97598a2bd2555d1aa'
                              '8cb08e48590dbb3da7b08b1056828838c5f61e6393ba7a0abcc9f662'
                              '76fc6ece0f4e1768cddf8853bb2d551b')
    assert gcm_open(key16, nonce16, sealed16, aad16) == plain16
    assert gcm_open(key16, nonce16, sealed16, aad16 + b'x') is None
    okm = hkdf(bytes.fromhex('0b' * 22), bytes.fromhex('000102030405060708090a0b0c'), bytes.fromhex('f0f1f2f3f4f5f6f7f8f9'), 42)
    assert okm.hex() == '3cb25f25faacd57a90434f64d0362f2a2d2d0a90cf1a5a4c5db02d56ecc4c5bf34007208d5b887185865'


# --- the wire -----------------------------------------------------------------------------------

def wire_key(key):
    return g.b64url(key.spki)


def transcript(audience, actor_fingerprint, session_id, client_key, server_key, expires):
    return '\n'.join([TRANSCRIPT_TAG, audience, actor_fingerprint, session_id, client_key, server_key, str(expires)])


def sign_transcript(signer, text):
    r, s = g.sign(signer.d, text.encode('utf-8'))
    return g.b64url(g.der(r, s))


def frame_keys(shared, text):
    salt = hashlib.sha256(text.encode('utf-8')).digest()
    return (salt,
            hkdf(shared, salt, (FRAME_TAG + ' client-to-server').encode('ascii'), 32),
            hkdf(shared, salt, (FRAME_TAG + ' server-to-client').encode('ascii'), 32))


def nonce(direction, sequence):
    return {'c2s': b'\x00\x00\x00\x01', 's2c': b'\x00\x00\x00\x02'}[direction] + sequence.to_bytes(8, 'big')


def associated_data(session_id, direction, sequence, content_type, last):
    return '\n'.join([FRAME_TAG, session_id, direction, str(sequence), content_type + (';last' if last else '')])


def frame_json(session_id, sequence, content_type, last, ciphertext):
    return json.dumps({'sessionId': session_id, 'sequence': sequence, 'contentType': content_type, 'last': last,
                       'ciphertext': ciphertext}, separators=(',', ':'))


def session(name, client_label, server_label, session_id, audience=AUDIENCE, signer=None):
    client = g.Key(client_label, '')
    server = g.Key(server_label, '')
    text = transcript(audience, g.ACTOR.fingerprint(), session_id, wire_key(client), wire_key(server), EXPIRES)
    shared = ecdh(client.d, server)
    assert shared == ecdh(server.d, client)
    salt, c2s, s2c = frame_keys(shared, text)
    return {
        'name': name,
        'audience': audience,
        'actorKey': 'actor',
        'actorKeyFingerprint': g.ACTOR.fingerprint(),
        'sessionId': session_id,
        'clientPrivateScalarHex': g.b32(client.d).hex(),
        'clientPublicKey': wire_key(client),
        'serverPrivateScalarHex': g.b32(server.d).hex(),
        'serverPublicKey': wire_key(server),
        'expires': EXPIRES,
        'transcript': text,
        'transcriptUtf8Hex': text.encode('utf-8').hex(),
        'signingKey': (signer or g.SERVER).name,
        'signature': sign_transcript(signer or g.SERVER, text),
        'sharedHex': shared.hex(),
        'saltHex': salt.hex(),
        'keyClientToServerHex': c2s.hex(),
        'keyServerToClientHex': s2c.hex(),
    }


def leading_zero_labels():
    """Session key labels whose shared secret begins with a zero byte, the case a careless library gets wrong."""
    server = g.Key('session server zero', '')
    for index in range(4096):
        label = 'session client zero %d' % index
        if ecdh(g.Key(label, '').d, server)[0] == 0:
            return label, 'session server zero'
    raise AssertionError('no leading-zero secret found')


def sessions():
    first = session('a session', 'session client 1', 'session server 1', '00112233445566778899aabbccddeeff')
    zero_client, zero_server = leading_zero_labels()
    zero = session('a shared secret with a leading zero byte', zero_client, zero_server, 'ffeeddccbbaa99887766554433221100')
    assert zero['sharedHex'].startswith('00')

    def refused(name, why, base=first, **changes):
        case = {key: base[key] for key in ('audience', 'actorKeyFingerprint', 'sessionId', 'clientPublicKey',
                                           'serverPublicKey', 'expires', 'signingKey', 'signature')}
        case.update(changes)
        return dict({'name': name, 'why': why, 'pinnedKey': 'server'}, **case)

    by_other = session('x', 'session client 1', 'session server 1', first['sessionId'], signer=g.OTHER)
    for_elsewhere = session('x', 'session client 1', 'session server 1', first['sessionId'], audience='https://elsewhere.example.test')
    compressed = g.Key('session server 1', '')
    compressed_key = g.b64url(g.SPKI_COMPRESSED + bytes([2 + (compressed.y & 1)]) + g.b32(compressed.x))
    g.write('sealed-session.json', {
        'description': 'Sealed-frame sessions (spec/sealed-frames.md, sections 2 to 4). Each case gives both session '
                       'private keys so either side can be checked: build the transcript from the fields, verify '
                       'signature over it with signingKey from keys.json, agree the shared secret, and derive the '
                       'two frame keys. In a refused case the client is configured with the audience and pins the '
                       'key named pinnedKey; it must refuse the session before deriving anything.',
        'transcriptTag': TRANSCRIPT_TAG,
        'frameTag': FRAME_TAG,
        'cases': [first, zero],
        'refused': [
            refused('signed by a key that is not pinned', 'The signature is good, by another key.',
                    signingKey='other', signature=by_other['signature']),
            refused('signed for another audience', 'The host signed a transcript naming another origin. The client '
                    'builds the transcript with its own audience, so the signature does not verify.',
                    signature=for_elsewhere['signature']),
            refused('a later expiry than the one signed', 'One field changed after signing.', expires=EXPIRES + 3600),
            refused('another session id than the one signed', 'One field changed after signing.',
                    sessionId='00112233445566778899aabbccddeef0'),
            refused('another actor than the one signed for', 'The transcript names the actor key. A session opened '
                    'by one actor is not valid for another.', actorKeyFingerprint=g.OTHER.fingerprint()),
            refused('a server key in compressed form', 'Only the named-curve, uncompressed encoding is accepted on '
                    'this wire.', serverPublicKey=compressed_key),
            refused('an empty signature', 'No signature.', signature=''),
        ],
    })
    return first


def frames(base):
    session_id = base['sessionId']
    keys = {'c2s': bytes.fromhex(base['keyClientToServerHex']), 's2c': bytes.fromhex(base['keyServerToClientHex'])}

    def seal(direction, sequence, content_type, last, plaintext):
        aad = associated_data(session_id, direction, sequence, content_type, last)
        sealed = gcm_seal(keys[direction], nonce(direction, sequence), plaintext, aad.encode('utf-8'))
        assert gcm_open(keys[direction], nonce(direction, sequence), sealed, aad.encode('utf-8')) == plaintext
        return aad, sealed

    def frame(name, direction, sequence, content_type, last, plaintext):
        aad, sealed = seal(direction, sequence, content_type, last, plaintext)
        return {
            'name': name, 'direction': direction, 'sequence': sequence, 'contentType': content_type, 'last': last,
            'plaintextHex': plaintext.hex(), 'nonceHex': nonce(direction, sequence).hex(),
            'associatedData': aad, 'associatedDataUtf8Hex': aad.encode('utf-8').hex(),
            'ciphertext': g.b64url(sealed),
            'json': frame_json(session_id, sequence, content_type, last, g.b64url(sealed)),
        }

    body = b'{"OperationIdentifier":"Sample.Greeter+Greet, Sample","Arguments":{"name":"Ada"}}'
    good = [
        frame('a request', 'c2s', 0, 'application/json', True, body),
        frame('its answer', 's2c', 0, 'application/json', True, b'{"Greeting":"Hello, Ada"}'),
        frame('a second request', 'c2s', 1, 'application/json', True, body),
        frame('the first frame of a stream', 's2c', 1, 'text/plain', False, 'Hello, '.encode('utf-8')),
        frame('the last frame of a stream', 's2c', 2, 'text/plain', True, 'Ada ✓'.encode('utf-8')),
        frame('an empty body with no content type', 'c2s', 2, '', True, b''),
        frame('a binary body longer than one block', 'c2s', 3, 'application/octet-stream', True, bytes(range(256)) * 2),
        frame('the highest sequence number a session may use', 'c2s', 2 ** 20 - 1, 'application/json', True, b'{}'),
    ]

    def refused(name, why, direction, expected, sequence, content_type, last, ciphertext, session=session_id):
        return {'name': name, 'why': why, 'direction': direction, 'expectedSequence': expected,
                'json': frame_json(session, sequence, content_type, last, g.b64url(ciphertext))}

    _, request = seal('c2s', 0, 'application/json', True, body)
    _, answer = seal('s2c', 0, 'application/json', True, b'{"Greeting":"Hello, Ada"}')
    _, stream = seal('s2c', 1, 'text/plain', False, b'Hello, ')
    flipped = bytearray(request)
    flipped[5] ^= 1
    tag_flipped = bytearray(request)
    tag_flipped[-1] ^= 1
    bad = [
        refused('one ciphertext bit changed', 'The tag does not verify.', 'c2s', 0, 0, 'application/json', True, bytes(flipped)),
        refused('one tag bit changed', 'The tag does not verify.', 'c2s', 0, 0, 'application/json', True, bytes(tag_flipped)),
        refused('the content type changed', 'The associated data differs.', 'c2s', 0, 0, 'text/plain', True, request),
        refused('a stream frame passed off as the last', 'The associated data differs.', 's2c', 1, 1, 'text/plain', True, stream),
        refused('a last frame passed off as not last', 'The associated data differs.', 'c2s', 0, 0, 'application/json', False, request),
        refused('a frame replayed in the other direction', 'A request frame presented as an answer: the key, the '
                'nonce and the associated data all differ.', 's2c', 0, 0, 'application/json', True, request),
        refused('an answer replayed as a request', 'The same, the other way.', 'c2s', 0, 0, 'application/json', True, answer),
        refused('a repeated frame', 'The sequence number is lower than the next expected. Refused before the tag is '
                'looked at.', 'c2s', 1, 0, 'application/json', True, request),
        refused('a skipped sequence number', 'The sequence number is higher than the next expected.', 'c2s', 0, 1,
                'application/json', True, seal('c2s', 1, 'application/json', True, body)[1]),
        refused('a renumbered frame', 'Frame 0 sent as frame 1, when 1 is expected: the tag does not verify.', 'c2s',
                1, 1, 'application/json', True, request),
        refused('a ciphertext too short to hold a tag', 'Fewer than 16 bytes.', 'c2s', 0, 0, 'application/json', True, request[-15:]),
        refused('a frame for another session', 'The session id is not this session.', 'c2s', 0, 0, 'application/json',
                True, request, session='00112233445566778899aabbccddeef0'),
        refused('a sequence number past the limit', 'A session carries at most 2^20 frames in a direction.', 'c2s',
                2 ** 20, 2 ** 20, 'application/json', True, seal('c2s', 2 ** 20, 'application/json', True, b'{}')[1]),
    ]
    g.write('sealed-frames.json', {
        'description': 'Sealed frames (spec/sealed-frames.md, sections 5 and 7) for the session named in session, '
                       'from sealed-session.json. For each frame: build the nonce and the associated data from the '
                       'fields, seal the plaintext with the key for its direction, and get ciphertext; and open '
                       'ciphertext to get the plaintext. json is the frame as it goes on the wire. A refused frame '
                       'arrives when the receiver for that direction expects expectedSequence; it must be refused, '
                       'and nothing decrypted.',
        'session': base['name'],
        'sessionId': session_id,
        'keyClientToServerHex': base['keyClientToServerHex'],
        'keyServerToClientHex': base['keyServerToClientHex'],
        'maximumFramesPerDirection': 2 ** 20,
        'frames': good,
        'refused': bad,
    })


def write_all():
    SBOX[:] = _build_sbox()
    self_test()
    frames(sessions())
