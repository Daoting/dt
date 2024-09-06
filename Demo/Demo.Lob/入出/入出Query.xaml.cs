#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.Lob
{
    public sealed partial class 入出Query : Tab
    {
        public 入出Query()
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
            row.Add<long>("部门id");
            row.Add<string>("部门名称");
            row.Add<DateTime?>("填制日期_min");
            row.Add<DateTime?>("填制日期_max");
            row.Add<DateTime?>("审核日期_min");
            row.Add<DateTime?>("审核日期_max");
            row.Add<单据状态>("状态");
            _fv.Data = row;
        }
    }
}
