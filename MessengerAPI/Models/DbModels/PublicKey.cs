using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerAPI.Models.DbModels
{
    public class PublicKey : DbModelBase
    {
        // TODO: enum KeyType

        public string KeyString { get; set; }

        public User Owner { get; set; }
    }
}
