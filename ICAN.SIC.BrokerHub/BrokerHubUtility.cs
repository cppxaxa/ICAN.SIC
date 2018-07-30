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
            namespaces.Sort(delegate(string a, string b)
            {
                if (a == null && b == null)
                    return 0;
                if (a == null)
                    return 1;
                if (b == null)
                    return -1;

                int countA = 0, countB = 0;

                if (a.StartsWith("ICAN.SIC"))
                    countA += 1;
                if (a.StartsWith("ICAN.SIC.Plugin"))
                    countA += 2;
                if (a.Length > "ICAN.SIC.Plugin".Length)
                    countA += 2;
                if (a.StartsWith("ICAN.SIC.Plugin") && a.LastIndexOf('.') > "ICAN.SIC.Plugin".Length)
                    countA -= 4;

                if (b.StartsWith("ICAN.SIC"))
                    countB += 1;
                if (b.StartsWith("ICAN.SIC.Plugin"))
                    countB += 2;
                if (b.Length > "ICAN.SIC.Plugin".Length)
                    countB += 2;
                if (b.StartsWith("ICAN.SIC.Plugin") && b.LastIndexOf('.') > "ICAN.SIC.Plugin".Length)
                    countB -= 4;

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
            return false;
        }
    }
}
