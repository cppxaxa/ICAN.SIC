using ICAN.SIC.Abstractions.IMessageVariants;
using Syn.Bot.Siml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICAN.SIC.Plugin.SIMLHub.DataTypes
{
    public class BotResponse : IBotResponse
    {
        ChatResult chatResult;

        public BotResponse(ChatResult result)
        {
            this.chatResult = result;
        }

        public Syn.Bot.Siml.ChatResult ChatResult
        {
            get { return chatResult; }
        }

        public string Text
        {
            get { return chatResult.BotMessage; }
        }
    }
}
