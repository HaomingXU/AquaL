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
    /// DownloadJava.xaml 的交互逻辑
    /// </summary>
    public partial class DownloadJava : Page
    {
        /// <summary>
        /// Win32Api 可以以管理员权限运行程序
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="lpOperation"></param>
        /// <param name="lpFile"></param>
        /// <param name="lpParameters"></param>
        /// <param name="lpDirectory"></param>
        /// <param name="nShowCmd"></param>
        [System.Runtime.InteropServices.DllImport("shell32.dll")]
        public extern static void ShellExecute(int hWnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd);
        /// <summary>
        /// 构造函数
        /// </summary>
        public DownloadJava()
        {
            InitializeComponent();
        }

        void StartJavaInstall()
        {
            ShellExecute(0, "runas", "java.exe", "", "", 1);
        }
    }
}
