#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-09 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dt.Base;
using Dt.MgrDemo.单实体;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
#endregion

namespace Dt.MgrDemo
{
    public partial class DemoMain : Tab
    {
        public DemoMain()
        {
            InitializeComponent();
            LoadData();
        }

        void LoadData()
        {
            Nl<GroupData<DemoItem>> ds = new Nl<GroupData<DemoItem>>();

            var group = new GroupData<DemoItem>
            {
                new DemoItem("实体基础", typeof(AccessDemo), "客户端实体增删改查，领域事件的定义、发布、处理，虚拟实体及父子实体的增删改查，实体缓存、领域服务等"),
                new DemoItem("Sqlite实体", typeof(SqliteAccessDemo), "Sqlite实体除了无缓存和无序列外，其余功能都包括"),
                new DemoItem("服务端实体", typeof(SvcAccessDemo), "服务端实体增删改查，领域事件的定义、发布、处理，虚拟实体及父子实体的增删改查，实体缓存等"),
            };
            group.Title = "基础";
            ds.Add(group);

            group = new GroupData<DemoItem>
            {
                new DemoItem("单实体框架", typeof(实体Win), "单表的增删改查操作框架"),
                new DemoItem("虚拟实体框架", typeof(虚拟Win), "因字段过多将单表拆分成多表时适用于虚拟实体，本质还是单实体框架"),
                new DemoItem("一对多框架", typeof(SvcAccessDemo), "服务端实体增删改查，领域事件的定义、发布、处理，虚拟实体及父子实体的增删改查，实体缓存等"),
            };
            group.Title = "框架";
            ds.Add(group);
            _lv.Data= ds;
        }

        void OnItemClick(object sender, ItemClickArgs e)
        {
            Kit.RunAsync(() =>
            {
                var item = e.Data.To<DemoItem>();
                Kit.OpenWin(item.Type, item.Title);
            });
        }
    }
}