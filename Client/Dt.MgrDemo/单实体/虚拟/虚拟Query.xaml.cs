#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-02-17 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
#endregion

namespace Dt.MgrDemo.单实体
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
        public event EventHandler<QueryClause> Query
        {
            add { _fv.Query += value; }
            remove { _fv.Query -= value; }
        }

        protected override void OnInit(object p_params)
        {
            var row = new Row();
            row.AddCell<string>("主表名称");
            row.AddCell<string>("限长4");
            row.AddCell<string>("不重复");
            row.AddCell<string>("扩展1名称");
            row.AddCell<bool>("禁止选中");
            row.AddCell<bool>("禁止保存");
            row.AddCell<string>("扩展2名称");
            row.AddCell<bool>("禁止删除");
            row.AddCell<string>("值变事件");

            _fv.Data = row;
        }

        虚拟Win _win => (虚拟Win)OwnWin;
    }
}
