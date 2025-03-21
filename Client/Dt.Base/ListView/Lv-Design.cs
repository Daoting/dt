#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-09-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 设计时
    /// </summary>
    public partial class Lv
    {
        #region 静态内容
        public readonly static DependencyProperty ViewXamlProperty = DependencyProperty.Register(
            "ViewXaml",
            typeof(string),
            typeof(Lv),
            new PropertyMetadata(null));
        #endregion

        /// <summary>
        /// 设计时用，行视图的xaml
        /// </summary>
        public string ViewXaml
        {
            get { return (string)GetValue(ViewXamlProperty); }
            set { SetValue(ViewXamlProperty, value); }
        }
    }
}