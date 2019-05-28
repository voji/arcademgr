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
        public Keys key;
        public string name;
        public string group;
        public string subgroup;
        public string message;

        public KeyMap() { }

        public KeyMap(Keys key, string name, string group, string subgroup, string message)
        {
            this.key = key;            
            this.name = name;
            this.group = group;
            this.subgroup = subgroup;
            this.message = message;
        }        

        public KeyMap(Keys key, string name, string group, string subgroup)
        {
            this.key = key;
            this.name = name;
            this.group = group;
            this.subgroup = subgroup;
        }
    }
}
