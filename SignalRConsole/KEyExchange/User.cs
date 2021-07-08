﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRConsole.KEyExchange
{
    public class User : DbModelBase
    {
        public string Name { get; set; }

        public List<Group> Groups { get; set; }

        public List<Message> Messages { get; set; }

//        public List<Message> ReceivedMessages { get; set; }

        public List<User> Contacts { get; set; }

        public List<Key> Keys { get; set; }
    }
}
