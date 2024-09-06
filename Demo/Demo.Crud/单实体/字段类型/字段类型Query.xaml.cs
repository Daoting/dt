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
    public sealed partial class 字段类型Query : Tab
    {
        public 字段类型Query()
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
            row.Add<string>("字符串");
            row.Add<int>("整型_min");
            row.Add<int>("整型_max");
            row.Add<int?>("可空整型_min");
            row.Add<int?>("可空整型_max");
            row.Add<bool>("布尔");
            row.Add<bool?>("可空布尔");
            row.Add<DateTime>("日期时间_min");
            row.Add<DateTime>("日期时间_max");
            row.Add<DateTime?>("可空时间_min");
            row.Add<DateTime?>("可空时间_max");
            row.Add<Gender>("枚举");
            row.Add<Gender?>("可空枚举");
            row.Add<float>("单精度_min");
            row.Add<float>("单精度_max");
            row.Add<float?>("可空单精度_min");
            row.Add<float?>("可空单精度_max");
            _fv.Data = row;
        }
    }
}
