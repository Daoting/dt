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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;

#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 矩阵数据行
    /// </summary>
    internal class RptMtxRow : RptItemBase
    {
        readonly RptMatrix _matrix;

        public RptMtxRow(RptMatrix p_matrix)
        {
            _matrix = p_matrix;
            Cells = new List<RptText>();
        }

        #region 外部属性
        /// <summary>
        /// 获取报表模板根对象
        /// </summary>
        public override RptRoot Root
        {
            get { return _matrix.Root; }
        }

        /// <summary>
        /// 获取报表项所属容器
        /// </summary>
        public override RptPart Part
        {
            get { return _matrix.Part; }
        }

        /// <summary>
        /// 获取报表项所属父项
        /// </summary>
        public override RptItemBase Parent
        {
            get { return _matrix; }
        }

        /// <summary>
        /// 获取设置报表元素起始行索引
        /// </summary>
        public override int Row
        {
            get 
            {
                int row = _matrix.Row;
                if(!_matrix.HideColHeader)
                {
                    row += _matrix.ColHeader.RowSpan;
                }
                return row + _matrix.Rows.IndexOf(this); 
            }
            set { }
        }

        /// <summary>
        /// 获取设置报表元素起始列索引
        /// </summary>
        public override int Col
        {
            get
            {
                int col = _matrix.Col;
                if (!_matrix.HideRowHeader)
                {
                    col += _matrix.RowHeader.ColSpan;
                }
                return col + _matrix.Rows.IndexOf(this); 
            }
            set { }
        }

        /// <summary>
        /// 获取设置报表元素所占行数
        /// </summary>
        public override int RowSpan
        {
            get { return Cells.Count > 0 ? Cells[0].RowSpan : 0; }
            set { }
        }

        /// <summary>
        /// 获取设置报表元素所占列数
        /// </summary>
        public override int ColSpan
        {
            get { return Cells.Count > 0 ? Cells[0].ColSpan : 0; }
            set { }
        }

        /// <summary>
        /// 获取行内容单元格
        /// </summary>
        public List<RptText> Cells { get; }
        #endregion

        /// <summary>
        /// 克隆行
        /// </summary>
        /// <param name="p_mat"></param>
        /// <returns></returns>
        public RptMtxRow Clone(RptMatrix p_mat)
        {
            RptMtxRow row = new RptMtxRow(p_mat);
            foreach (RptText txt in Cells)
            {
                RptText newtxt = new RptText(row);
                newtxt.Data.Copy(txt.Data);
                row.Cells.Add(newtxt);
            }
            return row;
        }

        #region xml
        protected override void ReadChildXml(XmlReader p_reader)
        {
            if (p_reader.Name == "Text")
            {
                RptText cell = new RptText(this);
                cell.ReadXml(p_reader);
                Cells.Add(cell);
            }
        }

        public override void WriteXml(XmlWriter p_writer)
        {
            p_writer.WriteStartElement("MRow");
            if (Cells.Count > 0)
            {
                foreach (RptText cell in Cells)
                {
                    cell.WriteXml(p_writer);
                }
            }
            p_writer.WriteEndElement();
        }
        #endregion
    }
}
