using AquaL.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Settings.xaml 的交互逻辑
    /// </summary>
    public partial class Settings : Page
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Settings()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 设置被加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PageGrid_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow context = (MainWindow)Window.GetWindow(this);
            context.Title = "设置";
            ConfigModel config = Config.GetConfig();
            LaunchSet_LoginTip.Content = 
                "目前是"
                + (config.users[0].type == "online" ? "正版" : "离线")
                + "模式，账号是" + config.users[0].name;
            if (config.users[0].java != null)
                LaunchSet_JavaJreMemorySize.Text = config.users[0].java.max_memory.ToString();
            if (LaunchSet_JavaJreMemorySize.Text == "")
                LaunchSet_JavaJreMemorySize.Text = "0";
            try
            {
                LaunchSet_JavaJreList.ItemsSource = Helper.OSHelper.GetJavaList();
                LaunchSet_JavaJreList.DisplayMemberPath = "JavaW";
                LaunchSet_JavaJreList.SelectedIndex = 0;
                if (config.users[0].java != null) // 选择之前设置的Java
                    if (config.users[0].java.jre_path != null || config.users[0].java.jre_path != "")
                    {
                        int i = 0;
                        List<JavaInfo> ji = Helper.OSHelper.GetJavaList();
                        foreach (JavaInfo ij in ji)
                        {
                            if (ij.JavaW == config.users[0].java.jre_path)
                            {
                                LaunchSet_JavaJreList.SelectedIndex = i;
                            }
                            i++;
                        }
                    }
            }
            catch
            {
                UI.MessageBox.QuickShow("Minecraft需要Java，但是你还没有安装Java，请去下载安装最新版本！否则无法启动Minecraft", (MainWindow)Window.GetWindow(this));
            } // Java列表
            About_Version.Content = "Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

        }
        /// <summary>
        /// Java虚拟机内存大小设置输入框，限制只能输入数字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LaunchSet_JavaJreMemorySize_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
            {
                e.Handled = false;
            }
            else if ((e.Key >= Key.D0 && e.Key <= Key.D9))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }
        /// <summary>
        /// Java虚拟机大小设置框被移出焦点，保存设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LaunchSet_JavaJreMemorySize_LostFocus(object sender, RoutedEventArgs e)
        {
            ConfigModel config = Config.GetConfig();
            if (config.users[0].java == null)
                config.users[0].java = new ConfigJava();
            int num = 0;
            int.TryParse(LaunchSet_JavaJreMemorySize.Text, out num);
            config.users[0].java.max_memory = num;
            Config.SaveConfig(config);
        }
        /// <summary>
        /// Java虚拟机选择框被移出焦点，保存设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LaunchSet_JavaJreList_LostFocus(object sender, RoutedEventArgs e)
        {
            ConfigModel config = Config.GetConfig();
            if (config.users[0].java == null)
                config.users[0].java = new ConfigJava();
            try
            {
                config.users[0].java.jre_path = ((JavaInfo)LaunchSet_JavaJreList.SelectedItem).JavaW;
            }
            catch { }
            Config.SaveConfig(config);
        }
        /// <summary>
        /// 用户选择用Mojang账户登录，弹出登陆框（保存由登陆框负责）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LaunchSet_LoginWithOnline_Click(object sender, RoutedEventArgs e)
        {
            UI.LoginMojangAccount lma = new UI.LoginMojangAccount();
            lma.LoginDone += () =>
            {
                Dispatcher.Invoke(delegate
                {
                    lma.Close();
                    ConfigModel config = Config.GetConfig();
                    LaunchSet_LoginTip.Content =
                        "目前是"
                        + (config.users[0].type == "online" ? "正版" : "盗版")
                        + "模式，账号是" + config.users[0].name;
                });
            };
            lma.ShowDialog();
        }
        /// <summary>
        /// 用户选择用离线模式登陆，弹出输入账号框（保存由账号框负责）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LaunchSet_LoginWithOffline_Click(object sender, RoutedEventArgs e)
        {
            UI.MessageBox input = new UI.MessageBox((MainWindow)Window.GetWindow(this));
            input.Title = "输入用户名";
            input.Message = "建议不要乱输入，这个对于联机的话很重要的";
            Page inputLayout = new Page();
            Grid inputLayoutGrid = new Grid();
            TextBox inputLayoutText = new TextBox();
            inputLayoutText.Width = 180;
            inputLayoutGrid.Children.Add(inputLayoutText);
            inputLayout.Content = inputLayoutGrid;
            input.CustomContent = inputLayout;
            input.LeftButtonText = "确定";
            input.RightButtonText = "取消";
            input.LeftButtonClick += (args) =>
            {
                if (inputLayoutText.Text == "")
                {
                    UI.MessageBox.QuickShow("请输入用户名！", (MainWindow)Window.GetWindow(this));
                    return;
                }
                ConfigModel config = Config.GetConfig();
                List<ConfigUser> users = (config.users == null ? new List<ConfigUser>() : config.users.ToList());
                ConfigUser user = config.users[0];
                user.name = inputLayoutText.Text;
                user.type = "offline";
                user.theme = "default";
                user.pass = "";
                if (users.Count == 0)
                    users.Add(user);
                else
                    users[0] = user;
                config.users = users.ToArray();
                Config.SaveConfig(config);
                UI.MessageBox.QuickShow("欢迎回来！" + inputLayoutText.Text, null);
                Config.IsFirstConfiging = false;
                input.Close();
                ConfigModel loadConfig = Config.GetConfig();
                LaunchSet_LoginTip.Content =
                    "目前是"
                    + (loadConfig.users[0].type == "online" ? "正版" : "盗版")
                    + "模式，账号是" + loadConfig.users[0].name;
            };
            input.ShowDialog();
        }
        /// <summary>
        /// 切换Tab
        /// </summary>
        /// <param name="switchTab"></param>
        /// <param name="switchTabGrid"></param>
        void SwitchTab (Label switchTab, Grid switchTabGrid)
        {
            // 0,42,137,203
            Tab_LaunchSet.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            Tab_AdvanceSet.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            Tab_PluginSet.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            Tab_DownloadSet.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            Tab_AqualSet.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            Tab_About.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            switchTab.Foreground = new SolidColorBrush(Color.FromArgb(255, 42, 137, 203));
            LaunchSet.Visibility = Visibility.Collapsed;
            AdvanceSet.Visibility = Visibility.Collapsed;
            PluginSet.Visibility = Visibility.Collapsed;
            DownloadSet.Visibility = Visibility.Collapsed;
            AquaLSet.Visibility = Visibility.Collapsed;
            About.Visibility = Visibility.Collapsed;
            if (switchTab == Tab_LaunchSet)
                LaunchSet.Visibility = Visibility.Visible;
            else if (switchTab == Tab_AdvanceSet)
                AdvanceSet.Visibility = Visibility.Visible;
            else if (switchTab == Tab_PluginSet)
                PluginSet.Visibility = Visibility.Visible;
            else if (switchTab == Tab_DownloadSet)
                DownloadSet.Visibility = Visibility.Visible;
            else if (switchTab == Tab_AqualSet)
                AquaLSet.Visibility = Visibility.Visible;
            else if (switchTab == Tab_About)
                About.Visibility = Visibility.Visible;
        }

        private void Tab_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SwitchTab((Label)sender, null);
        }
    }
}
