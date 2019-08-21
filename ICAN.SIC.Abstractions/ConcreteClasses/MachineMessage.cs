using ICAN.SIC.Abstractions.IMessageVariants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICAN.SIC.Abstractions.ConcreteClasses
{
    public class MachineMessage : IMachineMessage
    {
        private string message;

        public MachineMessage()
        {
            message = "";
        }

        public MachineMessage(string message)
        {
            this.message = message;
        }

        public string Message
        {
            get { return message; }
            set { message = value; }
        }
    }
}
