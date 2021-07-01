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

        // Typ? Zur Gruppe hinzugefügt, entfernt
        public MessageType MessageType { get; set; }

        public long? GroupdId { get; set; }
    }
}
