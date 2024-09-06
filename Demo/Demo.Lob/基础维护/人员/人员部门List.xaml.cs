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
    using A = 部门人员X;
    
    public partial class 人员部门List : List
    {
        public 人员部门List()
        {
            InitializeComponent();
            Menu = Menu.New(Mi.添加(OnAddRelated, enable: false));

            var cm = new Menu
            {
                Mi.删除(OnDelRelated),
                new Mi("设为缺省", Icons.选择, OnSetDef),
            };
            cm.Opening += OnMenuOpening;
            _lv.SetMenu(cm);
        }

        protected override async Task OnQuery()
        {
            if (_parentID > 0)
            {
                _lv.Data = await A.Query($"select b.部门id,a.名称,b.缺省,b.人员id from 部门 a, 部门人员 b where a.ID = b.部门id and b.人员id={_parentID}");
            }
            else
            {
                _lv.Data = null;
            }
            Menu["添加"].IsEnabled = _parentID > 0;
        }

        async void OnAddRelated(Mi e)
        {
            var dlg = new 部门4人员();
            if (await dlg.Show(_parentID.Value, e))
            {
                var x = new A(部门id: dlg.SelectedRow.ID, 人员id: _parentID.Value);
                if (_lv.Table.Count == 0)
                    x.缺省 = true;

                if (await x.Save())
                    await Refresh();
            }
        }

        async void OnDelRelated(Mi e)
        {
            var x = e.Data as A;
            if (x == null)
                x = _lv.SelectedRow as A;

            if (x != null)
            {
                if (x.缺省 == true)
                {
                    Kit.Msg("缺省部门无法删除");
                    return;
                }

                if (!await Kit.Confirm("确认要删除所属部门吗？"))
                {
                    Kit.Msg("已取消删除！");
                    return;
                }

                if (await x.Delete())
                    await Refresh();
            }
        }

        void OnMenuOpening(Menu menu, AsyncCancelArgs args)
        {
            var x = menu.TargetData as A;
            if (x.缺省 == true)
            {
                args.Cancel = true;
            }
        }

        async void OnSetDef(Mi e)
        {
            var x = e.Data as A;
            var def = (from row in _lv.Table
                       let en = row as A
                       where en.缺省 == true
                       select en).FirstOrDefault();

            x.缺省 = true;
            var ls = new List<A> { x };
            if (def != null)
            {
                def.缺省 = null;
                ls.Add(def);
            }
            
            if (await ls.Save())
                await Refresh();
        }
    }
}