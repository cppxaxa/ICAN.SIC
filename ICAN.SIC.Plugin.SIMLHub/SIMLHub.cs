using ICAN.SIC.Abstractions;
using ICAN.SIC.Abstractions.IMessageVariants;
using ICAN.SIC.PubSub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syn.Bot.Siml;
using System.Reflection;
using Syn.Bot.Siml.Interfaces;
using ICAN.SIC.Plugin.SIMLHub.DataTypes;

namespace ICAN.SIC.Plugin.SIMLHub
{
    public class SIMLHub : AbstractPlugin, ISIMLHub
    {
        SIMLHubHelper helper = new SIMLHubHelper();
        SIMLHubUtility utility = new SIMLHubUtility();

        SimlBot bot;
        BotUser currentUser = null;

        // Siml Bot info
        int adapterCount = 0;
        Dictionary<string, List<string>> pluginAdapterPathAndTypes = null;

        public int AdapterCount { get { return adapterCount; } }
        public List<string> LoadedDLLPath
        {
            get
            {
                List<string> loadedDllPath = new List<string>();
                foreach (var adapterPair in pluginAdapterPathAndTypes)
                {
                    loadedDllPath.Add(adapterPair.Key);
                }
                return loadedDllPath;
            }
        }

        public SIMLHub() : base("SIMLHub")
        {
            bot = new SimlBot();

            // Soon it will be substracted
            this.adapterCount = 0;

            // Add all adapters
            pluginAdapterPathAndTypes = helper.GetAllSIMLHubPluginIndexAdapterPathAndTypes();
            
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("[SIMLHub] Adapters loading : ");
            Console.ResetColor();

            int botAdapterCount = 0;
            foreach (var adapterPair in pluginAdapterPathAndTypes)
            {
                Assembly assembly = Assembly.LoadFrom(adapterPair.Key);

                foreach (var typename in adapterPair.Value)
                {
                    try
                    {
                        IAdapter adapter = (IAdapter)assembly.CreateInstance(typename);

                        AbstractPlugin simlHubPlugin = (AbstractPlugin)adapter;
                        this.Hub.PassThrough(simlHubPlugin.Hub);

                        // Add to bot in not null
                        if (adapter != null)
                        {
                            bot.Adapters.Add(adapter);
                            botAdapterCount++;
                        }
                    }
                    catch { /*Ignore*/ }
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("success");
            Console.ResetColor();

            // Now final value is set
            this.adapterCount = botAdapterCount;
            Console.WriteLine("[SIMLHub] SIMLHub Plugins count: " + this.adapterCount);

            // Add all index.siml files
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("[SIMLHub] Index siml merge : ");
            Console.ResetColor();

            string mergedIndexSimlPackage = helper.GetAllIndexSimlPackage();
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("success");
            Console.ResetColor();


            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("[SIMLHub] Merged index siml : ");
            Console.ResetColor();

            bot.PackageManager.LoadFromString(mergedIndexSimlPackage);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("success");
            Console.ResetColor();

            // Subscribe to IUserResponse for input
            hub.Subscribe<IUserResponse>(this.GenerateAndPublishBotResponse);
            hub.Subscribe<AllPluginsLoaded>(this.AllPluginsLoadedCallback);

            // Subscribe to IMachineMessage for processing machine generated message
            hub.Subscribe<IMachineMessage>(this.MakeMachineMessageUserFriendly);
        }

        private void AllPluginsLoadedCallback(AllPluginsLoaded message)
        {
            foreach (var item in bot.Examples)
            {
                Console.WriteLine(item.ToString());
                IBotResult result = new BotResult(item.ToString());

                hub.Publish(result);
            }
        }

        private void MakeMachineMessageUserFriendly(IMachineMessage message)
        {
            IUserFriendlyMachineMessage userFriendlyMessage;

            ChatResult result;

            if (currentUser == null)
                result = bot.Chat(message.Message);
            else
                result = bot.Chat(new ChatRequest(message.Message, currentUser));

            userFriendlyMessage = new UserFriendlyMachineMessage(result.BotMessage);

            hub.Publish<IUserFriendlyMachineMessage>(userFriendlyMessage);
        }

        private void GenerateAndPublishBotResponse(IUserResponse message)
        {
            ChatResult result;

            if (currentUser == null)
                result = bot.Chat(message.Text);
            else
                result = bot.Chat(new ChatRequest(message.Text, currentUser));


            IBotResult botResponse = new ICAN.SIC.Plugin.SIMLHub.DataTypes.BotResult(result, message);
            
            hub.Publish<IBotResult>(botResponse);
        }
    }
}
