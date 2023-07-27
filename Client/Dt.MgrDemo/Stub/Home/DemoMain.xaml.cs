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
using Dt.MgrDemo.一对多;
using Dt.MgrDemo.单实体;
using Dt.MgrDemo.多对多;
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
                new DemoItem("单实体", typeof(实体Win), "单表的增删改查框架"),
                new DemoItem("虚拟实体", typeof(虚拟Win), "因字段过多将单表拆分成多表时适用于虚拟实体，本质还是单实体框架"),
                new DemoItem("视图实体", typeof(视图Win), "单个视图的增删改查"),
            };
            group.Title = "单实体框架";
            ds.Add(group);

            group = new GroupData<DemoItem>
            {
                new DemoItem("父子实体", typeof(父表Win), ""),
                //new DemoItem("树", typeof(父表Win), "单表树形结构的增删改查框架"),
            };
            group.Title = "一对多框架";
            ds.Add(group);

            group = new GroupData<DemoItem>
            {
                new DemoItem("角色", typeof(角色Win), "主实体对多个关联实体"),
                new DemoItem("用户", typeof(用户Win), "用户对角色"),
                new DemoItem("权限", typeof(权限Win), "权限对角色"),
            };
            group.Title = "多对多框架";
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

    public class DemoItem
    {
        public DemoItem(string p_title, Type p_type, string p_desc)
        {
            Title = p_title;
            Type = p_type;
            Desc = p_desc;
        }

        /// <summary>
        /// 获取设置标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 获取设置描述信息
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 获取设置窗口类型
        /// </summary>
        public Type Type { get; set; }
    }
}