#region 文件描述
/******************************************************************************
* 创建: $username$
* 摘要: 
* 日志: $time$ 创建
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

namespace $rootnamespace$
{
    public partial class $clsroot$List : Mv
    {
        public $clsroot$List()
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
        public object Clause { get; set; }

        public void OnSearch(object p_clause)
        {
            if (p_clause is string str && !string.IsNullOrEmpty(str))
            {
                Clause = p_clause;
                Title = "查询 - " + str;
                Query();
            }
            else if (p_clause is Row row)
            {
                Clause = p_clause;
                Title = "查询结果";
                Query();
            }
            
            NaviTo(this);
        }

        void OnToSearch(object sender, Mi e)
        {
            NaviTo(_win.Search);
        }

        async void Query()
        {
            if (Clause == null
                || (Clause is string str && str == "#全部"))
            {
                _lv.Data = await $entity$.Query();
            }
            else if (Clause is string txt)
            {
                //_lv.Data = await $entity$.Query("$entitytitle$-模糊查询", new { input = $"%{QueryStr}%" });
            }
            else if (Clause is Row row)
            {
                
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

        $clsroot$Win _win => ($clsroot$Win)_tab.OwnWin;
    }
}