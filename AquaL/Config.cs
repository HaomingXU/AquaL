using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AquaL
{
    class Config
    {
        public static bool IsFirstConfiging = false;

        public static void Init()
        {
            if (!File.Exists("aqual.json"))
                File.Create("aqual.json").Close();
            string json = File.ReadAllText("aqual.json");
            if (json == "")
            {
                IsFirstConfiging = true;
                MainWindow fw = new MainWindow(new Pages.Setups.Start());
                fw.ShowDialog();
            }
        }

        public static ConfigModel GetConfig()
        {
            string json = File.ReadAllText("aqual.json");
            ConfigModel cm = new ConfigModel();
            if (JsonConvert.DeserializeObject<ConfigModel>(json) != null)
                cm = JsonConvert.DeserializeObject<ConfigModel>(json);
            return cm;
        }

        public static void SaveConfig(ConfigModel obj)
        {
            string json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            File.WriteAllText("aqual.json", json, Encoding.UTF8);
        }
    }

    public class ConfigModel
    {
        public ConfigUser[] users { get; set; }
    }

    public class ConfigUser
    {
        public string name { get; set; }
        public string pass { get; set; }
        public string type { get; set; }
        public string theme { get; set; }
        public string background { get; set; }
        public ConfigAutoJoinServer auto_join_server { get; set; }
        public ConfigCustomMinecraftWindowSize custom_minecraft_window_size { get; set; }
        public ConfigJava java { get; set; }
        public string launch_parms { get; set; }
        public ConfigJavas[] javas { get; set; }
    }

    public class ConfigAutoJoinServer
    {
        public string address { get; set; }
        public int port { get; set; }
    }

    public class ConfigCustomMinecraftWindowSize
    {
        public int width { get; set; }
        public int height { get; set; }
    }

    public class ConfigJava
    {
        public string jre_path { get; set; }
        public int min_memory { get; set; }
        public int max_memory { get; set; }
        public bool is_auto_memory { get; set; }
    }

    public class ConfigJavas
    {
        public bool? is_no_tip_update_32bit { get; set; }
        public string javaw_path { get; set; }
    }
}
