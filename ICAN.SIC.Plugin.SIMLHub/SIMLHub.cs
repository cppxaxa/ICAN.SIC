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

        public SIMLHub()
        {
            bot = new SimlBot();

            // Soon it will be substracted
            this.adapterCount = bot.Adapters.Count;

            // Add all adapters
            pluginAdapterPathAndTypes = helper.GetAllSIMLHubPluginIndexAdapterPathAndTypes();
            
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("[SIMLHub] Adapters loading : ");
            Console.ResetColor();

            foreach (var adapterPair in pluginAdapterPathAndTypes)
            {
                Assembly assembly = Assembly.LoadFrom(adapterPair.Key);

                foreach (var typename in adapterPair.Value)
                {
                    try
                    {
                        IAdapter adapter = (IAdapter)assembly.CreateInstance(typename);

                        // Add to bot in not null
                        if (adapter != null)
                        {
                            bot.Adapters.Add(adapter);
                        }
                    }
                    catch { /*Ignore*/ }
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("success");
            Console.ResetColor();

            // Now final value is set
            this.adapterCount = bot.Adapters.Count - this.adapterCount;

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
            hub.Subscribe<IBotResult>(this.ShowBotResult);
        }

        private void GenerateAndPublishBotResponse(IUserResponse message)
        {
            ChatResult result;

            if (currentUser == null)
                result = bot.Chat(message.Text);
            else
                result = bot.Chat(new ChatRequest(message.Text, currentUser));


            IBotResult botResponse = new ICAN.SIC.Plugin.SIMLHub.DataTypes.BotResult(result);

            Console.WriteLine("PrintMessage: " + botResponse.Text);
            hub.Publish<IBotResult>(botResponse);
        }

        private void ShowBotResult(IBotResult response)
        {
            Console.WriteLine("BotResult: " + response.Text);
        }
    }
}
