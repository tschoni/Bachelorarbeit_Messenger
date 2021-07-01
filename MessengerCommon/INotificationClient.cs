using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerCommon
{
    public interface INotificationClient
    {
        Task NotifyMessage(NotifyMessage notifyMessage);
    }
}
