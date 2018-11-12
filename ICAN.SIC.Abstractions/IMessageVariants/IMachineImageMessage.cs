using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ICAN.SIC.Abstractions.IMessageVariants
{
    public interface IMachineImageMessage : IMessage
    {
        string Filename { get; }
        Image Image { get; }
    }
}
