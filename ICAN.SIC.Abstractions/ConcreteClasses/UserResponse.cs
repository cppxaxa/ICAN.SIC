using ICAN.SIC.Abstractions.IMessageVariants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICAN.SIC.Abstractions.ConcreteClasses
{
    public class UserResponse : IUserResponse
    {
        private readonly string text;

        public UserResponse(string text)
        {
            this.text = text;
        }

        public string Text
        {
            get { return text; }
        }
    }
}
