using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ICAN.SIC.BrokerHub
{
    public class BrokerHubUtility
    {
        List<string> permanentDllDependencies = new List<string>
        {
            "ICAN.SIC.Abstractions.dll"
        };

        private List<string> DirectoryCopyExceptDLLDependencies(string sourceDirName, string destDirName, bool copySubDirs, bool firstCall = true)
        {
            List<string> directoriesCopied = new List<string>();
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);

                try
                {
                    file.CopyTo(temppath);

                    if (firstCall)
                        directoriesCopied.Add(temppath);
                }
                catch
                {
                    Console.WriteLine("[INFO] Ignoring file for copy at DirectoryCopyExceptDLLDependencies() : " + file.Name);
                }
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);

                    if (firstCall && subdir.Name == "DLLDependencies")
                        continue;
                    else if (firstCall)
                        directoriesCopied.Add(temppath);
                    
                    DirectoryCopyExceptDLLDependencies(subdir.FullName, temppath, copySubDirs, false);
                }
            }

            return directoriesCopied;
        }

        public List<string> GetAllDllFiles(string[] filePaths)
        {
            List<string> result = new List<string>();

            foreach (var filePath in filePaths)
            {
                if (Path.GetExtension(filePath) == ".dll")
                {
                    result.Add(filePath);
                }
            }

            return result;
        }

        public List<string> CopyAllDllsToBaseDirectory()
        {
            List<string> newContentCopiedList = new List<string>();

            // List the plugins directory
            foreach (var pluginDirectory in Directory.GetDirectories(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins")))
            {
                Console.WriteLine("[INFO] Reading plugin directory : " + pluginDirectory);

                // Find the DLLDependencies and dll
                // Except the DLLDependencies, copy all of them to BaseDirectory
                // Finally copy all the contents of DLLDependencies to BaseDirectory

                List<string> directoriesCopied = DirectoryCopyExceptDLLDependencies(pluginDirectory, AppDomain.CurrentDomain.BaseDirectory, true);
                newContentCopiedList.AddRange(directoriesCopied);

                string pluginDllDependencyDirectoryPath = pluginDirectory + Path.DirectorySeparatorChar + "DLLDependencies";

                if (Directory.Exists(pluginDllDependencyDirectoryPath))
                {
                    foreach (var dependencyDll in Directory.GetFiles(pluginDllDependencyDirectoryPath))
                    {
                        // Check if permanent dll dependency, then don't copy or remove
                        bool flag = false;
                        foreach (var permanentDll in permanentDllDependencies)
                        {
                            if (Path.GetFileName(dependencyDll) == permanentDll)
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (flag)
                            continue;


                        try
                        {
                            string destinationFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileName(dependencyDll));

                            if (!File.Exists(destinationFile))
                            {
                                File.Copy(dependencyDll, destinationFile, true);
                                newContentCopiedList.Add(destinationFile);
                            }
                        }
                        catch
                        {
                            Console.WriteLine("[INFO] Ignoring file for copy at CopyAllDllsToBaseDirectory() : " + dependencyDll);
                        }
                    }
                }
                else
                {
                    Directory.CreateDirectory(pluginDllDependencyDirectoryPath);
                }
            }

            //throw new Exception("Testing");

            return newContentCopiedList.Distinct().ToList();
        }

        public string FindMatchingKey(Dictionary<string, PluginConfiguration> pluginConfiguration, string guessedTypeName)
        {
            foreach (var item in pluginConfiguration)
            {
                if (item.Key.IndexOf(guessedTypeName) >= 0)
                    return item.Key;
            }
            return null;
        }

        public string GetPrimaryNamespace(Assembly assembly)
        {
            List<string> namespaces;

            try
            {
                namespaces = assembly.GetTypes().Select(t => t.Namespace).Distinct().ToList();

                if (namespaces.Count == 0)
                    return "";
                else if (namespaces.Count == 1)
                    return namespaces.First();
                else if (namespaces.Count > 1)
                {
                    namespaces = this.SortForBestNamespace(namespaces);

                    return namespaces.First();
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(assembly.ToString());
                Console.WriteLine(e.Message);
                Console.ForegroundColor = ConsoleColor.Gray;
            }

            // Unrechable code
            return "";
        }

        public List<string> SortForBestNamespace(List<string> namespaces)
        {
            namespaces.Sort(delegate (string a, string b)
            {
                if (a == null && b == null)
                    return 0;
                if (a == null)
                    return 1;
                if (b == null)
                    return -1;

                int countA = 0, countB = 0;
                bool aSeemsValid = true, bSeemsValid = true;

                if (a.StartsWith("ICAN.SIC"))
                    countA += 1;
                if (a.StartsWith("ICAN.SIC.Plugin"))
                    countA += 2;
                if (a.Length > "ICAN.SIC.Plugin".Length)
                    countA += 2;
                if (a.StartsWith("ICAN.SIC.Plugin") && a.LastIndexOf('.') > "ICAN.SIC.Plugin".Length)
                {
                    countA -= 4;
                    aSeemsValid = false;
                }

                if (b.StartsWith("ICAN.SIC"))
                    countB += 1;
                if (b.StartsWith("ICAN.SIC.Plugin"))
                    countB += 2;
                if (b.Length > "ICAN.SIC.Plugin".Length)
                    countB += 2;
                if (b.StartsWith("ICAN.SIC.Plugin") && b.LastIndexOf('.') > "ICAN.SIC.Plugin".Length)
                {
                    countB -= 4;
                    bSeemsValid = false;
                }

                if (aSeemsValid && bSeemsValid)
                {
                    if (a.Length < b.Length)
                        countA += 2;
                    else
                        countB += 2;
                }

                return (countB - countA);
            });

            return namespaces;
        }

        public string GetGuessedTypeName(Assembly assembly)
        {
            // Take the word after the last dot (.)

            string primaryNamespace = this.GetPrimaryNamespace(assembly);

            int lastIndexOfDot = primaryNamespace.LastIndexOf('.');

            // Don't forget to append the namespace for full typename

            if (lastIndexOfDot < 0 || lastIndexOfDot == primaryNamespace.Length - 1)
                return primaryNamespace + "." + primaryNamespace;
            else
                return primaryNamespace + "." + primaryNamespace.Substring(lastIndexOfDot + 1);
        }

        public bool IsBrokerHubSupportedPlugin(string guessedTypeName)
        {
            if (guessedTypeName.StartsWith("ICAN.SIC.Plugin") &&
                guessedTypeName.Length > "ICAN.SIC.Plugin".Length &&
                guessedTypeName.Substring("ICAN.SIC.Plugin.".Length).LastIndexOf('.') == guessedTypeName.Substring("ICAN.SIC.Plugin.".Length).IndexOf('.')
                )
                return true;
            if (guessedTypeName.StartsWith("ICAN.SIC.Plugin.Adapter") &&
                guessedTypeName.Length > "ICAN.SIC.Plugin.Adapter".Length &&
                guessedTypeName.Substring("ICAN.SIC.Plugin.Adapter.".Length).LastIndexOf('.') == guessedTypeName.Substring("ICAN.SIC.Plugin.Adapter.".Length).IndexOf('.')
                )
                return true;
            return false;
        }
    }
}
