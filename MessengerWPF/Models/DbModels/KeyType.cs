﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerWPF.Models.DbModels
{
    public enum KeyType
    {
        MasterKey,
        IdentityKeyPublic,
        IdentityKeyPrivate,
        SignedKeyPrivate,
        SignedKeyPublic,
        OneTimeKeyPublic,
        OneTimeKeyPrivate,
    }
}
