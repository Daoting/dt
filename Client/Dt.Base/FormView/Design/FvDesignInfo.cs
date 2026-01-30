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
    /// Fv设计信息
    /// </summary>
#if WIN
    [WinRT.GeneratedBindableCustomProperty]
#else
    [Microsoft.UI.Xaml.Data.Bindable]
#endif
    public partial class FvDesignInfo
    {
        /// <summary>
        /// Fv的Xaml
        /// </summary>
        public string Xaml { get; set; }

        /// <summary>
        /// 预置列信息
        /// </summary>
        public List<EntityCol> Cols { get; set; }

        /// <summary>
        /// 当存在预置列时，是否允许自定义列
        /// </summary>
        public bool AllowCustomCol { get; set; }

        /// <summary>
        /// 是否为查询面板
        /// </summary>
        public bool IsQueryFv { get; set; }
    }

    /// <summary>
    /// Fv设计时预置列信息
    /// </summary>
#if WIN
    [WinRT.GeneratedBindableCustomProperty]
#else
    [Microsoft.UI.Xaml.Data.Bindable]
#endif
    public partial class EntityCol
    {
        public EntityCol(string p_name, Type p_type)
        {
            Name = p_name;
            Type = p_type;
        }

        /// <summary>
        /// 列名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 列类型
        /// </summary>
        public Type Type { get; set; }
    }
}