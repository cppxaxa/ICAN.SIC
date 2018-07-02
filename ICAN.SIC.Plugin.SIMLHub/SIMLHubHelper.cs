using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ICAN.SIC.Plugin.SIMLHub
{
    public class SIMLHubHelper
    {
        private string SIMLHubPluginDirectoryName = @"Plugins\SIMLHub\SIMLHubPlugins";
        SIMLHubUtility utility = new SIMLHubUtility();

        public List<string> GetAllSIMLHubPluginIndexSIMLFiles()
        {
            return utility.GetAllSIMLHubPluginIndexSIMLFiles(this.SIMLHubPluginDirectoryName);
        }

        public string GetAllIndexSimlPackage()
        {
            List<string> allIndexSimlPath = this.GetAllSIMLHubPluginIndexSIMLFiles();
            List<XDocument> allIndexSimlXDocument = new List<XDocument>();

            foreach (var indexFilePath in allIndexSimlPath)
            {
                XDocument doc = XDocument.Load(indexFilePath);
                allIndexSimlXDocument.Add(doc);
            }

            string packageString = Syn.Bot.Siml.SimlBot.Instance.PackageManager.ConvertToPackage(allIndexSimlXDocument);

            return packageString;
        }

        public Dictionary<string, List<string>> GetAllSIMLHubPluginIndexAdapterPathAndTypes()
        {
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();

            List<string> allIndexAdapterDllPaths = utility.GetAllSIMLHubPluginIndexAdapterPathAndTypes(this.SIMLHubPluginDirectoryName);

            // Iterate allIndexAdapterDllPaths and load assembly
            foreach (var adapterPath in allIndexAdapterDllPaths)
            {
                Assembly assembly = Assembly.LoadFrom(adapterPath);

                // Get all typenames with suffix "Adapter" and prefix "ICAN.SIC.Plugin.SIMLHub.Plugin."
                foreach (var type in assembly.GetTypes())
                {
                    if (type.FullName.StartsWith("ICAN.SIC.Plugin.SIMLHub.Plugin.") && type.FullName.EndsWith("Adapter"))
                    {
                        if (!result.ContainsKey(adapterPath))
                        {
                            result.Add(adapterPath, new List<string>());
                        }

                        result[adapterPath].Add(type.ToString());
                    }
                }
            }

            return result;
        }
    }
}
