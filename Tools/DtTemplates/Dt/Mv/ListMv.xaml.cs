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
    public partial class $safeitemname$ : Mv
    {
        string _query;

        public $safeitemname$()
        {
            InitializeComponent();
        }

        public void Update()
        {
            //if (string.IsNullOrEmpty(_query) || _query == "#全部")
            //{
            //    _lv.Data = await AtSvc.Query<>("$title$-所有");
            //}
            //else
            //{
            //    _lv.Data = await AtSvc.Query<>("$title$-模糊查询", new { name = $"%{_query}%" });
            //}
        }

        protected override void OnInit(object p_params)
        {
            Update();
        }

        async void OnToSearch(object sender, Mi e)
        {
            var txt = await Forward<string>(_lzSm.Value);
            if (!string.IsNullOrEmpty(txt))
            {
                _query = txt;
                Title = "$title$ - " + txt;
                Update();
            }
        }

        Lazy<SearchMv> _lzSm = new Lazy<SearchMv>(() => new SearchMv
        {
            Placeholder = "名称",
            Fixed = { "全部", },
        });

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

        //$safeitemname$Win _win => ($safeitemname$Win)_tab.OwnWin;
    }
}