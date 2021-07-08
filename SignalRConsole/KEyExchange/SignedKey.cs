using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRConsole.KEyExchange
{
    public class SignedKey : Key
    {
        public byte[] Signature { get; set; }
    }
}
