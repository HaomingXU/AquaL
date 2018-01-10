using AquaL.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AquaL.Helper
{
    class MinecraftDownloadHelper
    {
        public static bool AutoDownloadMinecraft (string id, string usingApi)
        {
            string MINECRAFT_PATH = AppDomain.CurrentDomain.BaseDirectory + "/.minecraft/";
            string SELECT_VERSION = id;
            // 判断 .minecraft/versions/版本号 文件夹是否存在，否则创建
            if (!Directory.Exists(MINECRAFT_PATH + "versions/" + SELECT_VERSION + "/"))
                Directory.CreateDirectory(MINECRAFT_PATH + "/versions/" + SELECT_VERSION + "/");
            else // 文件夹存在，询问是否覆盖
            {
                // 资源文件完整情况下
                if (File.Exists(MINECRAFT_PATH + "/versions/" + SELECT_VERSION + "/" + SELECT_VERSION + ".jar") &&
                    File.Exists(MINECRAFT_PATH + "/versions/" + SELECT_VERSION + "/" + SELECT_VERSION + ".json"))
                {
                    UI.MessageBox mb = new UI.MessageBox(null);
                    mb.Title = "已存在";
                    mb.Message = "该版本已存在，是否重新下载？";
                    mb.LeftButtonText = "重新下载";
                    mb.RightButtonText = "不要重下";
                    mb.LeftButtonClick += (obj) => { mb.Close(); };
                    bool isUserDontDownload = false;
                    mb.RightButtonClick += (obj) => { mb.Close(); isUserDontDownload = true; };
                    mb.ShowDialog();
                    if (isUserDontDownload)
                        return false;
                }
                // 资源文件不完整情况下
                else if (File.Exists(MINECRAFT_PATH + "/versions/" + SELECT_VERSION + "/" + SELECT_VERSION + ".jar") ||
                    File.Exists(MINECRAFT_PATH + "/versions/" + SELECT_VERSION + "/" + SELECT_VERSION + ".json"))
                {
                    UI.MessageBox mb = new UI.MessageBox(null);
                    mb.Title = "已存在";
                    mb.Message = "该版本已存在，但是不完整，是否重新下载？\n如果不重新下载，可能会导致无法正常游戏";
                    mb.LeftButtonText = "重新下载";
                    mb.RightButtonText = "不要重下";
                    mb.LeftButtonClick += (obj) => { mb.Close(); };
                    bool isUserDontDownload = false;
                    mb.RightButtonClick += (obj) => { mb.Close(); isUserDontDownload = true; };
                    mb.ShowDialog();
                    if (isUserDontDownload)
                        return false;
                }
            }

            try
            {
                List<DownloadFileTask> task = new List<DownloadFileTask>();
                task.Add(new DownloadFileTask(
                    SELECT_VERSION + "-Jar",
                    MINECRAFT_PATH + "/versions/" + SELECT_VERSION + "/" + SELECT_VERSION + ".jar",
                    new Uri(usingApi + "/version/" + SELECT_VERSION + "/client")));
                task.Add(new DownloadFileTask(
                    SELECT_VERSION + "-Json",
                    MINECRAFT_PATH + "/versions/" + SELECT_VERSION + "/" + SELECT_VERSION + ".json",
                    new Uri(usingApi + "/version/" + SELECT_VERSION + "/json")));
                MainWindow context = new MainWindow(new AquaL.Pages.FileDownload(task));
                context.ShowDialog();
                return true;
            }
            catch (Exception ex)
            {
                UI.MessageBox.QuickShow("Minecraft本体下载失败！\n" + ex.Message, null);
                return false;
            }
        }
    }
}
