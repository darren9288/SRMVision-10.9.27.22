using System;
using System.IO;
using System.Security.Cryptography;


namespace Common
{
    public class Crypto
    {
        /// <summary>
        /// Encrypt password using symmetric algorithm
        /// </summary>
        /// <param name="clearData">sequence of bytes</param>
        /// <param name="Key">secret key to encrypt password</param>
        /// <param name="IV">initialization vector to encrypt password in byte array format</param>
        /// <returns>encrypted password in byte array format</returns>
        public static byte[] Encrypt(byte[] clearData, byte[] Key, byte[] IV)
        {
            MemoryStream ms = new MemoryStream();

            Rijndael alg = Rijndael.Create();

            alg.Key = Key;
            alg.IV = IV;

            CryptoStream cs = new CryptoStream(ms,
               alg.CreateEncryptor(), CryptoStreamMode.Write);

            cs.Write(clearData, 0, clearData.Length);

            cs.Close();

            byte[] encryptedData = ms.ToArray();

            return encryptedData;
        }
        /// <summary>
        /// Encrypt password using PBKDF1 algorithm
        /// </summary>
        /// <param name="clearText">secret key in string format to encrypt password</param>
        /// <param name="Password">password in string format</param>
        /// <returns></returns>
        public static string Encrypt(string clearText, string Password)
        {
            byte[] clearBytes =
              System.Text.Encoding.Unicode.GetBytes(clearText);

            PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password,
                new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 
            0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x71});

            byte[] encryptedData = Encrypt(clearBytes,
                      pdb.GetBytes(32), pdb.GetBytes(16));

