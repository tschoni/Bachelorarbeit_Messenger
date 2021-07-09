using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerAPI.Models.DbModels
{
    public class EphemeralKey : PublicKey
    {
        public virtual User Initiator{ get; set; }
    }
}
