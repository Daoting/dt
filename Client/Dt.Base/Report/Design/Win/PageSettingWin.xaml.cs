#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-10-10 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using Dt.Core;
using System;
using Windows.Foundation;
using Windows.Graphics.Printing;
#endregion

namespace Dt.Base.Report
{
    public sealed partial class PageSettingWin : Win
    {
        RptDesignInfo _info;

        public PageSettingWin(RptDesignInfo p_info)
        {
            InitializeComponent();
            _info = p_info;
            _info.TemplateChanged += (s, e) => LoadSetting();
            _info.Saved += OnSaved;
            LoadSetting();
            InitPaperName();
        }

        void OnSaved(object sender, EventArgs e)
        {
            _info.Root.PageSetting.Data.AcceptChanges();
        }

        void LoadSetting()
        {
            var row = _fv.Row;
            if (row != null)
                row.Changed -= OnValueChanged;
            _fv.Data = _info.Root.PageSetting.Data;
            _info.Root.PageSetting.Data.Changed += OnValueChanged;
        }

        void OnValueChanged(object sender, Core.Cell e)
        {
            if (_info.Root.PageSetting.ValidHeight - _info.Root.Header.ActualHeight - _info.Root.Footer.ActualHeight <= 0)
            {
                AtKit.Warn("报表的可用页面高度不足，请确认。");
            }
            else if (_info.Root.PageSetting.ValidWidth <= 0)
            {
                AtKit.Error("报表的可用页面宽度不足，请确认。");
            }
            else
            {
                _info.OnPageSettingChanged();
            }
        }

        void InitPaperName()
        {
            Nl<string> ls = new Nl<string>();
            foreach (var item in PaperSize.Dict.Keys)
            {
                ls.Add(item.ToString());
            }
            ((CList)_fv["papername"]).Data = ls;
        }

        void OnPaperChanged(object sender, object e)
        {
            Size size = PaperSize.Dict[(PrintMediaSize)Enum.Parse(typeof(PrintMediaSize), (string)e)];
            if (!size.IsEmpty)
            {
                _info.Root.PageSetting.Height = size.Height;
                _info.Root.PageSetting.Width = size.Width;
            }
        }
    }
}
