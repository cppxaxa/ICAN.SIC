using ICAN.SIC.PubSub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICAN.SIC.Abstractions
{
    public abstract class AbstractPlugin : IPlugin
    {
        protected IHub hub = new Hub();

        public IHub Hub
        {
            get { return hub; }
        }
    }
}
