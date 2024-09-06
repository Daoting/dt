#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-14 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.Lob
{
    using A = 部门X;
    
    public partial class 部门List : List
    {
        public 部门List()
        {
            InitializeComponent();
            Menu = CreateMenu();
            
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
            if (_parentID == null)
            {
                _lv.Data = null;
            }
            else if (_parentID > 0)
            {
                _lv.Data = await A.View1.Query($"where 上级id={_parentID} order by 编码");
            }
            else
            {
                _lv.Data = await A.View1.Query("where 上级id is null order by 编码");
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