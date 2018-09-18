using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICAN.SIC.Abstractions.IMessageVariants
{
    public interface IChatMessage : IMessage
    {
        string MarkupContent { get; }
    }
}