            return Base64Encoder.ToBase64(encryptedData);
        }
        /// <summary>
        /// Decrypt password depend on cipher text using symmetric algorithm
        /// </summary>
        /// <param name="cipherData">sequence of bytes</param>
        /// <param name="Key">secret key to decrypt password in byte array format</param>
        /// <param name="IV">initialization vector to decypt password in byte array format</param>
        /// <returns>decrypted password in byte format</returns>
        public static byte[] Decrypt(byte[] cipherData, byte[] Key, byte[] IV)
        {
            MemoryStream ms = new MemoryStream();

            Rijndael alg = Rijndael.Create();

            alg.Key = Key;
            alg.IV = IV;

            CryptoStream cs = new CryptoStream(ms,
                alg.CreateDecryptor(), CryptoStreamMode.Write);

            cs.Write(cipherData, 0, cipherData.Length);

            cs.Close();

            byte[] decryptedData = ms.ToArray();

            return decryptedData;
        }
        /// <summary>
        /// Decrypt password using PBKDF1 algorithm
        /// </summary>
        /// <param name="cipherText">key to decrypt password in string format</param>
        /// <param name="Password">encrpted password value in string format</param>
        /// <returns>decypted password in string format</returns>
        public static string Decrypt(string cipherText, string Password)
        {
            byte[] cipherBytes = Base64Encoder.FromBase64(cipherText);

            PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password,
                new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 
             0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x71});

            byte[] decryptedData = Decrypt(cipherBytes,
                pdb.GetBytes(32), pdb.GetBytes(16));

            return System.Text.Encoding.Unicode.GetString(decryptedData);
        }

    }

    class Base64Encoder
    {
        #region Member Variables
        static private char s_CharHyphen = '-';
        static private char s_CharUnderscore = '_';
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the plus sign character.
        /// Default is '+'.
        /// </summary>
        static public char CharHyphen
        {
            get
            {
                return s_CharHyphen;
            }
            set
            {
                s_CharHyphen = value;
            }
        }


        /// <summary>
        /// Gets or sets the slash character.
        /// Default is '/'.
        /// </summary>
        static public char CharUnderscore
        {
            get
            {
                return s_CharUnderscore;
            }
            set
            {
                s_CharUnderscore = value;
            }
        }
        #endregion

        /// <summary>
        /// check whether each character in string is A-Z, a-z, 0-9, =,_, -
        /// </summary>
        /// <param name="str">any value in string format</param>
        /// <returns>false = find some invalid symbol in string, true = otherwise</returns>
        static public bool IsBase64String(string str)
        {
            char[] chars = str.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                char c = chars[i];
                if (c >= 'A' && c <= 'Z')
                { }
                else if (c >= 'a' && c <= 'z')
                { }
                else if (c >= '0' && c <= '9')
                { }
                else if (c == s_CharHyphen)
                { }
                else if (c == s_CharUnderscore)
                { }
                else if (c == '=')
                { }
                else
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// convert string to byte array
        /// </summary>
        /// <param name="s">any value in string format</param>
        /// <returns>byte array representative</returns>
        static public byte[] FromBase64(string s)
        {
            int length = s == null ? 0 : s.Length;
            if (length == 0)
                return new byte[0];

            int padding = 0;
            if (length > 2 && s[length - 2] == '=')
                padding = 2;
            else if (length > 1 && s[length - 1] == '=')
                padding = 1;

            int blocks = (length - 1) / 4 + 1;
            int bytes = blocks * 3;

            byte[] data = new byte[bytes - padding];

            for (int i = 0; i < blocks; i++)
            {
                bool finalBlock = i == blocks - 1;
                bool pad2 = false;
                bool pad1 = false;
                if (finalBlock)
                {
                    pad2 = padding == 2;
                    pad1 = padding > 0;
                }

                int index = i * 4;
                byte temp1 = CharToSixBit(s[index]);
                byte temp2 = CharToSixBit(s[index + 1]);
                byte temp3 = CharToSixBit(s[index + 2]);
                byte temp4 = CharToSixBit(s[index + 3]);

                byte b = (byte)(temp1 << 2);
                byte b1 = (byte)((temp2 & 0x30) >> 4);
                b1 += b;

                b = (byte)((temp2 & 0x0F) << 4);
                byte b2 = (byte)((temp3 & 0x3C) >> 2);
                b2 += b;

                b = (byte)((temp3 & 0x03) << 6);
                byte b3 = temp4;
                b3 += b;

                index = i * 3;
                data[index] = b1;
                if (!pad2)
                    data[index + 1] = b2;
                if (!pad1)
                    data[index + 2] = b3;
            }

            return data;
        }
        /// <summary>
        /// convet byte array to string
        /// </summary>
        /// <param name="data">any value in byte array format</param>
        /// <returns>string representative</returns>
        static public string ToBase64(byte[] data)
        {
            int length = data == null ? 0 : data.Length;
            if (length == 0)
                return String.Empty;

            int padding = length % 3;
            if (padding > 0)
                padding = 3 - padding;
            int blocks = (length - 1) / 3 + 1;

            char[] s = new char[blocks * 4];

            for (int i = 0; i < blocks; i++)
            {
                bool finalBlock = i == blocks - 1;
                bool pad2 = false;
                bool pad1 = false;
                if (finalBlock)
                {
                    pad2 = padding == 2;
                    pad1 = padding > 0;
                }

                int index = i * 3;
                byte b1 = data[index];
                byte b2 = pad2 ? (byte)0 : data[index + 1];
                byte b3 = pad1 ? (byte)0 : data[index + 2];

                byte temp1 = (byte)((b1 & 0xFC) >> 2);

                byte temp = (byte)((b1 & 0x03) << 4);
                byte temp2 = (byte)((b2 & 0xF0) >> 4);
                temp2 += temp;

                temp = (byte)((b2 & 0x0F) << 2);
                byte temp3 = (byte)((b3 & 0xC0) >> 6);
                temp3 += temp;

                byte temp4 = (byte)(b3 & 0x3F);

                index = i * 4;
                s[index] = SixBitToChar(temp1);
                s[index + 1] = SixBitToChar(temp2);
                s[index + 2] = pad2 ? '=' : SixBitToChar(temp3);
                s[index + 3] = pad1 ? '=' : SixBitToChar(temp4);
            }

            return new string(s);
        }


        /// <summary>
        /// Convert character to bit
        /// </summary>
        /// <param name="b">any value in byte</param>
        /// <returns>character representative</returns>
        static private char SixBitToChar(byte b)
        {
            char c;
            if (b < 26)
            {
                c = (char)((int)b + (int)'A');
            }
            else if (b < 52)
            {
                c = (char)((int)b - 26 + (int)'a');
            }
            else if (b < 62)
            {
                c = (char)((int)b - 52 + (int)'0');
            }
            else if (b == 62)
            {
                c = s_CharHyphen;
            }
            else
            {
                c = s_CharUnderscore;
            }
            return c;
        }
        /// <summary>
        /// Convert character to byte
        /// </summary>
        /// <param name="c">any character</param>
        /// <returns>byte representative</returns>
        static private byte CharToSixBit(char c)
        {
            byte b;
            if (c >= 'A' && c <= 'Z')
            {
                b = (byte)((int)c - (int)'A');
            }
            else if (c >= 'a' && c <= 'z')
            {
                b = (byte)((int)c - (int)'a' + 26);
            }
            else if (c >= '0' && c <= '9')
            {
                b = (byte)((int)c - (int)'0' + 52);
            }
            else if (c == s_CharHyphen)
            {
                b = (byte)62;
            }
            else
            {
                b = (byte)63;
            }

            return b;
        }


    }
}
