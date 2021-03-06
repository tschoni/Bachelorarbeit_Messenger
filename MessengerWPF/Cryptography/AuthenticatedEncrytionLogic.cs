using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;


namespace MessengerWPF.Cryptography
{
    public static class AuthenticatedEncrytionLogic
    {     
        
        public static readonly int BlockBitSize = 128;
        public static readonly int KeyBitSize = 256;
        
        /// <summary>
        /// Encrypt messageContent with AES256-CBC-PKCS7 and authenticate with HMAC-SHA256
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] Encrypt(string plainText, byte[] key)
        {          
            if (string.IsNullOrEmpty(plainText))
            {
                throw new ArgumentException("Secret Message Required!", nameof(plainText));
            }

            var plainBytes = Encoding.UTF8.GetBytes(plainText);

            if (key == null || key.Length != KeyBitSize / 8)
            {
                throw new ArgumentException($"Key needs to be {KeyBitSize} bit!", nameof(key));
            }

            if (plainBytes == null || plainBytes.Length < 1)
            {
                throw new ArgumentException("Secret Message Required!", nameof(plainBytes));
            }

            byte[] cipherText;
            byte[] iv;

            using (var aes = CreateAes())
            {
                // Use random IV
                aes.GenerateIV();
                iv = aes.IV;

                using (var encrypter = aes.CreateEncryptor(key, iv))
                {
                    using (var cipherStream = new MemoryStream())
                    {
                        using (var cryptoStream = new CryptoStream(cipherStream, encrypter, CryptoStreamMode.Write))
                        {
                            using (var binaryWriter = new BinaryWriter(cryptoStream))
                            {
                                binaryWriter.Write(plainBytes);
                            }
                        }

                        cipherText = cipherStream.ToArray();
                    }
                }
            }

            // Assemble encrypted message and add authentication
            using (var hmac = new HMACSHA256(key))
            {
                using (var encryptedStream = new MemoryStream())
                {
                    using (var binaryWriter = new BinaryWriter(encryptedStream))
                    {
                        // Prepend IV
                        binaryWriter.Write(iv);

                        // Write Ciphertext
                        binaryWriter.Write(cipherText);
                        binaryWriter.Flush();

                        // Authenticate all data
                        var tag = hmac.ComputeHash(encryptedStream.ToArray());

                        // Postpend tag
                        binaryWriter.Write(tag);
                    }

                    return encryptedStream.ToArray();
                }
            }
        }


        /// <summary>
        /// Decrypts and authenticates the encrypted message.
        /// </summary>
        /// <param name="encryptedMessage"></param>
        /// <param name="key"></param>
        /// <returns>plaintext of serialized message content object</returns>
        public static string Decrypt(byte[] encryptedMessage, byte[] key)
        {
            if (key == null || key.Length != KeyBitSize / 8)
            {
                throw new ArgumentException($"CryptKey needs to be {KeyBitSize} bit!", nameof(key));
            }

            if (encryptedMessage == null || encryptedMessage.Length == 0)
            {
                throw new ArgumentException("Encrypted Message Required!", nameof(encryptedMessage));
            }

            using (var hmac = new HMACSHA256(key))
            {
                var sentTag = new byte[hmac.HashSize / 8];

                var calcTag = hmac.ComputeHash(encryptedMessage, 0, encryptedMessage.Length - sentTag.Length);
                var ivLength = BlockBitSize / 8;

                if (encryptedMessage.Length < sentTag.Length + ivLength)
                {
                    return null;
                }
                
                Array.Copy(encryptedMessage, encryptedMessage.Length - sentTag.Length, sentTag, 0, sentTag.Length);

                // Compare Tag with constant time comparison
                var compare = 0;
                for (var i = 0; i < sentTag.Length; i++)
                {
                    compare |= sentTag[i] ^ calcTag[i];
                }

                // If message doesn't authenticate return null
                if (compare != 0)
                {
                    return null;
                }

                using (var aes = CreateAes())
                {
                    // Grab IV from message
                    var iv = new byte[ivLength];
                    Array.Copy(encryptedMessage, 0, iv, 0, iv.Length);

                    using (var decrypter = aes.CreateDecryptor(key, iv))
                    {
                        using (var plainTextStream = new MemoryStream())
                        {
                            using (var decrypterStream = new CryptoStream(plainTextStream, decrypter, CryptoStreamMode.Write))
                            {
                                using (var binaryWriter = new BinaryWriter(decrypterStream))
                                {
                                    binaryWriter.Write(
                                        encryptedMessage,
                                        iv.Length,
                                        encryptedMessage.Length - iv.Length - sentTag.Length
                                    );
                                }
                            }
                            var plainText = plainTextStream.ToArray();
                            return plainText == null ? null : Encoding.UTF8.GetString(plainText);
                        }
                    }
                }
            }
           
        }

