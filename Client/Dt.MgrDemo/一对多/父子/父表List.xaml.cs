#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-11-21 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.MgrDemo
{
    public partial class 父表List : Tab
    {
        #region 构造方法
        public 父表List()
        {
            InitializeComponent();
        }
        #endregion

        #region 公开
        public async void Update()
        {
            if (Clause == null)
            {
                _lv.Data = await 父表X.Query(null);
            }
            else
            {
                var par = await Clause.Build<父表X>();
                _lv.Data = await 父表X.Query(par.Sql, par.Params);
    }
        }
        #endregion

        #region 初始化 
        protected override void OnFirstLoaded()
        {
            Update();
        }
        #endregion

        #region 交互
        async void OnAdd(object sender, Mi e)
        {
            NaviToChild();
            await _win.ParentForm.Update(-1);
        }

        async void OnItemClick(ItemClickArgs e)
        {
            if (_lv.SelectionMode != SelectionMode.Multiple)
            {
                NaviToChild();
                if (e.IsChanged)
                    await _win.ParentForm.Update(e.Row.ID);
            }
        }

        void NaviToChild()
        {
            if (Kit.IsPhoneUI)
                NaviTo(new List<Tab> { _win.ParentForm, _win.大儿List, _win.小儿List });
        }

        async void OnDel(object sender, Mi e)
        {
            if (!await Kit.Confirm("确认要删除选择的数据吗？"))
            {
                Kit.Msg("已取消删除！");
                return;
            }
            
            var d = e.Data.To<父表X>();
            if (await d.Delete())
            {
                Update();
                if (d == _win.ParentForm.Data)
                    _win.ParentForm.Clear();
            }
        }
        #endregion

        #region 搜索
        /// <summary>
        /// 查询参数
        /// </summary>
        public QueryClause Clause { get; set; }

        void OnToSearch(object sender, Mi e)
        {
            if (_dlgQuery == null)
                CreateQueryDlg();
            _dlgQuery.Show();
        }

        void CreateQueryDlg()
        {
            var tabs = new List<Tab>();
            var fs = new FuzzySearch();
            fs.Fixed.Add("全部");
            fs.CookieID = _win.GetType().FullName;
            fs.Search += (e) =>
            {
                Clause = new QueryClause(e);
                Update();
                _dlgQuery.Close();
            };
            tabs.Add(fs);

            var qs = new 父表Query();
            qs.Query += (e) =>
            {
                Clause = e;
                Update();
                _dlgQuery.Close();
            };
            tabs.Add(qs);

            _dlgQuery = new Dlg
            {
                IsPinned = true,
                ShowVeil = true
            };
            _dlgQuery.LoadTabs(tabs);
        }

        Dlg _dlgQuery;
        #endregion

        #region 内部
        父表Win _win => OwnWin as 父表Win;
        #endregion
    }
}