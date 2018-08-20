using System;

namespace Encryption
{
    public static class AES
    {
        private static Encryption _encryption = new Encryption();


        public static int GetKeySize()
        {
            return _encryption.KeySize;
        }


        public static void ChangeKeySize(int keySize)
        {
            if (keySize <= 128)
                _encryption.SettingKeySize128();
            else
                _encryption.SettingKeySize256();
        }

        public static String EncryptStringToString(String input, String key)
        {
            return _encryption.EncryptStringToString(input, key);
        }

        public static String EncryptByteToString(Byte[] input, String key)
        {
            return _encryption.EncryptByteToString(input, key);
        }

        public static Byte[] EncryptStringToByte(String input, String key)
        {
            return _encryption.EncryptStringToByte(input, key);
        }

        public static Byte[] EncryptByteToByte(Byte[] input, String key)
        {
            return _encryption.EncryptByteToByte(input, key);
        }


        public static String DecryptStringToString(String input, String key)
        {
            return _encryption.DecryptStringToString(input, key);
        }

        public static String DecryptByteToString(Byte[] input, String key)
        {
            return _encryption.DecryptByteToString(input, key);
        }

        public static Byte[] DecryptStringToByte(String input, String key)
        {
            return _encryption.DecryptStringToByte(input, key);
        }

        public static Byte[] DecryptByteToByte(Byte[] input, String key)
        {
            return _encryption.DecryptByteToByte(input, key);
        }
    }
}
