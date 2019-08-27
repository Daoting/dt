﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Sqlite;
#endregion

namespace Dt.Base.Docking
{
    /// <summary>
    /// 客户端Win布局设置
    /// </summary>
    [StateTable]
    public class DockLayout
    {
        /// <summary>
        /// 控件所属的xaml位置
        /// </summary>
        [PrimaryKey]
        public string BaseUri { get; set; }

        /// <summary>
        /// 布局内容
        /// </summary>
        public string Layout { get; set; }
    }
}
