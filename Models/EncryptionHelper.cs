using System.Security.Cryptography;
using System.Text;

namespace Swidge.Models;

public static class EncryptionHelper
{
    private const string KeySeed = "Swidge-2026-Card-Security-Key!";

    public static string Encrypt(string? plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return string.Empty;

        using var aes = Aes.Create();
        var key = SHA256.HashData(Encoding.UTF8.GetBytes(KeySeed));
        aes.Key = key;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        var combined = new byte[aes.IV.Length + encryptedBytes.Length];
        Buffer.BlockCopy(aes.IV, 0, combined, 0, aes.IV.Length);
        Buffer.BlockCopy(encryptedBytes, 0, combined, aes.IV.Length, encryptedBytes.Length);

        return Convert.ToBase64String(combined);
    }

    public static string Decrypt(string? cipherText)
    {
        if (string.IsNullOrWhiteSpace(cipherText))
            return string.Empty;

        try
        {
            var fullCipher = Convert.FromBase64String(cipherText);
            using var aes = Aes.Create();
            var key = SHA256.HashData(Encoding.UTF8.GetBytes(KeySeed));
            aes.Key = key;

            var ivLength = aes.BlockSize / 8;
            var iv = new byte[ivLength];
            var encrypted = new byte[fullCipher.Length - ivLength];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, ivLength);
            Buffer.BlockCopy(fullCipher, ivLength, encrypted, 0, encrypted.Length);

            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            var decryptedBytes = decryptor.TransformFinalBlock(encrypted, 0, encrypted.Length);
            return Encoding.UTF8.GetString(decryptedBytes);
        }
        catch
        {
            return cipherText;
        }
    }
}
