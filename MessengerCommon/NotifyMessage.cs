using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerCommon
{
    public class NotifyMessage
    {
        public long UserId { get; set; }

        public MessageType MessageType { get; set; }

        //public long? MessageId { get; set; }

        public long? GroupId { get; set; }
    }
}
