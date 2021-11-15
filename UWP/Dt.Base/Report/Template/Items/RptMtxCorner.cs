#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-21 创建
**************************************************************************/
#endregion

#region 命名空间
using System.Xml;

#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 矩阵角
    /// </summary>
    internal class RptMtxCorner : RptItemBase
    {
        readonly RptMatrix _matrix;

        public RptMtxCorner(RptMatrix p_matrix)
        {
            _matrix = p_matrix;
            Item = new RptText(this);
        }

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
        /// 获取矩阵角起始行索引
        /// </summary>
        public override int Row
        {
            get { return _matrix.Row; }
            set { }
        }

        /// <summary>
        /// 获取矩阵角起始列索引
        /// </summary>
        public override int Col
        {
            get { return _matrix.Col; }
            set { }
        }

        /// <summary>
        /// 获取矩阵角所占行数
        /// </summary>
        public override int RowSpan
        {
            get { return _matrix.RowHeader == null ? 1 : _matrix.ColHeader.RowSpan; }
            set { }
        }

        /// <summary>
        /// 获取矩阵角所占列数
        /// </summary>
        public override int ColSpan
        {
            get { return _matrix.ColHeader == null ? 1 : _matrix.RowHeader.ColSpan; }
            set { }
        }

        /// <summary>
        /// 获取对应文本框
        /// </summary>
        public RptText Item { get; }

        /// <summary>
        /// 读取子元素xml，结束时定位在该子元素的末尾元素上
        /// </summary>
        /// <param name="p_reader"></param>
        protected override void ReadChildXml(XmlReader p_reader)
        {
            if (p_reader.Name == "Text")
            {
                Item.ReadXml(p_reader);
            }
        }

        public override void WriteXml(XmlWriter p_writer)
        {
            p_writer.WriteStartElement("Corner");
            Item.WriteXml(p_writer);
            p_writer.WriteEndElement();
        }

        /// <summary>
        /// 克隆矩阵角
        /// </summary>
        /// <param name="p_mtx"></param>
        /// <returns></returns>
        public RptMtxCorner Clone(RptMatrix p_mtx)
        {
            RptMtxCorner corner = new RptMtxCorner(p_mtx);
            corner.Item.Data.Copy(Item.Data);
            return corner;
        }
    }
}
