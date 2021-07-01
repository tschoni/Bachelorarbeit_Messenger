using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerWPF.Models.DbModels
{
    class GroupUpdateMessage : GroupMessage
    {
        public GroupUpdateType Type { get; set; }
        public User User { get; set; }
    }
}
