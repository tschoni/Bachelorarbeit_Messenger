using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerWPF.Models.DbModels
{
    public class Key : DbModelBase
    {
        public byte[] KeyBytes { get; set; }

        public KeyType KeyType { get; set; }

        public User AssociatedUser { get; set; }
    }
}
