using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRConsole.KEyExchange
{
    public class Key : DbModelBase
    {
        public byte[] KeyString { get; set; }

        public KeyType KeyType { get; set; }

        public User AssociatedUser { get; set; }
    }
}
