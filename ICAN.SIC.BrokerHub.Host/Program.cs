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
            


            //BrokerHubUtility utility = new BrokerHubUtility();

            //List<string> names = new List<string>()
            //{
            //    "ICAN.SIC.Plugin",
            //    "ICAN.SIC.Plugin.Broker",
            //    "ICAN.SIC.Plugin.SIMLHub.Plugin",
            //    "ICAN.SIC.Plugin.SIMLHub"
            //};

            //names = utility.SortForBestNamespace(names);

            //foreach (var item in names)
            //{
            //    Console.WriteLine(item);
            //}

            Console.WriteLine("Done ?");
            Console.ReadKey();
        }
    }
}
