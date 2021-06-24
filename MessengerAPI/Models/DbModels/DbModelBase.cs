using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerAPI.Models.DbModels
{
    public abstract class DbModelBase
    {
        public long Id { get; set; }
    }
}
