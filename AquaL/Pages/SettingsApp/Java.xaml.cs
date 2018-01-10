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

namespace AquaL.Pages.SettingsApp
{
    /// <summary>
    /// Java.xaml 的交互逻辑
    /// </summary>
    public partial class Java : Page
    {
        public Java()
        {
            InitializeComponent();
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LaunchSet_JavaJreList.ItemsSource = Helper.OSHelper.GetJavaList();
            LaunchSet_JavaJreList.DisplayMemberPath = "JavaW";
            LaunchSet_JavaJreList.SelectedIndex = 0;
            ConfigModel config = Config.GetConfig();
            if (config.users[0].java != null) // 选择之前设置的Java
            {
                LaunchSet_JavaJreMemorySize.Text = config.users[0].java.max_memory.ToString();
                if (config.users[0].java.jre_path != null || config.users[0].java.jre_path != "")
                {
                    int i = 0;
                    List<JavaInfo> ji = OSHelper.GetJavaList();
                    foreach (JavaInfo ij in ji)
                    {
                        if (ij.JavaW == config.users[0].java.jre_path)
                        {
                            LaunchSet_JavaJreList.SelectedIndex = i;
                        }
                        i++;
                    }
                }
                if (config.users[0].java.is_auto_memory || config.users[0].java.max_memory == 0)
                {
                    AutoMemory.IsChecked = true;
                    LaunchSet_JavaJreMemorySize.IsEnabled = false;
                }
            }

        }
        /// <summary>
        /// 保存选择的Java虚拟机
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

        private void AutoMemory_Checked(object sender, RoutedEventArgs e)
        {
            LaunchSet_JavaJreMemorySize.IsEnabled = false;
            ConfigModel m = Config.GetConfig();
            m.users[0].java.is_auto_memory = true;
            Config.SaveConfig(m);
        }

        private void AutoMemory_Unchecked(object sender, RoutedEventArgs e)
        {
            LaunchSet_JavaJreMemorySize.IsEnabled = true;
            ConfigModel m = Config.GetConfig();
            m.users[0].java.is_auto_memory = false;
            Config.SaveConfig(m);
        }

        private void LaunchSet_JavaJreMemorySize_LostFocus(object sender, RoutedEventArgs e)
        {
            ConfigModel m = Config.GetConfig();
            int mem = 0;
            int.TryParse(LaunchSet_JavaJreMemorySize.Text, out mem);
            m.users[0].java.max_memory = mem;
            Config.SaveConfig(m);
        }
    }
}
