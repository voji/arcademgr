using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace ArcadeMgr.config
{
    public class Stats
    {       
        public long seconds;
        public Dictionary<Keys, long> keyStats = new Dictionary<Keys, long>();

        public long globalKeyPressCount()
        {
            long result = 0;
            foreach (long val in keyStats.Values)
            {
                result += val;
            }
            return result;
        }
    }
}
