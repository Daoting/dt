#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-12-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
#endregion

namespace Dt.UIDemo
{
    public partial class HookList : Mv
    {
        public HookList()
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

        async void Query()
        {
            _lv.Data = await AtCm.GetAll<HookObj>();
        }

        HookWin _win => (HookWin)_tab.OwnWin;
    }
}