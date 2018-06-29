using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICAN.SIC.Abstractions
{
    public interface IRunnable
    {
        void Start();
        void Stop();
    }
}
