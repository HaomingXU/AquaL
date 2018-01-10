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
    /// Main.xaml 的交互逻辑
    /// </summary>
    public partial class Main : Page
    {
        public Main()
        {
            InitializeComponent();
        }

        private void Sets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (Sets.SelectedIndex)
            {
                case 0:
                    settings_view.Content = new Java(); break;
                case 1:
                    settings_view.Content = new Account(); break;
                case 2:
                    settings_view.Content = new AutoJoinServer(); break;
                case 7:
                    settings_view.Content = new About(); break;
            }

        }
    }
}
