using System;
using System.Security.Cryptography;
using System.Text;

namespace Collector.BL.Extentions
{
    public static class Enctypting
    {
        public static string CreateMd5(this string input)
        {
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.ASCII.GetBytes(input);
                var hashBytes = md5.ComputeHash(inputBytes);

                var sb = new StringBuilder();
                foreach (var t in hashBytes)
                {
                    sb.Append(t.ToString("X2"));
                }

                return sb.ToString();
            }
        }

        public static string Encrypt(string toEncrypt, string key, bool useHashing)
        {
            byte[] keyArray;
            var toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);

            if (useHashing)
            {
                var hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(key));

                hashmd5.Clear();
            }
            else
                keyArray = Encoding.UTF8.GetBytes(key);

            var tdes = new TripleDESCryptoServiceProvider
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            var cTransform = tdes.CreateEncryptor();
            var resultArray = cTransform.TransformFinalBlock(
                toEncryptArray, 0, toEncryptArray.Length);

            tdes.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string Decrypt(string cipherString, string key, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray;
            try
            {
                toEncryptArray = Convert.FromBase64String(cipherString);
            }
            catch (Exception)
            {
                throw new FormatException();
            }

            if (useHashing)
            {
                var hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(key));

                hashmd5.Clear();
            }
            else
                keyArray = Encoding.UTF8.GetBytes(key);

            var tdes = new TripleDESCryptoServiceProvider
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            
            var cTransform = tdes.CreateDecryptor();
            byte[] resultArray;
            try
            {
                resultArray = cTransform.TransformFinalBlock(
                    toEncryptArray, 0, toEncryptArray.Length);
            }
            catch (Exception)
            {
                throw new FormatException();
            }

            tdes.Clear();
            return Encoding.UTF8.GetString(resultArray);
        }
    }
}