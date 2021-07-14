using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerAPI.Models.DbModels
{
    public class EphemeralKey : DbModelBase
    {
        public byte[] KeyBytes { get; set; }

        public virtual User Owner { get; set; }

        public virtual User Initiator { get; set; }
    }
}
