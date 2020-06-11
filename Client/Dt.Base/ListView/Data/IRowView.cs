#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-06-11 创建
******************************************************************************/
#endregion

#region 引用命名
using Windows.UI.Xaml;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 动态创建行视图接口
    /// </summary>
    public interface IRowView
    {
        /// <summary>
        /// 动态创建行视图内容
        /// </summary>
        /// <param name="p_item">行</param>
        /// <returns>返回行UI</returns>
        UIElement Create(LvItem p_item);
    }
}
