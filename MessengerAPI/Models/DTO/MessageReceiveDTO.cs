using MessengerAPI.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerAPI.Models.DTO
{
    public class MessageReceiveDTO
    {
        public long Id { get; set; }
        public UserDTO Sender { get; set; }

        public DateTime TimeStamp { get; set; }

        public byte[] CipherText { get; set; }

        //public string MAC { get; set; }
    }
}
