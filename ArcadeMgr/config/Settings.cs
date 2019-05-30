using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArcadeMgr.config
{
    public class Settings
    {
        public string portName;
        public int baudRate;
        public int startDelay;
        public string startMessage;
        public string stopMessage;
        public Dictionary<Keys,KeyMap> keyMap = new Dictionary<Keys, KeyMap>();

        public void initDefaultSettings()
        {
            portName = "COMX";
            baudRate = 115200;
            startDelay = 5;
            startMessage = "n"; //on
            stopMessage = "f"; //off
            //default jpac2 keymap
            keyMap[Keys.D5] = new KeyMap("P1", "Coin", "Coin", "c"); //coin
            keyMap[Keys.D6] = new KeyMap("P1", "Coin", "Coin", "c"); //coin
            keyMap[Keys.D1] = new KeyMap("P1", "Start", "Start");
            keyMap[Keys.D2] = new KeyMap("P2", "Start", "Start");
            keyMap[Keys.Up] = new KeyMap("P1", "Up", "Move");
            keyMap[Keys.Down] = new KeyMap("P1", "Down", "Move");
            keyMap[Keys.Left] = new KeyMap("P1", "Left", "Move");
            keyMap[Keys.Right] = new KeyMap("P1", "Right", "Move");
            keyMap[Keys.R] = new KeyMap("P2", "Up", "Move");
            keyMap[Keys.F] = new KeyMap("P2", "Down", "Move");
            keyMap[Keys.D] = new KeyMap("P2", "Left", "Move");
            keyMap[Keys.G] = new KeyMap("P2", "Right", "Move");
            keyMap[Keys.LControlKey] = new KeyMap("P1", "B1", "Button");
            keyMap[Keys.Alt] = new KeyMap("P1", "B2", "Button");
            keyMap[Keys.Space] = new KeyMap("P1", "B3", "Button");
            keyMap[Keys.LShiftKey] = new KeyMap("P1", "B4", "Button");
            keyMap[Keys.Z] = new KeyMap("P1", "B5", "Button");
            keyMap[Keys.X] = new KeyMap("P1", "B6", "Button");
            keyMap[Keys.A] = new KeyMap("P2", "B1", "Button");
            keyMap[Keys.S] = new KeyMap("P2", "B2", "Button");
            keyMap[Keys.Q] = new KeyMap("P2", "P3", "Button");
            keyMap[Keys.W] = new KeyMap("P2", "B4", "Button");
            keyMap[Keys.I] = new KeyMap("P2", "B5", "Button");
            keyMap[Keys.K] = new KeyMap("P2", "B6", "Button");
        }
             
    }

}
