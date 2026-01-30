#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2025-03-07 创建
******************************************************************************/
#endregion

#region 引用命名

#endregion

namespace Dt.Base
{
    /// <summary>
    /// Lv设计信息
    /// </summary>
#if WIN
    [WinRT.GeneratedBindableCustomProperty]
#else
    [Microsoft.UI.Xaml.Data.Bindable]
#endif
    public partial class LvDesignInfo
    {
        /// <summary>
        /// Lv的Xaml
        /// </summary>
        public string Xaml { get; set; }

        /// <summary>
        /// 预置列信息
        /// </summary>
        public List<EntityCol> Cols { get; set; }
    }
}