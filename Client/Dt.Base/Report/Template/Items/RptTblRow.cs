#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-21 创建
**************************************************************************/
#endregion

#region 命名空间
using System.Threading.Tasks;
using System.Xml;
#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 表格数据行部分
    /// </summary>
    internal class RptTblRow : RptTblPart
    {
        public RptTblRow(RptTable p_table)
            : base(p_table)
        {
            // 最少行数
            _data.AddCell("minrowcount", 0);
        }

        /// <summary>
        /// 获取设置数据区域最少行数，不够时增加空行
        /// </summary>
        public int MinRowCount
        {
            get { return _data.Int("minrowcount"); }
            set { _data["minrowcount"] = value; }
        }

        /// <summary>
        /// 构造报表项实例
        /// </summary>
        public override Task Build()
        {
            RptRootInst inst = Root.Inst;
            RptTblRowInst row = new RptTblRowInst(this);
            RptTblRowInst preRow = inst.CurrentParent as RptTblRowInst;

            if (preRow == null)
            {
                RptTblPartInst prePart = inst.CurrentParent as RptTblPartInst;
                row.Index = (prePart == null || prePart.Index == 0) ? 0 : prePart.Index + 1;
            }
            else
                row.Index = preRow.Index + 1;

            if (inst.CurrentTable != null)
                inst.CurrentTable.AddRow(row);
            inst.CurrentParent = row;
            return BuildChild();
        }

        public override void WriteXml(XmlWriter p_writer)
        {
            p_writer.WriteStartElement("TBody");
            string val = _data.Str("minrowcount");
            if (val != "0")
                p_writer.WriteAttributeString("minrowcount", val);
            WriteChildXml(p_writer);
            p_writer.WriteEndElement();
        }
    }
}
