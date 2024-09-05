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
using System.ComponentModel;
using System.Threading.Tasks;
using Dt.Base;
using Dt.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Base.Report
{
    public sealed partial class TextForm : UserControl
    {
        const string _prefix = ":";
        RptText _item;
        DataSourceDlg _dlgData;
        GlobalParamDlg _dlgGlobal;
        ParamSelectionDlg _dlgParam;

        public TextForm()
        {
            InitializeComponent();
        }

        internal void LoadItem(RptText p_item)
        {
            if (_item != p_item)
            {
                _item = p_item;
                _fv.Data = _item.Data;
                var cell = _item.Data.Cells["val"];
                cell.Changed -= OnValChanged;
                cell.Changed += OnValChanged;
            }
        }

        void OnScriptVal(object sender, RoutedEventArgs e)
        {
            _item.Val = RptText.ScriptValue;
        }

        async void OnAddDataVal(object sender, RoutedEventArgs e)
        {
            if (_dlgData == null)
                _dlgData = new DataSourceDlg();
            if (await _dlgData.Show((Button)sender, _item))
            {
                SaveExpression(_prefix + _dlgData.GetExpression());
            }
        }

        async void OnAddParamVal(object sender, RoutedEventArgs e)
        {
            if (_dlgParam == null)
                _dlgParam = new ParamSelectionDlg();
            if (await _dlgParam.Show((Button)sender, _item))
            {
                SaveExpression(_prefix + _dlgParam.GetExpression());
            }
        }

        async void OnAddGlobalVal(object sender, RoutedEventArgs e)
        {
            if (_dlgGlobal == null)
                _dlgGlobal = new GlobalParamDlg();
            if (await _dlgGlobal.Show((Button)sender))
            {
                SaveExpression(_prefix + _dlgGlobal.GetExpression());
            }
        }

        void SaveExpression(string exp)
        {
            if (!string.IsNullOrEmpty(exp))
            {
                string val = _fv.Row.Str("val");
                if (val == "" || val == RptText.ScriptValue)
                    _fv.Row["val"] = exp;
                else
                    _fv.Row["val"] = $"{val}\r\n|| {exp}";
            }
        }

        void OnValChanged(Cell cell)
        {
            _item.ParseVal();
        }

        async void OnIconVal(object sender, RoutedEventArgs e)
        {
            RptIconDlg dlg = new RptIconDlg();
            if (await dlg.ShowAsync())
            {
                var icon = dlg.SelectIcon;
                _fv.Row["fontfamily"] = "ms-appx:///icon.ttf#DtIcon";
                _fv.Row["val"] = Res.GetIconChar(icon);
            }
        }

        async void OnCallVal(object sender, RoutedEventArgs e)
        {
            var val = await ValueCallsDlg.ShowDlg((Button)sender);
            if (!string.IsNullOrEmpty(val))
                SaveExpression(val);
        }

        void OnClearVal(object sender, RoutedEventArgs e)
        {
            _item.Val = "";
        }
    }
}
