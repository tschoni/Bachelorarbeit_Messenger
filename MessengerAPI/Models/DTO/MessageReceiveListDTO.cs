using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerAPI.Models.DTO
{
    public class MessageReceiveListDTO
    {
        public ICollection<MessageReceiveDTO> ReceiveMessageList { get; set; }
    }
}
