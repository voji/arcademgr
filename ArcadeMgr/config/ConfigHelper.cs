using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace ArcadeMgr.config
{
    class ConfigHelper
    {
        private const string StatsFileName = "stats.xml";
        private const string SettingsFileName = "settings.xml";
        private static XmlWriterSettings xmlWriterSettings = new XmlWriterSettings { Indent = true };

    public static void SaveStats(Stats stats)
        {
            try
            {
                using (var writer = XmlWriter.Create(StatsFileName, xmlWriterSettings))
                {
                    IExtendedXmlSerializer serializer = new ConfigurationContainer().Create();
                    serializer.Serialize(writer, stats);
                    writer.Flush();
                }
            } catch (Exception e)
            {
                MainWindow.HandleError("Error during write file (stats.xml)", e, false);
            }
        }

        public static Stats LoadStats()
        {
            Stats result = null;
            try
            {
                if (File.Exists(StatsFileName))
                {
                    using (XmlReader reader = XmlReader.Create(StatsFileName))
                    {
                        IExtendedXmlSerializer serializer = new ConfigurationContainer().Create();
                        result = (Stats)serializer.Deserialize(reader);
                    }
                }
            } catch (Exception e)
            {
                MainWindow.HandleError("Error during read file (stats.xml)", e, false);
            }
            return (result != null ? result : new Stats());            
        }

        public static Settings LoadSettings()
        {
            try
            {
                if (File.Exists(SettingsFileName))
                {
                    using (XmlReader reader = XmlReader.Create(SettingsFileName))
                    {
                        IExtendedXmlSerializer serializer = new ConfigurationContainer().Create();
                        return (Settings)serializer.Deserialize(reader);
                    }
                }
                else
                {
                    //creating example conf
                    using (var writer = XmlWriter.Create(SettingsFileName, xmlWriterSettings))
                    {
                        IExtendedXmlSerializer serializer = new ConfigurationContainer().Create();
                        Settings exampleSettings = new Settings();
                        exampleSettings.initDefaultSettings();
                        serializer.Serialize(writer, exampleSettings);
                        writer.Flush();
                    }
                }
            }
            catch (Exception e)
            {
                MainWindow.HandleError("Error during write file (stats.xml)", e, false);
            }
            return null;
        }
    }
}
