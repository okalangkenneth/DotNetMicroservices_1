using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering_API.Services
{
    public interface IMessageService
    {
        void PublishMessage(string exchange, string routingKey, object message);
        string ConsumeMessage();
    }
}
