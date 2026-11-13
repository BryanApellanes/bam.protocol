using System.Text;
using Bam;
using Bam.Data.Dynamic.Objects;
using Bam.Data.Objects;
using Bam.DependencyInjection;
using Bam.Encryption;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Profile;
using Bam.Protocol.Profile;
using Bam.Storage;
using Bam.Test;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;

namespace Bam.Protocol.Tests.Unit.Profile;

/// <summary>
/// Pins bam.protocol#72: rotation proofs other than RSA, the proof chain, the anchoring rule
/// that replaced "the RSA key must be non-empty", and the tombstone of retired keys.
/// </summary>
[UnitTestMenu("KeySetRotationProofs Should", Selector = "ksrp")]
public class KeySetRotationProofsShould : UnitTestMenuContainer
{
    [UnitTest]
    public void ComposeThePayloadForEitherAnchor()
    {
        RsaPublicPrivateKeyPair rsa = new RsaPublicPrivateKeyPair();
        EccPublicPrivateKeyPair ecc = new EccPublicPrivateKeyPair();
        EccPublicPrivateKeyPair next = new EccPublicPrivateKeyPair();

        When.A<PublicKeySetData>("composes the rotation payload for an RSA or an ECC anchor",
            () => new PublicKeySetData { KeySetHandle = "holder", PublicEccKey = next.PublicKeyPem },
            (proposed) =>
            {
                PublicKeySetData rsaAnchored = new PublicKeySetData { KeySetHandle = "holder", PublicRsaKey = rsa.PublicKeyPem, PublicEccKey = ecc.PublicKeyPem };
                PublicKeySetData eccOnly = new PublicKeySetData { KeySetHandle = "holder", PublicEccKey = ecc.PublicKeyPem };
                bool rsaMatchesCompose = KeySetRotationPayload.ComposeFor(rsaAnchored, proposed) == KeySetRotationPayload.Compose(rsa.PublicKeyPem.Sha256(), proposed);
                string tag = KeySetRotationPayload.ContextTag;
                bool eccBindsEccKey = KeySetRotationPayload.ComposeFor(eccOnly, proposed) == $"{Encoding.UTF8.GetByteCount(tag)}:{tag}," + KeySetRotationPayload.Compose(ecc.PublicKeyPem.Sha256(), proposed);
                bool anchorsDiffer = KeySetRotationPayload.ComposeFor(rsaAnchored, proposed) != KeySetRotationPayload.ComposeFor(eccOnly, proposed);
                bool emptyThrows = false;
                try
                {
                    KeySetRotationPayload.AnchorSha256(new PublicKeySetData { KeySetHandle = "holder" });
                }
                catch (ArgumentException)
                {
                    emptyThrows = true;
                }
                return new PayloadOutcome(rsaMatchesCompose, eccBindsEccKey, anchorsDiffer, emptyThrows);
            })
        .TheTest
        .ShouldPass<PayloadOutcome>((because, outcome) =>
        {
            because.ItsTrue("an RSA-anchored payload is byte-identical to Compose", outcome.RsaMatchesCompose);
            because.ItsTrue("an ECC-only payload opens with the context tag and binds the ECC key", outcome.EccBindsEccKey);
            because.ItsTrue("the two anchors give different payloads", outcome.AnchorsDiffer);
            because.ItsTrue("a key set with no key has no anchor", outcome.EmptyThrows);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RotateAnEccOnlyKeySetWithAnEcdsaProof()
    {
        AsymmetricCipherKeyPair currentPair = EccPublicPrivateKeyPair.Generate();
        EccPublicPrivateKeyPair current = new EccPublicPrivateKeyPair(currentPair.ToPem(Encoding.UTF8));
        EccPublicPrivateKeyPair next = new EccPublicPrivateKeyPair();
        EccPublicPrivateKeyPair other = new EccPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair rsa = new RsaPublicPrivateKeyPair();
        ObjectDataRepository repository = CreateObjectDataRepository(nameof(RotateAnEccOnlyKeySetWithAnEcdsaProof));

        When.A<PublicKeySetRegistrar>("rotates an ECC-only key set with a proof by its ECC key, and nothing else",
            () => new PublicKeySetRegistrar(repository, KeySetRotationProofChain.Default()),
            (registrar) =>
            {
                registrar.Register(new PublicKeySetData { KeySetHandle = "ecc-holder", PublicEccKey = current.PublicKeyPem });
                PublicKeySetData currentRow = registrar.Resolve("ecc-holder")!;
                PublicKeySetData proposed = new PublicKeySetData { KeySetHandle = "ecc-holder", PublicEccKey = next.PublicKeyPem };
                PublicKeySetData elsewhere = new PublicKeySetData { KeySetHandle = "ecc-holder", PublicEccKey = other.PublicKeyPem };

                bool otherKeyRefused = RotationRefused(registrar, proposed, EcdsaSign(other, currentRow, proposed));
                bool otherProposalRefused = RotationRefused(registrar, proposed, EcdsaSign(current, currentRow, elsewhere));
                bool rsaShapedRefused = RotationRefused(registrar, proposed, new RsaSignatureProvider().Sign(rsa, KeySetRotationPayload.ComposeFor(currentRow, proposed), RsaKeySetRotationVerifier.Algorithm).SignatureBytes);
                bool emptyRefused = RotationRefused(registrar, proposed, Array.Empty<byte>());

                // the ECC-only sets this proof anchors are exactly the ES256 passkey key sets, so what
                // an authenticator signs must never read as a rotation proof (design risk table)
                string payload = KeySetRotationPayload.ComposeFor(currentRow, proposed);
                WebAuthnAssertion assertion = SignLikeAnAuthenticator(currentPair.Private, payload);
                bool authenticatorSignatureRefused = RotationRefused(registrar, proposed, assertion.Signature);
                bool assertionJsonRefused = RotationRefused(registrar, proposed, Encoding.UTF8.GetBytes(assertion.ToJson()));
                bool untaggedRefused = RotationRefused(registrar, proposed,
                    new EccSignatureProvider().Sign(current, KeySetRotationPayload.Compose(current.PublicKeyPem.Sha256(), proposed), EcdsaKeySetRotationProof.Algorithm).SignatureBytes);
                bool unchanged = registrar.Resolve("ecc-holder")!.PublicEccKey == current.PublicKeyPem;

                bool validRefused = RotationRefused(registrar, proposed, EcdsaSign(current, currentRow, proposed));
                string resolved = registrar.Resolve("ecc-holder")!.PublicEccKey;
                return new EcdsaRotationOutcome(otherKeyRefused, otherProposalRefused, rsaShapedRefused, emptyRefused, authenticatorSignatureRefused, assertionJsonRefused, untaggedRefused, unchanged, validRefused, resolved == next.PublicKeyPem);
            })
        .TheTest
        .ShouldPass<EcdsaRotationOutcome>((because, outcome) =>
        {
            because.ItsTrue("a proof by another ECC key is refused", outcome.OtherKeyRefused);
            because.ItsTrue("a proof over a different proposal is refused", outcome.OtherProposalRefused);
            because.ItsTrue("an RSA signature is refused", outcome.RsaShapedRefused);
            because.ItsTrue("an empty proof is refused", outcome.EmptyRefused);
            because.ItsTrue("an authenticator's signature over authenticatorData and the clientDataJSON hash is refused", outcome.AuthenticatorSignatureRefused);
            because.ItsTrue("a passkey assertion presented as JSON is refused", outcome.AssertionJsonRefused);
            because.ItsTrue("a signature over the payload without the context tag is refused", outcome.UntaggedRefused);
            because.ItsTrue("the refusals left the key set as it was", outcome.Unchanged);
            because.ItsTrue("a proof by the current ECC key rotates", !outcome.ValidRefused);
            because.ItsTrue("the handle resolves to the rotated-to ECC key", outcome.Resolved);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void AnchorRotationTargetsOnTheTrustedProofs()
    {
        RsaPublicPrivateKeyPair rsa = new RsaPublicPrivateKeyPair();
        EccPublicPrivateKeyPair ecc = new EccPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair rsa2 = new RsaPublicPrivateKeyPair();
        EccPublicPrivateKeyPair ecc2 = new EccPublicPrivateKeyPair();
        EccPublicPrivateKeyPair ecc3 = new EccPublicPrivateKeyPair();
        ObjectDataRepository legacyRepository = CreateObjectDataRepository(nameof(AnchorRotationTargetsOnTheTrustedProofs) + "Legacy");
        ObjectDataRepository chainRepository = CreateObjectDataRepository(nameof(AnchorRotationTargetsOnTheTrustedProofs) + "Chain");

        When.A<PublicKeySetData>("refuses an ECC-only target only when no trusted proof could rotate away from it",
            () => new PublicKeySetData { KeySetHandle = "mover", PublicEccKey = ecc.PublicKeyPem },
            (proposed) =>
            {
                PublicKeySetRegistrar legacy = new PublicKeySetRegistrar(legacyRepository, new RsaKeySetRotationVerifier(new RsaSignatureProvider()));
                legacy.Register(new PublicKeySetData { KeySetHandle = "mover", PublicRsaKey = rsa.PublicKeyPem });
                bool legacyRefused = RotationRefused(legacy, proposed, RsaSign(rsa, legacy.Resolve("mover")!, proposed));

                PublicKeySetRegistrar chained = new PublicKeySetRegistrar(chainRepository, KeySetRotationProofChain.Default());
                chained.Register(new PublicKeySetData { KeySetHandle = "mover", PublicRsaKey = rsa.PublicKeyPem });
                bool chainRefused = RotationRefused(chained, proposed, RsaSign(rsa, chained.Resolve("mover")!, proposed));
                bool movedToEcc = chained.Resolve("mover")!.PublicEccKey == ecc.PublicKeyPem;

                KeySetRotationProofChain rsaOnly = new KeySetRotationProofChain(new KeySetRotationProofs(new RsaKeySetRotationVerifier(new RsaSignatureProvider())));
                bool rsaOnlyChainAnchorsEcc = rsaOnly.CanRotateFrom(proposed);

                // the ECDSA proof never widens who can rotate a key set that has an RSA key: a proof by
                // its ECC key alone is refused
                chained.Register(new PublicKeySetData { KeySetHandle = "both", PublicRsaKey = rsa2.PublicKeyPem, PublicEccKey = ecc2.PublicKeyPem });
                PublicKeySetData fromBoth = new PublicKeySetData { KeySetHandle = "both", PublicEccKey = ecc3.PublicKeyPem };
                PublicKeySetData bothRow = chained.Resolve("both")!;
                bool eccProofOfRsaSetRefused = RotationRefused(chained, fromBoth,
                    new EccSignatureProvider().Sign(ecc2, KeySetRotationPayload.Compose(ecc2.PublicKeyPem.Sha256(), fromBoth), EcdsaKeySetRotationProof.Algorithm).SignatureBytes)
                    && RotationRefused(chained, fromBoth, EcdsaSign(ecc2, bothRow, fromBoth));
                return new AnchoringOutcome(legacyRefused, chainRefused, movedToEcc, rsaOnlyChainAnchorsEcc, eccProofOfRsaSetRefused);
            })
        .TheTest
        .ShouldPass<AnchoringOutcome>((because, outcome) =>
        {
            because.ItsTrue("a verifier written before #72 keeps the RSA-only rule", outcome.LegacyRefused);
            because.ItsTrue("the default chain rotates an RSA key set to an ECC-only one", !outcome.ChainRefused);
            because.ItsTrue("the handle now resolves to the ECC key", outcome.MovedToEcc);
            because.ItsTrue("a chain without the ECDSA proof can't rotate away from an ECC-only set", !outcome.RsaOnlyChainAnchorsEcc);
            because.ItsTrue("an ECC proof can't rotate a key set that has an RSA key", outcome.EccProofOfRsaSetRefused);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void LeaveOnlyTheProposedKeysWhenARotationDropsASlot()
    {
        RsaPublicPrivateKeyPair rsa = new RsaPublicPrivateKeyPair();
        EccPublicPrivateKeyPair ecc = new EccPublicPrivateKeyPair();
        EccPublicPrivateKeyPair ecc2 = new EccPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair rsa2 = new RsaPublicPrivateKeyPair();
        EccPublicPrivateKeyPair ecc3 = new EccPublicPrivateKeyPair();
        ObjectDataRepository repository = CreateObjectDataRepository(nameof(LeaveOnlyTheProposedKeysWhenARotationDropsASlot));

        // bam.protocol#90: Update used to skip null properties (bam.data.objects#9), so a dropped slot
        // kept the retired key and the handle still resolved to it.
        When.A<PublicKeySetRegistrar>("drops the retired key from the slot the proposed set leaves empty",
            () => new PublicKeySetRegistrar(repository, KeySetRotationProofChain.Default()),
            (registrar) =>
            {
                registrar.Register(new PublicKeySetData { KeySetHandle = "dropper", PublicRsaKey = rsa.PublicKeyPem, PublicEccKey = ecc.PublicKeyPem });
                PublicKeySetData toEccOnly = new PublicKeySetData { KeySetHandle = "dropper", PublicEccKey = ecc2.PublicKeyPem };
                registrar.Rotate(toEccOnly, RsaSign(rsa, registrar.Resolve("dropper")!, toEccOnly));
                PublicKeySetData afterDrop = new PublicKeySetRegistrar(repository, KeySetRotationProofChain.Default()).Resolve("dropper")!;
                bool rsaDropped = string.IsNullOrEmpty(afterDrop.PublicRsaKey) && afterDrop.PublicEccKey == ecc2.PublicKeyPem;

                // the retired RSA key no longer anchors a rotation of the handle
                PublicKeySetData hijack = new PublicKeySetData { KeySetHandle = "dropper", PublicEccKey = ecc3.PublicKeyPem };
                bool retiredRsaRefused = RotationRefused(registrar, hijack, RsaSign(rsa, afterDrop, hijack));

                PublicKeySetData toRsaOnly = new PublicKeySetData { KeySetHandle = "dropper", PublicRsaKey = rsa2.PublicKeyPem };
                registrar.Rotate(toRsaOnly, EcdsaSign(ecc2, registrar.Resolve("dropper")!, toRsaOnly));
                PublicKeySetData afterSwap = new PublicKeySetRegistrar(repository, KeySetRotationProofChain.Default()).Resolve("dropper")!;
                bool eccDropped = string.IsNullOrEmpty(afterSwap.PublicEccKey) && afterSwap.PublicRsaKey == rsa2.PublicKeyPem;

                int activeRows = repository.Query<PublicKeySetData>(p => p.KeySetHandle == "dropper" && p.RevokedUtc == null).Count();
                return new SlotDropOutcome(rsaDropped, retiredRsaRefused, eccDropped, activeRows);
            })
        .TheTest
        .ShouldPass<SlotDropOutcome>((because, outcome) =>
        {
            because.ItsTrue("RSA+ECC to ECC-only leaves only the proposed ECC key", outcome.RsaDropped);
            because.ItsTrue("the retired RSA key can't rotate the handle any more", outcome.RetiredRsaRefused);
            because.ItsTrue("ECC-only to RSA-only leaves only the proposed RSA key", outcome.EccDropped);
            because.ItsTrue("the handle keeps exactly one active row", outcome.ActiveRows == 1);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RetireTheKeysARotationMovesAwayFrom()
    {
        RsaPublicPrivateKeyPair rsaA = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair rsaB = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair rsaC = new RsaPublicPrivateKeyPair();
        EccPublicPrivateKeyPair ecc = new EccPublicPrivateKeyPair();
        ObjectDataRepository repository = CreateObjectDataRepository(nameof(RetireTheKeysARotationMovesAwayFrom));

        When.A<PublicKeySetRegistrar>("blocklists the retired key and only the retired key",
            () => new PublicKeySetRegistrar(repository, KeySetRotationProofChain.Default()),
            (registrar) =>
            {
                registrar.Register(new PublicKeySetData { KeySetHandle = "retiring", PublicRsaKey = rsaA.PublicKeyPem, PublicEccKey = ecc.PublicKeyPem });
                // the kept ECC key comes back re-encoded (other line endings): kept keys are compared by
                // canonical identity, never by PEM string
                string reencodedEcc = Reencode(ecc.PublicKeyPem);
                PublicKeySetData toB = new PublicKeySetData { KeySetHandle = "retiring", PublicRsaKey = rsaB.PublicKeyPem, PublicEccKey = reencodedEcc };
                registrar.Rotate(toB, RsaSign(rsaA, registrar.Resolve("retiring")!, toB));

                List<PublicKeySetData> tombstones = repository.Query<PublicKeySetData>(p => p.KeySetHandle == "retiring" && p.RevokedUtc != null).ToList();
                bool oneTombstone = tombstones.Count == 1;
                bool holdsOnlyRetired = oneTombstone && tombstones[0].PublicRsaKey == rsaA.PublicKeyPem && string.IsNullOrEmpty(tombstones[0].PublicEccKey);
                bool namesProof = oneTombstone && tombstones[0].RevokedBy == PublicKeySetData.RotationRevokedByPrefix + RsaKeySetRotationVerifier.ProofName;

                bool retiredRefusedElsewhere = false;
                try
                {
                    registrar.Register(new PublicKeySetData { KeySetHandle = "someone-else", PublicRsaKey = rsaA.PublicKeyPem });
                }
                catch (RevokedKeyMaterialException)
                {
                    retiredRefusedElsewhere = true;
                }

                PublicKeySetData backToA = new PublicKeySetData { KeySetHandle = "retiring", PublicRsaKey = rsaA.PublicKeyPem, PublicEccKey = ecc.PublicKeyPem };
                bool backRefused = false;
                try
                {
                    registrar.Rotate(backToA, RsaSign(rsaB, registrar.Resolve("retiring")!, backToA));
                }
                catch (RevokedKeyMaterialException)
                {
                    backRefused = true;
                }

                // the ECC key was kept, so it must not be on the blocklist: the next rotation that keeps it succeeds
                PublicKeySetData toC = new PublicKeySetData { KeySetHandle = "retiring", PublicRsaKey = rsaC.PublicKeyPem, PublicEccKey = ecc.PublicKeyPem };
                bool keptKeyRotates = !RotationRefused(registrar, toC, RsaSign(rsaB, registrar.Resolve("retiring")!, toC));
                int activeRows = repository.Query<PublicKeySetData>(p => p.KeySetHandle == "retiring" && p.RevokedUtc == null).Count();

                PublicKeySetData same = new PublicKeySetData { KeySetHandle = "retiring", PublicRsaKey = rsaC.PublicKeyPem, PublicEccKey = ecc.PublicKeyPem };
                registrar.Rotate(same, RsaSign(rsaC, registrar.Resolve("retiring")!, same));
                int tombstonesAfterNoOp = repository.Query<PublicKeySetData>(p => p.KeySetHandle == "retiring" && p.RevokedUtc != null).Count();
                return new RetirementOutcome(reencodedEcc != ecc.PublicKeyPem, oneTombstone, holdsOnlyRetired, namesProof, retiredRefusedElsewhere, backRefused, keptKeyRotates, activeRows, tombstonesAfterNoOp);
            })
        .TheTest
        .ShouldPass<RetirementOutcome>((because, outcome) =>
        {
            because.ItsTrue("the kept key was resubmitted as a different PEM string", outcome.KeptKeyReencoded);
            because.ItsTrue("the rotation wrote one tombstone", outcome.OneTombstone);
            because.ItsTrue("the tombstone holds the retired RSA key and not the kept ECC key", outcome.HoldsOnlyRetired);
            because.ItsTrue("the tombstone names the proof that authorized the rotation", outcome.NamesProof);
            because.ItsTrue("the retired key can't be registered under another handle", outcome.RetiredRefusedElsewhere);
            because.ItsTrue("the handle can't rotate back to its retired key", outcome.BackRefused);
            because.ItsTrue("a rotation that keeps the ECC key, in its original encoding, still succeeds", outcome.KeptKeyRotates);
            because.ItsTrue("the handle keeps exactly one active row", outcome.ActiveRows == 1);
            because.ItsTrue("a rotation that retires nothing writes no tombstone", outcome.TombstonesAfterNoOp == 2);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void NeverLetARotationTombstoneGovernReregistration()
    {
        RsaPublicPrivateKeyPair first = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair second = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair successor = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair intruder = new RsaPublicPrivateKeyPair();
        ObjectDataRepository repository = CreateObjectDataRepository(nameof(NeverLetARotationTombstoneGovernReregistration));

        When.A<PublicKeySetRegistrar>("keeps an admin successor binding in force over a later rotation tombstone",
            () => new PublicKeySetRegistrar(repository, KeySetRotationProofChain.Default()),
            (registrar) =>
            {
                registrar.Register(new PublicKeySetData { KeySetHandle = "bound", PublicRsaKey = first.PublicKeyPem });
                PublicKeySetData toSecond = new PublicKeySetData { KeySetHandle = "bound", PublicRsaKey = second.PublicKeyPem };
                registrar.Rotate(toSecond, RsaSign(first, registrar.Resolve("bound")!, toSecond));
                PublicKeySetData rotationTombstone = repository.Query<PublicKeySetData>(p => p.KeySetHandle == "bound" && p.RevokedUtc != null).Single();

                // an admin revocation bound to a successor, dated BEFORE the rotation tombstone, so only the
                // rotation-tombstone exclusion keeps the bound revocation governing
                PublicKeySetData active = repository.Query<PublicKeySetData>(p => p.KeySetHandle == "bound" && p.RevokedUtc == null).Single();
                active.RevokedUtc = rotationTombstone.RevokedUtc!.Value.AddMinutes(-1);
                active.RevokedBy = new string('a', 64);
                active.AuthorizedSuccessorFingerprint = PublicKeyFingerprint.Of(successor.PublicKeyPem);
                repository.Update(active);

                bool intruderRefused = false;
                try
                {
                    registrar.Register(new PublicKeySetData { KeySetHandle = "bound", PublicRsaKey = intruder.PublicKeyPem });
                }
                catch (UnauthorizedSuccessorException)
                {
                    intruderRefused = true;
                }
                registrar.Register(new PublicKeySetData { KeySetHandle = "bound", PublicRsaKey = successor.PublicKeyPem });
                bool successorHolds = registrar.Resolve("bound")!.PublicRsaKey == successor.PublicKeyPem;
                return new GoverningOutcome(PublicKeySetData.IsRotationTombstone(rotationTombstone), intruderRefused, successorHolds);
            })
        .TheTest
        .ShouldPass<GoverningOutcome>((because, outcome) =>
        {
            because.ItsTrue("the rotation wrote a rotation tombstone", outcome.IsRotationTombstone);
            because.ItsTrue("a key other than the bound successor is refused", outcome.IntruderRefused);
            because.ItsTrue("the bound successor registers", outcome.SuccessorHolds);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void TryEachAnchoringProofInOrder()
    {
        PublicKeySetData current = new PublicKeySetData { KeySetHandle = "chain", PublicRsaKey = "rsa" };
        PublicKeySetData proposed = new PublicKeySetData { KeySetHandle = "chain", PublicRsaKey = "next" };

        When.A<KeySetRotationProofs>("tries anchoring proofs in order and accepts the first that verifies",
            () => new KeySetRotationProofs(),
            (proofs) =>
            {
                FixedProof bystander = new FixedProof("bystander", anchors: false, ProofResult.Succeed);
                FixedProof thrower = new FixedProof("thrower", anchors: true, ProofResult.Throw);
                FixedProof refuser = new FixedProof("refuser", anchors: true, ProofResult.Fail);
                FixedProof accepter = new FixedProof("accepter", anchors: true, ProofResult.Succeed);
                bool noAnchorFails = !new KeySetRotationProofChain(new KeySetRotationProofs(bystander)).Verify(current, proposed, new byte[] { 1 }).Success;

                proofs.Add(bystander).Add(thrower).Add(refuser).Add(accepter).Add(new FixedProof("accepter", anchors: true, ProofResult.Fail));
                KeySetRotationProofChain chain = new KeySetRotationProofChain(proofs);
                KeySetRotationVerification verification = (KeySetRotationVerification)chain.Verify(current, proposed, new byte[] { 1 });

                KeySetRotationProofChain failing = new KeySetRotationProofChain(new KeySetRotationProofs(thrower, refuser));
                ISignatureVerification failed = failing.Verify(current, proposed, new byte[] { 1 });
                return new ChainOutcome(noAnchorFails, verification.Success, verification.ProofName, bystander.Calls, proofs.All.Count,
                    failed.Success, failed.Message.Contains("thrower") && failed.Message.Contains("refuser"), chain.CanRotateFrom(current),
                    new KeySetRotationProofChain(new KeySetRotationProofs(bystander)).CanRotateFrom(current));
            })
        .TheTest
        .ShouldPass<ChainOutcome>((because, outcome) =>
        {
            because.ItsTrue("with no anchoring proof the chain refuses", outcome.NoAnchorFails);
            because.ItsTrue("a throwing and a refusing proof don't stop a later one", outcome.Accepted);
            because.ItsTrue("the verification names the proof that accepted", outcome.ProofName == "accepter");
            because.ItsTrue("a proof that doesn't anchor the key set is never asked", outcome.BystanderCalls == 0);
            because.ItsTrue("a second proof with the same name is not added", outcome.ProofCount == 4);
            because.ItsTrue("when every proof refuses the chain refuses", !outcome.AllFailedSucceeded);
            because.ItsTrue("the refusal names each proof that refused", outcome.MessageNamesEach);
            because.ItsTrue("the chain can rotate from a set an anchoring proof covers", outcome.CanRotate);
            because.ItsTrue("the chain can't rotate from a set no proof covers", !outcome.BystanderOnlyCanRotate);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RegisterTheChainOverTheSharedProofList()
    {
        string rootPath = $"./.bam/tests/{nameof(RegisterTheChainOverTheSharedProofList)}";

        When.A<ServiceRegistry>("registers the proof chain over the proof list hosts add to",
            () =>
            {
                ServiceRegistry registry = new ServiceRegistry();
                registry.AddEncryptedProfileRepository(rootPath);
                return registry;
            },
            (registry) =>
            {
                KeySetRotationProofs proofs = registry.Get<KeySetRotationProofs>();
                IKeySetRotationVerifier verifier = registry.Get<IKeySetRotationVerifier>();
                KeySetRotationProofChain? chain = verifier as KeySetRotationProofChain;
                List<string> names = proofs.All.Select(proof => proof.Name).ToList();
                proofs.Add(new FixedProof("host", anchors: true, ProofResult.Succeed));
                bool hostProofSeen = chain != null && chain.Proofs.All.Any(proof => proof.Name == "host");
                return new RegistrationOutcome(chain != null, names, hostProofSeen);
            })
        .TheTest
        .ShouldPass<RegistrationOutcome>((because, outcome) =>
        {
            because.ItsTrue("the registered verifier is the proof chain", outcome.IsChain);
            because.ItsTrue("the default proofs are RSA then ECDSA", outcome.DefaultNames.SequenceEqual(new[] { RsaKeySetRotationVerifier.ProofName, EcdsaKeySetRotationProof.ProofName }));
            because.ItsTrue("a proof a host adds is seen by the chain", outcome.HostProofSeen);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    private static bool RotationRefused(PublicKeySetRegistrar registrar, PublicKeySetData proposed, byte[] proof)
    {
        try
        {
            registrar.Rotate(proposed, proof);
            return false;
        }
        catch (InvalidKeySetRotationException)
        {
            return true;
        }
    }

    private static byte[] EcdsaSign(EccPublicPrivateKeyPair signer, PublicKeySetData current, PublicKeySetData proposed)
    {
        return new EccSignatureProvider().Sign(signer, KeySetRotationPayload.ComposeFor(current, proposed), EcdsaKeySetRotationProof.Algorithm).SignatureBytes;
    }

    private static byte[] RsaSign(RsaPublicPrivateKeyPair signer, PublicKeySetData current, PublicKeySetData proposed)
    {
        return new RsaSignatureProvider().Sign(signer, KeySetRotationPayload.ComposeFor(current, proposed), RsaKeySetRotationVerifier.Algorithm).SignatureBytes;
    }

    private static string Reencode(string pem)
    {
        return pem.Contains("\r\n") ? pem.Replace("\r\n", "\n") : pem.Replace("\n", "\r\n");
    }

    private static string Base64Url(byte[] bytes)
    {
        return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

    // What a WebAuthn authenticator signs for an assertion whose challenge is SHA-256 of the
    // payload: authenticatorData (rpIdHash, flags UP|UV, counter) || SHA-256(clientDataJSON).
    private static WebAuthnAssertion SignLikeAnAuthenticator(AsymmetricKeyParameter privateKey, string payload)
    {
        byte[] challenge = DigestUtilities.CalculateDigest("SHA-256", Encoding.UTF8.GetBytes(payload));
        byte[] clientDataJson = Encoding.UTF8.GetBytes("{\"type\":\"webauthn.get\",\"challenge\":\"" + Base64Url(challenge) + "\",\"origin\":\"https://example.test\"}");
        byte[] authenticatorData = new byte[37];
        DigestUtilities.CalculateDigest("SHA-256", Encoding.UTF8.GetBytes("example.test")).CopyTo(authenticatorData, 0);
        authenticatorData[32] = 0x05;
        authenticatorData[36] = 0x01;
        byte[] clientDataHash = DigestUtilities.CalculateDigest("SHA-256", clientDataJson);
        ISigner signer = SignerUtilities.GetSigner(EcdsaKeySetRotationProof.Algorithm);
        signer.Init(true, privateKey);
        signer.BlockUpdate(authenticatorData, 0, authenticatorData.Length);
        signer.BlockUpdate(clientDataHash, 0, clientDataHash.Length);
        return new WebAuthnAssertion(authenticatorData, clientDataJson, signer.GenerateSignature());
    }

    private static ObjectDataRepository CreateObjectDataRepository(string testName)
    {
        string rootPath = $"./.bam/tests/{testName}";
        if (Directory.Exists(rootPath))
        {
            Directory.Delete(rootPath, true);
        }
        AesKey aesKey = new AesKey();
        ICompositeKeyCalculator compositeKeyCalculator = new CompositeKeyCalculator();
        IObjectDataIdentityCalculator identityCalculator = new ObjectDataIdentityCalculator();
        IObjectDataLocatorFactory locatorFactory = new ObjectDataLocatorFactory(identityCalculator);
        IObjectEncoderDecoder encoderDecoder = new JsonObjectDataEncoder();
        IObjectDataFactory factory = new ObjectDataFactory(locatorFactory, encoderDecoder);
        IRootStorageHolder rootStorage = new RootStorageHolder(rootPath);
        IObjectDataStorageManager storageManager = new EncryptedFsObjectDataStorageManager(rootStorage, factory, new AesEncryptor(aesKey), new AesDecryptor(aesKey));
        IObjectDataWriter writer = new ObjectDataWriter(factory, storageManager);
        IObjectDataReader reader = new ObjectDataReader(storageManager);
        IObjectDataIndexer indexer = new ObjectDataIndexer(storageManager, compositeKeyCalculator);
        IObjectDataSearchIndexer searchIndexer = new ObjectDataSearchIndexer(storageManager, indexer);
        IObjectDataSearcher searcher = new ObjectDataSearcher(searchIndexer, reader, indexer);
        IObjectDataDeleter deleter = new ObjectDataDeleter(factory, storageManager, compositeKeyCalculator);
        IObjectDataArchiver archiver = new ObjectDataArchiver(factory, storageManager, compositeKeyCalculator);
        return new ObjectDataRepository(factory, writer, indexer, deleter, archiver, reader, searcher, searchIndexer, compositeKeyCalculator);
    }

    private enum ProofResult
    {
        Succeed,
        Fail,
        Throw
    }

    private sealed class FixedProof : IKeySetRotationProof
    {
        private readonly bool _anchors;
        private readonly ProofResult _result;

        public FixedProof(string name, bool anchors, ProofResult result)
        {
            Name = name;
            _anchors = anchors;
            _result = result;
        }

        public string Name { get; }

        public int Calls { get; private set; }

        public bool Anchors(PublicKeySetData current) => _anchors;

        public ISignatureVerification Verify(PublicKeySetData current, PublicKeySetData proposed, byte[] proof)
        {
            Calls++;
            if (_result == ProofResult.Throw)
            {
                throw new InvalidOperationException("proof exploded");
            }
            return new SignatureVerification { Success = _result == ProofResult.Succeed, Message = _result == ProofResult.Fail ? "no" : string.Empty, Signature = null!, IssuerPublicKey = null! };
        }
    }

    private sealed record PayloadOutcome(bool RsaMatchesCompose, bool EccBindsEccKey, bool AnchorsDiffer, bool EmptyThrows);

    private sealed record EcdsaRotationOutcome(bool OtherKeyRefused, bool OtherProposalRefused, bool RsaShapedRefused, bool EmptyRefused, bool AuthenticatorSignatureRefused, bool AssertionJsonRefused, bool UntaggedRefused, bool Unchanged, bool ValidRefused, bool Resolved);

    private sealed record WebAuthnAssertion(byte[] AuthenticatorData, byte[] ClientDataJson, byte[] Signature)
    {
        public string ToJson()
        {
            return "{\"id\":\"cred\",\"type\":\"public-key\",\"response\":{\"authenticatorData\":\"" + Base64Url(AuthenticatorData)
                + "\",\"clientDataJSON\":\"" + Base64Url(ClientDataJson) + "\",\"signature\":\"" + Base64Url(Signature) + "\"}}";
        }
    }

    private sealed record AnchoringOutcome(bool LegacyRefused, bool ChainRefused, bool MovedToEcc, bool RsaOnlyChainAnchorsEcc, bool EccProofOfRsaSetRefused);

    private sealed record RetirementOutcome(bool KeptKeyReencoded, bool OneTombstone, bool HoldsOnlyRetired, bool NamesProof, bool RetiredRefusedElsewhere, bool BackRefused, bool KeptKeyRotates, int ActiveRows, int TombstonesAfterNoOp);

    private sealed record GoverningOutcome(bool IsRotationTombstone, bool IntruderRefused, bool SuccessorHolds);

    private sealed record ChainOutcome(bool NoAnchorFails, bool Accepted, string? ProofName, int BystanderCalls, int ProofCount, bool AllFailedSucceeded, bool MessageNamesEach, bool CanRotate, bool BystanderOnlyCanRotate);

    private sealed record RegistrationOutcome(bool IsChain, List<string> DefaultNames, bool HostProofSeen);

    private sealed record SlotDropOutcome(bool RsaDropped, bool RetiredRsaRefused, bool EccDropped, int ActiveRows);
}
