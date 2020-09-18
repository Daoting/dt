#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-21 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Core;

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
        /// 获取序列化时标签名称
        /// </summary>
        public override string XmlName
        {
            get { return "TBody"; }
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
        public override void Build()
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
            BuildChild();
        }
    }
}
