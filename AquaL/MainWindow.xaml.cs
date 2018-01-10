using KMCCC.Authentication;
using KMCCC.Launcher;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Diagnostics;
using AquaL.Helper;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace AquaL
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {


        Page DefaultPage = null; // 启动窗口后进入的Page
        bool thisWindowIsMainWindow = false; // 这个是不是主窗口
        /// <summary>
        /// 此窗口是否为新建窗口
        /// </summary>
        public readonly bool IsNewWindow = false;
        /// <summary>
        /// 构造函数
        /// </summary>
        public MainWindow()
        {
            Config.Init();
            TaskbarIconManager.InitTaskbarManager();
            TaskbarIconManager.nf.Click += (sender, e) =>
            {
                this.WindowState = WindowState.Normal;
            };
            DefaultPage = new Pages.Main();
            thisWindowIsMainWindow = true;
            InitializeComponent();
        }
        /// <summary>
        /// 指定进入哪个页面的构造函数
        /// </summary>
        /// <param name="page"></param>
        public MainWindow(Page page)
        {
            DefaultPage = page;
            IsNewWindow = true;
            InitializeComponent();
        }
        /// <summary>
        /// 窗口被加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (thisWindowIsMainWindow)
                if (Config.IsFirstConfiging)
                    FirstSetupTip.Visibility = Visibility.Visible; // 显示新手指导
            games.ItemsSource = App.Core.GetVersions().ToArray();
            Main.Content = DefaultPage;
            App.Core.GameExit += (launchHandle, i) =>
            {
                Dispatcher.Invoke(delegate {
                    this.WindowState = WindowState.Normal;
                });
            };
        }
        /// <summary>
        /// 移动窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TitleArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// 最小化窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MinBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        /// <summary>
        /// ActionBar上的启动按钮被按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LaunchBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (games.SelectedItem == null)
            {
                UI.MessageBox.QuickShow("不选择一个版本怎么玩啊 >_<\n当然也可以下一个", (MainWindow)Window.GetWindow(this), "请选择版本");
                return;
            }
            TaskbarIconManager.Toast("启动游戏中..", "启动器将自动隐藏，你可以点击任务栏图标打开");
            this.WindowState = WindowState.Minimized;
            var ver = (KMCCC.Launcher.Version)games.SelectedItem;
            LaunchOptions options = new LaunchOptions();
            options.Mode = LaunchMode.MCLauncher;
            // 正版验证
            if (Config.GetConfig().users[0].type == "online")
            {
                options.Authenticator = new YggdrasilLogin(Config.GetConfig().users[0].name, Config.GetConfig().users[0].pass, true);
            }
            else
            {
                options.Authenticator = new OfflineAuthenticator(Config.GetConfig().users[0].name);
            }
            // 自动选择Java
            string AutoSelectJavaJre()
            {
                if (Config.GetConfig().users[0].java.jre_path == null || Config.GetConfig().users[0].java.jre_path == "")
                {
                    // 取64位Java列表
                    if (OSHelper.IsWindows64Bit)
                    {
                        List<JavaInfo> tankDayBitJavas = new List<JavaInfo>(); // 64Bit JavaJres, Don't talk the tank day
                        foreach (JavaInfo javaInfo in OSHelper.GetJavaList())
                        {
                            if (javaInfo.Bit == 64) // Is Bit is tank day, bye, i'm going to jail
                                tankDayBitJavas.Add(javaInfo);
                        }
                        if (tankDayBitJavas.Count != 0)
                        {
                            ConfigModel cfModel = Config.GetConfig();
                            cfModel.users[0].java.jre_path = tankDayBitJavas[0].JavaW;
                            Config.SaveConfig(cfModel);
                            return tankDayBitJavas[0].JavaW;
                        }
                    }
                    if (OSHelper.GetJavaList().Count != 0)
                    {
                        ConfigModel cfModel = Config.GetConfig();
                        cfModel.users[0].java.jre_path = OSHelper.GetJavaList()[0].JavaW;
                        Config.SaveConfig(cfModel);
                        return OSHelper.GetJavaList()[0].JavaW;
                    }
                    return "";
                }
                else
                {
                    return Config.GetConfig().users[0].java.jre_path;
                }
            }
            // Java
            if (Config.GetConfig().users[0].java != null)
            {
                if (!Config.GetConfig().users[0].java.is_auto_memory)
                    options.MaxMemory = Config.GetConfig().users[0].java.max_memory;
                else
                {
                    if (Config.GetConfig().users[0].java.is_auto_memory)
                    {
                        // 自动分配内存大小
                        ulong AutoCalcJavaMaxMemory(string JavaWPath)
                        {
                            ulong giveMemory = 0;
                            JavaInfo ji = new JavaInfo() { JavaW = JavaWPath };
                            ulong deviceMemory = KMCCC.Tools.SystemTools.GetTotalMemory();
                            deviceMemory = deviceMemory / 1024; // B -> KB
                            deviceMemory = deviceMemory / 1024; // KB -> M
                            if (ji.Bit != 64)
                            {
                                // 非64位
                                if (deviceMemory <= 512)
                                    giveMemory = 128;
                                else if (deviceMemory <= 1024)
                                    giveMemory = 256;
                                else if (deviceMemory <= 2048)
                                    giveMemory = 512;
                                else
                                    giveMemory = 1024;
                            }
                            else
                            {
                                // 64位
                                if (deviceMemory <= 2048)
                                    giveMemory = 512;
                                else if (deviceMemory <= 4096)
                                    giveMemory = 2048;
                                else if (deviceMemory <= 8192)
                                    giveMemory = 4096;
                                else if (deviceMemory <= 16384)
                                    giveMemory = 8192;
                                else
                                    giveMemory = 8192;
                            }
                            return giveMemory;
                        }
                        if (AutoSelectJavaJre() == "")
                        {
                            UI.MessageBox.QuickShow("您，请去安装Java\nJava不安装你都想和MC玩？\nJava下载地址： http://www.java.com", this);
                            return;
                        }
                        else
                            options.MaxMemory = (int)AutoCalcJavaMaxMemory(AutoSelectJavaJre());
                    }
                }
                if (Config.GetConfig().users[0].java.min_memory != 0)
                    options.MinMemory = Config.GetConfig().users[0].java.min_memory;
                App.Core.JavaPath = AutoSelectJavaJre();
            }
            // 判断系统是否64位然后判断Java是否64位，如果Java是32位则提示更换64位
            if (Helper.OSHelper.IsWindows64Bit)
            {
                if (Config.GetConfig().users[0].javas != null)
                {
                    Helper.JavaInfo ji = new Helper.JavaInfo();
                    ji.JavaW = App.Core.JavaPath;
                    if (ji.Bit != 64)
                    {

                        int thisJavaInJavasIndex = 0;
                        for (int i = 0; i< Config.GetConfig().users[0].javas.Length; i++)
                        {
                            if(Config.GetConfig().users[0].javas[i].javaw_path == App.Core.JavaPath)
                            {
                                thisJavaInJavasIndex = i;
                                break;
                            }
                        }
                        if (Config.GetConfig().users[0].javas[thisJavaInJavasIndex].is_no_tip_update_32bit == true)
                            return;

                        UI.MessageBox mb = new UI.MessageBox(this);
                        mb.Title = "为了更流畅的游戏";
                        mb.Message = "我们检测到您的Java是32位的，但是您的系统是64位的\n而64位的Java性能更好，更适合进行游戏\n我们建议您在「设置」内更换或者在「下载Java」内下载";
                        mb.LeftButtonText = "我知道了，不要再提醒我了";
                        mb.RightButtonText = "我现在就换";
                        bool userWantChangeJava = false;
                        mb.LeftButtonClick += (arg) =>
                        {
                            ConfigModel m = Config.GetConfig();
                            m.users[0].javas[thisJavaInJavasIndex].is_no_tip_update_32bit = true;
                            Config.SaveConfig(m);
                        };
                        mb.RightButtonClick += (arg) =>
                        {
                            userWantChangeJava = true;
                        };
                        mb.ShowDialog();
                        if (userWantChangeJava)
                            return;
                    }
                }
            }
            
            options.Version = ver;
            options.VersionType = "AquaL";
            // 自动进入服务器
            if (Config.GetConfig().users[0].auto_join_server != null &&
                Config.GetConfig().users[0].auto_join_server.address != null)
            {
                ServerInfo si = new ServerInfo();
                si.Address = Config.GetConfig().users[0].auto_join_server.address;
                if (Config.GetConfig().users[0].auto_join_server.port != 0)
                    si.Port = (ushort)Config.GetConfig().users[0].auto_join_server.port;
                options.Server = si;
            }
            // 设置窗口大小
            if (Config.GetConfig().users[0].custom_minecraft_window_size != null)
            {
                if (Config.GetConfig().users[0].custom_minecraft_window_size.width != 0 &&
                    Config.GetConfig().users[0].custom_minecraft_window_size.height != 0)
                {
                    WindowSize ws = new WindowSize();
                    if (Config.GetConfig().users[0].custom_minecraft_window_size.width < 0 &&
                        Config.GetConfig().users[0].custom_minecraft_window_size.height < 0)
                    {
                        ws.FullScreen = true;
                    }
                    else
                    {
                        ws.Height = (ushort)Config.GetConfig().users[0].custom_minecraft_window_size.height;
                        ws.Width = (ushort)Config.GetConfig().users[0].custom_minecraft_window_size.width;
                    }
                    options.Size = ws;
                }
            }
            
            var result = App.Core.Launch(options);
            if (!result.Success)
            {
                switch (result.ErrorType)
                {
                    case ErrorType.NoJAVA:
                        UI.MessageBox.QuickShow("你系统的Java有异常，建议去重新下载最新版本安装", this, "Java 错误");
                        break;
                    case ErrorType.AuthenticationFailed:
                        UI.MessageBox.QuickShow("Minecraft正版验证失败！请检查你的账号密码", this, "验证失败");
                        break;
                    case ErrorType.UncompressingFailed:
                        UI.MessageBox mb = new UI.MessageBox(this);
                        mb.Message = "游戏运行库文件异常，请确定是不是已经开了一个游戏。\n如果确定不是则可以试试重新下载";
                        mb.Title = "运行库异常";
                        mb.LeftButtonText = "重新下载运行库";
                        mb.LeftButtonClick += (args) =>
                        {
                            mb.Close();
                            try
                            {
                                AquaL.Helper.LibrariesDownloadHelper.AutoDownloadLibraries(((KMCCC.Launcher.Version)games.SelectedItem).Id, Model.Download.Api.UsingApi);
                                LaunchBtn_MouseDown(null, null);
                                return;
                            }
                            catch
                            {
                                UI.MessageBox.QuickShow("版本信息获取失败！请确认已选择正确的核心版本", this);
                            }
                        };
                        mb.RightButtonText = "关闭";
                        mb.ShowDialog();
                        break;
                    default:
                        string writeData = "KMCCC报错信息： " + result.ErrorMessage;
                        writeData = writeData + "\nUnix时间： " + (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
                        if (result.Exception != null)
                            writeData = writeData + "\n异常信息： " + result.Exception.Message + "\n调用堆栈： " + result.Exception.StackTrace;
                        try
                        {
                            FileStream logFs = new FileStream("aqual.log", FileMode.Create);
                            StreamWriter logSw = new StreamWriter(logFs);
                            logSw.Write(writeData);
                            logSw.Flush();
                            logSw.Close();
                            logFs.Close();
                            UI.MessageBox.QuickShow("启动失败！请将游戏目录下的 aqual.log 文件发送给 cncreatec@gmail.com 尝试获得帮助！ ", this, "启动错误");
                        }
                        catch (Exception)
                        {
                            UI.MessageBox.QuickShow("启动失败！请将此窗口截图发送至 cncreatec@gmail.com 来寻求帮助\n"
                                + result.ErrorMessage + "\n"
                                + (result.Exception != null ? result.Exception.StackTrace : ""), this, "启动错误");
                        }
                        break;
                }
                this.Visibility = Visibility.Visible;
            }
        }
        /// <summary>
        /// 控制Frame返回上一级
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NaviBack_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.NavigationBackClick != null)
            {
                NavigationBackClick();
                return;
            }
            if (Main.CanGoBack)
                Main.GoBack();
        }
        /// <summary>
        /// 当Frame切换页面时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Navigated(object sender, NavigationEventArgs e)
        {
            if (!Main.CanGoBack)
                NaviBack.Visibility = Visibility.Collapsed;
            else
                NaviBack.Visibility = Visibility.Visible;
            games.ItemsSource = App.Core.GetVersions().ToArray();
            DisplayLaunchGameBar = false;
        }
        /// <summary>
        /// 窗口标题
        /// </summary>
        public new string Title { get { return base.Title; } set { WTitle.Content = value; base.Title = value; } }
        /// <summary>
        /// 是否显示启动游戏菜单
        /// </summary>
        public bool DisplayLaunchGameBar
        {
            get
            {
                if (LaunchBar.Visibility == Visibility.Visible)
                    return true;
                else
                    return false;
            }
            set
            {
                if (value)
                    LaunchBar.Visibility = Visibility.Visible;
                else
                    LaunchBar.Visibility = Visibility.Collapsed;
            }
        }
        /// <summary>
        /// 窗口将要关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (thisWindowIsMainWindow)
                TaskbarIconManager.UnLoad();
            if (Main.Content.GetType() == typeof(Pages.Setups.Start) )
            {
                if (!Config.IsFirstConfiging)
                {
                    Process[] myproc = Process.GetProcesses();
                    foreach (Process item in myproc)
                    {
                        if (item.ProcessName == System.Diagnostics.Process.GetCurrentProcess().ProcessName)
                        {
                            item.Kill();
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 引导页面的开始使用按钮被点下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartUse_Click(object sender, RoutedEventArgs e)
        {
            FirstSetupTip.Visibility = Visibility.Collapsed;
            Config.IsFirstConfiging = false;
        }
        /// <summary>
        /// 是否显示关闭按钮
        /// </summary>
        public bool IsDisplayCloseBtn
        {
            get
            {
                if (CloseBtn.Visibility == Visibility.Visible)
                    return true;
                else
                    return false;
            }
            set
            {
                if (value)
                    CloseBtn.Visibility = Visibility.Visible;
                else
                    CloseBtn.Visibility = Visibility.Collapsed;
            }
        }
        /// <summary>
        /// 是否显示返回按钮
        /// </summary>
        public bool IsDisplayBackBtn
        {
            get
            {
                if (NaviBack.Visibility == Visibility.Visible)
                    return true;
                else
                    return false;
            }
            set
            {
                if (value)
                    NaviBack.Visibility = Visibility.Visible;
                else
                    NaviBack.Visibility = Visibility.Collapsed;
            }
        }
        /// <summary>
        /// 是否显示灰色半透明背景（常用于MsgBox）
        /// </summary>
        public bool IsDisplayMsgBg
        {
            get
            {
                if (MsgBg.Visibility == Visibility.Visible)
                    return true;
                else
                    return false;
            }
            set
            {
                if (value)
                    MsgBg.Visibility = Visibility.Visible;
                else
                    MsgBg.Visibility = Visibility.Collapsed;
            }
        }
        /// <summary>
        /// 导航栏上的返回键被按下
        /// </summary>
        public delegate void NavigationBackClickHandle();
        /// <summary>
        /// 导航栏上的返回键被按下
        /// </summary>
        public event NavigationBackClickHandle NavigationBackClick;
    }
}
