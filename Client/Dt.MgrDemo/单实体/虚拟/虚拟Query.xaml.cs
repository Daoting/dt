#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-12-11 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.MgrDemo
{
    public sealed partial class 虚拟Query : Tab
    {
        public 虚拟Query()
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

        #region 初始化 
        protected override void OnFirstLoaded()
        {
            var row = new Row();
            row.Add<string>("主表名称");
            row.Add<string>("限长4");
            row.Add<string>("不重复");
            row.Add<string>("扩展1名称");
            row.Add<bool?>("禁止选中");
            row.Add<bool?>("禁止保存");
            row.Add<string>("扩展2名称");
            row.Add<bool?>("禁止删除");
            row.Add<string>("值变事件");

            _fv.Data = row;
        }
        #endregion
    }
}
