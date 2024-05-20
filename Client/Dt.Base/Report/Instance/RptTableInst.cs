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
    public class RptTableInst : RptItemInst
    {
        #region 成员变量
        protected readonly List<RptTblPartInst> _rows;
        protected RptTblColHeaderInst _colHeader;
        protected RptTblColFooterInst _colFooter;
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
        /// 获取设置列头
        /// </summary>
        public RptTblColHeaderInst ColHeader
        {
            get { return _colHeader; }
            set
            {
                _colHeader = value;
                _colHeader.Parent = this;
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
        /// 获取设置列尾
        /// </summary>
        public RptTblColFooterInst ColFooter
        {
            get { return _colFooter; }
            set
            {
                _colFooter = value;
                _colFooter.Parent = value;
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
        /// 输出单列表格布局，列头、列尾、行头可重复
        /// </summary>
        void OutputTable()
        {
            _region.RowSpan = 0;
            RptRootInst root = Inst;
            RptRegion region;

            root.VerPageBegin += OnVerPageBegin;
            root.VerPageEnd += OnPageEnd;
            // 输出列头时可能出现水平分页
            root.HorPageBegin += OnHorPageBegin;

            if (_colHeader != null)
            {
                _colHeader.Output();
                region = _colHeader.Region;
                _region.RowSpan = region.Row + region.RowSpan - _region.Row;
            }

            RptTable tbl = _item as RptTable;
            if (tbl.ColFooter != null && tbl.RepeatColFooter)
                root.TblFooterHeight = tbl.ColFooter.Height;

            if (_data != null && _rows.Count > 0)
            {
                _data.Current = 0;
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
                _data.Current = 0;
            }

            _curPart = null;
            root.TblFooterHeight = 0.0;
            root.VerPageBegin -= OnVerPageBegin;
            root.VerPageEnd -= OnPageEnd;
            root.HorPageBegin -= OnHorPageBegin;

            if (_colFooter != null)
            {
                RptItemBase item = _colFooter.Item;
                region = new RptRegion(
                    _region.Row + _region.RowSpan,
                    _region.Col,
                    item.RowSpan,
                    item.ColSpan);
                _colFooter.Region = region;
                _colFooter.Output();
                _region.RowSpan = region.Row + region.RowSpan - _region.Row;
            }
        }

        /// <summary>
        /// 垂直分页时在新页重复列头
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnVerPageBegin(object sender, RptPage e)
        {
            if (_colHeader == null || !((RptTable)_item).RepeatColHeader)
                return;

            int index = _data.Current;
            RptTblColHeaderInst inst = _colHeader.Clone() as RptTblColHeaderInst;

            RptRegion region = new RptRegion(
                e.Rows.Start,
                _region.Col,
                inst.Item.RowSpan,
                inst.Item.ColSpan);
            inst.Region = region;

            // 复制子项区域
            for (int i = 0; i < _colHeader.Children.Count; i++)
            {
                var r = _colHeader.Children[i].Region;
                inst.Children[i].Region = new RptRegion(
                    e.Rows.Start,
                    r.Col,
                    r.RowSpan,
                    r.ColSpan);
            }
            
            double height = _item.Part.GetRowHeight(region.Row + region.RowSpan - e.Rows.Start);
            if (e.Rows.Size.Count > 0 && height != e.Rows.Size[0])
                e.Rows.Size[0] = height;
            inst.Output();
            _data.Current = index;

            // 顺次下移
            if (_curPart != null)
                _curPart.Region.Row = region.Row + region.RowSpan;
        }

        /// <summary>
        /// 水平分页时重复输出行头
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        void OnHorPageBegin(object sender, RptPage e)
        {
            var tbl = _item as RptTable;
            if (tbl.RepeatRowHeaderCols <= 0 || e.IsColHasDefine())
                return;

            // 只在第一排水平分页时处理
            
            // 首次分页时，取第一页定义添加到当前页定义
            RptRootInst root = sender as RptRootInst;
            var firstPage = root.Cols[0];
            for (int i = 0; i < tbl.RepeatRowHeaderCols; i++)
            {
                e.Cols.Size.Insert(i, firstPage.Size[tbl.Col + i]);
            }

            // 当前分页的起始列索引
            int start = _colHeader != null ? _colHeader.OutputIndex : _rows[0].OutputIndex;

            // 重复输出列头的列
            if (_colHeader != null)
            {
                // 将后续列头的位置顺次后移
                for (int i = start; i < _colHeader.Children.Count; i++)
                {
                    var item = _colHeader.Children[i];
                    if (item.Region == null)
                        item.RefreshPosition();
                    item.Region.Col += tbl.RepeatRowHeaderCols;
                }

                // 插入行头
                int insert = start;
                for (int i = 0; i < tbl.RepeatRowHeaderCols; i++)
                {
                    // 列头可能多行
                    for (int j = 0; j < _colHeader.OutputIndex; j++)
                    {
                        var item = _colHeader.Children[j];
                        if (item.Region == null || item.Region.Col > i)
                            break;

                        if (item.Region.Col == i)
                        {
                            var newItem = item.Clone();
                            newItem.Region = new RptRegion(item.Region.Row, e.Cols.Start + item.Region.Col, item.Region.RowSpan, item.Region.ColSpan);
                            _colHeader.InsertChild(insert++, newItem);
                            newItem.Output();
                        }
                    }
                }
                _colHeader.Region.ColSpan += tbl.RepeatRowHeaderCols;
            }

            // 重复输出数据行的行头列
            foreach (var row in _rows)
            {
                // 后续列顺次后移
                for (int i = start; i < row.Children.Count; i++)
                {
                    var item = row.Children[i];
                    if (item.Region != null)
                        item.Region.Col += tbl.RepeatRowHeaderCols;
                    else
                        item.OffsetX += tbl.RepeatRowHeaderCols;
                }

                // 重复输出
                int insert = start;
                for (int i = 0; i < tbl.RepeatRowHeaderCols; i++)
                {
                    var item = row.Children[i];
                    var newItem = item.Clone();
                    newItem.OffsetX = e.Cols.Start;
                    row.InsertChild(insert++, newItem);
                }
            }

            // 增加列跨度
            _region.ColSpan += tbl.RepeatRowHeaderCols;
        }

        /// <summary>
        /// 切换页面时在旧页重复列尾
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnPageEnd(object sender, PageDefine e)
        {
            if (_colFooter != null && (_item as RptTable).RepeatColFooter)
            {
                int index = _data.Current;
                RptTblColFooterInst inst = _colFooter.Clone() as RptTblColFooterInst;
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
        /// 输出多列表格布局
        /// </summary>
        void OutputList()
        {
            RptItemBase tempItem;
            RptRootInst root = Inst;
            List<RptRegion> regions = new List<RptRegion>();
            RptRegion region = new RptRegion(_region.Row, _region.Col, 0, _item.ColSpan);
            regions.Add(region);

            if (_colHeader != null)
            {
                _colHeader.Output();
                region.RowSpan += _colHeader.Region.RowSpan;
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

                    // 输出列头
                    if (_colHeader != null)
                    {
                        RptTblColHeaderInst header = _colHeader.Clone() as RptTblColHeaderInst;
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

            if (_colFooter != null)
            {
                region = _region;
                if (regions.Count > 0)
                {
                    region = regions[regions.Count - 1];
                }
                tempItem = _colFooter.Item;
                _colFooter.Region = new RptRegion(
                    region.Row + region.RowSpan,
                    region.Col,
                    tempItem.RowSpan,
                    tempItem.ColSpan);
                _colFooter.Output();

                if (((_colFooter.Region.Row + _colFooter.Region.RowSpan) - _region.Row) > _region.RowSpan)
                {
                    _region.RowSpan = (_colFooter.Region.Row + _colFooter.Region.RowSpan) - _region.Row;
                }

                if (((_colFooter.Region.Col + _colFooter.Region.ColSpan) - _region.Col) > _region.ColSpan)
                {
                    _region.ColSpan = (_colFooter.Region.Col + _colFooter.Region.ColSpan) - _region.Col;
                }
            }
        }
    }
}
