using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerAPI.Models.DTO
{
    public class EphemKeyDTO 
    {
        public long Id { get; set; }

        public byte[] KeyBytes { get; set; }

        public UserDTO Owner { get; set; }

        public UserDTO Initiator { get; set; }
    }
}
