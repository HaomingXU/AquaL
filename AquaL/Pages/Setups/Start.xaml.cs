using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace AquaL.Pages.Setups
{
    /// <summary>
    /// Start.xaml 的交互逻辑
    /// </summary>
    public partial class Start : Page
    {
        public Start()
        {
            InitializeComponent();
        }

        private void step2_username_Click(object sender, RoutedEventArgs e)
        {
            UI.MessageBox input = new UI.MessageBox((MainWindow)Window.GetWindow(this));
            input.Title = "输入用户名";
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
                if(inputLayoutText.Text == "")
                {
                    UI.MessageBox.QuickShow("请输入用户名！", (MainWindow)Window.GetWindow(this));
                    return;
                }
                ConfigModel config = Config.GetConfig();
                ConfigUser user = new ConfigUser();
                user.name = inputLayoutText.Text;
                user.type = "offline";
                user.theme = "default";
                List<ConfigUser> users = (config.users == null ? new List<ConfigUser>() : config.users.ToList());
                users.Add(user);
                config.users = users.ToArray();
                Config.SaveConfig(config);
                UI.MessageBox.QuickShow("欢迎回来！" + inputLayoutText.Text, (MainWindow)Window.GetWindow(this));
                input.Close();
                MainWindow parent = (MainWindow)Window.GetWindow(this);
                parent.Close();
            };
            input.ShowDialog();
        }

        private void step2_login_Click(object sender, RoutedEventArgs e)
        {
            UI.LoginMojangAccount lma = new UI.LoginMojangAccount();
            lma.LoginDone += () =>
            {
                Dispatcher.Invoke(delegate 
                {
                    lma.Close();
                    MainWindow parent = (MainWindow)Window.GetWindow(this);
                    parent.Close();
                });
            };
            lma.ShowDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow context = (MainWindow)Window.GetWindow(this);
            context.IsDisplayCloseBtn = false;
        }
    }
}
