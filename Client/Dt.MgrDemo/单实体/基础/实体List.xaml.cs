#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-18 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
#endregion

namespace Dt.MgrDemo.单实体
{
    public partial class 实体List : Mv
    {
        public 实体List()
        {
            InitializeComponent();
        }

        public void Update()
        {
            Query();
        }

        protected override void OnInit(object p_params)
        {
            Query();
        }

        void OnAdd(object sender, Mi e)
        {
            _win.Form.Update(-1);
            NaviTo(_win.Form);
        }

        void OnItemClick(object sender, ItemClickArgs e)
        {
            if (e.IsChanged)
                _win.Form.Update(e.Row.ID);
            NaviTo(_win.Form);
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
            NaviTo(new List<Mv> { _win.Search, _win.Query });
        }

        async void Query()
        {
            if (Clause == null)
            {
                _lv.Data = await 基础Obj.Query();
            }
            else
            {
                _lv.Data = await 基础Obj.Query(Clause.Where, Clause.Params);
            }
        }
        #endregion

        #region 视图
        private void OnListSelected(object sender, EventArgs e)
        {
            _lv?.ChangeView(Resources["ListView"], ViewMode.List);
        }

        private void OnTableSelected(object sender, EventArgs e)
        {
            _lv?.ChangeView(Resources["TableView"], ViewMode.Table);
        }

        private void OnTileSelected(object sender, EventArgs e)
        {
            _lv?.ChangeView(Resources["TileView"], ViewMode.Tile);
        }
        #endregion

        实体Win _win => (实体Win)_tab.OwnWin;
    }
}