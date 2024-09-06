#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.Lob
{
    using A = 物资主单X;

    public partial class 入出List : WfList
    {
        public 入出List()
        {
            InitializeComponent();
            LoadMenu();
        }

        public override string PrcName => "入出入出";

        protected override async Task OnQuery()
        {
            if (_clause == null)
            {
                _lv.Data = await A.View1.Query("where 入出类别id=1");
            }
            else
            {
                var par = await _clause.Build<A>(false);
                string sql = par.Sql;
                if (string.IsNullOrEmpty(sql))
                {
                    sql = "where 入出类别id=1";
                }
                else
                {
                    sql += " and 入出类别id=1";
                }
                _lv.Data = await A.View1.Query(sql, par.Params);
            }
        }

        async void LoadMenu()
        {
            Menu menu = new Menu();
            if (await IsStartable())
            {
                menu.Items.Add(new Mi("填写", Icons.增加, call: OnAdd));
            }
            Menu = menu;

            menu = CreateContextMenu();
            if (await Gs.物资管理.入出.冲销)
                menu.Items.Add(new Mi("冲销", Icons.减少, OnWritedowns));
            menu.Opening += OnMenuOpening;
            _lv.SetMenu(menu);

        }

        void OnMenuOpening(Menu menu, AsyncCancelArgs args)
        {
            if (menu["冲销"] is Mi mi
                && menu.TargetData is A x)
            {
                if (x.状态 == 单据状态.已审核 || x.状态 == 单据状态.被冲销)
                {
                    mi.Visibility = Visibility.Visible;
                }
                else
                {
                    mi.Visibility = Visibility.Collapsed;
                }
            }
        }

        void OnWritedowns(Mi mi)
        {

        }
    }
    

}