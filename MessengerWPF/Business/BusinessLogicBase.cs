using MessengerApiClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerWPF.Business
{
    public abstract class BusinessLogicBase
    {
        protected readonly IMApiClient apiClient;
        protected readonly IMClientDbContext dbContext;
        protected readonly TokenAndIdProvider tokenAndId;

        public BusinessLogicBase(IMApiClient apiClient, IMClientDbContext dbContext, TokenAndIdProvider tokenAndId)
        {
            this.apiClient = apiClient;
            this.dbContext = dbContext;
            this.tokenAndId = tokenAndId;
        }
    }
}
