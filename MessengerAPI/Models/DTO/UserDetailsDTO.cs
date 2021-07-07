using MessengerAPI.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerAPI.Models.DTO
{
    public class UserDetailsDTO
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public ICollection<PublicKeyDTO> PublicKeys { get; set; }
    }
}
