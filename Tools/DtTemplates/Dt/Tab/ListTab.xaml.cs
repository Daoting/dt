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
            Query();
        }

        protected override void OnFirstLoaded()
        {
            Query();
        }

        void OnAdd(object sender, Mi e)
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

        public void OnSearch(QueryClause p_clause)
        {
            Clause = p_clause;
            Query();
            NaviTo(this);
        }

        void OnToSearch(object sender, Mi e)
        {
            //NaviTo(new List<Tab> { _win.Search, _win.Query });
        }

        void Query()
        {
            //if (Clause == null)
            //{
            //    _lv.Data = await MyEntityX.Query();
            //}
            //else
            //{
            //    _lv.Data = await MyEntityX.Query(Clause.Where, Clause.Params);
            //}
        }
        #endregion

        //$safeitemname$Win _win => ($safeitemname$Win)OwnWin;
    }
}