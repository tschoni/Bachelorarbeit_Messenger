﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerWPF.Models.DbModels
{
    public class SignedKey : Key
    {
        public byte[] Signature { get; set; }
    }
}