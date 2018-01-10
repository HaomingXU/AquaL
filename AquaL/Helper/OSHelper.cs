using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AquaL.Helper
{
    class OSHelper
    {
        /// <summary>
        /// 获取系统中注册表内存在的Java列表
        /// </summary>
        /// <returns></returns>
        internal static List<string> GetJavas()
        {
            List<string> returnList = new List<string>();
            try
            {
                var rootReg = Registry.LocalMachine.OpenSubKey("SOFTWARE");
                List<string> javas = new List<string>();
                if (IsWindows64Bit)
                {
                    try
                    {
                        var javaSoftReg32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                        javaSoftReg32 = javaSoftReg32.OpenSubKey("SOFTWARE");
                        javaSoftReg32 = javaSoftReg32.OpenSubKey("JavaSoft");
                        var javaRuntimeReg32 = javaSoftReg32.OpenSubKey("Java Runtime Environment");
                        foreach (var reg32 in javaRuntimeReg32.GetSubKeyNames())
                        {
                            if (javaRuntimeReg32.OpenSubKey(reg32).GetValue("JavaHome") == null)
                                continue;
                            else
                                javas.Add(javaRuntimeReg32.OpenSubKey(reg32).GetValue("JavaHome").ToString());
                        }
                    }
                    catch
                    {

                    }
                }
                var javaSoftReg = rootReg.OpenSubKey("JavaSoft");
                var javaRuntimeReg = javaSoftReg.OpenSubKey("Java Runtime Environment");
                foreach (var reg in javaRuntimeReg.GetSubKeyNames())
                {
                    if (javaRuntimeReg.OpenSubKey(reg).GetValue("JavaHome") == null)
                        continue;
                    else
                        javas.Add(javaRuntimeReg.OpenSubKey(reg).GetValue("JavaHome").ToString());
                }

                foreach (string path in javas)
                {
                    bool isThisInReturnList = false; // 判断目前这项是否存在于现有列表
                    foreach (string p in returnList)
                    {
                        if (path == p)
                            isThisInReturnList = true;
                    }
                    if (isThisInReturnList)
                        continue;
                    else
                        returnList.Add(path);
                }

                return returnList;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); return new List<string>(); }
        }
        /// <summary>
        /// 获取安装的Java详细信息
        /// </summary>
        /// <returns></returns>
        public static List<JavaInfo> GetJavaList()
        {
            List<JavaInfo> ji = new List<JavaInfo>();
            foreach (string path in GetJavas())
            {
                JavaInfo jInfo = new JavaInfo();
                jInfo.Home = path;
                jInfo.JavaW = path + "\\bin\\javaw.exe";
                ji.Add(jInfo);
            }
            return ji;
        }
        /// <summary>
        /// Windows系统是不是64位的
        /// </summary>
        public static bool IsWindows64Bit
        {
            get { return Environment.Is64BitOperatingSystem; }
        }
    }

    public class JavaInfo
    {
        /// <summary>
        /// Java 主目录
        /// </summary>
        public string Home { get; set; }
        /// <summary>
        /// JavaW 路径
        /// </summary>
        public string JavaW { get; set; }
        /// <summary>
        /// Java 虚拟机位数
        /// </summary>
        public int Bit
        {
            get
            {
                Process JavaProcess = new Process();
                JavaProcess.StartInfo.FileName = JavaW;
                if (FileVersionInfo.GetVersionInfo(JavaProcess.StartInfo.FileName).FileDescription == "Java(TM) Platform SE binary")
                {
                    JavaProcess.StartInfo.Arguments = "-version";
                    JavaProcess.StartInfo.RedirectStandardError = true;
                    JavaProcess.StartInfo.RedirectStandardInput = true;
                    JavaProcess.StartInfo.RedirectStandardOutput = true;
                    JavaProcess.StartInfo.UseShellExecute = false;
                    JavaProcess.StartInfo.StandardOutputEncoding = Encoding.UTF8;
                    JavaProcess.Start();
                    JavaProcess.StandardInput.WriteLine(" ");
                    JavaProcess.StandardInput.Flush();
                    string result = JavaProcess.StandardError.ReadToEnd();
                    try
                    {
                        string JavaVersion = result.Substring(14, result.LastIndexOf("\"") - 14);
                        int i = result.LastIndexOf("\"");
                        int ab1 = result.IndexOf("HotSpot(TM) ");
                        int ab2 = result.IndexOf(" VM");
                        string JavaSystem = result.Substring(ab2 - 6, 6);
                        int ab3 = result.IndexOf(JavaSystem);
                        string JavaBit = result.Substring(ab1 + 12, ab3 - ab1 - 12);
                        if (JavaBit.IndexOf("64") >= 0)
                            return 64;
                        else
                            return 32;
                    }
                    catch { }
                }
                return 0;
            }
        }
    }
}
