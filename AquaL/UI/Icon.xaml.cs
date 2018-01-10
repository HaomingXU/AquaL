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

namespace AquaL.UI
{
    /// <summary>
    /// Icon.xaml 的交互逻辑
    /// </summary>
    public partial class Icon : UserControl
    {
        public string AppPackage { get; private set; }
        public Brush AppIcon { get; private set; }
        public string AppLabel { get; private set; }

        public Icon(Brush Icon, string Label, string AppPackage)
        {
            this.AppPackage = AppPackage;
            AppIcon = Icon;
            AppLabel = Label;
            IconClick += (s) => { };
            IconPopMenu += (s) => { };
            InitializeComponent();
        }

        public delegate void IconClickHandle(string AppPackage);
        public event IconClickHandle IconClick;

        public delegate void IconPopMenuHandle(string AppPackage);
        public event IconPopMenuHandle IconPopMenu;

        private void Icon_Top_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IconClick(AppPackage);
        }

        private void Icon_Top_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            IconPopMenu(AppPackage);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Icon_Image.Background = AppIcon;
            Icon_Label.Content = AppLabel;
        }
    }
}
