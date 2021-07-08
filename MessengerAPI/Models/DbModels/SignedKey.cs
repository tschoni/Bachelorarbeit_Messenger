using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerAPI.Models.DbModels
{
    public class SignedKey : PublicKey
    {
        public byte[] Signature { get; set; }
    }
}
