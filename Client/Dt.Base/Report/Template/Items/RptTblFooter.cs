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
    /// 表尾
    /// </summary>
    internal class RptTblFooter : RptTblPart
    {
        public RptTblFooter(RptTable p_table)
            : base(p_table)
        {
        }

        /// <summary>
        /// 构造报表项实例
        /// </summary>
        public override Task Build()
        {
            RptRootInst inst = Root.Inst;
            RptTblFooterInst footer = new RptTblFooterInst(this);
            if (inst.CurrentTable != null)
                inst.CurrentTable.Footer = footer;
            inst.CurrentParent = footer;
            return BuildChild();
        }

        public override void WriteXml(XmlWriter p_writer)
        {
            p_writer.WriteStartElement("TFooter");
            WriteChildXml(p_writer);
            p_writer.WriteEndElement();
        }
    }
}
