using ICAN.SIC.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ICAN.SIC.BrokerHub
{
    public class PluginConfiguration
    {
        public string Name;
        public bool Enabled;
    }

    public class BrokerHubHelper
    {
        private readonly string pluginConfigurationFilename = "PluginConfiguration.json";

        private readonly BrokerHubUtility utility = null;
        private Dictionary<string, PluginConfiguration> pluginConfiguration = null;

        public BrokerHubHelper(BrokerHubUtility utility)
        {
            this.utility = utility;
        }

        public List<IPlugin> ScanPrepareAndInstantiate(string baseDirectory, HashSet<string> vitalPluginDllNames = null)
        {
            if (File.Exists(pluginConfigurationFilename))
            {
                string content = File.ReadAllText(pluginConfigurationFilename);
                pluginConfiguration = JsonConvert.DeserializeObject<Dictionary<string, PluginConfiguration>>(content);
            }

            if (pluginConfiguration == null)
                pluginConfiguration = new Dictionary<string, PluginConfiguration>();

            // Fresh start
            if (vitalPluginDllNames == null)
            {
                // Preparation & generate a CleanupAfterUse.py
                List<string> newContentsCreated = utility.CopyAllDllsToBaseDirectory();

                string cleanupCommands = "import os\nimport shutil\n\n";
                foreach (var contentPath in newContentsCreated)
                {
                    Console.WriteLine("[Removal after execution] " + contentPath);

                    if (File.Exists(contentPath))
                        cleanupCommands += "os.remove(\"" + contentPath.Replace("\\", "\\\\") + "\")" + "\n";

                    if (Directory.Exists(contentPath))
                        cleanupCommands += "shutil.rmtree(\"" + contentPath.Replace("\\", "\\\\") + "\", True)" + "\n";
                }
                cleanupCommands += "os.remove(\"CleanupAfterUse.py\")";

                File.WriteAllText("CleanupAfterUse.py", cleanupCommands);
            }

            // Scan and Instantiation process
            List<IPlugin> plugins = new List<IPlugin>();

            string[] allFiles = Directory.GetFiles(baseDirectory);
            List<string> allDllFiles = utility.GetAllDllFiles(allFiles);

            // Iterate all dll files
            foreach (var dllFile in allDllFiles)
            {
                Assembly assembly = Assembly.LoadFrom(dllFile);

                string guessedTypeName = utility.GetGuessedTypeName(assembly);

                if (utility.IsBrokerHubSupportedPlugin(guessedTypeName))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("[INFO] (");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("SUCCESS");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(") BrokerHub supported plugin : \n\t" + guessedTypeName + "\n\t in " + dllFile);
                    Console.ResetColor();
                    try
                    {
                        bool vitalPluginDllNamesContains = false;
                        if (vitalPluginDllNames != null)
                        {
                            foreach (var keyword in vitalPluginDllNames)
                            {
                                if (guessedTypeName.IndexOf(keyword) >= 0)
                                    vitalPluginDllNamesContains = true;
                            }
                        }

                        if (vitalPluginDllNamesContains)
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine("[INFO] Vital plugin found, will not be instantiated and old instance will stay in ecosystem");
                            Console.WriteLine("\tDLL: {0}", dllFile);
                            Console.WriteLine("\t\tType Name: {0}", guessedTypeName);
                            Console.WriteLine();
                            Console.ResetColor();
                        }
                        else
                        {
                            IPlugin plugin = (IPlugin)assembly.CreateInstance(guessedTypeName);
                            if (plugin != null)
                            {
                                bool newPlugin = true;
                                bool pluginConfigurationSaysDisabledEnabled = true;

                                string key = utility.FindMatchingKey(pluginConfiguration, guessedTypeName);
                                if (key != null && pluginConfiguration[key].Enabled == false)
                                {
                                    pluginConfigurationSaysDisabledEnabled = false;

                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("[INFO] This plugin is disabled in configuration");
                                    Console.WriteLine("\tDLL: {0}", dllFile);
                                    Console.WriteLine("\t\tType Name: {0}", guessedTypeName);
                                    Console.WriteLine();
                                    Console.ResetColor();
                                }


                                if (newPlugin || pluginConfigurationSaysDisabledEnabled)
                                {
                                    plugins.Add(plugin);

                                    // Build the configuration
                                    pluginConfiguration[guessedTypeName] = new PluginConfiguration { Name = guessedTypeName, Enabled = true };
                                }
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Plugin Instantiation Error");
                                Console.WriteLine("This guessed type name may be incorrect for the plugin. (If it is so, it's a bug)");
                                Console.WriteLine("DLL: {0}", dllFile);
                                Console.WriteLine("Type Name: {0}", guessedTypeName);
                                Console.WriteLine();
                                Console.ResetColor();
                            }
                        }
                    }
                    catch(Exception ex)
                    { /*Ignore*/
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("This plugin may not have inherited from AbstractPlugin");
                        Console.WriteLine("Plugin Error");
                        Console.WriteLine("DLL: {0}", dllFile);
                        Console.WriteLine("Type Name: {0}", guessedTypeName);
                        Console.WriteLine("Exception: {0}", ex.Message);
                        Console.WriteLine("InnerException: {0}", ex.InnerException?.Message);
                        Console.WriteLine();
                        Console.ResetColor();
                    }
                }
                else
                {
                    Console.WriteLine("[INFO] (FAILURE) Not a BrokerHub supported plugin : \n\t" + guessedTypeName + "\n\t in " + dllFile);
                }
            }

            File.WriteAllText(pluginConfigurationFilename, JsonConvert.SerializeObject(pluginConfiguration));

            return plugins;
        }

        public void DisablePlugin(string pluginName)
        {
            foreach (var item in pluginConfiguration)
            {
                if (item.Key.Contains(pluginName))
                {
                    item.Value.Enabled = false;

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[INFO] BrokerHubHelper set enable to false for item " + item.Key);
                    Console.ResetColor();

                    // Save the configuration
                    File.WriteAllText(pluginConfigurationFilename, JsonConvert.SerializeObject(pluginConfiguration));
                    return;
                }
            }

            Console.WriteLine("[INFO] Unable to find pluginname '" + pluginName + "'");
        }

        public void EnablePlugin(string pluginName)
        {
            foreach (var item in pluginConfiguration)
            {
                if (item.Key.Contains(pluginName))
                {
                    item.Value.Enabled = true;

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[INFO] BrokerHubHelper set enable to true for item " + item.Key);
                    Console.ResetColor();

                    // Save the configuration
                    File.WriteAllText(pluginConfigurationFilename, JsonConvert.SerializeObject(pluginConfiguration));
                    return;
                }
            }

            Console.WriteLine("[INFO] Unable to find pluginname '" + pluginName + "'");
        }

        public List<PluginConfiguration> GetPluginConfigurations()
        {
            List<PluginConfiguration> result = new List<PluginConfiguration>();
            foreach (var item in pluginConfiguration)
            {
                result.Add(item.Value);
            }
            return result;
        }
    }
}
