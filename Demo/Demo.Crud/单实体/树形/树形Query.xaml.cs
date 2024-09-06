#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-25 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.Crud
{
    public sealed partial class 树形Query : Tab
    {
        public 树形Query()
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
            row.Add<int>("序列_min");
            row.Add<int>("序列_max");
            row.Add<string>("名称");
            row.Add<string>("限长4");
            row.Add<string>("不重复");
            row.Add<bool>("禁止选中");
            row.Add<bool>("禁止保存");
            row.Add<bool>("禁止删除");
            row.Add<string>("值变事件");
            row.Add<bool>("发布插入事件");
            row.Add<bool>("发布删除事件");
            row.Add<DateTime>("创建时间_min");
            row.Add<DateTime>("创建时间_max");
            row.Add<DateTime>("修改时间_min");
            row.Add<DateTime>("修改时间_max");
            _fv.Data = row;
        }
    }
}
