using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Syn.Bot.Siml;


namespace ICAN.SIC.Abstractions.IMessageVariants
{
    public interface IBotResponse : IMessage
    {
        string Text { get; }
        ChatResult ChatResult { get; }
    }
}
