#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-02-04 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Rbac
{
    public sealed partial class UserQuery : Tab
    {
        public UserQuery()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 查询事件
        /// </summary>
        public event Action<QueryClause> Query
        {
            add { _fv.Query += value; }
            remove { _fv.Query -= value; }
        }

        protected override void OnFirstLoaded()
        {
            var row = new Row();
            row.Add<string>("acc");
            row.Add<string>("phone");
            row.Add<string>("name");
            row.Add<bool>("expired");
            row.Add<DateTime>("ctime");
            row.Add<DateTime>("mtime");

            _fv.Data = row;
        }
    }
}
