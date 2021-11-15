#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-09-18 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base.Report
{
    public sealed partial class MatrixSubtotalForm : UserControl
    {
        RptDesignInfo _info;
        RptMtxSubtotal _total;

        public MatrixSubtotalForm(RptDesignInfo p_info)
        {
            InitializeComponent();
            _info = p_info;
            _fvMtx.Info = _info;
        }

        internal void LoadItem(RptText p_item)
        {
            _total = p_item.Parent as RptMtxSubtotal;

            Row row = new Row();
            row.AddCell("span", _total.Data.Int("span"));
            row.AddCell("beforelevel", _total.Data.Bool("beforelevel"));
            row.Changed += OnChanged;
            _fv.Data = row;

            _fvMtx.LoadItem(_total.Level.Matrix);
        }

        void OnChanged(object sender, Cell e)
        {
            if (e.ID == "beforelevel")
            {
                _total.Data["beforelevel"] = e.Val;
                _info.ExecuteCmd(RptCmds.ChangeTotalLocCmd, new SubTotalCmdArgs(_total.Parent, _total));
            }
            else if (e.ID == "span")
            {
                _info.ExecuteCmd(RptCmds.ChangeTotalSpanCmd, new SubTotalCmdArgs(_total.Parent, _total, e.GetVal<int>()));
            }
        }

        void OnAddTotal(object sender, RoutedEventArgs e)
        {
            bool isOverlap = false;
            if (_total.SubTotals.Count > 0)
            {
                isOverlap = IsOverLap();
            }

            if (isOverlap)
            {
                Kit.Warn("增加行后与已有控件位置发生重叠，请调整控件位置后重试！");
                return;
            }

            _info.ExecuteCmd(RptCmds.AddSubTotal, new SubTotalCmdArgs(_total));
        }

        void OnDelTotal(object sender, RoutedEventArgs e)
        {
            _info.ExecuteCmd(RptCmds.DelSubTotal, new SubTotalCmdArgs(_total.Parent, _total));
        }

        bool IsOverLap()
        {
            if (_total.Level.Parent is RptMtxRowHeader)
            {
                return (_total.Level.Matrix).TestIncIntersect(0, 1);
            }
            else
            {
                return (_total.Level.Matrix).TestIncIntersect(1);
            }
        }
    }
}
