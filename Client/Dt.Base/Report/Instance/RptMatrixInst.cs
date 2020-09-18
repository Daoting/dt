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
    internal class RptMatrixInst : RptItemInst
    {
        List<RptTextInst> _cellInsts;
        List<int> _rowUsed;
        List<int> _colUsed;
        RptRegion _regColHeader;
        RptRegion _regRowHeader;
        RptRegion _regCells;
        int _colIndex;
        int _rowIndex;
        RptMtxHeaderInst _colPart;
        RptMtxHeaderInst _rowPart;
        RptTextInst _cellPart;
        bool _isHasHorBreak;
        MtxOutPutPart _outPutPart;

        public RptMatrixInst(RptItemBase p_item)
            : base(p_item)
        {
            _cellInsts = new List<RptTextInst>();
            RowHeaderInsts = new List<RptMtxHeaderInst>();
            ColHeaderInsts = new List<RptMtxHeaderInst>();
            _rowUsed = new List<int>();
            _colUsed = new List<int>();
        }

        #region 属性
        /// <summary>
        /// 获取设置矩阵角
        /// </summary>
        public RptTextInst CornerInst { get; set; }

        /// <summary>
        /// 获取行头列表
        /// </summary>
        public List<RptMtxHeaderInst> RowHeaderInsts { get; }

        /// <summary>
        /// 获取列头列表
        /// </summary>
        public List<RptMtxHeaderInst> ColHeaderInsts { get; }

        /// <summary>
        /// 获取设置数据源
        /// </summary>
        public RptData RptData { get; set; }
        #endregion

        #region 外部方法
        /// <summary>
        /// 添加列
        /// </summary>
        /// <param name="p_header"></param>
        public void AddColHeader(RptMtxHeaderInst p_header)
        {
            p_header.Parent = this;
            ColHeaderInsts.Add(p_header);
        }

        /// <summary>
        /// 添加行
        /// </summary>
        /// <param name="p_header"></param>
        public void AddRowHeader(RptMtxHeaderInst p_header)
        {
            p_header.Parent = this;
            RowHeaderInsts.Add(p_header);
        }

        /// <summary>
        /// 添加单元格
        /// </summary>
        /// <param name="p_cell"></param>
        public void AddCell(RptTextInst p_cell)
        {
            p_cell.Parent = this;
            _cellInsts.Add(p_cell);
        }
        #endregion

        #region 重写
        protected override void DoOutput()
        {
            RptRootInst root = Inst;
            RptMatrix matrix = base._item as RptMatrix;
            RptRegion regClone = _region.Clone();

            _region.RowSpan = 0;
            _region.ColSpan = 0;
            //矩阵角
            if (!matrix.HideColHeader && !matrix.HideRowHeader)
            {
                CornerInst.Region = new RptRegion(_region.Row, _region.Col, matrix.ColHeader.RowSpan, matrix.RowHeader.ColSpan);
                CornerInst.Output();
                _region.RowSpan += matrix.Corner.RowSpan;
                _region.ColSpan += matrix.Corner.ColSpan;
            }

            root.HorPageBegin += OnHorPageBegin;
            root.VerPageBegin += OnVerPageBegin;
            _rowIndex = _colIndex = 0;
            //列头
            //可能出现水平分页、垂直分页相互调用，所以行、列Region都定义
            _regColHeader = regClone.Clone();
            _regRowHeader = regClone.Clone();
            if (!matrix.HideRowHeader)
            {
                _regColHeader.Col += matrix.RowHeader.ColSpan;
            }
            if (!matrix.HideColHeader)
            {
                _outPutPart = MtxOutPutPart.ColHeader;
                _regRowHeader.Row += matrix.ColHeader.RowSpan;
                foreach (RptMtxHeaderInst inst in ColHeaderInsts)
                {
                    _colPart = inst;
                    inst.Region = new RptRegion(_regColHeader.Row, _regColHeader.Col, inst.Item.RowSpan, inst.Item.ColSpan);
                    inst.Output();
                    _regColHeader.Col += 1;
                    _colIndex++;
                    _region.ColSpan++;
                }
            }

            //行头
            //可能出现水平分页、垂直分页相互调用，所以行、列Region都定义
            _colIndex = 0;
            _regColHeader = regClone.Clone();
            _regRowHeader = regClone.Clone();
            if (!matrix.HideColHeader)
            {
                _regRowHeader.Row += matrix.ColHeader.RowSpan;
            }
            if (!matrix.HideRowHeader)
            {
                _outPutPart = MtxOutPutPart.RowHeader;
                _regColHeader.Col += matrix.RowHeader.ColSpan;
                foreach (RptMtxHeaderInst inst in RowHeaderInsts)
                {
                    _rowPart = inst;
                    inst.Region = new RptRegion(_regRowHeader.Row, _regRowHeader.Col, inst.Item.RowSpan, inst.Item.ColSpan);
                    inst.Output();
                    _regRowHeader.Row += 1;
                    _rowIndex++;
                    _region.RowSpan++;
                }
            }

            //数据
            //还原行头、列头为开始状态（用于分页）
            _rowIndex = _colIndex = 0; 
            _regColHeader = regClone.Clone();
            _regRowHeader = regClone.Clone();
            _regCells = regClone.Clone();
            if (!matrix.HideRowHeader)
            {
                _regColHeader.Col += matrix.RowHeader.ColSpan;
                _regCells.Col += matrix.RowHeader.ColSpan;
            }
            if (!matrix.HideColHeader)
            {
                _regRowHeader.Row += matrix.ColHeader.RowSpan;
                _regCells.Row += matrix.ColHeader.RowSpan;
            }
            int colindex = 0;
            int col = _regCells.Col;
            _outPutPart = MtxOutPutPart.Cells;
            foreach (RptTextInst inst in _cellInsts)
            {
                _cellPart = inst;
                RptData.Current = GetCurrent(inst.Filter);
                //换行
                if (colindex == ColHeaderInsts.Count)
                {
                    _regCells.Row += 1;
                    //当前行被占用，下移
                    while (_rowUsed.Contains(_regCells.Row))
                    {
                        _regCells.Row++;
                    }
                    _regCells.Col = col;
                    colindex = 0;
                }
                if (colindex != 0)
                {
                    _regCells.Col++;
                    //当前列被占用，下移
                    while (_colUsed.Contains(_regCells.Col))
                    {
                        _regCells.Col++;
                    }
                }
                inst.Region = new RptRegion(_regCells.Row, _regCells.Col, inst.Item.RowSpan, inst.Item.ColSpan);
                inst.Output();
                colindex++;
            }
            root.HorPageBegin -= OnHorPageBegin;
            root.VerPageBegin -= OnVerPageBegin;
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 切换页面时在新页重复列头
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnVerPageBegin(object sender, RptPage e)
        {
            RptMatrix matrix = base._item as RptMatrix;
            if (!matrix.HideColHeader && matrix.RepeatColHeader)
            {
                RptRootInst root = sender as RptRootInst;
                //当不存在水平分页时，加入列头的定义
                if (!_isHasHorBreak)
                {
                    e.Rows.Size.InsertRange(0, root.Rows[0].Size.GetRange(matrix.Row, matrix.ColHeader.RowSpan));
                }

                int span = matrix.ColHeader.RowSpan;
                int start = e.Rows.Start;
                //外层列号变更

                if (_outPutPart == MtxOutPutPart.Cells)
                {
                    _regCells.Row = start + span;
                }
                else
                {
                    _regRowHeader.Row = start + span;
                }

                //再次递归调用时确保数据完整
                //行头Region
                RptRegion headerClone = _regColHeader.Clone();

                //数据源记录索引
                int index = RptData.Current;

                //外部输出的列头索引
                int colIndex = _colIndex;

                //外部正在输出的部分
                RptMtxHeaderInst curPart = _colPart;
                if (!matrix.HideColHeader && !matrix.HideRowHeader)
                {
                    RptTextInst newCorner = CornerInst.Clone();
                    newCorner.Region = new RptRegion(start, _regColHeader.Col - matrix.RowHeader.ColSpan, matrix.ColHeader.RowSpan, matrix.RowHeader.ColSpan);
                    newCorner.Output();
                }

                for (int i = _colIndex; i < ColHeaderInsts.Count; i++)
                {
                    RptMtxHeaderInst instClone = ColHeaderInsts[i].Clone();
                    _colPart = instClone;
                    instClone.Region = new RptRegion(start, _regColHeader.Col, instClone.Item.RowSpan, instClone.Item.ColSpan);
                    //输出过程中可能触发分页事件，提前计算新列位置，输出后还原
                    _regColHeader.Col += matrix.RowHeader.ColSpan;
                    instClone.Output();
                    _regColHeader.Col -= matrix.RowHeader.ColSpan;
                    _regColHeader.Col += 1;
                    //下次输出位置已经输出过，不再输出
                    if (_colUsed.Contains(_regColHeader.Col) && _rowUsed.Contains(start))
                        break;
                    _colIndex++;
                }
                _colPart = curPart;
                _colIndex = colIndex;
                RptData.Current = index;
                _regColHeader = headerClone.Clone();
                //后移 span 个格
                if (_outPutPart == MtxOutPutPart.Cells)
                {
                    _cellPart.Region.Row = start + span;
                }
                else
                {
                    _rowPart.TxtInsts[0].Region.Row = start + span;
                }
                    
                //将占用行加入到列表
                for (int i = 0; i < span; i++)
                {
                    if (!_rowUsed.Contains(start + i))
                        _rowUsed.Add(start + i);
                }
            }
        }

        /// <summary>
        /// 切换页面时在新页重复行头
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnHorPageBegin(object sender, RptPage e)
        {
            RptMatrix matrix = base._item as RptMatrix;
            if (!matrix.HideRowHeader && matrix.RepeatRowHeader)
            {
                _isHasHorBreak = true;
                RptRootInst root = sender as RptRootInst;
                //不存在定义，取第一页定义添加到当前页定义
                //添加的定义应该以矩阵开始行（列）为开始位置
                if (!e.IsColHasDefine())
                {
                    e.Cols.Size.InsertRange(0, root.Cols[0].Size.GetRange(matrix.Col, matrix.RowHeader.ColSpan));
                }
                else if (!e.IsRowHasDefine())
                {
                    e.Rows.Size.InsertRange(0, root.Rows[0].Size.GetRange(matrix.Row, matrix.ColHeader.RowSpan));
                }

                int span = matrix.RowHeader.ColSpan;
                int start = e.Cols.Start;
                //外层列号变更
                if (_outPutPart == MtxOutPutPart.Cells)
                {
                    _regCells.Col = start + span;
                }
                else
                {
                    _regColHeader.Col = start + span;
                }

                //再次递归调用时确保数据完整
                //行头Region
                RptRegion headerClone = _regRowHeader.Clone();

                //数据源记录索引
                int index = RptData.Current;

                //外部输出的行头索引
                int rowIndex = _rowIndex;

                //外部正在输出的部分
                RptMtxHeaderInst curPart = _rowPart;
                if (!matrix.HideColHeader && !matrix.HideRowHeader)
                {
                    RptTextInst newCorner = CornerInst.Clone();
                    newCorner.Region = new RptRegion(_regRowHeader.Row - matrix.ColHeader.RowSpan, start, matrix.ColHeader.RowSpan, matrix.RowHeader.ColSpan);
                    newCorner.Output();
                }

                //从已输出的索引处开始输出
                for (int i = rowIndex; i < RowHeaderInsts.Count; i++)
                {
                    RptMtxHeaderInst instClone = RowHeaderInsts[i].Clone();
                    _rowPart = instClone;
                    instClone.Region = new RptRegion(_regRowHeader.Row, start, instClone.Item.RowSpan, instClone.Item.ColSpan);
                    //输出过程中可能触发分页事件，提前计算新行位置，输出后还原
                    _regRowHeader.Row += matrix.ColHeader.RowSpan;
                    instClone.Output();
                    _regRowHeader.Row -= matrix.ColHeader.RowSpan;
                    _regRowHeader.Row += 1;
                    //下次输出位置已经输出过，不再输出
                    if (_rowUsed.Contains(_regRowHeader.Row) && _colUsed.Contains(start))
                        break;
                    _rowIndex++;
                }
                _rowPart = curPart;
                _rowIndex = rowIndex;
                RptData.Current = index;
                _regRowHeader = headerClone.Clone();
                //后移 span 个格
                if (_outPutPart == MtxOutPutPart.Cells)
                {
                    _cellPart.Region.Col = start + span;
                }
                else
                {
                    _colPart.TxtInsts[0].Region.Col = start + span;
                }
                //将占用列加入到列表
                for (int i = 0; i < span; i++)
                {
                    if (!_colUsed.Contains(start + i))
                        _colUsed.Add(start + i);
                }
            }
        }

        /// <summary>
        /// 计算行号
        /// </summary>
        /// <param name="p_filter"></param>
        /// <returns></returns>
        int GetCurrent(Dictionary<string, string> p_filter)
        {
            if (p_filter != null)
            {
                Table dtData = RptData.Data;
                foreach (var dr in dtData)
                {
                    bool isOk = false;
                    foreach (string key in p_filter.Keys)
                    {
                        isOk = dr.Str(key) == p_filter[key];
                        if (!isOk)
                            break;
                    }
                    if (isOk)
                        return dr.Index;
                }
            }
            return -1;
        }

        /// <summary>
        /// 矩阵正在输出的部分
        /// </summary>
        enum MtxOutPutPart
        {
            /// <summary>
            /// 行头
            /// </summary>
            RowHeader,

            /// <summary>
            /// 列头
            /// </summary>
            ColHeader,

            /// <summary>
            /// 数据
            /// </summary>
            Cells
        }
        #endregion
    }
}
