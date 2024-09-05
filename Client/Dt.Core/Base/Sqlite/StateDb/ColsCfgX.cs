#region 文件描述
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
    /// Lv表格视图时的列布局设置
    /// </summary>
    [Sqlite("state")]
    public partial class ColsCfgX : EntityX<ColsCfgX>
    {
        #region 构造方法
        ColsCfgX() { }

        public ColsCfgX(
            string Key,
            string Layout = default)
        {
            Add("Key", Key);
            Add("Layout", Layout);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// Lv控件的唯一标识，如：BaseUri + '#' + 列id/列Title + '#' + 列id/列Title
        /// </summary>
        [PrimaryKey]
        public string Key
        {
            get { return (string)this["BaseUri"]; }
            set { this["BaseUri"] = value; }
        }

        /// <summary>
        /// 列布局内容
        /// </summary>
        public string Layout
        {
            get { return (string)this["Layout"]; }
            set { this["Layout"] = value; }
        }
    }
}
