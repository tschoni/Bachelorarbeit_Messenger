using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MessengerApiClient;
using MessengerWPF.Business;
using MessengerWPF.Models.DbModels;

namespace MessengerWPF.Cryptography
{
    public static class KeyGenerationLogic
    {
        

        private static readonly ECParameters eCParameters = new() { Curve = ECCurve.NamedCurves.nistP256};
        private static readonly int masterKeyLength = 256 / 8;

        public static Key GenerateMasterKeyAsInitiator(Key myPrivIdKey, byte[] myEphemKey, PublicKeyDTO yourSignedKey, PublicKeyDTO yourIdKey, User you)
        {
            var ecdh1 = GenerateDiffieHellmanShared(myPrivIdKey.KeyBytes, yourSignedKey.KeyBytes);
            var ecdh2 = GenerateDiffieHellmanShared(myEphemKey, yourIdKey.KeyBytes);
            var ecdh3 = GenerateDiffieHellmanShared(myEphemKey, yourSignedKey.KeyBytes);

            var combined = ecdh1.Concat(ecdh2).Concat(ecdh3);
            var masterKey = HKDF.DeriveKey(HashAlgorithmName.SHA256, combined.ToArray(), masterKeyLength);

            return new Key() { KeyType = KeyType.MasterKey, KeyBytes = masterKey, AssociatedUser = you };
        }

        public static Key GenerateMasterKeyAsReactor(Key myPrivIdKey, Key myPrivSigKey, EphemKeyDTO yourEphemKey, PublicKeyDTO yourIdKey, User you)
        {

            //var mySigCngKey = CngKey.Import(myPrivSigKey.KeyBytes, CngKeyBlobFormat.EccFullPrivateBlob);
            var ecdh1 = GenerateDiffieHellmanShared(myPrivSigKey.KeyBytes, yourIdKey.KeyBytes);
            var ecdh2 = GenerateDiffieHellmanShared(myPrivIdKey.KeyBytes, yourEphemKey.KeyBytes);
            var ecdh3 = GenerateDiffieHellmanShared(myPrivSigKey.KeyBytes, yourEphemKey.KeyBytes);

            var combined = ecdh1.Concat(ecdh2).Concat(ecdh3);
            var masterKey = HKDF.DeriveKey(HashAlgorithmName.SHA256, combined.ToArray() , masterKeyLength);

            return new Key() { KeyType = KeyType.MasterKey, KeyBytes = masterKey, AssociatedUser = you };
            //ecdh1.ImportECPrivateKey(myPrivSigKey.KeyBytes)

        }
        
        public static byte[] GenerateDiffieHellmanShared(byte[] privateKey, byte[] publicKey)
        {
            var pubKey = CngKey.Import(publicKey, CngKeyBlobFormat.EccFullPublicBlob);
            var privKey = CngKey.Import(privateKey, CngKeyBlobFormat.EccFullPrivateBlob);
            
            using (ECDiffieHellmanCng cng = new ECDiffieHellmanCng(privKey))
            {
                cng.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
                cng.HashAlgorithm = CngAlgorithm.Sha512;
                return cng.DeriveKeyMaterial(pubKey);
            }
        }

        /*
                 public static byte[] GenerateDiffieHellmanShared(byte[] privateKey, byte[] publicKey)
        {
            
            using (ECDiffieHellmanCng cng = new ECDiffieHellmanCng(CngKey.Import(privateKey, CngKeyBlobFormat.EccFullPrivateBlob)))
            {
                cng.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
                cng.HashAlgorithm = CngAlgorithm.Sha512;
                return cng.DeriveKeyMaterial(CngKey.Import(publicKey, CngKeyBlobFormat.EccFullPublicBlob));
            }
        }
         */

        public static List<Key> GenerateUserKeyList(User user)
        {
            var userKeys = new List<Key>();
            var idCngKey = CreateCngKey();
            var idKeys = GenerateKeyPair(idCngKey);
            userKeys.Add(GenerateKey(KeyType.IdentityKeyPrivate, idKeys, user));
            userKeys.Add(GenerateKey(KeyType.IdentityKeyPublic, idKeys, user));
            var sigKeys = GenerateKeyPair(CreateCngKey());
            userKeys.Add(GenerateSignedKey(idCngKey, sigKeys.PublicKey, user));
            userKeys.Add(GenerateKey(KeyType.SignedKeyPrivate, sigKeys, user));

            // without OPKs for now
            return userKeys;
        }

        public static SignedKey GenerateSignedKey(CngKey cngKey, byte[] sigPublicKey, User user)
        {

            ECDsaCng eCDsa = new ECDsaCng(cngKey);
            return new SignedKey { KeyType = KeyType.SignedKeyPublic, AssociatedUser = user, KeyBytes = sigPublicKey, Signature = eCDsa.SignData(sigPublicKey)}; 
        }

