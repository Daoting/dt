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
#endregion

namespace Dt.Mgr.Domain
{
    [Tbl("cm_wfd_prc")]
    public partial class WfdPrcObj : Entity
    {
        #region 构造方法
        WfdPrcObj() { }

        public WfdPrcObj(
            long ID,
            string Name = default,
            string Diagram = default,
            bool IsLocked = default,
            bool Singleton = default,
            string Note = default,
            int Dispidx = default,
            DateTime Ctime = default,
            DateTime Mtime = default)
        {
            AddCell("ID", ID);
            AddCell("Name", Name);
            AddCell("Diagram", Diagram);
            AddCell("IsLocked", IsLocked);
            AddCell("Singleton", Singleton);
            AddCell("Note", Note);
            AddCell("Dispidx", Dispidx);
            AddCell("Ctime", Ctime);
            AddCell("Mtime", Mtime);
            IsAdded = true;
        }
        #endregion

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

        #region 静态方法
        /// <summary>
        /// 根据主键获得实体对象(包含所有列值)，仅支持单主键，当启用实体缓存时：
        /// <para>1. 首先从缓存中获取，有则直接返回</para>
        /// <para>2. 无则查询数据库，并将查询结果添加到缓存以备下次使用</para>
        /// </summary>
        /// <param name="p_id">主键</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<WfdPrcObj> GetByID(string p_id)
        {
            return EntityEx.GetByID<WfdPrcObj>(p_id);
        }

        /// <summary>
        /// 根据主键获得实体对象(包含所有列值)，仅支持单主键，当启用实体缓存时：
        /// <para>1. 首先从缓存中获取，有则直接返回</para>
        /// <para>2. 无则查询数据库，并将查询结果添加到缓存以备下次使用</para>
        /// </summary>
        /// <param name="p_id">主键</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<WfdPrcObj> GetByID(long p_id)
        {
            return EntityEx.GetByID<WfdPrcObj>(p_id);
        }

        /// <summary>
        /// 根据主键或唯一索引列获得实体对象(包含所有列值)，仅支持单主键，当启用实体缓存时：
        /// <para>1. 首先从缓存中获取，有则直接返回</para>
        /// <para>2. 无则查询数据库，并将查询结果添加到缓存以备下次使用</para>
        /// </summary>
        /// <param name="p_keyName">主键或唯一索引列名</param>
        /// <param name="p_keyVal">键值</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<WfdPrcObj> GetByKey(string p_keyName, string p_keyVal)
        {
            return EntityEx.GetByKey<WfdPrcObj>(p_keyName, p_keyVal);
        }

        /// <summary>
        /// 返回所有实体的列表，每个实体包含所有列值
        /// </summary>
        /// <returns></returns>
        public static Task<Table<WfdPrcObj>> GetAll()
        {
            return EntityEx.GetAll<WfdPrcObj>();
        }

        /// <summary>
        /// 获取新ID
        /// </summary>
        /// <returns></returns>
        public static Task<long> NewID()
        {
            return EntityEx.GetNewID<WfdPrcObj>();
        }

        /// <summary>
        /// 获取新序列值
        /// </summary>
        /// <param name="p_colName">字段名称，不可为空</param>
        /// <returns>新序列值</returns>
        public static Task<int> NewSeq(string p_colName)
        {
            return EntityEx.GetNewSeq<WfdPrcObj>(p_colName);
        }
        #endregion
    }
}