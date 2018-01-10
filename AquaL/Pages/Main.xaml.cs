using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AquaL.Pages
{
    /// <summary>
    /// Main.xaml 的交互逻辑
    /// </summary>
    public partial class Main : Page
    {
        bool isLoaded = false; // 防止Frame导航到别的页面切回来后多次加载
        UI.Icon launchPadIcon = null; // “应用列表” 图标
        /// <summary>
        /// 构造函数
        /// </summary>
        public Main()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 页面加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            if (!isLoaded)
            {
                // 加载背景图片
                if (Directory.Exists("Backgrounds"))
                {
                    // TODO 判断程序目录下是否存在Backgrounds文件夹，
                    //      如果有则遍历文件夹内所有文件并随机一张jpg /png/jpeg/bmp作为壁纸
                    DirectoryInfo dirInfo = new DirectoryInfo("Backgrounds"); // 获取目录
                    List<FileInfo> filesInfo = dirInfo.GetFiles().ToList(); // 获取文件列表
                    List<FileInfo> caseFilesInfo = new List<FileInfo>(); // 图片列表
                                                                         // 遍历文件
                    foreach (FileInfo fi in filesInfo)
                    {
                        // 判断是否为图片后缀名
                        if (fi.Extension == ".png" || fi.Extension == ".jpg"
                            || fi.Extension == ".jpeg" || fi.Extension == ".bmp")
                            caseFilesInfo.Add(fi);
                    }
                    // 如果图片列表不为空则随机一张图片
                    if (caseFilesInfo.Count != 0)
                    {
                        int randomSelectImageIndex = 0;
                        Random rd = new Random();
                        randomSelectImageIndex = rd.Next(0, caseFilesInfo.Count);
                        // 加载图片并设置为壁纸
                        BitmapImage bm = new BitmapImage(new Uri(caseFilesInfo[randomSelectImageIndex].FullName, UriKind.Absolute));
                        Background.Source = bm;
                    }

                }
                ConfigModel cm = Config.GetConfig();
                if (cm.users[0].background != null)
                {
                    BitmapImage bm = new BitmapImage(new Uri(cm.users[0].background, UriKind.RelativeOrAbsolute));
                    Background.Source = bm;
                }

                InitIcons();
                


                isLoaded = true;
            }
            MainWindow context = (MainWindow)Window.GetWindow(this);
            context.Title = "AquaL";
            context.DisplayLaunchGameBar = true;
        }
        /// <summary>
        /// 初始化图标
        /// </summary>
        void InitIcons()
        {
            // LaunchPad 图标
            launchPadIcon = new UI.Icon(
                new ImageBrush(
                    new BitmapImage(
                        new Uri("pack://application:,,,/Icons/apps.png")
                    )
            ), "应用列表", "AquaL.LaunchPad");
            launchPadIcon.IconClick += (apppackage) =>
            {
                BlurEffect blurEffect = new BlurEffect();
                blurEffect.Radius = 20;
                Background.Effect = blurEffect;
                Apps.Visibility = Visibility.Visible;
                launchPadIcon.Visibility = Visibility.Collapsed;
            };
            launchPadIcon.HorizontalAlignment = HorizontalAlignment.Left;
            launchPadIcon.VerticalAlignment = VerticalAlignment.Bottom;
            launchPadIcon.Margin = new Thickness(20);
            MainGrid.Children.Add(launchPadIcon);

            #region 内置功能的图标
            // 下载Minecraft
            UI.Icon downloadMinecraft = new UI.Icon(
                new ImageBrush(
                    new BitmapImage(
                        new Uri("pack://application:,,,/Icons/download.png")
                    )
            ), "下载Minecraft", "AquaL.DownloadMinecraft");
            downloadMinecraft.IconClick += (apppackage) =>
            {
                NavigationService.GetNavigationService(this).Navigate(new Uri("pack://application:,,,/Pages/MinecraftList.xaml"));
            };
            downloadMinecraft.IconPopMenu += (apppackage) =>
            {
                ContextMenu menu = new ContextMenu();
                MenuItem openOnNewWindow = new MenuItem();
                openOnNewWindow.Header = "在新窗口中打开";
                openOnNewWindow.Click += (s, e) =>
                {
                    MainWindow fw = new MainWindow(new Pages.MinecraftList());
                    fw.Show();
                };
                menu.Items.Add(openOnNewWindow);
                menu.IsOpen = true;
            };
            AppIconPanel.Children.Add(downloadMinecraft);
            // 设置
            UI.Icon settings = new UI.Icon(
                new ImageBrush(
                    new BitmapImage(
                        new Uri("pack://application:,,,/Icons/settings.png")
                    )
                ), "设置", "AquaL.Settings");
            settings.IconClick += (apppackage) =>
            {
                NavigationService.GetNavigationService(this).Navigate(new Uri("pack://application:,,,/Pages/SettingsApp/Main.xaml"));
            };
            settings.IconPopMenu += (apppackage) =>
            {
                ContextMenu menu = new ContextMenu();
                MenuItem openOnNewWindow = new MenuItem();
                openOnNewWindow.Header = "在新窗口中打开";
                openOnNewWindow.Click += (s, e) =>
                {
                    MainWindow fw = new MainWindow(new Pages.Settings());
                    fw.Show();
                };
                menu.Items.Add(openOnNewWindow);
                menu.IsOpen = true;
            };
            AppIconPanel.Children.Add(settings);
            // 下载Java
            UI.Icon downloadJava = new UI.Icon(
                new ImageBrush(
                    new BitmapImage(
                        new Uri("pack://application:,,,/Icons/java.png")
                    )
            ), "下载Java", "AquaL.DownloadJava");
            downloadJava.IconClick += (apppackage) =>
            {
                NavigationService.GetNavigationService(this).Navigate(new Uri("pack://application:,,,/Pages/DownloadJava.xaml"));
            };
            downloadJava.IconPopMenu += (apppackage) =>
            {
                ContextMenu menu = new ContextMenu();
                MenuItem openOnNewWindow = new MenuItem();
                openOnNewWindow.Header = "在新窗口中打开";
                openOnNewWindow.Click += (s, e) =>
                {
                    MainWindow fw = new MainWindow(new AquaL.Pages.DownloadJava());
                    fw.Show();
                };
                menu.Items.Add(openOnNewWindow);
                menu.IsOpen = true;
            };
            AppIconPanel.Children.Add(downloadJava);
            #endregion
        }
        /// <summary>
        /// 关闭应用列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppsClose_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Background.Effect = null;
            Apps.Visibility = Visibility.Collapsed;
            launchPadIcon.Visibility = Visibility.Visible;
        }
    }
}
