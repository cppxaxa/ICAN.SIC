﻿using ICAN.SIC.PubSub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICAN.SIC.Abstractions
{
    public abstract class AbstractPlugin : IPlugin
    {
        protected IHub hub;

        public AbstractPlugin(string name)
        {
            hub = new Hub(name);
        }

        public IHub Hub
        {
            get { return hub; }
        }

        public abstract void Dispose();
    }
}
