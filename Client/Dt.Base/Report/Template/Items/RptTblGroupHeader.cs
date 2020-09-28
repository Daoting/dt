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
using System.Threading.Tasks;
using System.Xml;

#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 表格分组头行
    /// </summary>
    internal class RptTblGroupHeader : RptTblPart
    {
        public RptTblGroupHeader(RptTable p_table)
            : base(p_table)
        {
        }

        /// <summary>
        /// 构造报表项实例
        /// </summary>
        /// <param name="p_filter"></param>
        public Task Build(Dictionary<string, string> p_filter)
        {
            RptRootInst inst = Root.Inst;
            RptTblGroupHeaderInst header = new RptTblGroupHeaderInst(this);
            if (p_filter != null && p_filter.Count > 0) 
                header.Filter = p_filter;
            if (inst.CurrentParent is RptTblPartInst)
                header.Index = (inst.CurrentParent as RptTblPartInst).Index;
            if (inst.CurrentTable != null)
                inst.CurrentTable.AddRow(header);
            inst.CurrentParent = header;
            return BuildChild();
        }

        public override void WriteXml(XmlWriter p_writer)
        {
            p_writer.WriteStartElement("TGroupHeader");
            WriteChildXml(p_writer);
            p_writer.WriteEndElement();
        }
    }
}
