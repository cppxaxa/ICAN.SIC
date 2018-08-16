﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICAN.SIC.Abstractions.IMessageVariants
{
    public interface IAllPluginsLoaded : IMessage
    {
        List<string> PluginsNames { get; }
    }
}
