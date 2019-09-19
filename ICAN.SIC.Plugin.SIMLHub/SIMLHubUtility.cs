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
                // Check if index.siml file exists or any siml
                foreach (var filename in Directory.EnumerateFiles(directoryName))
                {
                    if (Path.GetExtension(filename) == ".siml")
                        result.Add(Path.GetFullPath(filename));
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

            // Traverse each plugin's own directory for IndexAdapter.dll files or any *Adapter.dll
            foreach (var directoryName in Directory.GetDirectories(SIMLHubPluginDirectoryName))
            {
                foreach (var dllFile in Directory.GetFiles(directoryName))
                {
                    if (Path.GetExtension(dllFile) == ".dll")
                    {
                        if (File.Exists(dllFile))
                        {
                            result.Add(Path.GetFullPath(dllFile));
                        }
                    }
                }
            }

            return result;
        }
    }
}
