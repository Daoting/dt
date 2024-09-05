#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-01-31 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Base
{
    /// <summary>
    /// Fv扩展方法
    /// </summary>
    public static class FvEx
    {
        /// <summary>
        /// 根据Fv单元格创建Row数据源
        /// </summary>
        /// <param name="p_fv"></param>
        /// <returns></returns>
        public static Row CreateRow(this Fv p_fv)
        {
            if (p_fv == null)
                Throw.Msg("Fv参数不可为空！");

            Row row = new Row();
            var ls = from item in p_fv.Items
                     where item is FvCell cell && !string.IsNullOrEmpty(cell.ID)
                     select item as FvCell;
            foreach (var c in ls)
            {
                Type tp = typeof(string);
                if (c is CNum)
                    tp = typeof(double);
                else if (c is CDate)
                    tp = typeof(DateTime);
                else if (c is CBool)
                    tp = typeof(bool);
                else if (c is CIcon)
                    tp = typeof(Icons);
                
                new Cell(row, c.ID, tp);
            }
            return row;
        }
    }
}
