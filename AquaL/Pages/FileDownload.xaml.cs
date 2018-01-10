using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
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
    /// FileDownload.xaml 的交互逻辑
    /// </summary>
    public partial class FileDownload : Page
    {
        /// <summary>
        /// 下载完成的文件数量
        /// </summary>
        public int DownloadCompletedFilesCount { get; private set; }
        /// <summary>
        /// 下载任务列表
        /// </summary>
        public List<DownloadFileTask> Tasks { get; private set; }
        /// <summary>
        /// 构造函数
        /// <param name="Tasks">下载列表</param>
        /// </summary>
        public FileDownload(List<DownloadFileTask> Tasks)
        {
            this.Tasks = Tasks;
            InitializeComponent();
        }
        /// <summary>
        /// 页面加载完成，开始下载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow context = (MainWindow)Window.GetWindow(this);
            if (Tasks.Count == 0)
            {
                context.Close();
                return;
            }
            ProgressBar.Maximum = Tasks.Count;
            context.IsDisplayCloseBtn = false;
            foreach (DownloadFileTask task in Tasks)
            {
                WebClient webClient = new WebClient();
                webClient.DownloadFileCompleted += FileDownloadCompleted;
                webClient.DownloadProgressChanged += (senderObj, downloadProgressChangedEventArgs) =>
                {
                    string thisFileName = task.SavePath;
                    foreach (DownloadFileTask taskF in Tasks)
                    {
                        if (task.SavePath == thisFileName)
                        {
                            task.ProgressPercentage = downloadProgressChangedEventArgs.ProgressPercentage;
                        }
                    }
                    UpdateDownloadListView();
                };
                webClient.DownloadFileAsync(task.ServerPath, task.SavePath);
            }
        }
        /// <summary>
        /// 文件下载完毕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void FileDownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            DownloadCompletedFilesCount++;
            if (DownloadCompletedFilesCount == Tasks.Count) // 下载完成的数量等于全部任务的数量
            {
                MainWindow context = (MainWindow)Window.GetWindow(this);
                context.Close();
                GC.Collect();
            }
            else
            {
                ProgressBar.Value = DownloadCompletedFilesCount;
            }
        }
        /// <summary>
        /// 更新GUI中的下载进度
        /// </summary>
        void UpdateDownloadListView()
        {
            if (Tasks.Count >= 50)
            {
                downloadTaskListView.Visibility = Visibility.Collapsed;
            }
            else
            {
                foreach (DownloadFileTask task in Tasks)
                {
                    downloadTaskListView.Items.Clear();
                    if (task.ProgressPercentage >= 100.00)
                        continue;
                    downloadTaskListView.Items.Add(task);
                }
            }
        }
    }
    /// <summary>
    /// 下载任务
    /// </summary>
    public class DownloadFileTask
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public DownloadFileTask(string DisplayName, string SavePath, Uri ServerPath)
        {
            this.DisplayName = DisplayName;
            this.SavePath = SavePath;
            this.ServerPath = ServerPath;
        }
        /// <summary>
        /// 在窗口中的显示名
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 保存路径
        /// </summary>
        public string SavePath { get; set; }
        /// <summary>
        /// 服务器上的路径
        /// </summary>
        public Uri ServerPath { get; set; }
        /// <summary>
        /// 下载进度
        /// </summary>
        public double ProgressPercentage { get; set; } = 0.0;
        /// <summary>
        /// 下载进度（文本格式）
        /// </summary>
        public string ProgressPercentageStr { get { return ProgressPercentage + "%"; } }
        /// <summary>
        /// 服务器路径地址
        /// </summary>
        public string ServerPathStr { get { return ServerPath.AbsolutePath; } }
    }
}
