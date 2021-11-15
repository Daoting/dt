#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-09-18 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dt.Base;
using Dt.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base.Report
{
    public sealed partial class MatrixLevelForm : UserControl
    {
        RptDesignInfo _info;
        RptMtxLevel _level;

        public MatrixLevelForm(RptDesignInfo p_info)
        {
            InitializeComponent();
            _info = p_info;
            _fvMtx.Info = _info;
        }

        internal void LoadItem(RptText p_item)
        {
            _level = p_item.Parent as RptMtxLevel;
            _fv.Data = _level.Data;
            _fvMtx.LoadItem(_level.Parent.Parent as RptMatrix);
        }

        void OnAddLevel(object sender, RoutedEventArgs e)
        {
            bool isOverlap = false;
            if (_level.SubTitles.Count > 0)
                isOverlap = IsOverLap();

            if (isOverlap)
                Kit.Warn("增加行后与已有控件位置发生重叠，请调整控件位置后重试！");
            else
                _info.ExecuteCmd(RptCmds.AddSubLevel, new SubLevelCmdArgs(_level.Parent as RptMtxHeader));
        }

        void OnAddTotal(object sender, RoutedEventArgs e)
        {
            if (IsOverLap())
                Kit.Warn("增加行后与已有控件位置发生重叠，请调整控件位置后重试！");
            else
                _info.ExecuteCmd(RptCmds.AddSubTotal, new SubTotalCmdArgs(_level));
        }

        void OnAddTitle(object sender, RoutedEventArgs e)
        {
            bool isOverlap = false;
            if (_level.SubTitles.Count > 0)
                isOverlap = IsOverLap();

            if (isOverlap)
                Kit.Warn("增加行后与已有控件位置发生重叠，请调整控件位置后重试！");
            else
                _info.ExecuteCmd(RptCmds.AddSubTitle, new SubTitleCmdArgs(_level));
        }

        void OnDelLevel(object sender, RoutedEventArgs e)
        {
            if ((_level.Parent as RptMtxHeader).Levels.IndexOf(_level) == 0)
                Kit.Warn("根层次不可删除！");
            else
                _info.ExecuteCmd(RptCmds.DelSubLevel, new SubLevelCmdArgs(_level.Parent as RptMtxHeader, _level));
        }

        /// <summary>
        /// 扩展位置是否与其他控件冲突
        /// </summary>
        /// <returns></returns>
        bool IsOverLap()
        {
            if (_level.Parent is RptMtxRowHeader)
            {
                return (_level.Matrix).TestIncIntersect(0, 1);
            }
            else
            {
                return (_level.Matrix).TestIncIntersect(1);
            }
        }

        void OnLoadField(object sender, AsyncEventArgs e)
        {
            ((CList)_fv["field"]).Data = _info.Root.Data.GetColsData(_level.Matrix.Tbl);
        }
    }
}
