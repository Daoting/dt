#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2025-02-28 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using System.Text.Json;
#endregion

namespace Dt.Base.Views
{
    [ViewParamsEditor("通用一对多视图")]
    public sealed partial class OneToManyDesign : Dlg, IViewParamsEditor
    {
        OneToManyCfg _cfg;
        
        public OneToManyDesign()
        {
            InitializeComponent();
            IsPinned = true;
        }

        public async Task<string> ShowDlg(string p_params)
        {
            if (string.IsNullOrEmpty(p_params))
            {
                _cfg = new OneToManyCfg();
            }
            else
            {
                _cfg = JsonSerializer.Deserialize<OneToManyCfg>(p_params);
            }

            _lv.Data = _cfg.ChildCfgs;
            if (await ShowAsync())
            {
                return GetResult();
            }
            return null;
        }

        string GetResult()
        {
            return _cfg.Serialize();
        }

        void OnAddChild(object sender, RoutedEventArgs e)
        {

        }

        void OnEditParent(object sender, TappedRoutedEventArgs e)
        {

        }
    }
}