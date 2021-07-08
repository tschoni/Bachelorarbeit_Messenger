using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRConsole.KEyExchange
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
