#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Threading.Tasks;
#endregion

namespace Dt.App.Model
{
    public partial class MenuObj
    {
        Task OnSaving()
        {
            // 调序时无name列
            if (Contains("Name"))
                Throw.IfNullOrEmpty(Name, "菜单名称不可为空！");
            return Task.CompletedTask;
        }

        async Task OnDeleting()
        {
            if (IsGroup)
            {
                int count = await AtCm.GetScalar<int>("菜单-是否有子菜单", new { parentid = ID });
                Throw.If(count > 0, "含子菜单无法删除！");
            }
        }
    }

    #region 自动生成
    [Tbl("cm_menu")]
    public partial class MenuObj : Entity
    {
        #region 构造方法
        MenuObj() { }

        public MenuObj(
            long ID,
            long? ParentID = default,
            string Name = default,
            bool IsGroup = default,
            string ViewName = default,
            string Params = default,
            string Icon = default,
            string SvcName = default,
            string Note = default,
            int Dispidx = default,
            bool IsLocked = false,
            DateTime Ctime = default,
            DateTime Mtime = default)
        {
            AddCell<long>("ID", ID);
            AddCell<long?>("ParentID", ParentID);
            AddCell<string>("Name", Name);
            AddCell<bool>("IsGroup", IsGroup);
            AddCell<string>("ViewName", ViewName);
            AddCell<string>("Params", Params);
            AddCell<string>("Icon", Icon);
            AddCell<string>("SvcName", SvcName);
            AddCell<string>("Note", Note);
            AddCell<int>("Dispidx", Dispidx);
            AddCell<bool>("IsLocked", IsLocked);
            AddCell<DateTime>("Ctime", Ctime);
            AddCell<DateTime>("Mtime", Mtime);
            IsAdded = true;
            AttachHook();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 父菜单标识
        /// </summary>
        public long? ParentID
        {
            get { return (long?)this["ParentID"]; }
            set { this["ParentID"] = value; }
        }

        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
        }

        /// <summary>
        /// 分组或实例。0表实例，1表分组
        /// </summary>
        public bool IsGroup
        {
            get { return (bool)this["IsGroup"]; }
            set { this["IsGroup"] = value; }
        }

        /// <summary>
        /// 视图名称
        /// </summary>
        public string ViewName
        {
            get { return (string)this["ViewName"]; }
            set { this["ViewName"] = value; }
        }

        /// <summary>
        /// 传递给菜单程序的参数
        /// </summary>
        public string Params
        {
            get { return (string)this["Params"]; }
            set { this["Params"] = value; }
        }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon
        {
            get { return (string)this["Icon"]; }
            set { this["Icon"] = value; }
        }

        /// <summary>
        /// 提供提示信息的服务名称，空表示无提示信息
        /// </summary>
        public string SvcName
        {
            get { return (string)this["SvcName"]; }
            set { this["SvcName"] = value; }
        }

        /// <summary>
        /// 备注
        /// </summary>
        public string Note
        {
            get { return (string)this["Note"]; }
            set { this["Note"] = value; }
        }

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int Dispidx
        {
            get { return (int)this["Dispidx"]; }
            set { this["Dispidx"] = value; }
        }

        /// <summary>
        /// 定义了菜单是否被锁定。0表未锁定，1表锁定不可用
        /// </summary>
        public bool IsLocked
        {
            get { return (bool)this["IsLocked"]; }
            set { this["IsLocked"] = value; }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Ctime
        {
            get { return (DateTime)this["Ctime"]; }
            set { this["Ctime"] = value; }
        }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime Mtime
        {
            get { return (DateTime)this["Mtime"]; }
            set { this["Mtime"] = value; }
        }
        #endregion
    }
    #endregion
}
