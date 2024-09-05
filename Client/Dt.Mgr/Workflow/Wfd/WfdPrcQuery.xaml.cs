#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-24 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Workflow
{
    public sealed partial class WfdPrcQuery : Tab
    {
        public WfdPrcQuery()
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
            row.Add<string>("name");
            row.Add<bool>("is_locked");
            row.Add<bool>("singleton");
            row.Add<DateTime>("ctime_min");
            row.Add<DateTime>("ctime_max");
            row.Add<DateTime>("mtime_min");
            row.Add<DateTime>("mtime_max");
            _fv.Data = row;
        }
    }
}
