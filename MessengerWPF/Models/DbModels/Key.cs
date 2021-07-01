﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerWPF.Models.DbModels
{
    public class Key : DbModelBase
    {
        public string KeyString { get; set; }

        public KeyType KeyType { get; set; }

        public User AssociatedUser { get; set; }
    }
}
