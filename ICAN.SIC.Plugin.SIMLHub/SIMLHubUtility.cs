using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICAN.SIC.Plugin.SIMLHub
{
    public class SIMLHubUtility
    {
        public List<string> GetAllSIMLHubPluginIndexSIMLFiles(string SIMLHubPluginDirectoryName)
        {
            List<string> result = new List<string>();

            // Check if plugins directory exists
            if (!Directory.Exists(SIMLHubPluginDirectoryName))
                return result;

            // Traverse each plugin's own directory for index.siml files
            foreach (var directoryName in Directory.GetDirectories(SIMLHubPluginDirectoryName))
            {
                // Check if index.siml file exists
                string indexSimlFilePath = Path.Combine(directoryName, "index.siml");
                if (File.Exists(indexSimlFilePath))
                {
                    result.Add(Path.GetFullPath(indexSimlFilePath));
                }
            }

            return result;
        }

        public List<string> GetAllSIMLHubPluginIndexAdapterPathAndTypes(string SIMLHubPluginDirectoryName)
        {
            List<string> result = new List<string>();

            // Check if plugins directory exists
            if (!Directory.Exists(SIMLHubPluginDirectoryName))
                return result;

            // Traverse each plugin's own directory for IndexAdapter.dll files
            foreach (var directoryName in Directory.GetDirectories(SIMLHubPluginDirectoryName))
            {
                // Check if index.siml file exists
                string indexSimlFilePath = Path.Combine(directoryName, "IndexAdapter.dll");
                if (File.Exists(indexSimlFilePath))
                {
                    result.Add(Path.GetFullPath(indexSimlFilePath));
                }
            }

            return result;
        }
    }
}
