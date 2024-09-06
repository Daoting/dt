#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-19 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.Lob
{
    using A = 供应商X;
    
    public partial class 供应商List : List
    {
        public 供应商List()
        {
            InitializeComponent();
            Menu = CreateMenu(del:false);

            var cm = CreateContextMenu();
            cm.Add(Mi.停用(click: OnStop));
            cm.Opening += OnMenuOpening;
            _lv.SetMenu(cm);

            _lv.ItemStyle = e =>
            {
                var x = e.Data as A;
                e.Foreground = x.撤档时间 != null ? Res.RedBrush : Res.BlackBrush;
            };
        }

        protected override async Task OnQuery()
        {
            if (_clause == null)
            {
                _lv.Data = await A.Query(null);
            }
            else
            {
                var par = await _clause.Build<A>();
                _lv.Data = await A.Query(par.Sql, par.Params);
            }
        }

        async void OnStop(Mi mi)
        {
            var x = mi.Data as A;
            x.撤档时间 = x.撤档时间 == null ? Kit.Now : null;
            if (await x.Save(false))
            {
                await Refresh();
            }
        }

        void OnMenuOpening(Menu menu, AsyncCancelArgs args)
        {
            var x = menu.TargetData as A;
            menu[2].ID = x.撤档时间 == null ? "停用" : "启用";
        }
    }
}