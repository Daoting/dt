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
            row.AddCell<string>("Phone");
            row.AddCell<string>("Name");
            row.AddCell<string>("Pwd");
            row.AddCell<Gender>("Sex");
            row.AddCell<string>("Photo");
            row.AddCell<bool>("Expired");
            row.AddCell<DateTime>("Ctime");
            row.AddCell<DateTime>("Mtime");

            _fv.Data = row;
        }
        #endregion
    }
}
