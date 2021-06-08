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
    public sealed partial class MatrixSubtitleForm : UserControl
    {
        RptDesignInfo _info;
        RptMtxSubtitle _title;

        public MatrixSubtitleForm(RptDesignInfo p_info)
        {
            InitializeComponent();
            _info = p_info;
            _fvMtx.Info = _info;
        }

        internal void LoadItem(RptText p_item)
        {
            _title = p_item.Parent as RptMtxSubtitle;

            Row row = new Row();
            row.AddCell("span", _title.Data.Int("span"));
            row.Changed += OnChanged;
            _fv.Data = row;

            _fvMtx.LoadItem(_title.Level.Matrix);
        }

        void OnChanged(object sender, Cell e)
        {
            _info.ExecuteCmd(RptCmds.ChangeTitleSpanCmd, new SubTitleCmdArgs(_title.Parent, _title, e.GetVal<int>()));
        }

        void OnAddTitle(object sender, RoutedEventArgs e)
        {
            bool isOverlap = false;
            if (_title.SubTitles.Count > 0)
            {
                isOverlap = IsOverLap();
            }

            if (isOverlap)
            {
                Kit.Warn("增加行后与已有控件位置发生重叠，请调整控件位置后重试！");
                return;
            }

            _info.ExecuteCmd(RptCmds.AddSubTitle, new SubTitleCmdArgs(_title));
        }

        void OnDelTitle(object sender, RoutedEventArgs e)
        {
            _info.ExecuteCmd(RptCmds.DelSubTitle, new SubTitleCmdArgs(_title.Parent, _title));
        }

        bool IsOverLap()
        {
            if (_title.Level.Parent is RptMtxRowHeader)
            {
                return (_title.Level.Matrix).TestIncIntersect(0, 1);
            }
            else
            {
                return (_title.Level.Matrix).TestIncIntersect(1);
            }
        }
    }
}
