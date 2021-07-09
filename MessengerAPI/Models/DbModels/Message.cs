using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerAPI.Models.DbModels
{
    public class Message : DbModelBase
    {
        public virtual User Sender { get; set; }

        public virtual User Recipient { get; set; }

        public DateTime TimeStamp { get; set; }

        public byte[] CipherText { get; set; }

        //public string MAC { get; set; }
    }
}
