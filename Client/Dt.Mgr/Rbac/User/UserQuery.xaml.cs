#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-03-06 创建
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
        public event EventHandler<QueryClause> Query
        {
            add { _fv.Query += value; }
            remove { _fv.Query -= value; }
        }

        #region 初始化 
        protected override void OnFirstLoaded()
        {
            var row = new Row();
            row.Add<string>("Phone");
            row.Add<string>("Name");
            row.Add<string>("Pwd");
            row.Add<Gender>("Sex");
            row.Add<string>("Photo");
            row.Add<bool>("Expired");
            row.Add<DateTime>("Ctime");
            row.Add<DateTime>("Mtime");

            _fv.Data = row;
        }
        #endregion
    }
}
