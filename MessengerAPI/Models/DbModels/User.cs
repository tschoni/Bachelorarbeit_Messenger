using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MessengerAPI.Models.DbModels
{
    public class User : DbModelBase
    {
        
        //public string Name { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        public List<PublicKey> PublicKeys { get; set; }

        //TODO: Verbindungsinformationen

        public List<Message> Sender { get; set; }

        public List<Message> Recipient { get; set; }
    }
}
