using ICAN.SIC.Abstractions.IMessageVariants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICAN.SIC.BrokerHub.Host
{
    class UserResponse : IUserResponse
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

    class Program
    {
        static void Main(string[] args)
        {
            BrokerHub brokerHub = new BrokerHub();

            brokerHub.Start();

            UserResponse response = new UserResponse("Hello bot");
            brokerHub.GlobalPublish<UserResponse>(response);

            Console.Read();
        }
    }
}
