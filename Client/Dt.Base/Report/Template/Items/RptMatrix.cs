#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-21 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 矩阵
    /// </summary>
    internal class RptMatrix : RptItem
    {
        #region 构造
        public RptMatrix(RptPart p_owner)
            : base(p_owner)
        {
            Rows = new List<RptMtxRow>();
            _data.AddCell<string>("tbl");
            _data.AddCell<bool>("hiderowheader");
            _data.AddCell<bool>("hidecolheader");
            _data.AddCell<bool>("repeatrowheader");
            _data.AddCell<bool>("repeatcolheader");
            _data.AddCell<string>("rowsort");
            _data.AddCell<string>("colsort");
        }
        #endregion

        #region 属性
        //获取设置数据源ID
        public string Tbl
        {
            get { return _data.Str("tbl"); }
            set { _data["tbl"] = value; }
        }

        /// <summary>
        /// 获取设置是否显示 行 标题
        /// </summary>
        public bool HideRowHeader
        {
            get { return _data.Bool("hiderowheader"); }
            set { _data["hiderowheader"] = value; }
        }

        /// <summary>
        /// 获取设置是否显示 列 标题
        /// </summary>
        public bool HideColHeader
        {
            get { return _data.Bool("hidecolheader"); }
            set { _data["hidecolheader"] = value; }
        }

        /// <summary>
        /// 获取设置是否在每页重复 行 标题
        /// </summary>
        public bool RepeatRowHeader
        {
            get { return _data.Bool("repeatrowheader"); }
            set { _data["repeatrowheader"] = value; }
        }

        /// <summary>
        /// 获取设置是否在每页重复 列 标题
        /// </summary>
        public bool RepeatColHeader
        {
            get { return _data.Bool("repeatcolheader"); }
            set { _data["repeatcolheader"] = value; }
        }

        /// <summary>
        /// 获取设置 行 排序串
        /// </summary>
        public string RowSort
        {
            get { return _data.Str("rowsort"); }
            set { _data["rowsort"] = value; }
        }

        /// <summary>
        /// 获取设置 列 排序串
        /// </summary>
        public string ColSort
        {
            get { return _data.Str("colsort"); }
            set { _data["colsort"] = value; }
        }

        /// <summary>
        /// 获取设置矩阵角
        /// </summary>
        public RptMtxCorner Corner { get; set; }

        /// <summary>
        /// 获取设置行头
        /// </summary>
        public RptMtxRowHeader RowHeader { get; set; }

        /// <summary>
        /// 获取设置列头
        /// </summary>
        public RptMtxColHeader ColHeader { get; set; }

        /// <summary>
        /// 获取数据行
        /// </summary>
        public List<RptMtxRow> Rows { get; set; }
        #endregion

        #region 重写
        /// <summary>
        /// 生成新的矩阵
        /// </summary>
        /// <returns></returns>
        public override RptItem Clone()
        {
            RptMatrix mtx = new RptMatrix(this.Part);
            mtx._data.Copy(_data);
            mtx.Corner = Corner.Clone(mtx);
            mtx.ColHeader = ColHeader.Clone(mtx);
            mtx.RowHeader = RowHeader.Clone(mtx);
            foreach(RptMtxRow row in Rows)
            {
                mtx.Rows.Add(row.Clone(mtx));
            }
            return mtx;
        }

        public override async Task Build()
        {
            RptRootInst inst = _part.Inst;
            if (string.IsNullOrEmpty(Tbl))
                return;

            // 使用时再加载
            var rptData = await inst.Info.GetData(Tbl);
            if (rptData == null)
                return;

            RptMatrixInst matrixInst = new RptMatrixInst(this);
            matrixInst.RptData = rptData;
            ReBuildData(matrixInst.RptData.Data);
            inst.Body.AddChild(matrixInst);
            if (!HideRowHeader && !HideColHeader)
            {
                RptTextInst corner = new RptTextInst(Corner.Item);
                matrixInst.CornerInst = corner;
            }
            //列头
            BuildHeader(rptData, ColHeader, matrixInst);
            //行头
            BuildHeader(rptData, RowHeader, matrixInst);
            //数据
            foreach (RptMtxHeaderInst rowHeaderInst in matrixInst.RowHeaderInsts)
            {
                foreach (RptMtxHeaderInst colHeaderInst in matrixInst.ColHeaderInsts)
                {
                    RptText cell = GetCellByRowCol(rowHeaderInst.MtxRowsRow, colHeaderInst.MtxRowsCol);
                    if (cell != null)
                    {
                        RptTextInst txtInst = new RptTextInst(cell);
                        Dictionary<string, string> filter = new Dictionary<string, string>();
                        foreach (string key in rowHeaderInst.Filter.Keys)
                        {
                            filter.Add(key, rowHeaderInst.Filter[key]);
                        }
                        foreach (string key in colHeaderInst.Filter.Keys)
                        {
                            filter.Add(key, colHeaderInst.Filter[key]);
                        }
                        txtInst.Filter = filter;
                        matrixInst.AddCell(txtInst);
                    }
                }
            }
        }
        #endregion

        #region 外部方法

        /// <summary>
        /// 获取指定位置所属区域类型
        /// </summary>
        /// <param name="p_row"></param>
        /// <param name="p_col"></param>
        /// <returns></returns>
        public MtxRangeType GetRangeType(int p_row, int p_col)
        {
            if (Contains(p_row, p_col, 1, 1))
            {
                if (Corner != null)
                {
                    if ((Corner.Row <= p_row && p_row < Corner.Row + Corner.RowSpan)
                        && Corner.Col <= p_col && p_col < Corner.Col + Corner.ColSpan)
                    {
                        return MtxRangeType.Corner;
                    }
                }
                if (RowHeader != null)
                {
                    if ((RowHeader.Row <= p_row && p_row < RowHeader.Row + RowHeader.RowSpan)
                        && RowHeader.Col <= p_col && p_col < RowHeader.Col + RowHeader.ColSpan)
                    {
                        return GetRangeTypeByLevel(RowHeader.Levels.ToList(), p_row, p_col);
                    }
                }
                if (ColHeader != null)
                {
                    if ((ColHeader.Row <= p_row && p_row < ColHeader.Row + ColHeader.RowSpan)
                        && ColHeader.Col <= p_col && p_col < ColHeader.Col + ColHeader.ColSpan)
                    {
                        return GetRangeTypeByLevel(ColHeader.Levels.ToList(), p_row, p_col);
                    }
                }
                return MtxRangeType.Body;
            }
            return MtxRangeType.Body;
        }

        /// <summary>
        /// 获取内部指定位置的文本
        /// </summary>
        /// <param name="p_row"></param>
        /// <param name="p_col"></param>
        /// <returns></returns>
        public RptText GetText(int p_row, int p_col)
        {
            MtxRangeType mtxType = GetRangeType(p_row, p_col);
            switch (mtxType)
            {
                case MtxRangeType.Corner:
                    {
                        return Corner.Item;
                    }
                case MtxRangeType.Level:
                    {
                        foreach (RptMtxLevel level in RowHeader.Levels)
                        {
                            if ((level.Row <= p_row && p_row < level.Row + level.RowSpan)
                                && level.Col <= p_col && p_col < level.Col + level.ColSpan)
                                return level.Item;
                        }
                        foreach (RptMtxLevel level in ColHeader.Levels)
                        {
                            if ((level.Row <= p_row && p_row < level.Row + level.RowSpan)
                                && level.Col <= p_col && p_col < level.Col + level.ColSpan)
                                return level.Item;
                        }
                        return null;
                    }
                case MtxRangeType.Subtotal:
                    {
                        foreach (RptMtxLevel level in RowHeader.Levels)
                        {
                            RptMtxSubtotal total = GetSubtotal(level.SubTotals.ToList(), p_row, p_col);
                            if (total != null)
                                return total.Item;
                        }
                        foreach (RptMtxLevel level in ColHeader.Levels)
                        {
                            RptMtxSubtotal total = GetSubtotal(level.SubTotals.ToList(), p_row, p_col);
                            if (total != null)
                                return total.Item;
                        }
                        return null;
                    }
                case MtxRangeType.Subtitle:
                    {
                        foreach (RptMtxLevel level in RowHeader.Levels)
                        {
                            RptMtxSubtitle title = GetSubtitle(level.SubTitles.ToList(), p_row, p_col);
                            if (title != null)
                                return title.Item;
                        }
                        foreach (RptMtxLevel level in ColHeader.Levels)
                        {
                            RptMtxSubtitle title = GetSubtitle(level.SubTitles.ToList(), p_row, p_col);
                            if (title != null)
                                return title.Item;
                        }
                        return null;
                    }
                case MtxRangeType.Body:
                    {
                        return Rows[p_row - Row - Corner.RowSpan].Cells[p_col - Col - Corner.ColSpan];
                    }
                case MtxRangeType.HeaderEmpty:
                    {
                        return null;
                    }
                default:
                    return null;
            }
        }

        /// <summary>
        /// 获取当前小计最大数 + 1
        /// </summary>
        /// <returns></returns>
        public int GetMaxTotal()
        {
            int maxIndex = -1;
            foreach (RptMtxLevel level in RowHeader.Levels)
            {
                foreach (RptMtxSubtotal total in level.SubTotals)
                {
                    int max = TotalMax(total);
                    if (maxIndex < max)
                        maxIndex = max;
                }
            }
            foreach (RptMtxLevel level in ColHeader.Levels)
            {
                foreach (RptMtxSubtotal total in level.SubTotals)
                {
                    int max = TotalMax(total);
                    if (maxIndex < max)
                        maxIndex = max;
                }
            }
            return maxIndex + 1;
        }

        /// <summary>
        /// 获取当前静态标题最大数 + 1
        /// </summary>
        /// <returns></returns>
        public int GetMaxTitle()
        {
            int maxIndex = -1;
            foreach (RptMtxLevel level in RowHeader.Levels)
            {
                foreach (RptMtxSubtitle title in level.SubTitles)
                {
                    int max = TitleMax(title);
                    if (maxIndex < max)
                        maxIndex = max;
                }
            }
            foreach (RptMtxLevel level in ColHeader.Levels)
            {
                foreach (RptMtxSubtitle title in level.SubTitles)
                {
                    int max = TitleMax(title);
                    if (maxIndex < max)
                        maxIndex = max;
                }
            }
            return maxIndex + 1;
        }

        /// <summary>
        /// 获取数据单元格个数
        /// </summary>
        /// <returns></returns>
        public int GetCellsCount()
        {
            return Rows.Sum(itm => itm.Cells.Count);
        }
        #endregion

        #region xml
        protected override void ReadChildXml(XmlReader p_reader)
        {
            switch (p_reader.Name)
            {
                case "Corner":
                    Corner = new RptMtxCorner(this);
                    Corner.ReadXml(p_reader);
                    break;
                case "RowHeader":
                    RowHeader = new RptMtxRowHeader(this);
                    RowHeader.ReadXml(p_reader);
                    break;
                case "ColHeader":
                    ColHeader = new RptMtxColHeader(this);
                    ColHeader.ReadXml(p_reader);
                    break;
                case "MRow":
                    if (Rows == null)
                        Rows = new List<RptMtxRow>();
                    RptMtxRow row = new RptMtxRow(this);
                    row.ReadXml(p_reader);
                    Rows.Add(row);
                    break;
                default:
                    break;
            }
        }

        public override void WriteXml(XmlWriter p_writer)
        {
            p_writer.WriteStartElement("Matrix");
            WritePosition(p_writer);

            string val = _data.Str("tbl");
            if (val != "")
                p_writer.WriteAttributeString("tbl", val);
            if (HideRowHeader)
                p_writer.WriteAttributeString("hiderowheader", "True");
            if (HideColHeader)
                p_writer.WriteAttributeString("hidecolheader", "True");
            if (RepeatRowHeader)
                p_writer.WriteAttributeString("repeatrowheader", "True");
            if (RepeatColHeader)
                p_writer.WriteAttributeString("repeatcolheader", "True");
            val = _data.Str("rowsort");
            if (val != "")
                p_writer.WriteAttributeString("rowsort", val);
            val = _data.Str("colsort");
            if (val != "")
                p_writer.WriteAttributeString("colsort", val);

            if (Corner != null)
                Corner.WriteXml(p_writer);
            if (RowHeader != null)
                RowHeader.WriteXml(p_writer);
            if (ColHeader != null)
                ColHeader.WriteXml(p_writer);
            if (Rows != null)
            {
                foreach (RptMtxRow row in Rows)
                {
                    row.WriteXml(p_writer);
                }
            }

            p_writer.WriteEndElement();
        }
        #endregion

        #region 方法
        /// <summary>
        /// 按排序条件重新整理数据
        /// </summary>
        /// <param name="p_data"></param>
        void ReBuildData(Table p_data)
        {
            string[] rowOrder = RowSort.ToLower().Split(',');
            string[] colOrder = ColSort.ToLower().Split(',');
            IOrderedEnumerable<Row> orderData = null;
            if (rowOrder.Length > 0)
            {
                orderData = Order(orderData, p_data.Clone(), rowOrder);
            }
            if (colOrder.Length > 0)
            {
                orderData = Order(orderData, p_data.Clone(), colOrder);
            }
            if (orderData != null)
            {
                p_data.Clear();
                foreach (Row dr in orderData)
                {
                    p_data.Add(dr);
                }
            }
        }

        /// <summary>
        /// 数据排序
        /// </summary>
        /// <param name="p_orderData"></param>
        /// <param name="p_data"></param>
        /// <param name="p_orders"></param>
        /// <returns></returns>
        IOrderedEnumerable<Row> Order(IOrderedEnumerable<Row> p_orderData, Table p_data, string[] p_orders)
        {
            foreach (string order in p_orders)
            {
                bool isDesc = order.IndexOf("desc") >= 0;
                string orderStr = isDesc ? order.Substring(0, order.IndexOf("desc")).Trim() : order;
                if (p_data.Columns.Contains(orderStr))
                {
                    if (p_orderData == null)
                    {
                        if (isDesc)
                        {
                            p_orderData = p_data.OrderByDescending(itm => itm.Str(orderStr));
                        }
                        else
                        {
                            p_orderData = p_data.OrderBy(itm => itm.Str(orderStr));
                        }
                    }
                    else
                    {
                        if (isDesc)
                        {
                            p_orderData = p_orderData.ThenByDescending(itm => itm.Str(orderStr));
                        }
                        else
                        {
                            p_orderData = p_orderData.ThenBy(itm => itm.Str(orderStr));
                        }
                    }
                }
            }
            return p_orderData;
        }

        /// <summary>
        /// 构造行头或列头
        /// </summary>
        /// <param name="p_rptData"></param>
        /// <param name="p_header"></param>
        /// <param name="p_matInst"></param>
        void BuildHeader(RptData p_rptData, RptMtxHeader p_header, RptMatrixInst p_matInst)
        {
            List<string> colNames = p_header.GetFieldNames();
            List<Row> allHeaderData = SelectDistinctDataRows(p_rptData.Data, colNames);
            Row preRow = null;
            for (int i = 0; i < allHeaderData.Count; i++)
            {
                Row curRow = allHeaderData[i];
                if (preRow == null || !string.IsNullOrEmpty(CompareTwoRows(preRow, curRow, colNames)))
                {
                    //前面小计
                    foreach (RptMtxLevel level in p_header.Levels)
                    {
                        if (level.SubTotals.Count <= 0)
                            continue;
                        RptMtxLevel beforLevel = null;
                        if (p_header.Levels.IndexOf(level) - 1 >= 0)
                            beforLevel = p_header.Levels[p_header.Levels.IndexOf(level) - 1];
                        //第一行（列） 或者 前一行（列）层值与当前行层值不同 创建小计
                        if (preRow == null || (beforLevel != null && preRow.Str(beforLevel.Field) != curRow.Str(beforLevel.Field)))
                        {
                            CreateTotalInst(level.SubTotals, p_header, p_matInst, curRow, p_rptData, TotalLocation.Before);
                        }
                    }

                    //数据
                    RptMtxHeaderInst headerInst = new RptMtxHeaderInst(p_header);
                    foreach (RptMtxLevel level in p_header.Levels)
                    {
                        if (level.SubTitles.Count > 0)
                        {
                            CreateSubTiltelInst(level.SubTitles, p_header, p_matInst, curRow, p_rptData);
                        }
                        else
                        {
                            //手动置1，因为输出头的时候按一行或一列输出，不需要控件合并
                            level.Item.ColSpan = 1;
                            level.Item.RowSpan = 1;
                            RptTextInst txtlevel = new RptTextInst(level.Item);
                            headerInst.TxtInsts.Add(txtlevel);
                        }
                    }
                    if (headerInst.TxtInsts.Count > 0)
                    {
                        headerInst.RptData = p_rptData;
                        headerInst.Index = curRow.Index;
                        if (p_header is RptMtxColHeader)
                        {
                            headerInst.MtxRowsCol = p_header.Levels[p_header.Levels.Count - 1].Col;
                            p_matInst.AddColHeader(headerInst);
                        }
                        else
                        {
                            headerInst.MtxRowsRow = p_header.Levels[p_header.Levels.Count - 1].Row;
                            p_matInst.AddRowHeader(headerInst);
                        }
                        //生成条件
                        foreach (string name in colNames)
                        {
                            headerInst.Filter.Add(name, curRow.Str(name));
                        }
                    }

                    //后面小计
                    for (int k = p_header.Levels.Count - 1; k >= 0; k--)
                    {
                        RptMtxLevel level = p_header.Levels[k];
                        if (level.SubTotals.Count <= 0)
                            continue;
                        RptMtxLevel beforLevel = null;
                        if (p_header.Levels.IndexOf(level) - 1 >= 0)
                            beforLevel = p_header.Levels[p_header.Levels.IndexOf(level) - 1];
                        Row nextRow = null;
                        if (i < allHeaderData.Count - 1)
                            nextRow = allHeaderData[i + 1];
                        //最后一行（列） 或者 下一行（列）层值与当前行层值不同 创建小计
                        if (nextRow == null || (beforLevel != null && curRow.Str(beforLevel.Field) != nextRow.Str(beforLevel.Field)))
                        {
                            foreach (RptMtxSubtotal total in level.SubTotals)
                            {
                                CreateTotalInst(level.SubTotals, p_header, p_matInst, curRow, p_rptData, TotalLocation.After);
                            }
                        }
                    }
                }
                preRow = curRow;
            }
        }

        /// <summary>
        /// 创建小计
        /// </summary>
        /// <param name="totals"></param>
        /// <param name="p_header"></param>
        /// <param name="p_matInst"></param>
        /// <param name="p_curRow"></param>
        /// <param name="p_data"></param>
        /// <param name="p_loc"></param>
        void CreateTotalInst(List<RptMtxSubtotal> totals, RptMtxHeader p_header, RptMatrixInst p_matInst, Row p_curRow, RptData p_data, TotalLocation p_loc)
        {
            foreach (RptMtxSubtotal total in totals)
            {
                if (total.TotalLoc == p_loc)
                {
                    if (total.SubTotals.Count > 0)
                    {
                        CreateTotalInst(total.SubTotals, p_header, p_matInst, p_curRow, p_data, p_loc);
                    }
                    else
                    {
                        RptMtxHeaderInst totalInst = new RptMtxHeaderInst(p_header);
                        for (int j = 0; j < p_header.Levels.IndexOf(total.Level); j++)
                        {
                            RptMtxLevel nLevel = p_header.Levels[j];
                            nLevel.Item.ColSpan = 1;
                            nLevel.Item.RowSpan = 1;
                            RptTextInst txtlevel = new RptTextInst(nLevel.Item);
                            totalInst.TxtInsts.Add(txtlevel);
                            totalInst.Filter.Add(nLevel.Field, p_curRow.Str(nLevel.Field));
                        }
                        RptItemBase parent = total.Parent;
                        while (parent is RptMtxSubtotal)
                        {
                            RptTextInst parTotal = new RptTextInst((parent as RptMtxSubtotal).Item);
                            totalInst.TxtInsts.Add(parTotal);
                            parent = parent.Parent;
                        }
                        RptTextInst txtTotal = new RptTextInst(total.Item);
                        totalInst.TxtInsts.Add(txtTotal);
                        totalInst.RptData = p_data;
                        totalInst.RptData.Current = p_curRow.Index;
                        totalInst.Index = p_curRow.Index;
                        if (p_header is RptMtxColHeader)
                        {
                            totalInst.MtxRowsCol = total.Col;
                            p_matInst.AddColHeader(totalInst);
                        }
                        else
                        {
                            totalInst.MtxRowsRow = total.Row;
                            p_matInst.AddRowHeader(totalInst);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 创建标题
        /// </summary>
        /// <param name="p_titles"></param>
        /// <param name="p_header"></param>
        /// <param name="p_matInst"></param>
        /// <param name="p_curRow"></param>
        /// <param name="p_data"></param>
        void CreateSubTiltelInst(List<RptMtxSubtitle> p_titles, RptMtxHeader p_header, RptMatrixInst p_matInst, Row p_curRow, RptData p_data)
        {
            foreach (RptMtxSubtitle title in p_titles)
            {
                if (title.SubTitles.Count > 0)
                {
                    CreateSubTiltelInst(title.SubTitles, p_header, p_matInst, p_curRow, p_data);
                }
                else
                {
                    RptMtxHeaderInst titleInst = new RptMtxHeaderInst(p_header);
                    for (int j = 0; j <= p_header.Levels.IndexOf(title.Level); j++)
                    {
                        RptMtxLevel nLevel = p_header.Levels[j];
                        nLevel.Item.ColSpan = 1;
                        nLevel.Item.RowSpan = 1;
                        RptTextInst txtlevel = new RptTextInst(nLevel.Item);
                        titleInst.TxtInsts.Add(txtlevel);
                        titleInst.Filter.Add(nLevel.Field, p_curRow.Str(nLevel.Field));
                    }
                    RptItemBase parent = title.Parent;
                    while (parent is RptMtxSubtotal)
                    {
                        RptTextInst parTotal = new RptTextInst((parent as RptMtxSubtotal).Item);
                        titleInst.TxtInsts.Add(parTotal);
                        parent = parent.Parent;
                    }
                    RptTextInst txtTotal = new RptTextInst(title.Item);
                    titleInst.TxtInsts.Add(txtTotal);
                    titleInst.RptData = p_data;
                    titleInst.RptData.Current = p_curRow.Index;
                    titleInst.Index = p_curRow.Index;
                    if (p_header is RptMtxColHeader)
                    {
                        titleInst.MtxRowsCol = title.Col;
                        p_matInst.AddColHeader(titleInst);
                    }
                    else
                    {
                        titleInst.MtxRowsRow = title.Row;
                        p_matInst.AddRowHeader(titleInst);
                    }
                }
            }
        }

        /// <summary>
        /// 获取参数计算列的数据
        /// </summary>
        /// <param name="p_dataTable"></param>
        /// <param name="p_columnNames"></param>
        /// <returns></returns>
        public List<Row> SelectDistinctDataRows(Table p_dataTable, List<string> p_columnNames)
        {
            List<Row> collection = new List<Row>();
            foreach (Row row in p_dataTable)
            {
                if (!IsDataRowExists(collection, row, p_columnNames))
                {
                    collection.Add(row);
                }
            }
            return collection;
        }

        /// <summary>
        /// 判断是否存在行
        /// </summary>
        /// <param name="p_dataRows"></param>
        /// <param name="p_dataRow"></param>
        /// <param name="p_columnNames"></param>
        /// <returns></returns>
        bool IsDataRowExists(List<Row> p_dataRows, Row p_dataRow, List<string> p_columnNames)
        {
            foreach (Row row in p_dataRows)
            {
                if (CompareTwoRows(row, p_dataRow, p_columnNames) == "")
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 比较两和f数据
        /// </summary>
        /// <param name="p_dataRow1"></param>
        /// <param name="p_dataRow2"></param>
        /// <param name="p_columnNames"></param>
        /// <returns></returns>
        string CompareTwoRows(Row p_dataRow1, Row p_dataRow2, List<string> p_columnNames)
        {
            if ((p_dataRow1 != null) && (p_dataRow2 != null))
            {
                foreach (string str in p_columnNames)
                {
                    if (p_dataRow1[str] != null && p_dataRow2[str] != null)
                    {
                        if (!p_dataRow1[str].Equals(p_dataRow2[str]))
                        {
                            return str;
                        }
                    }
                }
            }
            return "";
        }

        /// <summary>
        /// 根据行号、列号获取Cell
        /// </summary>
        /// <param name="p_row"></param>
        /// <param name="p_col"></param>
        /// <returns></returns>
        RptText GetCellByRowCol(int p_row, int p_col)
        {
            RptMtxRow rptRow = (from c in Rows
                                where c.Row == p_row
                                select c).FirstOrDefault();
            if (rptRow != null)
            {
                return (from c in rptRow.Cells
                        where c.Col == p_col
                        select c).FirstOrDefault();
            }
            return null;
        }

        /// <summary>
        /// 获取所属层下的 类型
        /// </summary>
        /// <param name="p_levels"></param>
        /// <param name="p_row"></param>
        /// <param name="p_col"></param>
        /// <returns></returns>
        MtxRangeType GetRangeTypeByLevel(List<RptMtxLevel> p_levels, int p_row, int p_col)
        {
            foreach (RptMtxLevel level in p_levels)
            {
                if ((level.Row <= p_row && p_row < level.Row + level.RowSpan)
                    && level.Col <= p_col && p_col < level.Col + level.ColSpan)
                {
                    return MtxRangeType.Level;
                }
                RptMtxSubtotal total = GetSubtotal(level.SubTotals.ToList(), p_row, p_col);
                if (total != null)
                {
                    return MtxRangeType.Subtotal;
                }
                RptMtxSubtitle title = GetSubtitle(level.SubTitles.ToList(), p_row, p_col);
                if (title != null)
                {
                    return MtxRangeType.Subtitle;
                }
            }
            return MtxRangeType.HeaderEmpty;
        }

        /// <summary>
        /// 根据行列获取小计
        /// </summary>
        /// <param name="p_totals"></param>
        /// <param name="p_row"></param>
        /// <param name="p_col"></param>
        /// <returns></returns>
        RptMtxSubtotal GetSubtotal(List<RptMtxSubtotal> p_totals, int p_row, int p_col)
        {
            foreach (RptMtxSubtotal total in p_totals)
            {
                if ((total.Row <= p_row && p_row < total.Row + total.RowSpan)
                    && total.Col <= p_col && p_col < total.Col + total.ColSpan)
                {
                    return total;
                }
                RptMtxSubtotal sub = GetSubtotal(total.SubTotals.ToList(), p_row, p_col);
                if (sub != null)
                {
                    return sub;
                }
            }
            return null;
        }

        /// <summary>
        /// 根据行列获取标题
        /// </summary>
        /// <param name="p_titles"></param>
        /// <param name="p_row"></param>
        /// <param name="p_col"></param>
        /// <returns></returns>
        RptMtxSubtitle GetSubtitle(List<RptMtxSubtitle> p_titles, int p_row, int p_col)
        {
            foreach (RptMtxSubtitle title in p_titles)
            {
                if ((title.Row <= p_row && p_row < title.Row + title.RowSpan)
                    && title.Col <= p_col && p_col < title.Col + title.ColSpan)
                {
                    return title;
                }
                RptMtxSubtitle sub = GetSubtitle(title.SubTitles.ToList(), p_row, p_col);
                if (sub != null)
                {
                    return sub;
                }
            }
            return null;
        }

        /// <summary>
        /// 递归获取小计最大数据值
        /// </summary>
        /// <param name="p_total"></param>
        /// <returns></returns>
        int TotalMax(RptMtxSubtotal p_total)
        {
            int maxIndex = GetValidInt(p_total, "subtotal");
            if (p_total.SubTotals.Count > 0)
            {
                maxIndex = p_total.SubTotals.Max(itm => GetValidInt(itm, "subtotal"));
                foreach (RptMtxSubtotal total in p_total.SubTotals)
                {
                    int max = TotalMax(total);
                    if (maxIndex < max)
                        maxIndex = max;
                }
            }
            return maxIndex;
        }

        /// <summary>
        /// 递归获取标题最大数据值
        /// </summary>
        /// <param name="p_title"></param>
        /// <returns></returns>
        int TitleMax(RptMtxSubtitle p_title)
        {
            int maxIndex = GetValidInt(p_title, "subtitle");
            if (p_title.SubTitles.Count > 0)
            {
                maxIndex = p_title.SubTitles.Max(itm => GetValidInt(itm, "subtitle"));
                foreach (RptMtxSubtitle title in p_title.SubTitles)
                {
                    int max = TitleMax(title);
                    if (maxIndex < max)
                        maxIndex = max;
                }
            }
            return maxIndex;
        }

        /// <summary>
        /// 获取最大ID的转换方法
        /// </summary>
        /// <param name="p_item"></param>
        /// <param name="p_preName"></param>
        /// <returns></returns>
        int GetValidInt(RptItemBase p_item, string p_preName)
        {
            if (p_item is RptMtxSubtotal)
            {
                RptMtxSubtotal total = p_item as RptMtxSubtotal;
                if (total.Item.Val.Length < p_preName.Length)
                    return 0;
                int id = 0;
                if (int.TryParse(total.Item.Val.Substring(p_preName.Length), out id))
                    return id;
                return 0;
            }
            else
            {
                RptMtxSubtitle title = p_item as RptMtxSubtitle;
                if (title.Item.Val.Length < p_preName.Length)
                    return 0;
                int id = 0;
                if (int.TryParse(title.Item.Val.Substring(p_preName.Length), out id))
                    return id;
                return 0;
            }
        }

        #endregion
    }


    /// <summary>
    /// 矩阵内部位置类型
    /// </summary>
    public enum MtxRangeType
    {
        /// <summary>
        /// 矩阵角
        /// </summary>
        Corner,

        /// <summary>
        /// 层次
        /// </summary>
        Level,

        /// <summary>
        /// 小计
        /// </summary>
        Subtotal,

        /// <summary>
        /// 标题
        /// </summary>
        Subtitle,

        /// <summary>
        /// 数据区
        /// </summary>
        Body,

        /// <summary>
        /// 内部空白区
        /// </summary>
        HeaderEmpty,

        /// <summary>
        /// 外部
        /// </summary>
        Outer
    }
}
