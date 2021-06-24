using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerAPI.Models.DbModels
{
    public class Message : DbModelBase
    {
        public User Sender { get; set; }

        public User Recipient { get; set; }

        public string CipherText { get; set; }

        public string MAC { get; set; }
    }
}
