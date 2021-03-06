using MessengerAPI.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerAPI.Models.DTO
{
    public class GroupDetailsDTO
    {
        public string Name { get; set; }

        public List<UserDTO> Members { get; set; }

        public List<UserDTO> Admins { get; set; }
    }
}
