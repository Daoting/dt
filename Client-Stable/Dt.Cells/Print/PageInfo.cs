#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Cells.UI
{
    internal class PageInfo
    {
        public PageInfo()
        {
            ItemStart = -1;
            ItemEnd = -1;
            RepeatItemStart = -1;
            RepeatItemEnd = -1;
        }

        public double ContentSize { get; set; }

        public double HeaderSize { get; set; }

        public int ItemEnd { get; set; }

        public int ItemStart { get; set; }

        public int RepeatItemEnd { get; set; }

        public int RepeatItemStart { get; set; }

        /// <summary>
        /// 横轴开始坐标
        /// </summary>
        public double XStart { get; set; }

        /// <summary>
        /// 横轴结束坐标
        /// </summary>
        public double XEnd { get; set; }

        /// <summary>
        /// 纵轴开始坐标
        /// </summary>
        public double YStart { get; set; }

        /// <summary>
        /// 纵轴结束坐标
        /// </summary>
        public double YEnd { get; set; }
    }
}

