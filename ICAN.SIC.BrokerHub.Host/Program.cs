﻿using ICAN.SIC.Abstractions.ConcreteClasses;
using ICAN.SIC.Abstractions.IMessageVariants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ICAN.SIC.BrokerHub.Host
{
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
        static BrokerHub brokerHub = null;

        static void InitBrokerHub(IMachineMessage msg)
        {
            if (msg.Message == "Start_ICAN.SIC")
            {
                brokerHub?.UnsubscribeAll();
                brokerHub?.Stop();

                brokerHub = new BrokerHub();
                brokerHub.Hub.Subscribe<IMachineMessage>(InitBrokerHub);
                brokerHub.Start();
            }
        }

        static void Main(string[] args)
        {
            Dictionary<string, string> cmdParams = ExtractParams(args);

            int startupDelay = 0;

            if (cmdParams.ContainsKey("-delay"))
                startupDelay = int.Parse(cmdParams["-delay"]);

            for (int i = startupDelay; i > 0; i--)
            {
                Console.WriteLine("Pause before loading " + i);
                Thread.Sleep(1000);
            }

            //BrokerHub brokerHub = new BrokerHub();
            //brokerHub.Start();

            InitBrokerHub(new MachineMessage("Start_ICAN.SIC"));
            Console.ReadKey();

            brokerHub.GlobalPublish<IUserResponse>(new UserResponse("what is my coordinates"));

            Log log = new Log(LogType.Info, "Hello");

            brokerHub.GlobalPublish<ILog>(log);

            Console.WriteLine("Done ?");
            Console.ReadKey();
        }

        private static Dictionary<string, string> ExtractParams(string[] args)
        {
            Dictionary<string, string> cmdParams = new Dictionary<string, string>();

            string paramName = string.Empty;
            string paramValue = string.Empty;
            for (int i = 0; i < args.Length; i++)
            {
                if (i % 2 == 0)
                    paramName = args[i];
                else
                {
                    paramValue = args[i];

                    cmdParams[paramName] = paramValue;
                }
            }

            return cmdParams;
        }
    }
}
