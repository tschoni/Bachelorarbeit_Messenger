using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerAPI.Models.DTO
{
    public class SignedKeyDTO : PublicKeyDTO
    {
        public byte[] Signature { get; set; }
    }
}
