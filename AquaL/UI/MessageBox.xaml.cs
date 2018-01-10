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
using System.Windows.Shapes;

namespace AquaL.UI
{
    /// <summary>
    /// MessageBox.xaml 的交互逻辑
    /// </summary>
    public partial class MessageBox : Window
    {
        /// <summary>
        /// 消息框上下文
        /// </summary>
        public MainWindow Context { get; private set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        public MessageBox(MainWindow context)
        {
            Context = context;
            InitializeComponent();
        }
        /// <summary>
        /// 消息框标题
        /// </summary>
        public new string Title
        {
            get
            {
                return title;
            }
            set
            {
                base.Title = value;
                title = value;
            }
        }
        private string title = "";
        /// <summary>
        /// 消息框内容
        /// </summary>
        public string Message  { get; set; }
        /// <summary>
        /// 左侧按钮的文字
        /// </summary>
        public string LeftButtonText { get; set; }
        /// <summary>
        /// 右侧按钮的文字
        /// </summary>
        public string RightButtonText { get; set; }
        /// <summary>
        /// 左侧按钮被点击的委托
        /// </summary>
        /// <param name="e"></param>
        public delegate void LeftButtonClickHandle(MouseButtonEventArgs e);
        /// <summary>
        /// 注册左侧按钮被点击事件
        /// </summary>
        public event LeftButtonClickHandle LeftButtonClick;
        /// <summary>
        /// 右侧按钮被点击的委托
        /// </summary>
        /// <param name="e"></param>
        public delegate void RightButtonClickHandle(MouseButtonEventArgs e);
        /// <summary>
        /// 注册右侧按钮被点击事件
        /// </summary>
        public event RightButtonClickHandle RightButtonClick;
        /// <summary>
        /// 默认的按钮事件处理程序
        /// </summary>
        /// <param name="e"></param>
        private void DefaultButtonClick(MouseButtonEventArgs e) { this.Close(); }
        /// <summary>
        /// 自定义信息框内部布局
        /// </summary>
        public Page CustomContent { get; set; }
        /// <summary>
        /// 窗口移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        /// <summary>
        /// 左侧按钮被按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Msg_Btn1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LeftButtonClick(e);
        }
        /// <summary>
        /// 右侧按钮被按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Msg_Btn2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RightButtonClick(e);
        }
        /// <summary>
        /// 消息框被加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MsgBoxLoaded(object sender, RoutedEventArgs e)
        {
            if (Context != null)
                Context.IsDisplayMsgBg = true;
            if (Title == "")
            {
                base.Title = "AquaL";
                Msg_Title.Visibility = Visibility.Collapsed;
            }
            else
                Msg_Title.Content = Title;

            if (Message == "")
                Msg_Content.Visibility = Visibility.Collapsed;
            else
                Msg_Content.Content = Message;

            if (LeftButtonText == "")
                Msg_Btn1.Visibility = Visibility.Collapsed;
            else
                Msg_Btn1.Content = LeftButtonText;

            if (RightButtonText == "")
                Msg_Btn2.Visibility = Visibility.Collapsed;
            else
                Msg_Btn2.Content = RightButtonText;

            if (Msg_Btn1.Visibility == Visibility.Collapsed && Msg_Btn2.Visibility == Visibility.Collapsed)
                Msg_BtnLayout.Visibility = Visibility.Collapsed;

            if (RightButtonClick == null)
                RightButtonClick += DefaultButtonClick;

            if (LeftButtonClick == null)
                LeftButtonClick += DefaultButtonClick;

            if (CustomContent != null)
            {
                Msg_CustomContent.Visibility = Visibility;
                Msg_CustomContent.Content = CustomContent;
            }
        }
        /// <summary>
        /// 快速显示一个信息框
        /// </summary>
        public static void QuickShow(string Message, MainWindow Context, string Title = "")
        {
            UI.MessageBox mb = new MessageBox(Context);
            mb.LeftButtonText = "好";
            mb.Title = Title;
            mb.Message = Message;
            mb.ShowDialog();
        }
        /// <summary>
        /// 信息框关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Context != null)
                Context.IsDisplayMsgBg = false;
        }
    }
}
