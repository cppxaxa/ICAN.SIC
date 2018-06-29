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

        public List<IPlugin> ScanAndInstantiate(string baseDirectory)
        {
            List<IPlugin> plugins = new List<IPlugin>();

            string[] allFiles = Directory.GetFiles(baseDirectory);
            List<string> allDllFiles = utility.GetAllDllFiles(allFiles);

            // Iterate all dll files
            foreach (var dllFile in allDllFiles)
            {
                Assembly assembly = Assembly.LoadFrom(dllFile);

                //Console.WriteLine("FILE: " + Path.GetFileName(dllFile));
                //Console.WriteLine("Primary Namespace: " + utility.GetPrimaryNamespace(assembly));


                // We don't need to iterate through all the types, we will determine the type
                // from the namespace, refer documentation
                string guessedTypeName = utility.GetGuessedTypeName(assembly);

                //Console.WriteLine("GuessedTypeName: " + guessedTypeName);

                try
                {
                    IPlugin plugin = (IPlugin)assembly.CreateInstance(guessedTypeName);
                    if (plugin != null)
                        plugins.Add(plugin);
                }
                catch { /*Ignore*/ }
            }

            return plugins;
        }
    }
}
