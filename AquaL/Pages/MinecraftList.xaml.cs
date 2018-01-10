using AquaL.Model.Download.Minecraft;
using AquaL.Pages;
using KMCCC.Launcher;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AquaL.Pages
{
    /// <summary>
    /// MinecraftList.xaml 的交互逻辑
    /// </summary>
    public partial class MinecraftList : Page
    {
        /// <summary>
        /// 所有的正式版发布列表
        /// </summary>
        List<VersionsInfo> Releases { get; set; } = new List<VersionsInfo>();
        /// <summary>
        /// 所有的快照版发布列表
        /// </summary>
        List<VersionsInfo> Snapshots { get; set; } = new List<VersionsInfo>();
        /// <summary>
        /// 构造函数
        /// </summary>
        public MinecraftList()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 页面被加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow context = (MainWindow)Window.GetWindow(this);
            context.Title = "下载Minecraft";
            Thread getMinecraftListThread = new Thread((ThreadStart)delegate
            {
                VersionsModel m = Versions.GetAll("http://bmclapi.bangbang93.com/");
                foreach (VersionsInfo info in m.versions)
                {
                    if (info.type == "release")
                        Releases.Add(info);
                    else
                        Snapshots.Add(info);
                }
                Thread.Sleep(1000);
                Dispatcher.Invoke(delegate
                {
                    Version.DisplayMemberPath = "id";
                    lastest.Content = "最新预览版：" + m.latest.snapshot + " 最新正式版：" + m.latest.release;
                    Version.ItemsSource = Releases;
                    ProgressBar.Visibility = Visibility.Collapsed;
                    ProgressBarMain.IsActive = false;
                });
                GC.Collect();

            });
            getMinecraftListThread.SetApartmentState(ApartmentState.STA);
            getMinecraftListThread.IsBackground = true;
            getMinecraftListThread.Start();


        }
        /// <summary>
        /// 显示版本详细信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Version_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Version.SelectedItem != null)
            {
                VersionName.Content = "Minecraft " + ((VersionsInfo)Version.SelectedItem).id;
                VersionInfo.Content = "版本类型 " + ((VersionsInfo)Version.SelectedItem).type + "\n发布时间 " + ((VersionsInfo)Version.SelectedItem).releaseTime;
            }
        }
        /// <summary>
        /// 切换VersionListView显示正式版
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Snapshot_Checked(object sender, RoutedEventArgs e)
        {
            Version.ItemsSource = Snapshots;
            Version.SelectedIndex = -1;
        }
        /// <summary>
        /// 切换VersionListView显示快照版
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Release_Checked(object sender, RoutedEventArgs e)
        {
            Version.ItemsSource = Releases;
            Version.SelectedIndex = -1;
        }
        /// <summary>
        /// 下载游戏本体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Download_Click(object sender, RoutedEventArgs e)
        {
            if (Version.SelectedItem == null)
            {
                UI.MessageBox.QuickShow("选择一个版本再来下载 >__<", (MainWindow)Window.GetWindow(this));
                return;
            }
            bool result = true;
            result = AquaL.Helper.MinecraftDownloadHelper.AutoDownloadMinecraft(((VersionsInfo)Version.SelectedItem).id, Model.Download.Api.UsingApi);
            if (!result)
                return;
            result = AquaL.Helper.LibrariesDownloadHelper.AutoDownloadLibraries(((VersionsInfo)Version.SelectedItem).id, Model.Download.Api.UsingApi);
            if (!result)
                return;
            UI.MessageBox.QuickShow(
                 "游戏基础文件下载完成！您现在可以启动游戏了！\n但是目前还没有下载游戏资源文件\n您可以现在或者稍后在「下载Minecraft」App里重新下载",
                 (MainWindow)Window.GetWindow(this), "大功告成！");
        }
        /// <summary>
        /// 下载游戏资源文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownloadResources_Click(object sender, RoutedEventArgs e)
        {
            if (Version.SelectedItem == null)
            {
                UI.MessageBox.QuickShow("选择一个版本再来下载 >__<", (MainWindow)Window.GetWindow(this));
                return;
            }
            UI.MessageBox mb = new UI.MessageBox((MainWindow)Window.GetWindow(this));
            mb.Title = "注意";
            mb.Message = "Minecraft的游戏资源文件很大，可能在200M以上\n建议不要使用计费网络下载（如手机热点、上网卡）\n下载完成后将通知您";
            mb.LeftButtonText = "继续下载";
            mb.RightButtonText = "算了吧";
            mb.LeftButtonClick += (obj) => { mb.Close(); };
            bool isUserDontDownload = false;
            mb.RightButtonClick += (obj) => { mb.Close(); isUserDontDownload = true; };
            mb.ShowDialog();
            if (isUserDontDownload)
                return;
            UI.MessageBox.QuickShow("启动器将开启新线程下载，在下载完成前您可以继续游戏\n但是可能没有声音文件或者无法选择适合您的语言", (MainWindow)Window.GetWindow(this), "下载资源文件");
            Thread downloadThread = new Thread((ThreadStart)delegate
            {
                string version = "", useApi = "";
                Dispatcher.Invoke(delegate{
                    version = ((VersionsInfo)Version.SelectedItem).id;
                    useApi = Model.Download.Api.UsingApi;
                });
                bool result = AquaL.Helper.ResourcesDownloadHelper.AutoDownloadResources(
                    version,
                    useApi);
                if (!result)
                    return;
                TaskbarIconManager.Toast("AquaL资源下载", "资源文件下载完成！");
            });
            downloadThread.SetApartmentState(ApartmentState.STA);
            downloadThread.IsBackground = true;
            downloadThread.Start();
        }
    }
}
