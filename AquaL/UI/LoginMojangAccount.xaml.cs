using KMCCC.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Shapes;

namespace AquaL.UI
{
    /// <summary>
    /// LoginMojangAccount.xaml 的交互逻辑
    /// </summary>
    public partial class LoginMojangAccount : Window
    {
        public LoginMojangAccount()
        {
            InitializeComponent();
        }

        public delegate void LoginDoneHandle();
        public event LoginDoneHandle LoginDone;

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            ProgressBarMain.IsActive = true;
            ProgressBar.Visibility = Visibility.Visible;
            Thread thread = new Thread((ThreadStart)delegate
            {
                bool noError = true; string displayName = "", error = "";
                string account = "", pass = "";
                try
                {
                    Dispatcher.Invoke(delegate
                    {
                        account = Account.Text;
                        pass = Pass.Password;
                    });
                    YggdrasilLogin yl = new YggdrasilLogin(account, pass, false);
                    AuthenticationInfo ai = yl.Do();
                    displayName = ai.DisplayName;
                    if (ai.Error != null)
                    {
                        noError = false;
                        error = ai.Error;
                    }
                }
                catch (Exception ex) { noError = false; error = ex.Message; }
                Dispatcher.Invoke(delegate
                {
                    ProgressBarMain.IsActive = false;
                    ProgressBar.Visibility = Visibility.Collapsed;
                });
                if (!noError)
                {
                UI.MessageBox.QuickShow("验证失败，可能是账号或密码错误，网络连接失败等原因！"
                    + (error != null ? "\n" + (error.Replace(" ", "_").ToUpper()) : ""), null);
                    return;
                }
                UI.MessageBox.QuickShow("欢迎回来！" + displayName, null);
                ConfigModel config = Config.GetConfig();
                if (config == null)
                    config = new ConfigModel();
                List<ConfigUser> users = (config.users == null ? new List<ConfigUser>() : config.users.ToList());
                if (users.Count <= 0)
                    users.Add(new ConfigUser());
                ConfigUser user = users[0];
                user.name = account;
                user.pass = pass;
                user.type = "online";
                user.theme = "default";
                if (users.Count == 0)
                    users.Add(user);
                else
                    users[0] = user;
                config.users = users.ToArray();
                Config.SaveConfig(config);
                LoginDone();
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();
        }
    }
}
