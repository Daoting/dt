#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-18 创建
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

        protected override void OnInit(object p_params)
        {
            var row = new Row();
            row.AddCell<int>("序列");
            row.AddCell<string>("限长4");
            row.AddCell<string>("不重复");
            row.AddCell<bool>("禁止选中");
            row.AddCell<int>("校验后台列");
            row.AddCell<bool>("禁止保存");
            row.AddCell<bool>("禁止删除");
            row.AddCell<string>("值变事件");
            row.AddCell<DateTime>("创建时间");
            row.AddCell<DateTime>("修改时间");

            _fv.Data = row;
        }

        实体Win _win => (实体Win)OwnWin;
    }
}
