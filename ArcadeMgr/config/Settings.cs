using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcadeMgr.config
{
    public class Settings
    {
        public string portName;
        public int baudRate;
        public string startMessage;
        public string stopMessage;
        public KeyMap[] keyMap;

        public void initDefaultSettings()
        {
            portName = "COMX";
            baudRate = 115200;
            startMessage = "n"; //on
            startMessage = "f"; //off
            //default jpac2 keymap
            keyMap = new KeyMap[]
            {
                new KeyMap(System.Windows.Forms.Keys.D5, "P1 Coin", "P1", "Coin", "C"), //coin
                new KeyMap(System.Windows.Forms.Keys.D6, "P2 Coin", "P2", "Coin", "C"), //coin
                new KeyMap(System.Windows.Forms.Keys.D1, "P1 Start", "P1", "Start"),
                new KeyMap(System.Windows.Forms.Keys.D2, "P2 Start", "P2", "Start"),
                new KeyMap(System.Windows.Forms.Keys.Up, "P1 Up", "P1", "Move"),
                new KeyMap(System.Windows.Forms.Keys.Down, "P1 Down", "P1", "Move"),
                new KeyMap(System.Windows.Forms.Keys.Left, "P1 Left", "P1", "Move"),
                new KeyMap(System.Windows.Forms.Keys.Right, "P1 Right", "P1", "Move"),
                new KeyMap(System.Windows.Forms.Keys.R, "P2 Up", "P2", "Move"),
                new KeyMap(System.Windows.Forms.Keys.F, "P2 Down", "P2", "Move"),
                new KeyMap(System.Windows.Forms.Keys.D, "P2 Left", "P2", "Move"),
                new KeyMap(System.Windows.Forms.Keys.G, "P2 Right", "P2", "Move"),
                new KeyMap(System.Windows.Forms.Keys.LControlKey, "P1 B1", "P1", "Button"),
                new KeyMap(System.Windows.Forms.Keys.Alt, "P1 B2", "P1", "Button"),
                new KeyMap(System.Windows.Forms.Keys.Space, "P1 B3", "P1", "Button"),
                new KeyMap(System.Windows.Forms.Keys.LShiftKey, "P1 B4", "P1", "Button"),
                new KeyMap(System.Windows.Forms.Keys.Z, "P1 B5", "P1", "Button"),
                new KeyMap(System.Windows.Forms.Keys.X, "P1 B6", "P1", "Button"),
                new KeyMap(System.Windows.Forms.Keys.A, "P2 B1", "P1", "Button"),
                new KeyMap(System.Windows.Forms.Keys.S, "P2 B2", "P2", "Button"),
                new KeyMap(System.Windows.Forms.Keys.Q, "P2 B3", "P2", "Button"),
                new KeyMap(System.Windows.Forms.Keys.W, "P2 B4", "P2", "Button"),
                new KeyMap(System.Windows.Forms.Keys.I, "P2 B5", "P2", "Button"),
                new KeyMap(System.Windows.Forms.Keys.K, "P2 B6", "P2", "Button"),
            };
        }
             
    }

}
