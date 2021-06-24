using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerAPI.Models.DbModels
{
    public class Group : DbModelBase
    {
        public string Name { get; set; }

        public List<User> Members { get; set; }
    }
}
