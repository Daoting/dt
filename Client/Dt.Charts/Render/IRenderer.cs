#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.ComponentModel;
using Windows.UI.Xaml;
#endregion

namespace Dt.Charts
{
    public interface IRenderer
    {
        event EventHandler Changed;

        event EventHandler Rendered;

        void AddSeries(IDataSeriesInfo seriesInfo);
        void Clear();

        ICoordConverter CoordConverter { get; set; }

        bool Dirty { get; set; }

        string Options { get; set; }

        UIElement Visual { get; set; }
    }
}

