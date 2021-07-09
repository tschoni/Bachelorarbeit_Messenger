using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerAPI.Models.DTO
{
    public class EphemKeyDTO : PublicKeyDTO
    {
        public long OwnerId { get; set; }

        public long InitiatorId { get; set; }
    }
}
