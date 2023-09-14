using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using IOEx;
using System.IO;
using System.Text;
using Microsoft.Win32;
using System.Security.Cryptography;

namespace ActivationForm
{
    [RunInstaller(true)]
    public partial class ActiavateHD : Installer
    {
        #region Variable Members
        private DriveListEx m_ListHDD = new DriveListEx();
        private const string strDriverSerial = "2083141790";
        private string strHDDSerial;

        #endregion

        public ActiavateHD()
        {
            InitializeComponent();

            if (this.m_ListHDD.Load() > 0)
            {
                string strHardiskKey = DataCrypt.Encrypt(this.strHDDSerial = this.m_ListHDD[0].SerialNumber, "SVG11888");
                RegistryKey Key = Registry.LocalMachine.CreateSubKey(@"Software\SVG");
                Key.SetValue("HDDSerial", strHardiskKey, RegistryValueKind.String);
            }
        }
    }

    internal class Base64Encoder
    {
        #region Variables Members

        private static char s_CharHyphen = '-';
        private static char s_CharUnderscore = '_';

        #endregion

        #region Properties
        public static char CharHyphen
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

        public static char CharUnderscore
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
        /// convert character to byte in six bits
        /// </summary>
        /// <param name="c">character</param>
        /// <returns>byte which represents that character</returns>
        private static byte CharToSixBit(char c)
        {
            if ((c >= 'A') && (c <= 'Z'))
            {
                return (byte)(c - 'A');
            }
            if ((c >= 'a') && (c <= 'z'))
            {
                return (byte)((c - 'a') + 0x1a);
            }
            if ((c >= '0') && (c <= '9'))
            {
                return (byte)((c - '0') + 0x34);
            }
            if (c == s_CharHyphen)
            {
                return 0x3e;
            }
            return 0x3f;
        }
        /// <summary>
        /// convert string to representative in base 64 bits
        /// </summary>
        /// <param name="s">any value in string format</param>
        /// <returns>representative value in 64-bits</returns>
        public static byte[] FromBase64(string s)
        {
            int length = (s == null) ? 0 : s.Length;
            if (length == 0)
            {
                return new byte[0];
            }
            int padding = 0;
            if ((length > 2) && (s[length - 2] == '='))
            {
                padding = 2;
            }
            else if ((length > 1) && (s[length - 1] == '='))
            {
                padding = 1;
            }
            int blocks = ((length - 1) / 4) + 1;
            int bytes = blocks * 3;
            byte[] data = new byte[bytes - padding];
            for (int i = 0; i < blocks; i++)
            {
                bool finalBlock = i == (blocks - 1);
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
                b1 = (byte)(b1 + b);
                b = (byte)((temp2 & 15) << 4);
                byte b2 = (byte)((temp3 & 60) >> 2);
                b2 = (byte)(b2 + b);
                b = (byte)((temp3 & 3) << 6);
                byte b3 = temp4;
                b3 = (byte)(b3 + b);
                index = i * 3;
                data[index] = b1;
                if (!pad2)
                {
                    data[index + 1] = b2;
                }
                if (!pad1)
                {
                    data[index + 2] = b3;
                }
            }
            return data;
        }
        /// <summary>
        /// check whether each character is in string is A-Z, a-z, 0-9, =, _
        /// </summary>
        /// <param name="str">any value in string format</param>
        /// <returns>false = there is other character than above, true = otherwise</returns>
        public static bool IsBase64String(string str)
        {
            foreach (char c in str.ToCharArray())
            {
                if ((((c < 'A') || (c > 'Z')) && ((c < 'a') || (c > 'z'))) && (((c < '0') || (c > '9')) && (((c != s_CharHyphen) && (c != s_CharUnderscore)) && (c != '='))))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// convert bytes to character
        /// </summary>
        /// <param name="b">any value in byte</param>
        /// <returns>character</returns>
        private static char SixBitToChar(byte b)
        {
            if (b < 0x1a)
            {
                return (char)(b + 0x41);
            }
            if (b < 0x34)
            {
                return (char)((b - 0x1a) + 0x61);
            }
            if (b < 0x3e)
            {
                return (char)((b - 0x34) + 0x30);
            }
            if (b == 0x3e)
            {
                return s_CharHyphen;
            }
            return s_CharUnderscore;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ToBase64(byte[] data)
        {
            int length = (data == null) ? 0 : data.Length;
            if (length == 0)
            {
                return string.Empty;
            }
            int padding = length % 3;
            if (padding > 0)
            {
                padding = 3 - padding;
            }
            int blocks = ((length - 1) / 3) + 1;
            char[] s = new char[blocks * 4];
            for (int i = 0; i < blocks; i++)
            {
                bool finalBlock = i == (blocks - 1);
                bool pad2 = false;
                bool pad1 = false;
                if (finalBlock)
                {
                    pad2 = padding == 2;
                    pad1 = padding > 0;
                }
                int index = i * 3;
                byte b1 = data[index];
                byte b2 = pad2 ? ((byte)0) : data[index + 1];
                byte b3 = pad1 ? ((byte)0) : data[index + 2];
                byte temp1 = (byte)((b1 & 0xfc) >> 2);
                byte temp = (byte)((b1 & 3) << 4);
                byte temp2 = (byte)((b2 & 240) >> 4);
                temp2 = (byte)(temp2 + temp);
                temp = (byte)((b2 & 15) << 2);
                byte temp3 = (byte)((b3 & 0xc0) >> 6);
                temp3 = (byte)(temp3 + temp);
                byte temp4 = (byte)(b3 & 0x3f);
                index = i * 4;
                s[index] = SixBitToChar(temp1);
                s[index + 1] = SixBitToChar(temp2);
                s[index + 2] = pad2 ? '=' : SixBitToChar(temp3);
                s[index + 3] = pad1 ? '=' : SixBitToChar(temp4);
            }
            return new string(s);
        }


    }

    internal class DataCrypt
    {
        /// <summary>
        /// Decrypt password using PBKDF1 algorithm
        /// </summary>
        /// <param name="cipherText">key to decrypt password in string format</param>
        /// <param name="Password">encrpted password value in string format</param>
        /// <returns>decypted password in string format</returns>
        public static string Decrypt(string cipherText, string Password)
        {
            byte[] cipherBytes = Base64Encoder.FromBase64(cipherText);
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password, new byte[] { 0x49, 0x76, 0x61, 110, 0x20, 0x4d, 0x65, 100, 0x76, 0x65, 100, 0x65, 0x71 });
            byte[] decryptedData = Decrypt(cipherBytes, pdb.GetBytes(0x20), pdb.GetBytes(0x10));
            return Encoding.Unicode.GetString(decryptedData);
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
            CryptoStream cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(cipherData, 0, cipherData.Length);
            cs.Close();
            return ms.ToArray();
        }

        /// <summary>
        /// Encrypt password using PBKDF1 algorithm
        /// </summary>
        /// <param name="clearText">secret key in string format to encrypt password</param>
        /// <param name="Password">password in string format</param>
        /// <returns></returns>
        public static string Encrypt(string clearText, string Password)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password, new byte[] { 0x49, 0x76, 0x61, 110, 0x20, 0x4d, 0x65, 100, 0x76, 0x65, 100, 0x65, 0x71 });
            return Base64Encoder.ToBase64(Encrypt(clearBytes, pdb.GetBytes(0x20), pdb.GetBytes(0x10)));
        }
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
            CryptoStream cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(clearData, 0, clearData.Length);
            cs.Close();
            return ms.ToArray();
        }
    }

}