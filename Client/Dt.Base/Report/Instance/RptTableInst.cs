#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-11 创建
**************************************************************************/
#endregion

#region 命名空间
using System.Collections.Generic;
#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 表格实例
    /// </summary>
    internal class RptTableInst : RptItemInst
    {
        #region 成员变量
        protected readonly List<RptTblPartInst> _rows;
        protected RptTblHeaderInst _header;
        protected RptTblFooterInst _footer;
        protected RptData _data;
        RptTblPartInst _curPart;
        #endregion

        #region 构造方法
        public RptTableInst(RptItemBase p_item)
            : base(p_item)
        {
            _rows = new List<RptTblPartInst>();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取设置数据源
        /// </summary>
        public RptData Data
        {
            get { return _data; }
            set { _data = value; }
        }

        /// <summary>
        /// 获取设置表头
        /// </summary>
        public RptTblHeaderInst Header
        {
            get { return _header; }
            set
            {
                _header = value;
                _header.Parent = this;
            }
        }

        /// <summary>
        /// 获取数据行
        /// </summary>
        public List<RptTblPartInst> Rows
        {
            get { return _rows; }
        }

        /// <summary>
        /// 获取设置表尾
        /// </summary>
        public RptTblFooterInst Footer
        {
            get { return _footer; }
            set
            {
                _footer = value;
                _footer.Parent = value;
            }
        }
        #endregion

        /// <summary>
        /// 添加数据行
        /// </summary>
        /// <param name="p_row"></param>
        public void AddRow(RptTblPartInst p_row)
        {
            p_row.Parent = this;
            _rows.Add(p_row);
        }

        /// <summary>
        /// 输出报表项内容
        /// </summary>
        protected override void DoOutput()
        {
            RptTable tbl = _item as RptTable;
            if (tbl.RowBreakCount == 0)
                OutputTable();
            else
                OutputList();
        }

        /// <summary>
        /// 输出单列表头表尾可重复表格布局
        /// </summary>
        void OutputTable()
        {
            _region.RowSpan = 0;
            _data.Current = 0;
            RptRootInst root = Inst;
            RptRegion region;

            if (_header != null)
            {
                _header.Output();
                region = _header.Region;
                _region.RowSpan = region.Row + region.RowSpan - _region.Row;
            }

            root.VerPageBegin += OnPageBegin;
            root.VerPageEnd += OnPageEnd;
            RptTable tbl = _item as RptTable;
            if (tbl.Footer != null && tbl.RepeatFooter)
                root.TblFooterHeight = tbl.Footer.Height;

            foreach (RptTblPartInst inst in _rows)
            {
                _curPart = inst;
                RptItemBase item = inst.Item;
                region = new RptRegion(
                    _region.Row + _region.RowSpan,
                    _region.Col,
                    item.RowSpan,
                    item.ColSpan);
                inst.Region = region;
                inst.Output();
                _region.RowSpan = region.Row + region.RowSpan - _region.Row;
            }

            _curPart = null;
            root.TblFooterHeight = 0.0;
            root.VerPageBegin -= OnPageBegin;
            root.VerPageEnd -= OnPageEnd;

            if (_footer != null)
            {
                RptItemBase item = _footer.Item;
                region = new RptRegion(
                    _region.Row + _region.RowSpan,
                    _region.Col,
                    item.RowSpan,
                    item.ColSpan);
                _footer.Region = region;
                _footer.Output();
                _region.RowSpan = region.Row + region.RowSpan - _region.Row;
            }
            _data.Current = 0;
        }

        /// <summary>
        /// 切换页面时在新页重复表头
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnPageBegin(object sender, RptPage e)
        {
            if (_header != null && (_item as RptTable).RepeatHeader)
            {
                int index = _data.Current;
                RptTblHeaderInst inst = _header.Clone() as RptTblHeaderInst;
                
                RptRegion region = new RptRegion(
                    e.Rows.Start,
                    _region.Col,
                    inst.Item.RowSpan,
                    inst.Item.ColSpan);
                inst.Region = region;

                double height = _item.Part.GetRowHeight(region.Row + region.RowSpan - e.Rows.Start);
                if (e.Rows.Size.Count > 0 && height != e.Rows.Size[0])
                    e.Rows.Size[0] = height;
                inst.Output();
                _data.Current = index;

                // 顺次下移
                if (_curPart != null)
                    _curPart.Region.Row = region.Row + region.RowSpan;
            }
        }

        /// <summary>
        /// 切换页面时在旧页重复表尾
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnPageEnd(object sender, PageDefine e)
        {
            if (_footer != null && (_item as RptTable).RepeatFooter)
            {
                int index = _data.Current;
                RptTblFooterInst inst = _footer.Clone() as RptTblFooterInst;
                RptRegion region = new RptRegion(
                    e.Start + e.Count,
                    _region.Col,
                    inst.Item.RowSpan,
                    inst.Item.ColSpan);
                inst.Region = region;
                inst.Output();
                _data.Current = index;

                // 顺次下移
                if (_curPart != null)
                    _curPart.Region.Row = region.Row + region.RowSpan;
            }
        }

        /// <summary>
        /// 输出单列布局
        /// </summary>
        void OutputList()
        {
            RptItemBase tempItem;
            RptRootInst root = Inst;
            List<RptRegion> regions = new List<RptRegion>();
            RptRegion region = new RptRegion(_region.Row, _region.Col, 0, _item.ColSpan);
            regions.Add(region);

            if (_header != null)
            {
                _header.Output();
                region.RowSpan += _header.Region.RowSpan;
            }

            RptTable tbl = _item as RptTable;
            int rowCount = tbl.RowBreakCount;
            int colCount = tbl.ColBreakCount;
            int rowNum = 0;

            foreach (RptTblPartInst tabInst in _rows)
            {
                RptItemBase item = tabInst.Item;
                tabInst.Region = new RptRegion(
                    region.Row + region.RowSpan,
                    region.Col,
                    item.RowSpan,
                    item.ColSpan);

                bool rowBreak = false;
                bool colBreak = false;
                if (rowCount == -1)
                {
                    // 自动计算重复行数
                    if (root.TestPageBreak(tabInst))
                    {
                        // 需要换页
                        if (colCount != 0 && (regions.Count % colCount) == 0)
                        {
                            rowBreak = true;
                            colBreak = true;
                        }
                        else
                        {
                            rowBreak = false;
                            colBreak = true;
                        }
                    }
                }
                else if (rowNum == rowCount)
                {
                    // 应该换列
                    if (colCount != 0 && (regions.Count % colCount) == 0)
                    {
                        rowBreak = true;
                        colBreak = true;
                    }
                    else
                    {
                        rowBreak = false;
                        colBreak = true;
                    }
                }

                if (colBreak)
                {
                    // 换新列，重新累计行数
                    rowNum = 0;
                    if (rowBreak)
                    {
                        // 列数达到后换行显示
                        RptRegion region2 = regions[regions.Count - colCount];
                        region = new RptRegion(
                            region2.Row + region2.RowSpan,
                            region2.Col,
                            0,
                            region2.ColSpan);
                        regions.Add(region);
                    }
                    else
                    {
                        RptRegion region3 = regions[regions.Count - 1];
                        region = new RptRegion(
                            region3.Row,
                            region3.Col + region3.ColSpan,
                            0,
                            region3.ColSpan);
                        regions.Add(region);
                    }

                    // 输出表头
                    if (_header != null)
                    {
                        RptTblHeaderInst header = _header.Clone() as RptTblHeaderInst;
                        header.Region = region.Clone() as RptRegion;
                        header.Region.RowSpan = header.Item.RowSpan;
                        header.Output();
                        region.RowSpan = (header.Region.Row + header.Region.RowSpan) - region.Row;
                    }

                    tempItem = tabInst.Item;
                    tabInst.Region = new RptRegion(
                        region.Row + region.RowSpan,
                        region.Col,
                        tempItem.RowSpan,
                        tempItem.ColSpan);
                    tabInst.Output();
                }
                else
                {
                    tabInst.Output();
                }

                region.RowSpan = (tabInst.Region.Row + tabInst.Region.RowSpan) - region.Row;
                if (((tabInst.Region.Row + tabInst.Region.RowSpan) - _region.Row) > _region.RowSpan)
                {
                    _region.RowSpan = (tabInst.Region.Row + tabInst.Region.RowSpan) - _region.Row;
                }

                if (((tabInst.Region.Col + tabInst.Region.ColSpan) - _region.Col) > _region.ColSpan)
                {
                    _region.ColSpan = (tabInst.Region.Col + tabInst.Region.ColSpan) - _region.Col;
                }
                rowNum++;
            }

            if (_footer != null)
            {
                region = _region;
                if (regions.Count > 0)
                {
                    region = regions[regions.Count - 1];
                }
                tempItem = _footer.Item;
                _footer.Region = new RptRegion(
                    region.Row + region.RowSpan,
                    region.Col,
                    tempItem.RowSpan,
                    tempItem.ColSpan);
                _footer.Output();

                if (((_footer.Region.Row + _footer.Region.RowSpan) - _region.Row) > _region.RowSpan)
                {
                    _region.RowSpan = (_footer.Region.Row + _footer.Region.RowSpan) - _region.Row;
                }

                if (((_footer.Region.Col + _footer.Region.ColSpan) - _region.Col) > _region.ColSpan)
                {
                    _region.ColSpan = (_footer.Region.Col + _footer.Region.ColSpan) - _region.Col;
                }
            }
        }
    }
}
