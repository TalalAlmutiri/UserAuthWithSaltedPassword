using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace UserAuthWithSaltedPassword
{
    public class CryptographyHelper
    {
        private static string ByteToHexString(byte[] Data)
        {
            StringBuilder sBuilder = new StringBuilder();
            try
            {
                int i;
                for (i = 0; i <= Data.Length - 1; i++)
                    sBuilder.Append(Data[i].ToString("x2"));
            }
            catch (Exception ex)
            {
                EventsLogger.WriteLog(ex.ToString());
                return "";
            }
            return sBuilder.ToString();
        }

        public static string CreateSHAHashWithSalt(string Password, string Salt)
        {
            try
            {
                SHA512 sha512 = new SHA512CryptoServiceProvider();
                byte[] hash = sha512.ComputeHash(Encoding.UTF8.GetBytes(Password + Salt));
                return ByteToHexString(hash);
            }
            catch (Exception ex)
            {
                EventsLogger.WriteLog(ex.ToString());
                return "";
            }
        }

        public static string GenerateRandomSalt(int KeyLength)
        {
            try
            {
                byte[] data = new byte[KeyLength];
                using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(data);
                }

                // The length of 32 chars will be 44 according to base64 string.
                // Base64 formula to calculate output = CEILING.MATH(4*n/3)
                // for 32 chars: CEILING.MATH(4*32/3) = 43
                // if Len(output) % 4 != 0 then the base64 will add (padding) '=' until equals Len(output) % 4 = 0
                return Convert.ToBase64String(data);

            }
            catch (Exception ex)
            {
                EventsLogger.WriteLog(ex.ToString());
                return "";
            }
        }
        //128
        private static readonly string keyStr = ConfigurationManager.AppSettings["Key"];
        private static readonly string ivStr = ConfigurationManager.AppSettings["IV"];

        // AES Advanced Encryption Standard
        public static string Encrypt(string plainText)
        {
            byte[] key = ASCIIEncoding.ASCII.GetBytes(keyStr);
            byte[] iV = ASCIIEncoding.ASCII.GetBytes(ivStr);
            byte[] cipher;
            // Create a new RijndaelManaged.    
            using (RijndaelManaged aes = new RijndaelManaged())
            {
                aes.Key = key;
                aes.IV = iV;
  
                ICryptoTransform encryptorTran = aes.CreateEncryptor(aes.Key, aes.IV);
                // Create MemoryStream    
                using (MemoryStream mStream = new MemoryStream())
                {
                    // Creating a stream that links data streams to cryptographic transformations.
                    using (CryptoStream crystm = new CryptoStream(mStream, encryptorTran, CryptoStreamMode.Write))
                    {
                        // writing data to a stream    
                        using (StreamWriter sw = new StreamWriter(crystm))
                            sw.Write(plainText);
                        cipher = mStream.ToArray();
                    }
                }
            }
  
            return (Convert.ToBase64String(cipher));
        }

        internal static string Decrypt(string cipherText)
        {
            byte[] key = ASCIIEncoding.ASCII.GetBytes(keyStr);
            byte[] iV = ASCIIEncoding.ASCII.GetBytes(ivStr);
            string plaintext = null;
            using (RijndaelManaged aes = new RijndaelManaged())
            {
                aes.Key = key;
                aes.IV = iV;
                ICryptoTransform encryptorTran = aes.CreateDecryptor(aes.Key, aes.IV);
                byte[] cipher = Convert.FromBase64String(cipherText);
                using (MemoryStream mStream = new MemoryStream(cipher))
                {
                    using (CryptoStream crystm = new CryptoStream(mStream, encryptorTran, CryptoStreamMode.Read))
                    {
                        // Read  a stream    
                        using (StreamReader reader = new StreamReader(crystm))
                            plaintext = reader.ReadToEnd();
                    }
                }
            }
            return plaintext;
        }
    }
}