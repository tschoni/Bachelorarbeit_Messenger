using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRConsole.KEyExchange
{
    public abstract class GroupMessage : Message
    {
        public Group Group { get; set; }

    }
}
