#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-11-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Threading.Tasks;
#endregion

namespace Dt.Cm.Workflow
{
    #region 自动生成
    [Tbl("cm_wfd_prc")]
    public partial class WfdPrc : Entity
    {
        #region 构造方法
        WfdPrc() { }

        public WfdPrc(
            long ID,
            string Name = default,
            string Diagram = default,
            bool IsLocked = default,
            bool Singleton = default,
            string FormType = default,
            string ListType = default,
            string Note = default,
            int Dispidx = default,
            DateTime Ctime = default,
            DateTime Mtime = default)
        {
            AddCell<long>("ID", ID);
            AddCell<string>("Name", Name);
            AddCell<string>("Diagram", Diagram);
            AddCell<bool>("IsLocked", IsLocked);
            AddCell<bool>("Singleton", Singleton);
            AddCell<string>("FormType", FormType);
            AddCell<string>("ListType", ListType);
            AddCell<string>("Note", Note);
            AddCell<int>("Dispidx", Dispidx);
            AddCell<DateTime>("Ctime", Ctime);
            AddCell<DateTime>("Mtime", Mtime);
            IsAdded = true;
            AttachHook();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 流程名称
        /// </summary>
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
        }

        /// <summary>
        /// 流程图
        /// </summary>
        public string Diagram
        {
            get { return (string)this["Diagram"]; }
            set { this["Diagram"] = value; }
        }

        /// <summary>
        /// 锁定标志，0表未锁定；1表锁定，不能创建流程实例，已启动的流程实例继续执行
        /// </summary>
        public bool IsLocked
        {
            get { return (bool)this["IsLocked"]; }
            set { this["IsLocked"] = value; }
        }

        /// <summary>
        /// 同一时刻只允许有一个激活的流程实例，0表非单实例，1表单实例
        /// </summary>
        public bool Singleton
        {
            get { return (bool)this["Singleton"]; }
            set { this["Singleton"] = value; }
        }

        /// <summary>
        /// 表单窗口类
        /// </summary>
        public string FormType
        {
            get { return (string)this["FormType"]; }
            set { this["FormType"] = value; }
        }

        /// <summary>
        /// 列表窗口类
        /// </summary>
        public string ListType
        {
            get { return (string)this["ListType"]; }
            set { this["ListType"] = value; }
        }

        /// <summary>
        /// 描述
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