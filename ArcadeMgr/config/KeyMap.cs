using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArcadeMgr.config
{
    public class KeyMap
    {
        public string name;
        public string group;
        public string subgroup;
        public string message;

        public KeyMap() { }

        public KeyMap(string name, string group, string subgroup, string message)
        {            
            this.name = name;
            this.group = group;
            this.subgroup = subgroup;
            this.message = message;
        }        

        public KeyMap(string name, string group, string subgroup)
        {
            this.name = name;
            this.group = group;
            this.subgroup = subgroup;
        }
    }
}
