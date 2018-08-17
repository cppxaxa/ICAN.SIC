using ICAN.SIC.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ICAN.SIC.BrokerHub
{
    public class BrokerHubHelper
    {
        private readonly BrokerHubUtility utility = new BrokerHubUtility();

        public List<IPlugin> ScanPrepareAndInstantiate(string baseDirectory)
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
            File.WriteAllText("CleanupAfterUse.py", cleanupCommands);


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
                        IPlugin plugin = (IPlugin)assembly.CreateInstance(guessedTypeName);
                        if (plugin != null)
                            plugins.Add(plugin);
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Plugin Instantiation Error");
                            Console.WriteLine("DLL: {0}", dllFile);
                            Console.WriteLine("Type Name: {0}", guessedTypeName);
                            Console.WriteLine();
                            Console.ResetColor();
                        }
                    }
                    catch
                    { /*Ignore*/
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Plugin Error");
                        Console.WriteLine("DLL: {0}", dllFile);
                        Console.WriteLine("Type Name: {0}", guessedTypeName);
                        Console.WriteLine();
                        Console.ResetColor();
                    }
                }
                else
                {
                    Console.WriteLine("[INFO] (FAILURE) Not a BrokerHub supported plugin : \n\t" + guessedTypeName + "\n\t in " + dllFile);
                }
            }

            return plugins;
        }
    }
}