        private static Aes CreateAes()
        {
            var aes = Aes.Create();
            aes.KeySize = KeyBitSize;
            aes.BlockSize = BlockBitSize;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            return aes;
        }
    }
}
/*
		private static readonly RandomNumberGenerator Random = RandomNumberGenerator.Create();

		//Preconfigured Encryption Parameters
		public static readonly int BlockBitSize = 128;
		public static readonly int KeyBitSize = 256;

		/// <summary>
		/// Helper that generates a random key on each call.
		/// </summary>
		/// <returns></returns>
		public static byte[] NewKey()
		{
			var key = new byte[KeyBitSize / 8];
			Random.GetBytes(key);
			return key;
		}

		/// <summary>
		/// Simple Encryption(AES) then Authentication (HMAC) for a UTF8 Message.
		/// </summary>
		/// <param name="secretMessage">The secret message.</param>
		/// <param name="cryptKey">The crypt key.</param>
		/// <param name="nonSecretPayload">(Optional) Non-Secret Payload.</param>
		/// <returns>
		/// Encrypted Message
		/// </returns>
		/// <remarks>
		/// Adds overhead of (Optional-Payload + BlockSize(16) + Message-Padded-To-Blocksize +  HMac-Tag(32)) * 1.33 Base64
		/// </remarks>
		public static byte[] SimpleEncrypt(byte[] secretMessage, byte[] cryptKey, byte[] nonSecretPayload = null)
		{
			//User Error Checks
			if (cryptKey == null || cryptKey.Length != KeyBitSize / 8)
				throw new ArgumentException(String.Format("Key needs to be {0} bit!", KeyBitSize), "cryptKey");

			if (secretMessage == null || secretMessage.Length < 1)
				throw new ArgumentException("Secret Message Required!", "secretMessage");

			//non-secret payload optional
			nonSecretPayload = nonSecretPayload ?? new byte[] { };

			byte[] cipherText;
			byte[] iv;

			using (var aes = new AesManaged
			{
				KeySize = KeyBitSize,
				BlockSize = BlockBitSize,
				Mode = CipherMode.CBC,
				Padding = PaddingMode.PKCS7
			})
			{
				//Use random IV
				aes.GenerateIV();
				iv = aes.IV;

				using (var encrypter = aes.CreateEncryptor(cryptKey, iv))
				using (var cipherStream = new MemoryStream())
				{
					using (var cryptoStream = new CryptoStream(cipherStream, encrypter, CryptoStreamMode.Write))
					using (var binaryWriter = new BinaryWriter(cryptoStream))
					{
						//Encrypt Data
						binaryWriter.Write(secretMessage);
					}

					cipherText = cipherStream.ToArray();
				}
			}

			using (var messageStream = new MemoryStream())
			{
				using (var binaryWriter = new BinaryWriter(messageStream))
				{
					//Prepend non-secret payload if any
					binaryWriter.Write(nonSecretPayload);

					//Prepend IV
					binaryWriter.Write(iv);

					//Write Ciphertext
					binaryWriter.Write(cipherText);
				}

				return messageStream.ToArray();
			}
		}

		/// <summary>
		/// Simple Authentication (HMAC) then Decryption (AES) for a secrets UTF8 Message.
		/// </summary>
		/// <param name="encryptedMessage">The encrypted message.</param>
		/// <param name="cryptKey">The crypt key.</param>
		/// <param name="nonSecretPayloadLength">Length of the non secret payload.</param>
		/// <returns>Decrypted Message</returns>
		public static byte[] SimpleDecrypt(byte[] encryptedMessage, byte[] cryptKey, int nonSecretPayloadLength = 0)
		{
			//Basic Usage Error Checks
			if (cryptKey == null || cryptKey.Length != KeyBitSize / 8)
				throw new ArgumentException(String.Format("CryptKey needs to be {0} bit!", KeyBitSize), "cryptKey");

			if (encryptedMessage == null || encryptedMessage.Length == 0)
				throw new ArgumentException("Encrypted Message Required!", "encryptedMessage");

			var ivLength = (BlockBitSize / 8);

			//if message length is to small just return null
			if (encryptedMessage.Length < nonSecretPayloadLength + ivLength)
				return null;

			using (var aes = new AesManaged
			{
				KeySize = KeyBitSize,
				BlockSize = BlockBitSize,
				Mode = CipherMode.CBC,
				Padding = PaddingMode.PKCS7
			})
			{
				//Grab IV from message
				var iv = new byte[ivLength];
				Array.Copy(encryptedMessage, nonSecretPayloadLength, iv, 0, iv.Length);

				using (var decryptor = aes.CreateDecryptor(cryptKey, iv))
				using (var plainTextStream = new MemoryStream())
				{
					using (var decryptorStream = new CryptoStream(plainTextStream, decryptor, CryptoStreamMode.Write))
					using (var binaryWriter = new BinaryWriter(decryptorStream))
					{
						//Decrypt Cipher Text from Message
						binaryWriter.Write(
							encryptedMessage,
							nonSecretPayloadLength + iv.Length,
							encryptedMessage.Length - nonSecretPayloadLength - iv.Length
						);
					}

					//Return Plain Text
					return plainTextStream.ToArray();
				}
			}
		}*/