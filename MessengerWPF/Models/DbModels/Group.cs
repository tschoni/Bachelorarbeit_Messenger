using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerWPF.Models.DbModels
{
    public class Group : DbModelBase
    {
        public string Name { get; set; }

        public List<User> Members { get; set; }

        public List<User> Admins { get; set; }

        public List<GroupMessage> Messages { get; set; }
    }
}
