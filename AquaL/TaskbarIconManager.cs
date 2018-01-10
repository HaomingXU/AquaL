using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AquaL
{
    class TaskbarIconManager
    {
        /// <summary>
        /// 任务栏图标对象
        /// </summary>
        public static NotifyIcon nf = new NotifyIcon();
        /// <summary>
        /// 初始化
        /// </summary>
        public static void InitTaskbarManager()
        {
            nf.Text = "AquaL启动器";
            nf.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            nf.Visible = true;
        }
        /// <summary>
        /// 销毁
        /// </summary>
        public static void UnLoad()
        {
            nf.Visible = false;
        }
        /// <summary>
        /// 显示通知
        /// </summary>
        /// <param name="Title">标题</param>
        /// <param name="Content">内容</param>
        /// <param name="TipType">类型</param>
        /// <param name="second">时长(单位为秒)</param>
        public static void Toast(string Title, string Content, ToolTipIcon TipType = ToolTipIcon.Info, int second = 1000)
        {
            nf.ShowBalloonTip(second, Title, Content, TipType);
        }
    }
}
