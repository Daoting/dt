#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-09-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Windows.Foundation;
using Dt.Base.ListView;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 表格视图列头相关
    /// </summary>
    public partial class Lv
    {
        /// <summary>
        /// 加载表格视图列头事件
        /// </summary>
        public event Action<ColHeaderCell> LoadColHeaderCell;

        /// <summary>
        /// 触发加载表格视图列头事件
        /// </summary>
        /// <param name="e"></param>
        internal void OnLoadColHeaderCell(ColHeaderCell e)
        {
            LoadColHeaderCell?.Invoke(e);
        }
    }
}