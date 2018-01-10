using AquaL.Pages;
using LitJson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AquaL.Helper
{
    class ResourcesDownloadHelper
    {
        public static bool AutoDownloadResources(string id, string usingApi)
        {
            try
            {
                string MINECRAFT_PATH = AppDomain.CurrentDomain.BaseDirectory + ".minecraft/";
                string SELECT_VERSION = id;
                if (!File.Exists(MINECRAFT_PATH + "versions/" + SELECT_VERSION + "/" + SELECT_VERSION + ".json"))
                {
                    UI.MessageBox.QuickShow("请先下载该版本完整的Minecraft！", null);
                    return false;
                }
                JObject versionJson = 
                    JsonConvert.DeserializeObject(File.ReadAllText(MINECRAFT_PATH + "versions/" + SELECT_VERSION + "/" + SELECT_VERSION + ".json")) as JObject;

                string resourcesJsonUrl = versionJson["assetIndex"]["url"].ToString().Replace("https://launchermeta.mojang.com", usingApi).Replace("bmclapi2", "bmclapi");
                WebClient wc = new WebClient();
                wc.Credentials = CredentialCache.DefaultCredentials;
                Encoding enc = Encoding.GetEncoding("UTF-8");
                Byte[] pageData = wc.DownloadData(resourcesJsonUrl);
                string json = enc.GetString(pageData);

                string resourcesDownloadApiDomain = usingApi;
                resourcesDownloadApiDomain = 
                    resourcesDownloadApiDomain.Replace("launchermeta.mojang.com", "resources.download.minecraft.net")
                                              .Replace("bmclapi2.bangbang93.com", "bmclapi2.bangbang93.com/assets");

                List<DownloadFileTask> task = new List<DownloadFileTask>();
                JsonData jd = JsonMapper.ToObject(json);
                IDictionary dict = jd as IDictionary; //第一个子元素中包含多个子元素
                foreach (string key in jd["objects"].Keys)
                {
                    string hash = jd["objects"][key]["hash"].ToString();
                    if (!Directory.Exists(MINECRAFT_PATH + "assets/objects/" + hash.Substring(0, 2)))
                        Directory.CreateDirectory(MINECRAFT_PATH + "assets/objects/" + hash.Substring(0, 2));
                    task.Add(new DownloadFileTask(
                        key,
                        MINECRAFT_PATH + "assets/objects/" + hash.Substring(0, 2) + "/" + hash,
                        new Uri(resourcesDownloadApiDomain + "/" + hash.Substring(0, 2) + "/" + hash)
                        ));
                }
                if (!Directory.Exists(MINECRAFT_PATH + "assets/indexes/"))
                    Directory.CreateDirectory(MINECRAFT_PATH + "assets/indexes/");
                task.Add(new DownloadFileTask(
                    "Index 文件",
                    MINECRAFT_PATH + "assets/indexes/" + versionJson["assetIndex"]["id"].ToString() + ".json",
                    new Uri(versionJson["assetIndex"]["url"].ToString().Replace("https://launchermeta.mojang.com", usingApi).Replace("bmclapi2", "bmclapi"))));

                MainWindow context = new MainWindow(new AquaL.Pages.FileDownload(task));
                context.ShowDialog();

                try
                {
                    // 判断是否使用需要将文件复制一份
                    if (jd["virtual"] != null && (bool)jd["virtual"] == true)
                    {
                        bool isNoException = true;
                        TaskbarIconManager.Toast("AquaL 下载游戏", "开始解压资源");
                        foreach (string key in jd["objects"].Keys)
                        {
                            try
                            {
                                string hash = jd["objects"][key]["hash"].ToString();
                                string hashPath = MINECRAFT_PATH + "assets/objects/" + hash.Substring(0, 2) + "/" + hash;
                                string savePath = MINECRAFT_PATH + "assets/virtual/legacy/" + key;
                                string saveDir = Path.GetDirectoryName(savePath);
                                if (!Directory.Exists(saveDir))
                                    Directory.CreateDirectory(saveDir);
                                Console.WriteLine("解压文件：文件Hash" + hash + "，目标路径：" + savePath);
                                File.Copy(hashPath, savePath);
                            }
                            catch (Exception ex) { isNoException = false; Console.WriteLine(ex.Message); }
                        }
                        TaskbarIconManager.Toast("AquaL 下载游戏", (isNoException ? "解压成功！" : "解压失败，可能部分文件出现异常"));
                    }
                }
                catch { }

                return true;
            }
            catch (Exception ex)
            {
                UI.MessageBox.QuickShow("资源文件下载失败！\n" + ex.Message, null);
                return false;
            }
        }
    }
}
