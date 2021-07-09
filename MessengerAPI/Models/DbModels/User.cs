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

        public string UserToken { get; set; }

        public virtual List<PublicKey> PublicKeys { get; set; }

        public virtual List<Message> SentMessages { get; set; }

        public virtual List<Message> ReceivedMessages { get; set; }

        public virtual List<Group> Groups { get; set; }
        
        public virtual List<Group> AdminOfGroups { get; set; }

        public virtual List<EphemeralKey> EphemeralKeys { get; set; }
    }
}
//add-migration Initial-Migration