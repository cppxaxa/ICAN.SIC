using ICAN.SIC.Abstractions.IMessageVariants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICAN.SIC.BrokerHub.DataTypes
{
    public class Log : ILog
    {
        public LogType LogType { get; set; }
        public string Message { get; set; }
    }
}
