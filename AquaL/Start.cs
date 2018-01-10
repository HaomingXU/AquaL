using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AquaL
{
    class Start
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                bool isCreateDll = false;
                if (!File.Exists("KMCCC.Pro.dll"))
                {
                    FileStream fs = new FileStream("KMCCC.Pro.dll", FileMode.Create);
                    fs.Write(Dlls.KMCCC_Pro, 0, Dlls.KMCCC_Pro.Length);
                    FileInfo fi = new FileInfo("KMCCC.Pro.dll");
                    File.SetAttributes("KMCCC.Pro.dll", fi.Attributes | FileAttributes.Hidden);
                    isCreateDll = true;
                }
                if (!File.Exists("MahApps.Metro.dll"))
                {
                    FileStream fs = new FileStream("MahApps.Metro.dll", FileMode.Create);
                    fs.Write(Dlls.MahApps_Metro, 0, Dlls.MahApps_Metro.Length);
                    FileInfo fi = new FileInfo("MahApps.Metro.dll");
                    File.SetAttributes("MahApps.Metro.dll", fi.Attributes | FileAttributes.Hidden);
                    isCreateDll = true;
                }
                if (!File.Exists("MaterialDesignColors.dll"))
                {
                    FileStream fs = new FileStream("MaterialDesignColors.dll", FileMode.Create);
                    fs.Write(Dlls.MaterialDesignColors, 0, Dlls.MaterialDesignColors.Length);
                    FileInfo fi = new FileInfo("MaterialDesignColors.dll");
                    File.SetAttributes("MaterialDesignColors.dll", fi.Attributes | FileAttributes.Hidden);
                    isCreateDll = true;
                }
                if (!File.Exists("MaterialDesignThemes.Wpf.dll"))
                {
                    FileStream fs = new FileStream("MaterialDesignThemes.Wpf.dll", FileMode.Create);
                    fs.Write(Dlls.MaterialDesignThemes_Wpf, 0, Dlls.MaterialDesignThemes_Wpf.Length);
                    FileInfo fi = new FileInfo("MaterialDesignThemes.Wpf.dll");
                    File.SetAttributes("MaterialDesignThemes.Wpf.dll", fi.Attributes | FileAttributes.Hidden);
                    isCreateDll = true;
                }
                if (!File.Exists("Newtonsoft.Json.dll"))
                {
                    FileStream fs = new FileStream("Newtonsoft.Json.dll", FileMode.Create);
                    fs.Write(Dlls.Newtonsoft_Json, 0, Dlls.Newtonsoft_Json.Length);
                    FileInfo fi = new FileInfo("Newtonsoft.Json.dll");
                    File.SetAttributes("Newtonsoft.Json.dll", fi.Attributes | FileAttributes.Hidden);
                    isCreateDll = true;
                }
                if (!File.Exists("System.Windows.Interactivity.dll"))
                {
                    FileStream fs = new FileStream("System.Windows.Interactivity.dll", FileMode.Create);
                    fs.Write(Dlls.System_Windows_Interactivity, 0, Dlls.System_Windows_Interactivity.Length);
                    FileInfo fi = new FileInfo("System.Windows.Interactivity.dll");
                    File.SetAttributes("System.Windows.Interactivity.dll", fi.Attributes | FileAttributes.Hidden);
                    isCreateDll = true;
                }
                if (isCreateDll)
                {
                    throw new Exception();
                }
            }
            catch
            {
                Process.Start(Process.GetCurrentProcess().MainModule.FileName);
                return;
            }
            App app = new App();
            app.InitializeComponent();
            app.Run();
        }
    }
}
