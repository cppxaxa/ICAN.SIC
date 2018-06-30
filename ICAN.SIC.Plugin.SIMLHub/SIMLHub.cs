using ICAN.SIC.Abstractions;
using ICAN.SIC.Abstractions.IMessageVariants;
using ICAN.SIC.PubSub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICAN.SIC.Plugin.SIMLHub
{
    public class SIMLHub : AbstractPlugin, ISIMLHub
    {
        SIMLHubHelper helper = new SIMLHubHelper();
        SIMLHubUtility utility = new SIMLHubUtility();

        public SIMLHub()
        {
            hub.Subscribe<IUserResponse>(this.printMessage);
        }

        private void printMessage(IUserResponse message){
            Console.WriteLine("PrintMessage: " + message.Text);
        }
    }
}
