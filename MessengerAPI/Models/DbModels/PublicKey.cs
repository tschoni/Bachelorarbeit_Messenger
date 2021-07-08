using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerAPI.Models.DbModels
{
    public class PublicKey : DbModelBase
    {
        public byte[] KeyBytes { get; set; }

        public virtual User Owner { get; set; }

        public PublicKeyType KeyType { get; set; }
    }
}
