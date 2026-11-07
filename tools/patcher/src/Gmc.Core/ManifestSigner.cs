using System.Security.Cryptography;
using System.Text;

namespace Gmc.Core;

/// <summary>
/// Signs / verifies manifest.json with ECDSA P-256 (SHA-256). The private key stays on the publisher's PC;
/// the patcher only carries the public key. A valid signature is what allows the patcher to install
/// executables (.exe/.dll/.bat/...) and to replace itself - a swapped file on the web host cannot pass it.
/// Keys are stored as base64 text: private = PKCS#8, public = SubjectPublicKeyInfo.
/// </summary>
public static class ManifestSigner
{
    public static (string privateKey, string publicKey) GenerateKeyPair()
    {
        using var ec = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        return (Convert.ToBase64String(ec.ExportPkcs8PrivateKey()), Convert.ToBase64String(ec.ExportSubjectPublicKeyInfo()));
    }

    /// <summary>Returns the signature as base64 text.</summary>
    public static string Sign(byte[] data, string privateKeyBase64)
    {
        using var ec = ECDsa.Create();
        ec.ImportPkcs8PrivateKey(Convert.FromBase64String(privateKeyBase64.Trim()), out _);
        return Convert.ToBase64String(ec.SignData(data, HashAlgorithmName.SHA256));
    }

    /// <summary>False for a wrong, damaged or missing signature (never throws on bad input).</summary>
    public static bool Verify(byte[] data, string signatureBase64, string publicKeyBase64)
    {
        try
        {
            using var ec = ECDsa.Create();
            ec.ImportSubjectPublicKeyInfo(Convert.FromBase64String(publicKeyBase64.Trim()), out _);
            return ec.VerifyData(data, Convert.FromBase64String(signatureBase64.Trim()), HashAlgorithmName.SHA256);
        }
        catch (Exception ex) when (ex is FormatException or CryptographicException or ArgumentException) { return false; }
    }

    public static bool Verify(byte[] data, byte[] signatureFileBytes, string publicKeyBase64) =>
        Verify(data, Encoding.ASCII.GetString(signatureFileBytes), publicKeyBase64);
}
