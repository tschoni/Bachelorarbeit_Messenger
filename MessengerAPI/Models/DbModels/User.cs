using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MessengerAPI.Models.DbModels
{
    public class User : DbModelBase
    {
         //public string PhoneNumber { get; set; }       

        [Required]
        public string Name { get; set; }

        public string Password { get; set; }

        public List<PublicKey> PublicKeys { get; set; }

        public List<Message> SentMessages { get; set; }

        public List<Message> ReceivedMessages { get; set; }

        public List<Group> Groups { get; set; }
        
        public List<Group> AdminOfGroups { get; set; }
    }
}
