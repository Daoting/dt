#region 文件描述
/******************************************************************************
* 创建: $username$
* 摘要: 
* 日志: $time$ 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace $rootnamespace$
{
    public partial class $safeitemname$ : Tab
    {
        public $safeitemname$()
        {
            InitializeComponent();
        }

        public void Update()
        {
            //if (Clause == null)
            //{
            //    _lv.Data = await MyEntityX.Query(null);
            //}
            //else
            //{
            //    var par = await Clause.Build<MyEntityX>();
            //    _lv.Data = await MyEntityX.Query(par.Sql, par.Params);
            //}
        }

        protected override void OnFirstLoaded()
        {
            Update();
        }

        void OnAdd()
        {
            //_win.Form.Update(-1);
            //NaviTo(_win.Form);
        }

        void OnItemClick(object sender, ItemClickArgs e)
        {
            //if (e.IsChanged)
            //    _win.Form.Update(e.Row.ID);
            //NaviTo(_win.Form);
        }

        #region 搜索
        /// <summary>
        /// 获取设置查询内容
        /// </summary>
        public QueryClause Clause { get; set; }

        void OnQuery()
        {
            //if (_dlgQuery == null)
            //{
            //    var fs = new FuzzySearch();
            //    fs.Fixed.Add("全部");
            //    fs.CookieID = _win.GetType().FullName;
            //    fs.Search += (s, e) =>
            //    {
            //        Clause = new QueryClause(e);
            //        _ = Update();
            //        _dlgQuery.Close();
            //    };

            //    _dlgQuery = new Dlg
            //    {
            //        IsPinned = true,
            //        ShowVeil = true,
            //    };
            //    _dlgQuery.LoadTab(fs);
            //}
            //_dlgQuery.Show();
        }

        //Dlg _dlgQuery;
        #endregion

        //$safeitemname$Win _win => ($safeitemname$Win)OwnWin;
    }
}