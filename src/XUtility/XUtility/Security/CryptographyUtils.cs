// Copyright (c) junjie sun. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace XUtility.Security
{
    /// <summary>
    /// 加密工具类
    /// </summary>
    public static class CryptographyUtils
    {
        #region MD5

        /// <summary>
        /// MD5 加密
        /// </summary>
        /// <param name="original">The input.</param>
        /// <returns></returns>
        public static string MD5Encrypt(string original)
        {
            byte[] hash = null;
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(original);
                hash = md5.ComputeHash(inputBytes);
            }

            if (hash == null)
            {
                return null;
            }

            return GetHashString(hash);
        }

        /// <summary>
        /// 获取指定文件的MD5哈希值
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>输出Hash值</returns>
        public static string GetFileMD5Hash(string filePath)
        {
            using (MD5 md5 = MD5.Create())
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    return BitConverter.ToString(md5.ComputeHash(fs)).Replace("-", "");
                }
            }
        }

        #endregion

        #region SHA1

        /// <summary>
        /// SHA1 加密
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public static string SHA1Encrypt(string original)
        {
            byte[] hash = null;
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(original);
                hash = sha1.ComputeHash(inputBytes);
            }

            if (hash == null)
            {
                return null;
            }

            return GetHashString(hash);
        }

        /// <summary>
        /// 获取指定文件的SHA1哈希值
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>输出Hash值</returns>
        public static string GetFileSHA1Hash(string filePath)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    return BitConverter.ToString(sha1.ComputeHash(fs)).Replace("-", "");
                }
            }
        }

        #endregion

        #region SHA256

        /// <summary>
        /// SHA256 加密
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public static string SHA256Encrypt(string original)
        {
            byte[] hash = null;
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(original);
                hash = sha256.ComputeHash(inputBytes);
            }

            if (hash == null)
            {
                return null;
            }

            return GetHashString(hash);
        }

        #endregion

        #region SHA384

        /// <summary>
        /// SHA384 加密
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public static string SHA384Encrypt(string original)
        {
            byte[] hash = null;
            using (SHA384 sha384 = SHA384.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(original);
                hash = sha384.ComputeHash(inputBytes);
            }

            if (hash == null)
            {
                return null;
            }

            return GetHashString(hash);
        }

        #endregion

        #region SHA512

        /// <summary>
        /// SHA512 加密
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public static string SHA512Encrypt(string original)
        {
            byte[] hash = null;
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(original);
                hash = sha512.ComputeHash(inputBytes);
            }

            if (hash == null)
            {
                return null;
            }

            return GetHashString(hash);
        }

        #endregion

        #region AES

        private static byte[] aesSlat = new byte[] { 0x53, 0x6e, 0x64, 0x61, 0x20, 0x43, 0x52, 0x4d, 0x20, 0x58, 0x75, 0x61, 0x6e, 0x79, 0x65, 0xB3 };
        
        /// <summary>
        /// AES 加密
        /// </summary>
        /// <param name="original"></param>
        /// <param name="password"></param>
        /// <param name="slat"></param>
        /// <returns></returns>
        public static string AESEncrypt(string original, string password, byte[] slat = null)
        {
            slat = slat ?? aesSlat;
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, slat, 1024);
            // Encrypt the string to an array of bytes.
            byte[] encrypted = AESEncryptStringToBytes(original, pdb.GetBytes(32), pdb.GetBytes(16));
            return Convert.ToBase64String(encrypted, 0, encrypted.Length);
        }

        /// <summary>
        /// AES 解密
        /// </summary>
        /// <param name="encryptText">To decrypt.</param>
        /// <param name="password">The password.</param>
        /// <param name="slat"></param>
        /// <returns></returns>
        public static string AESDecrypt(string encryptText, string password, byte[] slat = null)
        {
            slat = slat ?? aesSlat;
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, slat, 1024);
            byte[] toEncryptArray = Convert.FromBase64String(encryptText);
            return AESDecryptStringFromBytes(toEncryptArray, pdb.GetBytes(32), pdb.GetBytes(16));
        }

        private static byte[] AESEncryptStringToBytes(string original, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (original == null || original.Length <= 0)
                throw new ArgumentNullException("No Original");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("No Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("No IV");
            byte[] encrypted;
            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(original);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        private static string AESDecryptStringFromBytes(byte[] encryptText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (encryptText == null || encryptText.Length <= 0)
                throw new ArgumentNullException("No EncryptText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("No Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("No IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(encryptText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;
        }

        #endregion

        #region RSA

        /// <summary>
        /// RSA 加密
        /// </summary>
        /// <param name="original"></param>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public static string RSAEncrypt(string original, string publicKey)
        {
            RSA rsa = CreateRsaFromPublicKey(publicKey);
            var originalBytes = Encoding.UTF8.GetBytes(original);
            var cipherBytes = rsa.Encrypt(originalBytes, RSAEncryptionPadding.Pkcs1);
            return Convert.ToBase64String(cipherBytes);
        }

        /// <summary>
        /// RSA 解密
        /// </summary>
        /// <param name="encryptText"></param>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public static string RSADecrypt(string encryptText, string privateKey)
        {
            RSA rsa = CreateRsaFromPrivateKey(privateKey);
            var cipherBytes = System.Convert.FromBase64String(encryptText);
            var plainTextBytes = rsa.Decrypt(cipherBytes, RSAEncryptionPadding.Pkcs1);
            return Encoding.UTF8.GetString(plainTextBytes);
        }

        private static RSA CreateRsaFromPublicKey(string publicKeyString)
        {
            byte[] SeqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
            byte[] x509key;
            byte[] seq = new byte[15];
            int x509size;

            x509key = Convert.FromBase64String(publicKeyString);
            x509size = x509key.Length;

            using (var mem = new MemoryStream(x509key))
            {
                using (var binr = new BinaryReader(mem))
                {
                    byte bt = 0;
                    ushort twobytes = 0;

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8130)
                        binr.ReadByte();
                    else if (twobytes == 0x8230)
                        binr.ReadInt16();
                    else
                        return null;

                    seq = binr.ReadBytes(15);
                    if (!CompareBytearrays(seq, SeqOID))
                        return null;

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8103)
                        binr.ReadByte();
                    else if (twobytes == 0x8203)
                        binr.ReadInt16();
                    else
                        return null;

                    bt = binr.ReadByte();
                    if (bt != 0x00)
                        return null;

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8130)
                        binr.ReadByte();
                    else if (twobytes == 0x8230)
                        binr.ReadInt16();
                    else
                        return null;

                    twobytes = binr.ReadUInt16();
                    byte lowbyte = 0x00;
                    byte highbyte = 0x00;

                    if (twobytes == 0x8102)
                        lowbyte = binr.ReadByte();
                    else if (twobytes == 0x8202)
                    {
                        highbyte = binr.ReadByte();
                        lowbyte = binr.ReadByte();
                    }
                    else
                        return null;
                    byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                    int modsize = BitConverter.ToInt32(modint, 0);

                    int firstbyte = binr.PeekChar();
                    if (firstbyte == 0x00)
                    {
                        binr.ReadByte();
                        modsize -= 1;
                    }

                    byte[] modulus = binr.ReadBytes(modsize);

                    if (binr.ReadByte() != 0x02)
                        return null;
                    int expbytes = (int)binr.ReadByte();
                    byte[] exponent = binr.ReadBytes(expbytes);

                    var rsa = RSA.Create();
                    var rsaKeyInfo = new RSAParameters
                    {
                        Modulus = modulus,
                        Exponent = exponent
                    };
                    rsa.ImportParameters(rsaKeyInfo);
                    return rsa;
                }

            }
        }

        private static RSA CreateRsaFromPrivateKey(string privateKey)
        {
            var privateKeyBits = System.Convert.FromBase64String(privateKey);
            var rsa = RSA.Create();
            var RSAparams = new RSAParameters();

            using (var binr = new BinaryReader(new MemoryStream(privateKeyBits)))
            {
                byte bt = 0;
                ushort twobytes = 0;
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)
                    binr.ReadByte();
                else if (twobytes == 0x8230)
                    binr.ReadInt16();
                else
                    throw new Exception("Unexpected value read binr.ReadUInt16()");

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102)
                    throw new Exception("Unexpected version");

                bt = binr.ReadByte();
                if (bt != 0x00)
                    throw new Exception("Unexpected value read binr.ReadByte()");

                RSAparams.Modulus = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.Exponent = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.D = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.P = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.Q = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.DP = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.DQ = binr.ReadBytes(GetIntegerSize(binr));
                RSAparams.InverseQ = binr.ReadBytes(GetIntegerSize(binr));
            }

            rsa.ImportParameters(RSAparams);
            return rsa;
        }

        private static int GetIntegerSize(BinaryReader binr)
        {
            byte bt = 0;
            byte lowbyte = 0x00;
            byte highbyte = 0x00;
            int count = 0;
            bt = binr.ReadByte();
            if (bt != 0x02)
                return 0;
            bt = binr.ReadByte();

            if (bt == 0x81)
                count = binr.ReadByte();
            else
                if (bt == 0x82)
            {
                highbyte = binr.ReadByte();
                lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;
            }

            while (binr.ReadByte() == 0x00)
            {
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);
            return count;
        }

        private static bool CompareBytearrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;
            int i = 0;
            foreach (byte c in a)
            {
                if (c != b[i])
                    return false;
                i++;
            }
            return true;
        }

    #endregion

        #region Mask

        /// <summary>
        /// 对手机号码进行掩码处理
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static string PhoneMask(string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                return phone;
            }
            return phone.Substring(0, 3) + "****" + phone.Substring(7);
        }

        /// <summary>
        /// 对姓名进行掩码处理
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public static string FullNameMask(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
            {
                return fullName;
            }
            return Regex.Replace(fullName, @"(?!^)(.{1})", "*");
        }

        #endregion

        #region 私有方法

        private static string GetHashString(byte[] hash)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return sb.ToString();
        }

        #endregion
    }
}
