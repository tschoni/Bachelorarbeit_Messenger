using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerAPI.Models.DTO
{
    public class GroupListDTO
    {
        public virtual List<GroupDetailsDTO> GroupDetails { get; set; }
    }
}
