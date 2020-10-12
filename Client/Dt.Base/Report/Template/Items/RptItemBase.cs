#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-21 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Core;
using System;
using System.Threading.Tasks;
using System.Xml;
#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 报表元素基类
    /// </summary>
    internal abstract class RptItemBase
    {
        protected Row _data;

        /// <summary>
        /// 构造方法
        /// </summary>
        public RptItemBase()
        {
            _data = new Row();
            _data.Changed += OnValueChanged;
        }

        /// <summary>
        /// 获取数据源
        /// </summary>
        public Row Data
        {
            get { return _data; }
        }

        /// <summary>
        /// 获取报表模板根对象
        /// </summary>
        public abstract RptRoot Root { get; }

        /// <summary>
        /// 获取报表项所属容器
        /// </summary>
        public abstract RptPart Part { get; }

        /// <summary>
        ///  获取报表元素父项
        /// </summary>
        public abstract RptItemBase Parent { get; }

        /// <summary>
        /// 获取设置报表元素起始行索引
        /// </summary>
        public abstract int Row { get; set; }

        /// <summary>
        /// 获取设置报表元素起始列索引
        /// </summary>
        public abstract int Col { get; set; }

        /// <summary>
        /// 获取设置报表元素所占行数
        /// </summary>
        public abstract int RowSpan { get; set; }

        /// <summary>
        /// 获取设置报表元素所占列数
        /// </summary>
        public abstract int ColSpan { get; set; }

        /// <summary>
        /// 获取报表元素宽度
        /// </summary>
        public double Width
        {
            get
            {
                int index = Col;
                double[] cols = Root.Cols;
                if (index + ColSpan > cols.Length)
                    throw new Exception("位置超出列数！");

                double total = 0;
                for (int i = 0; i < ColSpan; i++)
                {
                    total += cols[i + index];
                }
                return total;
            }
        }

        /// <summary>
        /// 获取报表元素高度
        /// </summary>
        public double Height
        {
            get
            {
                RptPart part = Part;
                if (part.PartType == RptPartType.Header || part.PartType == RptPartType.Footer)
                    return part.Height;

                int index = Row;
                double[] rows = Root.Body.Rows;
                if (rows == null || index + RowSpan > rows.Length)
                    throw new Exception("位置超出行数！");

                double total = 0;
                for (int i = 0; i < RowSpan; i++)
                {
                    total += rows[i + index];
                }
                return total;
            }
        }

        /// <summary>
        /// 获取内部行高
        /// </summary>
        /// <param name="p_offset">偏移位置</param>
        /// <returns></returns>
        public double GetRowHeight(int p_offset)
        {
            RptPart part = Part;
            if (part.PartType == RptPartType.Header || part.PartType == RptPartType.Footer)
                return part.Height;

            int index = Row + p_offset;
            double[] rows = Root.Body.Rows;
            if (rows == null || index >= rows.Length)
                throw new Exception("位置超出行数！");
            return rows[index];
        }

        /// <summary>
        /// 获取内部列宽
        /// </summary>
        /// <param name="p_offset">偏移位置</param>
        /// <returns></returns>
        public double GetColWidth(int p_offset)
        {
            double[] cols = Root.Cols;
            int index = Col + p_offset;
            if (cols == null || index >= cols.Length)
                throw new Exception("位置超出列数！");
            return cols[index];
        }

        /// <summary>
        /// 是否有重叠行
        /// </summary>
        /// <param name="p_row"></param>
        /// <param name="p_rowCount"></param>
        /// <returns></returns>
        public bool IsCrossWithRows(int p_row, int p_rowCount)
        {
            int row = Row;
            int rowCount = RowSpan;
            return ((row > p_row && row < (p_row + p_rowCount))
                || ((row + rowCount) > p_row && (row + rowCount) < (p_row + p_rowCount))
                || (row <= p_row && (row + rowCount) >= (p_row + p_rowCount)));
        }

        /// <summary>
        /// 是否有重叠列
        /// </summary>
        /// <param name="p_col"></param>
        /// <param name="p_colCount"></param>
        /// <returns></returns>
        public bool IsCrossWithColumns(int p_col, int p_colCount)
        {
            int col = Col;
            int colCount = ColSpan;
            return (((col > p_col) && (col < (p_col + p_colCount)))
                || (((col + colCount) > p_col) && ((col + colCount) < (p_col + p_colCount)))
                || ((col <= p_col) && ((col + colCount) >= (p_col + p_colCount))));
        }

        /// <summary>
        /// 是否存在重叠区域
        /// </summary>
        /// <param name="p_row"></param>
        /// <param name="p_column"></param>
        /// <param name="p_rowCount"></param>
        /// <param name="p_columnCount"></param>
        /// <returns></returns>
        public bool IsCrossWithRegion(int p_row, int p_column, int p_rowCount, int p_columnCount)
        {
            return (IsCrossWithRows(p_row, p_rowCount) && IsCrossWithColumns(p_column, p_columnCount));
        }

        /// <summary>
        /// 构造报表元素实例
        /// </summary>
        public virtual Task Build()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 值变化时通知Root
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnValueChanged(object sender, Cell e)
        {
            Root?.OnItemValueChanged(this, e);
        }

        #region xml
        /// <summary>
        /// 加载xml
        /// </summary>
        /// <param name="p_reader"></param>
        public virtual void ReadXml(XmlReader p_reader)
        {
            string name = p_reader.Name;
            _data.ReadXml(p_reader);
            if (!p_reader.IsEmptyElement)
            {
                p_reader.Read();
                while (p_reader.NodeType != XmlNodeType.None)
                {
                    if (p_reader.NodeType == XmlNodeType.EndElement && p_reader.Name == name)
                        break;

                    ReadChildXml(p_reader);
                }
            }
            p_reader.Read();
        }

        /// <summary>
        /// 读取子元素xml，结束时定位在该子元素的下一元素上
        /// </summary>
        /// <param name="p_reader"></param>
        protected virtual void ReadChildXml(XmlReader p_reader)
        {
        }

        /// <summary>
        /// 序列化xml
        /// </summary>
        /// <param name="p_writer"></param>
        public abstract void WriteXml(XmlWriter p_writer);
        #endregion
    }
}
