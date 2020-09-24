#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-21 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Core;
using Dt.Cells.Data;
using System;
using System.Collections.ObjectModel;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 报表项基类
    /// </summary>
    internal abstract class RptItem : RptItemBase
    {
        protected RptPart _part;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="p_owner"></param>
        public RptItem(RptPart p_owner)
        {
            _part = p_owner;
            _data.AddCell("row", 0);
            _data.AddCell("col", 0);
            _data.AddCell("rowspan", 1);
            _data.AddCell("colspan", 1);
        }

        /// <summary>
        /// 获取报表模板根对象
        /// </summary>
        public override RptRoot Root
        {
            get { return _part.Root; }
        }

        /// <summary>
        /// 获取报表项所属容器
        /// </summary>
        public override RptPart Part
        {
            get { return _part; }
        }

        /// <summary>
        /// 获取报表项所属父项
        /// </summary>
        public override RptItemBase Parent
        {
            get { return null; }
        }

        /// <summary>
        /// 获取设置报表元素起始行索引
        /// </summary>
        public override int Row
        {
            get { return _data.Int("row"); }
            set { _data["row"] = value; }
        }

        /// <summary>
        /// 获取设置报表元素起始列索引
        /// </summary>
        public override int Col
        {
            get { return _data.Int("col"); }
            set { _data["col"] = value; }
        }

        /// <summary>
        /// 获取设置报表元素所占行数
        /// </summary>
        public override int RowSpan
        {
            get { return _data.Int("rowspan"); }
            set { _data["rowspan"] = value; }
        }

        /// <summary>
        /// 获取设置报表元素所占列数
        /// </summary>
        public override int ColSpan
        {
            get { return _data.Int("colspan"); }
            set { _data["colspan"] = value; }
        }

        /// <summary>
        /// 指定区域是否在报表项内
        /// </summary>
        /// <param name="p_range"></param>
        /// <returns></returns>
        public bool Contains(CellRange p_range)
        {
            return Contains(p_range.Row, p_range.Column, p_range.RowCount, p_range.ColumnCount);
        }

        /// <summary>
        /// 指定区域是否在报表项内
        /// </summary>
        /// <param name="p_row"></param>
        /// <param name="p_column"></param>
        /// <param name="p_rowCount"></param>
        /// <param name="p_columnCount"></param>
        /// <returns></returns>
        public bool Contains(int p_row, int p_column, int p_rowCount, int p_columnCount)
        {
            if (Row != -1
                && (Row > p_row || (p_row + p_rowCount) > (Row + RowSpan)))
            {
                return false;
            }

            return ((Col == -1)
                || ((Col <= p_column) && ((p_column + p_columnCount) <= (Col + ColSpan))));
        }

        /// <summary>
        /// RptItem 增加行或列时判断是否与其他对象相交
        /// </summary>
        /// <param name="p_rowIncrease"></param>
        /// <param name="p_colIncrease"></param>
        /// <returns></returns>
        public bool TestIncIntersect(int p_rowIncrease, int p_colIncrease = 0)
        {
            var range = new Dt.Cells.Data.CellRange(Row, Col, RowSpan, ColSpan);
            range = new Dt.Cells.Data.CellRange(range.Row, range.Column, range.RowCount + p_rowIncrease, range.Column + p_colIncrease);
            return ValidEmptyRange(Part, range, this);
        }

        /// <summary>
        ///  RptItem 移动时判断是否与其他对象相交
        /// </summary>
        /// <param name="p_moveRow"></param>
        /// <param name="p_moveCol"></param>
        /// <returns></returns>
        public bool TestMovIntersect(int p_moveRow, int p_moveCol)
        {
            var range = new Dt.Cells.Data.CellRange(p_moveRow, p_moveCol, RowSpan, ColSpan);
            return ValidEmptyRange(Part, range, this);
        }

        /// <summary>
        /// 判定某区域是否有控件 或 是否有指定对象之外的控件
        /// </summary>
        /// <param name="p_part"></param>
        /// <param name="p_range"></param>
        /// <param name="p_item"></param>
        /// <returns></returns>
        public static bool ValidEmptyRange(RptPart p_part, Dt.Cells.Data.CellRange p_range, RptItem p_item = null)
        {
            if (p_part == null || p_range == null)
                return false;

            if (p_item == null)
            {
                return (from item in (IEnumerable<RptItem>)p_part.Items
                        where p_range.Intersects(item.Row, item.Col, item.RowSpan, item.ColSpan)
                        select item).Any();
            }
            else
            {
                return (from item in (IEnumerable<RptItem>)p_part.Items
                        where p_item != item && p_range.Intersects(item.Row, item.Col, item.RowSpan, item.ColSpan)
                        select item).Any();
            }
        }

        /// <summary>
        /// 克隆方法。
        /// </summary>
        /// <returns></returns>
        public virtual RptItem Clone()
        {
            return null;
        }

        /// <summary>
        /// 触发报表项更新事件
        /// </summary>
        /// <param name="p_clearOld">是否擦除原区域</param>
        public void Update(bool p_clearOld)
        {
            RptRoot root = Root;
            if (root != null)
                root.OnUpdated(this, p_clearOld);
        }

        /// <summary>
        /// 序列化位置
        /// </summary>
        /// <param name="p_writer"></param>
        protected void WritePosition(XmlWriter p_writer)
        {
            if (Row != 0)
                p_writer.WriteAttributeString("row", _data.Str("row"));
            if (Col != 0)
                p_writer.WriteAttributeString("col", _data.Str("col"));
            if (RowSpan != 1)
                p_writer.WriteAttributeString("rowspan", _data.Str("rowspan"));
            if (ColSpan != 1)
                p_writer.WriteAttributeString("colspan", _data.Str("colspan"));
        }
    }
}
