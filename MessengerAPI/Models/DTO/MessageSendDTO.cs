using MessengerAPI.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerAPI.Models.DTO
{
    public class MessageSendDTO
    {
        public long SenderId { get; set; }

        public long RecipientId { get; set; }

        public string CipherText { get; set; }

        public string MAC { get; set; }
    }
}
