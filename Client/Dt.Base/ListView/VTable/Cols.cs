#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-06-04 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.ObjectModel;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 列集合
    /// </summary>
    public class Cols : KeyedCollection<string, Col>
    {
        /// <summary>
        /// 列位置失效事件
        /// </summary>
        public event EventHandler Update;

        public Cols()
            : base(StringComparer.OrdinalIgnoreCase)
        { }

        /// <summary>
        /// 获取设置点击列头是否可以排序
        /// </summary>
        public bool AllowSorting { get; set; } = true;

        /// <summary>
        /// 获取设置是否隐藏行号，默认false
        /// </summary>
        public bool HideIndex { get; set; }

        /// <summary>
        /// 列总宽
        /// </summary>
        internal double TotalWidth { get; set; }

        /// <summary>
        /// 列位置失效，触发重新测量布局
        /// </summary>
        internal void Invalidate()
        {
            FixWidth();
            Update?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 更新水平位置及总宽
        /// </summary>
        internal void FixWidth()
        {
            TotalWidth = 0;
            foreach (Col col in this)
            {
                col.Left = TotalWidth;
                TotalWidth += col.Width;
            }
        }

        protected override string GetKeyForItem(Col p_item)
        {
            if (p_item != null)
                return p_item.ID;
            throw new Exception("Cols中不可插入列名为空或重复的Col！");
        }
    }
}
