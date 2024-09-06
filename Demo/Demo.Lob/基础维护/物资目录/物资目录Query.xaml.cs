#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.Lob
{
    public sealed partial class 物资目录Query : Tab
    {
        public 物资目录Query()
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
            row.Add<string>("名称");
            row.Add<string>("规格");
            row.Add<string>("产地");
            row.Add<float?>("成本价_min");
            row.Add<float?>("成本价_max");
            row.Add<物资核算方式?>("核算方式");
            row.Add<int?>("摊销月数_min");
            row.Add<int?>("摊销月数_max");
            row.Add<DateTime?>("建档时间_min");
            row.Add<DateTime?>("建档时间_max");
            row.Add<DateTime?>("撤档时间_min");
            row.Add<DateTime?>("撤档时间_max");
            _fv.Data = row;
        }
    }
}
