#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Linq;
using System.Reflection;
using Windows.System;
using Microsoft.UI.Xaml.Input;
using Dt.Base.FormView;
#endregion

namespace Dt.Base.Report
{
    public sealed partial class RptIconDlg : Dlg
    {
        public RptIconDlg()
        {
            InitializeComponent();
            _lv.Data = IconItem.GetAllIcons();
            _lv.Filter = OnFilter;
            
            if (!Kit.IsPhoneUI)
            {
                Width = 300;
                Height = 500;
            }
        }

        public Icons SelectIcon => ((IconItem)_lv.SelectedItem).Icon;
        
        void OnSearch(string e)
        {
            _lv.Refresh();
        }

        bool OnFilter(object p_obj)
        {
            return ((IconItem)p_obj).IsMatched(_sb.Text);
        }
    }
}