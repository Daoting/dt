#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Mgr;
#endregion

namespace Dt.UIDemo
{
    [WfList("收文样例")]
    public partial class 收文List : Win
    {
        public 收文List()
        {
            InitializeComponent();

            _fv.Data = new 收文Obj(ID: 0);
            AtWf.AddMenu(_lv);
        }

        async void OnSearch(object sender, Mi e)
        {
            _lv.Data = await AtCm.GetAll<收文Obj>();
            NaviTo("结果");
        }

    }
}