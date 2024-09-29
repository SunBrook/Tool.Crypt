/*
 * @author: S 2024/9/29 11:45:19
 */

using System.Security.Cryptography;
using System.Text;

namespace Tool.Crypt
{
    /// <summary>
    /// 加解密方法
    /// </summary>
    public class MainCrypt
    {
        /// <summary>
        /// 加密方法
        /// </summary>
        /// <param name="plainText">需要加密数据源</param>
        /// <param name="key">must length > 24 </param>
        /// <returns></returns>
        public static string Encrypt(string plainText, string key)
        {
            try
            {
                byte[] iv = new byte[16];
                byte[] array;

                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(key);
                    aes.IV = iv;

                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                            {
                                streamWriter.Write(plainText);
                            }

                            array = memoryStream.ToArray();
                        }
                    }
                }

                return Convert.ToBase64String(array);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 解密方法
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="key">must length >24 </param>
        /// <returns></returns>
        public static string Decrypt(string cipherText, string key)
        {
            try
            {
                byte[] iv = new byte[16];
                byte[] buffer = Convert.FromBase64String(cipherText);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(key);
                    aes.IV = iv;

                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (MemoryStream memoryStream = new MemoryStream(buffer))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                            {
                                return streamReader.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
