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

namespace AquaL.Pages.SettingsApp
{
    /// <summary>
    /// AutoJoinServer.xaml 的交互逻辑
    /// </summary>
    public partial class AutoJoinServer : Page
    {
        public AutoJoinServer()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 服务器端口设置输入框，限制只能输入数字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void serverPort_JavaJreMemorySize_KeyDown(object sender, KeyEventArgs e)
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

        private void serverAddress_LostFocus(object sender, RoutedEventArgs e)
        {
            ConfigModel cm = Config.GetConfig();
            cm.users[0].auto_join_server = (cm.users[0].auto_join_server == null ? new ConfigAutoJoinServer() : cm.users[0].auto_join_server);
            cm.users[0].auto_join_server.address = serverAddress.Text;
            Config.SaveConfig(cm);
        }

        private void serverPort_LostFocus(object sender, RoutedEventArgs e)
        {
            ConfigModel cm = Config.GetConfig();
            cm.users[0].auto_join_server = (cm.users[0].auto_join_server == null ? new ConfigAutoJoinServer() : cm.users[0].auto_join_server);
            int i = 0;
            int.TryParse(serverPort.Text, out i);
            cm.users[0].auto_join_server.port = i;
            Config.SaveConfig(cm);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ConfigModel cm = Config.GetConfig();
            try
            {
                serverAddress.Text = cm.users[0].auto_join_server.address;
                if (cm.users[0].auto_join_server.port != 0)
                    serverPort.Text = cm.users[0].auto_join_server.port.ToString();
            }
            catch { }
        }
    }
}
