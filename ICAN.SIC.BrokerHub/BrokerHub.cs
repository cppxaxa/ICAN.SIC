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
        private readonly BrokerHubUtility utility = new BrokerHubUtility();
        private readonly BrokerHubHelper helper = null;
        private readonly HashSet<string> vitalPluginDllNames;
        private List<IPlugin> plugins;

        private readonly bool firstStart = true;

        public BrokerHub(List<string> vitalPluginDllNames, bool firstStart)
        {
            this.helper = new BrokerHubHelper(utility);

            hub = new Hub("BrokerHub");
            this.firstStart = firstStart;
            this.vitalPluginDllNames = new HashSet<string>(vitalPluginDllNames);

            hub.Subscribe<ILog>(this.LogMessages);
        }

        public IHub Hub { get => hub; }

        private void HookHub(IPlugin plugin)
        {
            hub.PassThrough(plugin.Hub);
        }

        public void AddAndHook(IPlugin plugin)
        {
            this.plugins.Add(plugin);
            this.HookHub(plugin);
        }

        public List<IPlugin> GetVitalPlugins()
        {
            List<IPlugin> result = new List<IPlugin>();

            foreach (var plugin in plugins)
            {
                bool vitalPluginDllNamesContains = false;
                if (vitalPluginDllNames != null)
                {
                    foreach (var keyword in vitalPluginDllNames)
                    {
                        string guessedTypeName = utility.GetGuessedTypeName(plugin.GetType().Assembly);
                        if (guessedTypeName.IndexOf(keyword) >= 0)
                            vitalPluginDllNamesContains = true;
                    }
                }

                if (vitalPluginDllNamesContains)
                    result.Add(plugin);
            }

            return result;
        }

        public void Start()
        {
            if (!firstStart)
                plugins = helper.ScanPrepareAndInstantiate(AppDomain.CurrentDomain.BaseDirectory, vitalPluginDllNames);
            else
                plugins = helper.ScanPrepareAndInstantiate(AppDomain.CurrentDomain.BaseDirectory);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("[INFO] Instantiated plugins count: " + plugins.Count);
            Console.ResetColor();

            List<string> list = new List<string>();

            foreach (var plugin in plugins)
            {
                list.Add(plugin.ToString());
                this.HookHub(plugin);
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("[INFO] All hubs hooked");
            Console.ResetColor();
            
            // Publish all plugins loaded message
            AllPluginsLoaded message = new AllPluginsLoaded(list);
            hub.Publish<AllPluginsLoaded>(message);
        }

        public void Stop()
        {
            
        }

        public void Dispose()
        {
            hub.Unsubscribe<ILog>(this.LogMessages);

            foreach (var plugin in plugins)
            {
                bool vitalPluginDllNamesContains = false;
                if (vitalPluginDllNames != null)
                {
                    foreach (var keyword in vitalPluginDllNames)
                    {
                        string guessedTypeName = utility.GetGuessedTypeName(plugin.GetType().Assembly);
                        if (guessedTypeName.IndexOf(keyword) >= 0)
                            vitalPluginDllNamesContains = true;
                    }
                }

                if (!vitalPluginDllNamesContains)
                    plugin.Dispose();
            }
            plugins.Clear();
        }

        public void GlobalPublish<T>(T message) where T : IMessage
        {
            this.hub.Publish<T>(message);
        }

        private void LogMessages(ILog log)
        {
            switch(log.LogType)
            {
                case LogType.Debug:
                    Console.WriteLine("[DEBUG] {0}", log.Message);
                    break;

                case LogType.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[ERROR] {0}", log.Message);
                    Console.ResetColor();
                    break;

                case LogType.Info:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("[INFO] ");
                    Console.ResetColor();
                    Console.WriteLine(log.Message);
                    break;

                case LogType.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("[WARNING] ");
                    Console.ResetColor();
                    Console.WriteLine(log.Message);
                    break;
            }
        }

        // Use cautiously
        public void UnsubscribeAll()
        {
            hub.UnsubscribeAll();
        }
    }
}
