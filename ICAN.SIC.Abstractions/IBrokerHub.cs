using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICAN.SIC.Abstractions
{
    public interface IBrokerHub : IRunnable
    {
        void AddAndHook(IPlugin plugin);
        void GlobalPublish<T>(T message) where T : IMessage;
    }
}
