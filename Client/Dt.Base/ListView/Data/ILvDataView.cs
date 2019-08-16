#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2015-07-14 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections;
#endregion

namespace Dt.Base.ListView
{
    /// <summary>
    /// 数据视图接口
    /// </summary>
    public interface ILvDataView
    {
        /// <summary>
        /// 刷新视图列表
        /// </summary>
        void Refresh();

        /// <summary>
        /// 在指定的索引处批量插入数据，未参加过滤排序和分组！
        /// </summary>
        /// <param name="p_rows">外部数据行</param>
        /// <param name="p_index">插入位置，-1表示添加到最后</param>
        /// <returns>插入位置</returns>
        int Insert(IEnumerable p_rows, int p_index);

        /// <summary>
        /// 批量删除给定的行，统一更新
        /// </summary>
        /// <param name="p_rows"></param>
        void Delete(IEnumerable p_rows);

        /// <summary>
        /// 卸载数据
        /// </summary>
        void Unload();
    }
}
