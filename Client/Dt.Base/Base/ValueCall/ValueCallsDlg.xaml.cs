#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-04-25 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Reflection;
#endregion

namespace Dt.Base.Report
{
    public partial class ValueCallsDlg : Dlg
    {
        static Table _methods;
        
        public ValueCallsDlg()
        {
            InitializeComponent();
            _lv.FilterCfg = new FilterCfg() {  IsRealtime = true };
            LoadMethods();
            if (!Kit.IsPhoneUI)
            {
                Width = 400;
                MinHeight = 300;
                MaxHeight = 600;
            }
        }

        public static async Task<string> ShowDlg(FrameworkElement p_target)
        {
            var dlg = new ValueCallsDlg();
            if (!Kit.IsPhoneUI)
            {
                dlg.PlacementTarget = p_target;
                dlg.WinPlacement = DlgPlacement.TargetOuterLeftTop;
            }
            if (await dlg.ShowAsync() && dlg._lv.SelectedItem is Row row)
                return row.Str("name");
            return null;
        }

        void LoadMethods()
        {
            if (_methods == null)
            {
                _methods = new Table { { "name" } };
                var ls = Kit.GetAllTypesByAttrType(typeof(ValueCallAttribute));
                if (ls != null)
                {
                    foreach (var tp in ls)
                    {
                        var arr = tp.GetMethods(BindingFlags.Public | BindingFlags.Static);
                        if (arr != null && arr.Length > 0)
                        {
                            foreach (var m in arr)
                            {
                                var pars = m.GetParameters();
                                if (pars == null || pars.Length == 0)
                                {
                                    var r = _methods.AddRow();
                                    r[0] = $"@{tp.Name}.{m.Name}()";
                                }
                                else if (pars.Length == 1 && pars[0].ParameterType == typeof(string))
                                {
                                    var r = _methods.AddRow();
                                    r[0] = $"@{tp.Name}.{m.Name}({pars[0].Name})";
                                }
                            }
                        }
                    }
                }
            }
            _lv.Data = _methods;
        }

        void OnCopy()
        {
            Close(true);
        }

        void OnItemDoubleClick(object obj)
        {
            Close(true);
        }
    }
}