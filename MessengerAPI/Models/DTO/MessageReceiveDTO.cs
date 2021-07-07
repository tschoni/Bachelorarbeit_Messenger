using MessengerAPI.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerAPI.Models.DTO
{
    public class MessageReceiveDTO
    {
        public long SenderId { get; set; }

        public DateTime TimeStamp { get; set; }

        public string CipherText { get; set; }

        public string MAC { get; set; }
    }
}
