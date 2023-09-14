using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace User
{
    public class DataCrypt
    {
        int m_intRead = 0;
        int m_intReadTotal = 0;
        byte[] m_bytBuffer = new byte[3];
        MemoryStream m_sin = new MemoryStream();
        MemoryStream m_sout = new MemoryStream();
        SymmetricAlgorithm m_encodeMethod = new RijndaelManaged();

        public DataCrypt()
        {
        }

        public string Encrypt(string source, string key)
        {
            if (source == "" || key == "")
                return null;

            byte[] sourceData = System.Text.ASCIIEncoding.ASCII.GetBytes(source);

            m_sin.Write(sourceData, 0, sourceData.Length);
            m_sin.Position = 0;

            key = key.ToLower();
            m_encodeMethod.Key = GetValidKey(key);
            m_encodeMethod.IV = GetValidIV(key, m_encodeMethod.IV.Length);

            CryptoStream encodeStream = new CryptoStream(m_sout,
                m_encodeMethod.CreateEncryptor(), CryptoStreamMode.Write);

            m_intReadTotal = 0;
            while (m_intReadTotal < m_sin.Length)
            {
                m_intRead = m_sin.Read(m_bytBuffer, 0, m_bytBuffer.Length);
                encodeStream.Write(m_bytBuffer, 0, m_intRead);
                m_intReadTotal += m_intRead;
            }

            encodeStream.Close();

            return (Convert.ToBase64String(m_sout.ToArray()));
        }

        public string Decrypt(string source, string key)
        {
            if (source == "" || key == "")
                return null;

            byte[] encodeData = Convert.FromBase64String(source);
            m_sin = new MemoryStream(encodeData);
            m_sout = new MemoryStream();

            key = key.ToLower();
            m_encodeMethod.Key = GetValidKey(key);
            m_encodeMethod.IV = GetValidIV(key, m_encodeMethod.IV.Length);

            CryptoStream decodeStream = new CryptoStream(m_sin,
                m_encodeMethod.CreateDecryptor(), CryptoStreamMode.Read);

            m_intReadTotal = 0;
            while (m_intReadTotal < m_sin.Length)
            {
                m_intRead = decodeStream.Read(m_bytBuffer, 0, m_bytBuffer.Length);
                if (m_intRead == 0)
                    break;

                m_sout.Write(m_bytBuffer, 0, m_intRead);
                m_intReadTotal += m_intRead;
            }

            decodeStream.Close();

            return ASCIIEncoding.ASCII.GetString(m_sout.ToArray());
        }

        private byte[] GetValidKey(string key)
        {
            string tempKey = key;
            if (m_encodeMethod.LegalKeySizes.Length > 0)
            {
                int minSize = 0;
                int skipSize = m_encodeMethod.LegalKeySizes[0].MinSize;

                while (key.Length * 8 > skipSize &&
                       m_encodeMethod.LegalKeySizes[0].SkipSize > 0 &&
                       skipSize < m_encodeMethod.LegalKeySizes[0].MaxSize)
                {
                    minSize = skipSize;
                    skipSize += m_encodeMethod.LegalKeySizes[0].SkipSize;
                }

                if (key.Length * 8 > skipSize)
                    tempKey = key.Substring(0, (skipSize / 8));
                else
                    tempKey = key.PadRight(skipSize / 8, ' ');
            }

            // convert the secret key to byte array
            return ASCIIEncoding.ASCII.GetBytes(tempKey);
        }

        private byte[] GetValidIV(string initVector, int validLength)
        {
            if (initVector.Length > validLength)
                return ASCIIEncoding.ASCII.GetBytes(initVector.Substring(0, validLength));
            else
                return ASCIIEncoding.ASCII.GetBytes(initVector.PadRight(validLength, ' '));
        }
    }
}
