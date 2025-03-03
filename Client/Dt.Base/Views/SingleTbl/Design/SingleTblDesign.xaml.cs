#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2025-02-28 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Base.Views
{
    [ViewParamsEditor("通用单表")]
    public sealed partial class SingleTblDesign : Dlg, IViewParamsEditor
    {
        public SingleTblDesign()
        {
            InitializeComponent();
        }

        public async Task<string> ShowDlg(string p_params)
        {
            if (!Kit.IsPhoneUI)
            {
                Width = 600;
                Height = 600;
            }

            if (await ShowAsync())
            {
                return GetResult();
            }
            return null;
        }

        string GetResult()
        {
            return "abc";
            
        }
    }
}