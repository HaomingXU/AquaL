using AquaL.Pages;
using KMCCC.Launcher;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AquaL.Helper
{
    class LibrariesDownloadHelper
    {
        public static bool AutoDownloadLibraries (string id, string usingApi)
        {
            try
            {
                var core = LauncherCore.Create();
                var version = App.Core.GetVersion(id);
                var libs = version.Libraries.Select(lib => core.GetLibPath(lib));

                List<string> file_path = new List<string>();

                var natives = version.Natives.Select(native => core.GetNativePath(native));
                foreach (string libflie in libs)
                {
                    if (!File.Exists(libflie))
                    {
                        file_path.Add(libflie.Replace("\\", "/").Replace(AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/") + ".minecraft/", ""));
                    }
                }
                foreach (string libflie in natives)
                {
                    if (!File.Exists(libflie))
                    {
                        file_path.Add(libflie.Replace("\\", "/").Replace(AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/") + ".minecraft/", ""));
                    }
                }

                string obj = AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/") + ".minecraft/\n";
                foreach (string s in file_path)
                {
                    obj = obj + s + "\n";
                }

                List<DownloadFileTask> Task = new List<DownloadFileTask>();
                foreach (string file in file_path)
                {
                    string dir = ""; // 当前下载文件所在的相对目录 （"./minecraft/libraries/xxx"）
                    dir = AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/")
                            + ".minecraft/"
                            + file.Substring(0, file.LastIndexOf("/"))
                            + "/";
                    // 判断文件夹是否存在，不存在则创建
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    string savePath = ""; // 文件下载到的位置
                    savePath = AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/")
                                    + ".minecraft/"
                                    + file;
                    string fileOnServerUrl = usingApi + "/" + file;
                    Task.Add(new DownloadFileTask(new FileInfo(file).Name, savePath, new Uri(fileOnServerUrl, UriKind.Absolute)));
                }

                MainWindow context = new MainWindow(new AquaL.Pages.FileDownload(Task));
                context.ShowDialog();
                return true;
            }
            catch(Exception ex)
            {
                UI.MessageBox.QuickShow("Libraries下载失败！\n" + ex.Message, null);
                return false;
            }
        }
    }
}
