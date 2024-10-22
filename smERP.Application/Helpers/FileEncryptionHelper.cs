using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;

namespace smERP.Application.Helpers;

public class FileEncryptionHelper(IConfiguration configuration)
{
    private readonly string _encryptionKey = configuration["EncryptionKey"];

    public async Task<byte[]> EncryptAsync(byte[] data)
    {
        using var aes = Aes.Create();
        aes.Key = Convert.FromBase64String(_encryptionKey);
        aes.GenerateIV();

        using var memoryStream = new MemoryStream();
        await memoryStream.WriteAsync(aes.IV, 0, aes.IV.Length);

        using (var cryptoStream = new CryptoStream(
            memoryStream,
            aes.CreateEncryptor(),
            CryptoStreamMode.Write))
        {
            await cryptoStream.WriteAsync(data, 0, data.Length);
        }

        return memoryStream.ToArray();
    }

    public async Task<byte[]> DecryptAsync(byte[] encryptedData)
    {
        using var aes = Aes.Create();
        aes.Key = Convert.FromBase64String(_encryptionKey);

        var iv = new byte[aes.IV.Length];
        Array.Copy(encryptedData, iv, iv.Length);
        aes.IV = iv;

        using var memoryStream = new MemoryStream();
        using (var cryptoStream = new CryptoStream(
            new MemoryStream(encryptedData, iv.Length, encryptedData.Length - iv.Length),
            aes.CreateDecryptor(),
            CryptoStreamMode.Read))
        {
            await cryptoStream.CopyToAsync(memoryStream);
        }

        return memoryStream.ToArray();
    }
}
