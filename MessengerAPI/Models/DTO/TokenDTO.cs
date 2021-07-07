using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerAPI.Models.DTO
{
    public class TokenDTO
    {
        public long UserID { get; set; }

        public string UserToken { get; set; }
    }
}
