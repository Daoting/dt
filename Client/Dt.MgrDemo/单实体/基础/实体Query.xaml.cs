#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-10-17 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.MgrDemo
{
    public sealed partial class 实体Query : Tab
    {
        public 实体Query()
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
            row.Add<int>("序列");
            row.Add<string>("限长4");
            row.Add<string>("不重复");
            row.Add<bool>("禁止选中");
            row.Add<bool>("禁止保存");
            row.Add<bool>("禁止删除");
            row.Add<string>("值变事件");
            row.Add<DateTime>("创建时间");
            row.Add<DateTime>("修改时间");

            _fv.Data = row;
        }
        #endregion
    }
}
