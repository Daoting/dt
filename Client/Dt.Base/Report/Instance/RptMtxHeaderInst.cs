#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-11 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Cells.Data;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Xml;
using Windows.UI;
using Windows.UI.Xaml.Media;
using System.Linq;
#endregion

namespace Dt.Base.Report
{
    internal class RptMtxHeaderInst : RptItemInst
    {
        public RptMtxHeaderInst(RptItemBase p_item)
            : base(p_item)
        {
            RptMatrix mat = p_item.Parent as RptMatrix;
            HeaderType = p_item is RptMtxRowHeader ? RptMtxHeaderType.Row : RptMtxHeaderType.Col;
            Visible = HeaderType == RptMtxHeaderType.Row ? !mat.HideRowHeader : !mat.HideColHeader;
            TxtInsts = new List<RptTextInst>();
            Filter = new Dictionary<string, string>();
        }

        #region 属性
        /// <summary>
        /// 获取设置过滤条件
        /// </summary>
        public Dictionary<string, string> Filter { get; set; }

        /// <summary>
        /// 获取设置头类型
        /// </summary>
        public RptMtxHeaderType HeaderType { get; set; }

        /// <summary>
        /// 获取设置数据源
        /// </summary>
        public RptData RptData { get; set; }

        /// <summary>
        /// 获取设置当前对应数据索引
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 获取Inst列表
        /// </summary>
        public List<RptTextInst> TxtInsts { get; }

        /// <summary>
        /// 获取设置对应模板单元格行号
        /// </summary>
        public int MtxRowsRow { get; set; }

        /// <summary>
        /// 获取设置对应模板单元格列号
        /// </summary>
        public int MtxRowsCol { get; set; }
        #endregion

        #region 公共方法
        /// <summary>
        /// 复制行（列）头
        /// </summary>
        /// <returns></returns>
        public RptMtxHeaderInst Clone()
        {
            RptMtxHeaderInst inst = new RptMtxHeaderInst(base.Item);
            inst.Index = Index;
            inst.Filter = Filter;
            inst.RptData = RptData;
            foreach (RptTextInst txtInst in TxtInsts)
            {
                inst.TxtInsts.Add(txtInst.Clone());
            }
            return inst;
        }
        #endregion

        #region 重写
        protected override void DoOutput()
        {
            RptData.Current = Index;
            int index = 0;
            RptTextInst beforeTxt = null;
            foreach (RptTextInst txt in TxtInsts)
            {
                txt.Region = beforeTxt == null ? _region.Clone() : beforeTxt.Region.Clone();
                if (HeaderType == RptMtxHeaderType.Col)
                {
                    txt.Region.Row += index++;
                }
                else
                {
                    txt.Region.Col += index++;
                }
                txt.Region.RowSpan = txt.Item.RowSpan;
                txt.Region.ColSpan = txt.Item.ColSpan;
                txt.Output();
                beforeTxt = txt;
            }
        }
        #endregion
    }
}
