using ICAN.SIC.Abstractions.IMessageVariants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICAN.SIC.Plugin.SIMLHub.DataTypes
{
    public class UserFriendlyMachineMessage : IUserFriendlyMachineMessage
    {
        string message;

        public UserFriendlyMachineMessage()
        {
            message = "";
        }

        public UserFriendlyMachineMessage(string message)
        {
            this.message = message;
        }

        public string PrettyMessage
        {
            get { return message; }
            set { message = value; }
        }
    }
}
