#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
#endregion

namespace Dt.Mgr.Model
{
    public partial class UserParamsList : Tab
    {
        string _query;

        public UserParamsList()
        {
            InitializeComponent();
        }

        public void OnSearch(string p_txt)
        {
            if (!string.IsNullOrEmpty(p_txt))
            {
                _query = p_txt;
                Title = "参数列表 - " + p_txt;
                Update();
            }

            NaviTo(this);
        }

        public async void Update()
        {
            if (string.IsNullOrEmpty(_query) || _query == "#全部")
            {
                _lv.Data = await ParamsX.Query();
            }
            else if (_query == "#最近修改")
            {
                _lv.Data = await AtCm.Query<ParamsX>("参数-最近修改");
            }
            else
            {
                _lv.Data = await AtCm.Query<ParamsX>("参数-模糊查询", new { ID = $"%{_query}%" });
            }
        }

        protected override void OnInit(object p_params)
        {
            Update();
        }

        void OnNaviToSearch(object sender, Mi e)
        {
            NaviTo(_win.Search);
        }

        void OnAdd(object sender, Mi e)
        {
            _win.Form.Update(null);
            NaviTo(_win.Form);
        }

        void OnItemClick(object sender, ItemClickArgs e)
        {
            if (e.IsChanged)
                _win.Form.Update(e.Data.To<ParamsX>().ID);
            NaviTo(_win.Form);
        }

        UserParamsWin _win => (UserParamsWin)OwnWin;
    }
}