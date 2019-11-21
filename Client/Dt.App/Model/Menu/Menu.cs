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
#endregion

namespace Dt.App.Model
{
    [Tbl("cm_menu", "cm")]
    public partial class Menu : Entity
    {
        public Menu()
        { }

        public Menu(
            long ID,
            long? ParentID = default,
            string Name = default,
            bool IsGroup = default,
            string ViewName = default,
            string Params = default,
            string Icon = default,
            string SrvName = default,
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
            AddCell<string>("SrvName", SrvName);
            AddCell<string>("Note", Note);
            AddCell<int>("Dispidx", Dispidx);
            AddCell<bool>("IsLocked", IsLocked);
            AddCell<DateTime>("Ctime", Ctime);
            AddCell<DateTime>("Mtime", Mtime);
            IsAdded = true;
        }

        /// <summary>
        /// 父菜单标识
        /// </summary>
        public long? ParentID
        {
            get { return (long?)_cells["ParentID"].Val; }
            private set { _cells["ParentID"].Val = value; }
        }

        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name
        {
            get { return (string)_cells["Name"].Val; }
            private set { _cells["Name"].Val = value; }
        }

        /// <summary>
        /// 分组或实例。0表实例，1表分组
        /// </summary>
        public bool IsGroup
        {
            get { return (bool)_cells["IsGroup"].Val; }
            private set { _cells["IsGroup"].Val = value; }
        }

        /// <summary>
        /// 视图名称
        /// </summary>
        public string ViewName
        {
            get { return (string)_cells["ViewName"].Val; }
            private set { _cells["ViewName"].Val = value; }
        }

        /// <summary>
        /// 传递给菜单程序的参数
        /// </summary>
        public string Params
        {
            get { return (string)_cells["Params"].Val; }
            private set { _cells["Params"].Val = value; }
        }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon
        {
            get { return (string)_cells["Icon"].Val; }
            private set { _cells["Icon"].Val = value; }
        }

        /// <summary>
        /// 提供提示信息的服务名称，空表示无提示信息
        /// </summary>
        public string SrvName
        {
            get { return (string)_cells["SrvName"].Val; }
            private set { _cells["SrvName"].Val = value; }
        }

        /// <summary>
        /// 备注
        /// </summary>
        public string Note
        {
            get { return (string)_cells["Note"].Val; }
            private set { _cells["Note"].Val = value; }
        }

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int Dispidx
        {
            get { return (int)_cells["Dispidx"].Val; }
            private set { _cells["Dispidx"].Val = value; }
        }

        /// <summary>
        /// 定义了菜单是否被锁定。0表未锁定，1表锁定不可用
        /// </summary>
        public bool IsLocked
        {
            get { return (bool)_cells["IsLocked"].Val; }
            private set { _cells["IsLocked"].Val = value; }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Ctime
        {
            get { return (DateTime)_cells["Ctime"].Val; }
            private set { _cells["Ctime"].Val = value; }
        }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime Mtime
        {
            get { return (DateTime)_cells["Mtime"].Val; }
            private set { _cells["Mtime"].Val = value; }
        }
    }
}
