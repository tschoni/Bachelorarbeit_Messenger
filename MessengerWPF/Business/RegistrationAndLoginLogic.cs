using MessengerApiClient;
using MessengerWPF.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MessengerWPF.Business
{
    public class RegistrationAndLoginLogic : BusinessLogicBase
    {


        public RegistrationAndLoginLogic(IMApiClient apiClient, IMClientDbContext dbContext, TokenAndIdProvider tokenAndId) : base(apiClient, dbContext, tokenAndId)
        {

        }

        /// <summary>
        /// Register user with public keys and save user with public and private keys in db
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task RegisterUserAsync(string name, string password)
        {
            //TODO: Generate public and private Keys
            var registerDTO = new UserRegisterDTO() { Name = name, Password = password };// public Keys
            var tokenDTO = await apiClient.RegisterUserAsync(registerDTO);
            tokenAndId.Id = tokenDTO.UserID;
            tokenAndId.Token = tokenDTO.UserToken;
            dbContext.Users.Add(new User() { Name = name, Id = tokenDTO.UserID });//, Keys = 
            await dbContext.SaveChangesAsync();
            throw new NotImplementedException();
        }
        
        private PublicKeyDTO GenerateKey()
        {

            var cngKey = CngKey.Create(CngAlgorithm.ECDiffieHellmanP256, null, new CngKeyCreationParameters { ExportPolicy = CngExportPolicies.AllowPlaintextExport });
            var publicKey = cngKey.Export(CngKeyBlobFormat.EccFullPrivateBlob);
            var privateKey = cngKey.Export(CngKeyBlobFormat.EccFullPrivateBlob);


            var ecdh = new ECDiffieHellmanCng();
            ecdh.ExportECPrivateKey();
            var publickey = ecdh.PublicKey.ToByteArray();


            var ecdh2 = new ECDiffieHellmanCng();
            //ecdh.ImportECPrivateKey(privateKey);

            var cngKey2 = CngKey.Create(CngAlgorithm.ECDiffieHellmanP256, null, new CngKeyCreationParameters { ExportPolicy = CngExportPolicies.AllowPlaintextExport });
            var key = CngKey.Import(publickey, CngKeyBlobFormat.EccFullPublicBlob);
            var we = CngKey.Import(privateKey, CngKeyBlobFormat.EccFullPrivateBlob);
            var derivedKey = ecdh2.DeriveKeyMaterial(key);
            

            throw new NotImplementedException();
        }

        /// <summary>
        /// Login user 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task LoginUserAsync(string name, string password)
        {
            var loginDTO = new UserLoginDTO() { Name = name, Password = password };
            var tokenDTO = await apiClient.LoginUserAsync(loginDTO);
            tokenAndId.Id = tokenDTO.UserID;
            tokenAndId.Token = tokenDTO.UserToken;
        }
    }
}
