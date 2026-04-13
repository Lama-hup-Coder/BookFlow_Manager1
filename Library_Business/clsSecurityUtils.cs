using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Library_Common; // استدعاء نظام التوثيق المشترك

namespace Library_Business
{
    public static class clsSecurityUtils
    {
        // إضافة حدث لتبليغ الأخطاء الأمنية للواجهات
        public static event Action<string> OnError;

        // مفتاح افتراضي للطوارئ فقط
        private static readonly string GlobalDefaultKey = "ExpertSecretKey123";

        // ==========================================
        // 1. التشفير المتماثل (Symmetric - AES)
        // ==========================================
        public static string EncryptSymmetric(string plainText, string key)
        {
            try
            {
                if (string.IsNullOrEmpty(plainText)) return "";

                byte[] keyBytes = Encoding.UTF8.GetBytes(ComputeHash(key).Substring(0, 16));

                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = keyBytes;
                    aesAlg.IV = new byte[16];

                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                _HandleError("Symmetric Encryption Failed", ex);
                return "";
            }
        }

        public static string DecryptSymmetric(string cipherText, string key)
        {
            try
            {
                if (string.IsNullOrEmpty(cipherText)) return "";
                byte[] keyBytes = Encoding.UTF8.GetBytes(ComputeHash(key).Substring(0, 16));

                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = keyBytes;
                    aesAlg.IV = new byte[16];
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    using (var msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                _HandleError("Symmetric Decryption Failed", ex);
                return "Error: Decryption Failed";
            }
        }

        // ==========================================
        // 2. الهاشينج (Hashing - SHA256)
        // ==========================================
        public static string ComputeHash(string input)
        {
            try
            {
                if (string.IsNullOrEmpty(input)) return "";
                using (SHA256 sha256Hash = SHA256.Create())
                {
                    byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < bytes.Length; i++) builder.Append(bytes[i].ToString("x2"));
                    return builder.ToString();
                }
            }
            catch (Exception ex)
            {
                _HandleError("Hashing Computation Failed", ex);
                return "";
            }
        }

        // ==========================================
        // 3. التشفير غير المتماثل (Asymmetric - RSA)
        // ==========================================

        public static void GenerateRSAKeys(out string publicKey, out string privateKey)
        {
            try
            {
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048))
                {
                    publicKey = rsa.ToXmlString(false);
                    privateKey = rsa.ToXmlString(true);
                }
                EventLogger.Log("New RSA Key Pair Generated.", "INFO", EventLogger.LogTarget.TextFile);
            }
            catch (Exception ex)
            {
                publicKey = ""; privateKey = "";
                _HandleError("RSA Key Generation Failed", ex);
            }
        }

        public static string EncryptAsymmetric(string plainText, string publicKeyXML)
        {
            try
            {
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                    rsa.FromXmlString(publicKeyXML);
                    byte[] dataToEncrypt = Encoding.UTF8.GetBytes(plainText);
                    byte[] encryptedData = rsa.Encrypt(dataToEncrypt, false);
                    return Convert.ToBase64String(encryptedData);
                }
            }
            catch (Exception ex)
            {
                _HandleError("Asymmetric Encryption Failed", ex);
                return "";
            }
        }

        public static string DecryptAsymmetric(string cipherText, string privateKeyXML)
        {
            try
            {
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                    rsa.FromXmlString(privateKeyXML);
                    byte[] dataToDecrypt = Convert.FromBase64String(cipherText);
                    byte[] decryptedData = rsa.Decrypt(dataToDecrypt, false);
                    return Encoding.UTF8.GetString(decryptedData);
                }
            }
            catch (Exception ex)
            {
                _HandleError("Asymmetric Decryption Failed", ex);
                return "";
            }
        }

        // دالة مساعدة خاصة للتعامل مع الأخطاء وتوثيقها
        private static void _HandleError(string action, Exception ex)
        {
            string errorMsg = $"Security Error during {action}: {ex.Message}";
            EventLogger.Log(errorMsg, "ERROR", EventLogger.LogTarget.Both);
            OnError?.Invoke(errorMsg);
        }
    }
}