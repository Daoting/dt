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
    public sealed partial class TextForm : UserControl
    {
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
                SaveExpression(_dlgData.GetExpression());
            }
        }

        async void OnAddParamVal(object sender, RoutedEventArgs e)
        {
            if (_dlgParam == null)
                _dlgParam = new ParamSelectionDlg();
            if (await _dlgParam.Show((Button)sender, _item))
            {
                SaveExpression(_dlgParam.GetExpression());
            }
        }

        async void OnAddGlobalVal(object sender, RoutedEventArgs e)
        {
            if (_dlgGlobal == null)
                _dlgGlobal = new GlobalParamDlg();
            if (await _dlgGlobal.Show((Button)sender))
            {
                SaveExpression(_dlgGlobal.GetExpression());
            }
        }

        void SaveExpression(string exp)
        {
            if (!string.IsNullOrEmpty(exp))
            {
                string val = _fv.Row.Str("val");
                if (val == "" || val == RptText.ScriptValue)
                    _fv.Row["val"] = ":" + exp;
                else
                    _fv.Row["val"] = $"{val}\r\n+ {exp}";
            }
        }
    }
}
