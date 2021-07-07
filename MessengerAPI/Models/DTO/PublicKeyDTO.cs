using MessengerAPI.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerAPI.Models.DTO
{
    public class PublicKeyDTO
    {
        public long Id { get; set; }

        public string KeyString { get; set; }

        public KeyType KeyType { get; set; }
    }
}
