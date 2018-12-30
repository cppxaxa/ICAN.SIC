using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Syn.Bot.Siml;


namespace ICAN.SIC.Abstractions.IMessageVariants
{
    public interface IBotResult : IMessage
    {
        string Text { get; }
        ChatResult ChatResult { get; }
        IUserResponse UserResponse { get; }
    }
}