        public static bool VerifySignedKey(byte[] signature, byte[] publicSigKey, byte[] publicIdKey)
        {
            var key = CngKey.Import(publicIdKey, CngKeyBlobFormat.EccFullPublicBlob);
            ECDsaCng eCDsa = new ECDsaCng(key);
            return eCDsa.VerifyData(publicSigKey, signature);
        }

        public static Key GenerateKey(KeyType type, KeyPair keyPair, User user )
        {
            Key key= new();
            switch (type)
            {
                case KeyType.IdentityKeyPrivate:
                    key = new Key { KeyType = KeyType.IdentityKeyPrivate, AssociatedUser = user, KeyBytes = keyPair.PrivateKey };
                    break;
                case KeyType.IdentityKeyPublic:
                    key = new Key { KeyType = KeyType.IdentityKeyPublic, AssociatedUser = user, KeyBytes = keyPair.PublicKey };
                    break;
                case KeyType.SignedKeyPrivate:
                    key = new Key { KeyType = KeyType.SignedKeyPrivate, AssociatedUser = user, KeyBytes = keyPair.PrivateKey };
                    break;
                case KeyType.OneTimeKeyPrivate:
                    key = new Key { KeyType = KeyType.OneTimeKeyPrivate, AssociatedUser = user, KeyBytes = keyPair.PrivateKey };
                    break;
                case KeyType.OneTimeKeyPublic:
                    key = new Key { KeyType = KeyType.OneTimeKeyPublic, AssociatedUser = user, KeyBytes = keyPair.PublicKey };
                    break;
            }
            return key;
        }

        public static CngKey CreateCngKey()
        {


                return CngKey.Create(CngAlgorithm.ECDiffieHellmanP256, null, new CngKeyCreationParameters { ExportPolicy = CngExportPolicies.AllowPlaintextExport });
        }
        

        public static KeyPair GenerateKeyPair(CngKey cngKey)
        {
            var keyPair = new KeyPair();
            keyPair.PrivateKey = cngKey.Export(CngKeyBlobFormat.EccFullPrivateBlob);
            keyPair.PublicKey = cngKey.Export(CngKeyBlobFormat.EccFullPublicBlob);
            return keyPair;
        }


    }

    public class SignedKeyPair : KeyPair
    {
        public byte[] Signature { get; set; }
    }

    public class KeyPair
    {
        public byte[] PrivateKey { get; set; }

        public byte[] PublicKey { get; set; }
    }

}            /*using (ECDiffieHellmanCng cng = new ECDiffieHellmanCng(
                // need to do this to be able to export private key
                CngKey.Create(
                    CngAlgorithm.ECDiffieHellmanP256,
                    null,
                    new CngKeyCreationParameters
                    { ExportPolicy = CngExportPolicies.AllowPlaintextExport })))
            {
                cng.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
                cng.HashAlgorithm = CngAlgorithm.Sha512;
                // export both private and public keys and return
                var pr = cng.Key.Export(CngKeyBlobFormat.EccPrivateBlob);
                var pub = cng.PublicKey.ToByteArray();
                return cng;
            }*/
            /*
          
            //private static readonly KeyCreationParameters keyCreationParameters = new KeyCreationParameters() { ExportPolicy = KeyExportPolicies.AllowPlaintextExport };
        private readonly X25519 x25519 = new X25519() { };
        private static readonly SharedSecretCreationParameters secretCreationParameters = new

  
            var key = new Key(x25519, new KeyCreationParameters() { ExportPolicy = KeyExportPolicies.AllowPlaintextExport }) { };
            var privateKey = key.Export(KeyBlobFormat.NSecPrivateKey);
            var pKey = key.PublicKey;
            var publicKey = pKey.Export(KeyBlobFormat.NSecPrivateKey);
            
            x25519.Agree(privateKey, publicKey )
            //x25519.Agree(key, )



            var ecdh = new ECDiffieHellmanCng();
            ecdh.ExportECPrivateKey();
            var publickey = ecdh.PublicKey.ToByteArray();

            ecdh.ImportParameters(eCParameters);

            var ecdh2 = new ECDiffieHellmanCng();
            //ecdh.ImportECPrivateKey(privateKey);

            var cngKey2 = CngKey.Create(CngAlgorithm.ECDiffieHellmanP256, null, new CngKeyCreationParameters { ExportPolicy = CngExportPolicies.AllowPlaintextExport });
            var key = CngKey.Import(publickey, CngKeyBlobFormat.EccFullPublicBlob);
            var we = CngKey.Import(privateKey, CngKeyBlobFormat.EccFullPrivateBlob);
            var derivedKey = ecdh2.DeriveKeyMaterial(key);*/