#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-19 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.Lob
{
    public sealed partial class 供应商Query : Tab
    {
        public 供应商Query()
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
            row.Add<string>("执照号");
            row.Add<DateTime?>("执照效期");
            row.Add<string>("税务登记号");
            row.Add<string>("地址");
            row.Add<string>("电话");
            row.Add<string>("开户银行");
            row.Add<string>("帐号");
            row.Add<string>("联系人");
            row.Add<DateTime?>("建档时间");
            row.Add<DateTime?>("撤档时间");
            row.Add<string>("备注");

            _fv.Data = row;
        }
    }
}
