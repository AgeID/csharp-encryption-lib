using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace AgeID
{
	public static class AgeIDEncryptionHelper
    {
		private const int SALT_LENGTH_BYTES = 16;

        private static readonly RNGCryptoServiceProvider _random = new RNGCryptoServiceProvider();

        private static readonly Dictionary<APIVersion, int> _iterations = new Dictionary<APIVersion, int>() {
			{ APIVersion.V1, 32768 },
			{ APIVersion.V2, 1024 },
		};

        public static string Encrypt (string value, string clientSecret, string salt = null, APIVersion apiVersion = APIVersion.V2)
        {
            try
            {
                if (string.IsNullOrEmpty(clientSecret))
                    throw new Exception("The clientSecret must not be empty!");

                if (salt == null)
                {
					;
					byte[] newSaltBytes = new byte[SALT_LENGTH_BYTES];

					_random.GetNonZeroBytes(newSaltBytes);

					salt = Convert.ToBase64String(newSaltBytes);

                }
                else if (Encoding.UTF8.GetBytes(salt).Length < SALT_LENGTH_BYTES)
                    throw new Exception("The salt must be at least 16 bytes long!");

                byte[] valueBytes = Encoding.UTF8.GetBytes(value);
                byte[] clientSecretBytes = Encoding.UTF8.GetBytes(clientSecret);
                byte[] saltBytes = Encoding.UTF8.GetBytes(salt);

                var base64encryptedString = Convert.ToBase64String(AESEncryptBytes(valueBytes, clientSecretBytes, saltBytes, apiVersion));
                var base64salt = Convert.ToBase64String(saltBytes);

                var mac = ComputeMac(clientSecretBytes, Encoding.UTF8.GetBytes(base64salt), Encoding.UTF8.GetBytes(base64encryptedString));

                var payload = new Payload()
                {
                    salt = base64salt,
                    encrypted = base64encryptedString,
                    mac = mac
                };

                return Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload)));
            }
            catch (Exception ex)
            {
                throw new AgeIDEncryptionException(ex.Message);
            }
        }

        public static string Decrypt (string value, string clientSecret, APIVersion apiVersion = APIVersion.V2)
        {
            try
            {
                Payload _payload;
                try
                {
                    _payload = JsonConvert.DeserializeObject<Payload>(Encoding.UTF8.GetString(Convert.FromBase64String(value)));
                }
                catch { throw new Exception("The payload is invalid."); }

                if (string.IsNullOrEmpty(_payload.salt))
                    throw new Exception("The payload is invalid.");

                var base64SaltString = _payload.salt;
                var saltString = Encoding.UTF8.GetString(Convert.FromBase64String(base64SaltString));

                if (saltString.Length < 8)
                    throw new Exception("The payload is invalid.");

                var base64EncryptedString = _payload.encrypted;

                byte[] cryptBytes = Convert.FromBase64String(base64EncryptedString);
                byte[] clientSecretBytes = Encoding.UTF8.GetBytes(clientSecret);
                byte[] saltBytes = Encoding.UTF8.GetBytes(saltString);

                var computedMac = ComputeMac(clientSecretBytes, Encoding.UTF8.GetBytes(base64SaltString), Encoding.UTF8.GetBytes(base64EncryptedString));
                if (!string.Equals(computedMac, _payload.mac))
                    throw new Exception("The MAC is invalid.");

                return Encoding.UTF8.GetString(AESDecryptBytes(cryptBytes, clientSecretBytes, saltBytes, apiVersion));
            }
            catch (Exception ex)
            {
                throw new AgeIDEncryptionException(ex.Message);
            }
        }

        private static byte[] AESDecryptBytes (byte[] cryptBytes, byte[] passBytes, byte[] saltBytes, APIVersion apiVersion)
        {
            byte[] clearBytes = null;
            var key = new Rfc2898DeriveBytes(passBytes, saltBytes, _iterations[apiVersion]);

            using (Aes aes = new AesManaged())
            {
                aes.KeySize = 256;
                aes.Key = key.GetBytes(aes.KeySize / 8);
                aes.IV = key.GetBytes(aes.BlockSize / 8);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cryptBytes, 0, cryptBytes.Length);
                        cs.Close();
                    }
                    clearBytes = ms.ToArray();
                }
            }
            return clearBytes;
        }

        private static byte[] AESEncryptBytes (byte[] clearBytes, byte[] passBytes, byte[] saltBytes, APIVersion apiVersion)
        {
            byte[] encryptedBytes = null;
            var key = new Rfc2898DeriveBytes(passBytes, saltBytes, _iterations[apiVersion]);

            using (Aes aes = new AesManaged())
            {
                aes.KeySize = 256;
                aes.Key = key.GetBytes(aes.KeySize / 8);
                aes.IV = key.GetBytes(aes.BlockSize / 8);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }
            return encryptedBytes;
        }

        private static string ComputeMac (byte[] clientSecretBytes, byte[] base64SaltBytes, byte[] base64EncryptedBytes = null)
        {
            var byteConcat = new byte[base64SaltBytes.Length + base64EncryptedBytes.Length];
            base64SaltBytes.CopyTo(byteConcat, 0);
            base64EncryptedBytes.CopyTo(byteConcat, base64SaltBytes.Length);

            byte[] hashedArray;
            using (var hmacsha256 = new HMACSHA256(clientSecretBytes))
            {
                hmacsha256.ComputeHash(byteConcat);
                hashedArray = hmacsha256.Hash;
            }

            string mac = "";
            for (int i = 0; i < hashedArray.Length; i++)
                mac += hashedArray[i].ToString("X2");
            return mac.Replace("-", string.Empty).ToLower();
        }
    }
}
