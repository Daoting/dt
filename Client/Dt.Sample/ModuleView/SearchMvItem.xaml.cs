#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-22 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Base.FormView;
using Dt.Core;
using System;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
#endregion

namespace Dt.Sample
{
    public partial class SearchMvItem : Mv
    {
        public SearchMvItem()
        {
            InitializeComponent();
        }

        async void OnForwardSearch(object sender, Mi e)
        {
            var res = await Forward<string>(_lzSm.Value);
            Kit.Msg("查询内容：" + res);
        }

        public void DoSearch(string e)
        {
            Kit.Msg("查询内容：" + e);
            NaviTo(this);
        }

        void OnDlgMv(object sender, RoutedEventArgs e)
        {
            var sm = new SearchMv
            {
                Title = "搜索3",
                Placeholder = "姓名或简拼",
                Fixed = { "全部", "最近修改" },
            };

            Dlg dlg = new Dlg { Title = "含搜索面板" };
            dlg.LoadMv(sm);
            if (!Kit.IsPhoneUI)
            {
                dlg.Width = 300;
                dlg.Height = 400;
            }
            dlg.Show();
        }

        void OnDlgMenu(object sender, RoutedEventArgs e)
        {
            Dlg dlg = new Dlg();
            dlg.LoadMv(new SearchMvItem { Title = "含搜索菜单" });
            if (!Kit.IsPhoneUI)
            {
                dlg.Width = 300;
                dlg.Height = 400;
            }
            dlg.Show();
        }

        Lazy<SearchMv> _lzSm = new Lazy<SearchMv>(() => new SearchMv
        {
            Title = "搜索2",
            Placeholder = "姓名或简拼",
            Fixed = { "全部", "最近修改", "abc" },
        });
    }
}