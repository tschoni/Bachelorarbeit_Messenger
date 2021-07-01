using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerWPF.Models.DbModels
{
    public abstract class Message : DbModelBase
    {
        public User Sender { get; set; }

        public MessageState MessageState { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}
