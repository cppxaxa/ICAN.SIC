using ICAN.SIC.Abstractions;
using ICAN.SIC.PubSub;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using ICAN.SIC.Abstractions.IMessageVariants;

namespace ICAN.SIC.BrokerHub
{
    public class BrokerHub : IBrokerHub
    {
        private readonly IHub hub;
        private readonly BrokerHubHelper helper = new BrokerHubHelper();

        private List<IPlugin> plugins;

        public BrokerHub()
        {
            hub = new Hub("BrokerHub");
            // Here is the problem
            //hub.Subscribe<IBotResult>(this.ShowBotResult);
        }

        private void HookHub(IPlugin plugin)
        {
            // Maybe this may not be working fine
            hub.PassThrough(plugin.Hub);
        }

        public void AddAndHook(IPlugin plugin)
        {
            this.plugins.Add(plugin);
            this.HookHub(plugin);
        }

        public void Start()
        {
            plugins = helper.ScanAndInstantiate(AppDomain.CurrentDomain.BaseDirectory);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("[INFO] Instantiated plugins count: " + plugins.Count);
            Console.ResetColor();

            foreach (var plugin in plugins)
            {
                this.HookHub(plugin);
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("[INFO] All hubs hooked");
            Console.ResetColor();

            hub.Subscribe<IBotResult>(this.ShowBotResult);
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void GlobalPublish<T>(T message) where T : IMessage
        {
            this.hub.Publish<T>(message);
        }

        public void ShowBotResult(IBotResult result)
        {
            Console.WriteLine("BrokerHub.cs: BotResult : " + result.ToString());
        }
    }
}
