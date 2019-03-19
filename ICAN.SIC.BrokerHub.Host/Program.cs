using ICAN.SIC.Abstractions.IMessageVariants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

    class Log : ILog
    {
        LogType logType;
        string message;

        public Log(LogType logType, string message)
        {
            this.logType = logType;
            this.message = message;
        }

        public LogType LogType
        {
            get { return logType; }
        }
        public string Message
        {
            get { return this.message; }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 5; i > 0; i--)
            {
                Console.WriteLine("Pause before loading " + i);
                Thread.Sleep(1000);
            }

            BrokerHub brokerHub = new BrokerHub();
            brokerHub.Start();

            Console.ReadKey();
            Log log = new Log(LogType.Info, "Hello");

            brokerHub.GlobalPublish<ILog>(log);

            Console.WriteLine("Done ?");
            Console.ReadKey();
        }
    }
}
