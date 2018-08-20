using System;
using System.Security.Cryptography;
using System.Text;

namespace Encryption
{
    class Encryption
    {
        private int _keySize;
        public int KeySize { get => this._keySize; }


        public Encryption()
        {
            this._keySize = 256;
        }


        public void SettingKeySize128()
        {
            this._keySize = 128;
        }

        public void SettingKeySize256()
        {
            this._keySize = 256;
        }

        public String EncryptStringToString(String input, String key)
        {
            Byte[] result = ProcessingEncryption(Encoding.UTF8.GetBytes(input), key, "encrypt");
            String output = Convert.ToBase64String(result);

            return output;
        }


        public String EncryptByteToString(Byte[] input, String key)
        {
            Byte[] result = ProcessingEncryption(input, key, "encrypt");
            String output = Convert.ToBase64String(result);

            return output;
        }


        public Byte[] EncryptStringToByte(String input, String key)
        {
            return ProcessingEncryption(Encoding.UTF8.GetBytes(input), key, "encrypt");
        }


        public Byte[] EncryptByteToByte(Byte[] input, String key)
        {
            return ProcessingEncryption(input, key, "encrypt");
        }


        public String DecryptStringToString(String input, String key)
        {
            Byte[] result = ProcessingEncryption(Convert.FromBase64String(input), key, "decrypt");
            String output = Encoding.UTF8.GetString(result);

            return output;
        }


        public String DecryptByteToString(Byte[] input, String key)
        {
            Byte[] result = ProcessingEncryption(input, key, "decrypt");
            String output = Encoding.UTF8.GetString(result);

            return output;
        }


        public Byte[] DecryptStringToByte(String input, String key)
        {
            return ProcessingEncryption(Convert.FromBase64String(input), key, "decrypt");
        }


        public Byte[] DecryptByteToByte(Byte[] input, String key)
        {
            return ProcessingEncryption(input, key, "decrypt");
        }


        private byte[] ProcessingEncryption(Byte[] input, String key, String type)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();

            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.PKCS7;
            rijndaelCipher.KeySize = 256;
            rijndaelCipher.BlockSize = 128;

            byte[] pwdBytes = Encoding.UTF8.GetBytes(key);
            byte[] keyBytes = new byte[16];
            int len = pwdBytes.Length;

            if (len > keyBytes.Length)
                len = keyBytes.Length;

            Array.Copy(pwdBytes, keyBytes, len);

            rijndaelCipher.Key = keyBytes;
            rijndaelCipher.IV = keyBytes;

            if (type.ToLower().Equals("encrypt"))
            {
                ICryptoTransform transform = rijndaelCipher.CreateEncryptor();
                return transform.TransformFinalBlock(input, 0, input.Length);
            }
            else
            {
                return rijndaelCipher.CreateDecryptor().TransformFinalBlock(input, 0, input.Length);
            }
        }
    }
}
