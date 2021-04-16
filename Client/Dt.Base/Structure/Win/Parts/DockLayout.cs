#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Sqlite;
#endregion

namespace Dt.Base.Docking
{
    /// <summary>
    /// 客户端Win布局设置
    /// </summary>
    [Sqlite("state")]
    public class DockLayout : Entity
    {
        #region 构造方法
        DockLayout() { }

        public DockLayout(
            string BaseUri,
            string Layout = default)
        {
            AddCell("BaseUri", BaseUri);
            AddCell("Layout", Layout);
            IsAdded = true;
            AttachHook();
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
