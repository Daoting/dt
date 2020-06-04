#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 消息提示内容结构
    /// </summary>
    public class NotifyInfo
    {
        /// <summary>
        /// 提示内容
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 提示框类型
        /// </summary>
        public NotifyType NotifyType { get; set; }

        /// <summary>
        /// 链接描述
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// 几秒后自动关闭，0表示不自动关闭
        /// </summary>
        public int DelaySeconds { get; set; }

        /// <summary>
        /// 附加信息
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// 点击链接时的回调方法
        /// </summary>
        public Action<NotifyInfo> LinkCallback { get; set; }

        /// <summary>
        /// 关闭消息提示
        /// </summary>
        public Action Close { get; internal set; }
    }

    /// <summary>
    /// 提示框类型
    /// </summary>
    public enum NotifyType
    {
        /// <summary>
        /// 无警示级别，对于无需交互的操作结果，
        /// 如保存成功（失败）、删除成功（失败）、发送成功（失败）、复制成功等
        /// </summary>
        Information,

        /// <summary>
        /// 普通警示级别
        /// </summary>
        Warning,
    }
}
