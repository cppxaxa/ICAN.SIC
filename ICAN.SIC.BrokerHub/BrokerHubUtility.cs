using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

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

            namespaces = assembly.GetTypes().Select(t => t.Namespace).Distinct().ToList();

            if (namespaces.Count == 0)
                return "";
            else if (namespaces.Count == 1)
                return namespaces.First();
            else if (namespaces.Count > 1)
            {
                namespaces.Sort(delegate(string a, string b)
                {
                    int countA = 0, countB = 0;

                    if (a.StartsWith("ICAN.SIC"))
                        countA += 1;
                    if (a.StartsWith("ICAN.SIC.Plugin"))
                        countA += 2;

                    if (b.StartsWith("ICAN.SIC"))
                        countB += 1;
                    if (b.StartsWith("ICAN.SIC.Plugin"))
                        countB += 2;


                    return (countA - countB);
                });

                return namespaces.First();
            }

            // Unrechable code
            return null;
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
    }
}
