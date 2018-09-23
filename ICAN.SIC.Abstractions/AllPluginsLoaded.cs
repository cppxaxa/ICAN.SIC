using ICAN.SIC.Abstractions.IMessageVariants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICAN.SIC.Abstractions
{
    public class AllPluginsLoaded : IAllPluginsLoaded
    {
        private List<string> pluginsNames;

        public AllPluginsLoaded(List<string> pluginsNames)
        {
            this.pluginsNames = pluginsNames;
        }

        public List<string> PluginsNames
        {
            get { return this.pluginsNames; }
        }
    }
}
