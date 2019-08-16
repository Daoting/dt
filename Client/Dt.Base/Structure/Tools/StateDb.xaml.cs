#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System.Linq;
using Windows.UI.Xaml;
#endregion

namespace Dt.Base.Tools
{
    public partial class StateDb : Win
    {
        public StateDb()
        {
            InitializeComponent();
            _lvTbl.Data = AtLocal.QueryStateTblsName();
            _lvTbl.ItemClick += OnTblClick;
        }

        void OnTblClick(object sender, ItemClickArgs e)
        {
            if (e.IsChanged)
                _lvData.Data = AtLocal.Query($"select * from {e.Row.Str("name")}");
            NaviTo("数据");
        }

        async void OnDel(object sender, Mi e)
        {
            if (!await AtKit.Confirm($"确认要删除这{_lvData.SelectedCount}行吗？"))
                return;

            if (AtLocal.Delete(_lvData.SelectedItems.Cast<Row>(), _lvTbl.SelectedRow.Str("name")))
            {
                _lvData.DeleteSelection();
                AtKit.Msg("删除成功！");
            }
        }
    }
}