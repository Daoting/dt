#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-12-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml.Media;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 托盘通知相关
    /// </summary>
    partial class DefUICallback : IUICallback
    {
        /// <summary>
        /// 获取托盘通知列表
        /// </summary>
        public Table AllTrayMsg => Data;

        /// <summary>
        /// 发布新托盘通知
        /// </summary>
        /// <param name="p_content">通知内容</param>
        /// <param name="p_isWarning">是否为警告通知</param>
        public void TrayMsg(string p_content, bool p_isWarning)
        {
            var row = Data.NewRow(new
            {
                time = Kit.Now.ToString("HH:mm:ss"),
                message = p_content,
                level = p_isWarning ? Res.RedBrush : Res.BlackBrush,
            });
            Data.Insert(0, row);
        }

        /// <summary>
        /// 发布新托盘通知
        /// </summary>
        /// <param name="p_info">消息提示实例</param>
        public void TrayMsg(NotifyInfo p_info)
        {
            var row = Data.NewRow(new
            {
                time = Kit.Now.ToString("HH:mm:ss"),
                message = p_info.Message,
                level = p_info.NotifyType == NotifyType.Warning ? Res.RedBrush : Res.BlackBrush,
            });
            row.Tag = p_info;
            Data.Insert(0, row);
        }
        
        static readonly Table Data = new Table
        {
            { "time" },
            { "message" },
            { "level", typeof(SolidColorBrush) },
        };
    }
}