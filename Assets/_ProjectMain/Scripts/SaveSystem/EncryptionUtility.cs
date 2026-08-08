using UnityEngine;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System;

public static class EncryptionUtility
{
    private static readonly string encryptionKey = "7dfb1b9f328906e8454f7c6364993b1bfa88f7798f4aa47a4233fd337044ba60";
    public static string EncryptString(string plainText)
    {
        byte[] key = Encoding.UTF8.GetBytes(encryptionKey.Substring(0,32));
        using (Aes aesAlgorythm = Aes.Create())
        {
            aesAlgorythm.Key = key;
            aesAlgorythm.GenerateIV();
            ICryptoTransform encryptor = aesAlgorythm.CreateEncryptor(aesAlgorythm.Key, aesAlgorythm.IV);

            using(var msEncrypt = new MemoryStream())
            {
                msEncrypt.Write(aesAlgorythm.IV, 0, aesAlgorythm.IV.Length); //Write IV to the beggining of the stream
                using(var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                }
                return Convert.ToBase64String(msEncrypt.ToArray()); //Return encrypted data as Base64 string
            }
        }
    }

    public static string DecryptString(string cipherText)
    {
        byte[] fullCipher = Convert.FromBase64String(cipherText);
        byte[] iv = new byte[16];
        byte[] cipher = new byte[fullCipher.Length - 16];

        Array.Copy(fullCipher, iv, iv.Length);
        Array.Copy(fullCipher, 16, cipher, 0, cipher.Length);

        byte[] key = Encoding.UTF8.GetBytes(encryptionKey.Substring(0, 32));
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = key;
            aesAlg.IV = iv;
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            using(var msDecrypt = new MemoryStream(cipher))
            {
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }
}
