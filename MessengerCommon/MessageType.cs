using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerCommon
{
    public enum MessageType
    {
        GroupMemberAdded,
        GroupMemberRemoved,
        GroupAdminAdded,
        GroupAdminRemoved,
        MessagePosted
    }
}
