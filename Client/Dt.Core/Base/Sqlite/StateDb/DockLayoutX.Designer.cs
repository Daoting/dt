﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 客户端Win布局设置
    /// </summary>
    [Sqlite("state")]
    public partial class DockLayoutX : EntityX<DockLayoutX>
    {
        #region 构造方法
        DockLayoutX() { }

        public DockLayoutX(
            string BaseUri,
            string Layout = default)
        {
            Add("BaseUri", BaseUri);
            Add("Layout", Layout);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 控件所属的xaml位置
        /// </summary>
        [PrimaryKey]
        public string BaseUri
        {
            get { return (string)this["BaseUri"]; }
            set { this["BaseUri"] = value; }
        }

        /// <summary>
        /// 布局内容
        /// </summary>
        public string Layout
        {
            get { return (string)this["Layout"]; }
            set { this["Layout"] = value; }
        }
    }
}
