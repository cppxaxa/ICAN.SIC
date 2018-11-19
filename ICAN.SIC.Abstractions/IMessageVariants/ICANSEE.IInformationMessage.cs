using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICAN.SIC.Abstractions.IMessageVariants.ICANSEE
{
    public interface IInformationMessage : IMessage
    {
        string Json { get; }
        string Text { get; }
    }
}
