using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Robot.WebApi.utils
{
    public class AESEncryptor
    {
        /// <summary>
        /// 使用任意 KEY 加密明文
        /// </summary>
        /// <param name="plainText">明文</param>
        /// <param name="password">任意字符串密码/KEY</param>
        /// <returns>Base64格式的密文（包含Salt+Nonce+Tag+Ciphertext）</returns>
        public static string Encrypt(string plainText, string password)
        {
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

            // 生成随机盐（32字节）
            byte[] salt = RandomNumberGenerator.GetBytes(32);
            // 生成随机Nonce（12字节，GCM推荐）
            byte[] nonce = RandomNumberGenerator.GetBytes(12);

            // 使用PBKDF2从密码派生32字节密钥
            byte[] key = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256).GetBytes(32);

            byte[] ciphertext = new byte[plainBytes.Length];
            byte[] tag = new byte[16]; // 128位认证标签

            using (var aes = new AesGcm(key, 16))
            {
                aes.Encrypt(nonce, plainBytes, ciphertext, tag);
            }

            // 组装：Salt + Nonce + Tag + Ciphertext
            byte[] result = new byte[salt.Length + nonce.Length + tag.Length + ciphertext.Length];
            Buffer.BlockCopy(salt, 0, result, 0, salt.Length);
            Buffer.BlockCopy(nonce, 0, result, salt.Length, nonce.Length);
            Buffer.BlockCopy(tag, 0, result, salt.Length + nonce.Length, tag.Length);
            Buffer.BlockCopy(ciphertext, 0, result, salt.Length + nonce.Length + tag.Length, ciphertext.Length);

            return Convert.ToBase64String(result);
        }

        /// <summary>
        /// 解密密文
        /// </summary>
        /// <param name="encryptedBase64">加密后的Base64字符串</param>
        /// <param name="password">加密时使用的密码</param>
        /// <returns>明文字符串</returns>
        public static string Decrypt(string encryptedBase64, string password)
        {
            byte[] fullData = Convert.FromBase64String(encryptedBase64);

            // 提取各个部分
            int saltLength = 32;
            int nonceLength = 12;
            int tagLength = 16;

            byte[] salt = new byte[saltLength];
            byte[] nonce = new byte[nonceLength];
            byte[] tag = new byte[tagLength];
            byte[] ciphertext = new byte[fullData.Length - saltLength - nonceLength - tagLength];

            Buffer.BlockCopy(fullData, 0, salt, 0, saltLength);
            Buffer.BlockCopy(fullData, saltLength, nonce, 0, nonceLength);
            Buffer.BlockCopy(fullData, saltLength + nonceLength, tag, 0, tagLength);
            Buffer.BlockCopy(fullData, saltLength + nonceLength + tagLength, ciphertext, 0, ciphertext.Length);

            // 使用相同的密码和盐派生密钥
            byte[] key = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256).GetBytes(32);

            byte[] plaintext = new byte[ciphertext.Length];

            using (var aes = new AesGcm(key, 16))
            {
                aes.Decrypt(nonce, ciphertext, tag, plaintext);
            }

            return Encoding.UTF8.GetString(plaintext);
        }
    }
}
