using MessengerAPI.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerAPI.Models.DTO
{
    public class MessageSendDTO
    {
        public UserDTO Sender { get; set; }

        public UserDTO Recipient { get; set; }

        public byte[] CipherText { get; set; }

        //public string MAC { get; set; }
    }
}
