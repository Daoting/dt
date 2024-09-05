#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-01-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml.Input;
#endregion

namespace Dt.Base.ListView
{
    /// <summary>
    /// LvPanel TvPanel
    /// </summary>
    interface IFilterHost
    {
        /// <summary>
        /// 刷新数据视图
        /// </summary>
        void Refresh();

        /// <summary>
        /// 快捷键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnKeyDown(object sender, KeyRoutedEventArgs e);

        /// <summary>
        /// 关闭筛选栏
        /// </summary>
        void CloseFilterUI();

        /// <summary>
        /// 获取筛选列
        /// </summary>
        /// <param name="p_settingCols">外部设置的过滤列，多列用逗号隔开</param>
        /// <returns></returns>
        Table GetAllCols(string p_settingCols);

        /// <summary>
        /// 第一行数据
        /// </summary>
        /// <returns></returns>
        object GetFirstRowData();
    }
}
