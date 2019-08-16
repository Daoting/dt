#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-06-04 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 列定义
    /// </summary>
    public class Col
    {
        /// <summary>
        /// 获取设置列名(字段名)
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 获取设置列标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 获取设置列宽，默认100
        /// </summary>
        public double Width { get; set; } = 100;

        /// <summary>
        /// 获取设置占用的行数，默认1行
        /// </summary>
        public int RowSpan { get; set; } = 1;

        /// <summary>
        /// 获取设置点击列头是否可以排序
        /// </summary>
        public bool AllowSorting { get; set; } = true;

        /// <summary>
        /// 水平位置
        /// </summary>
        internal double Left { get; set; }
    }
}
