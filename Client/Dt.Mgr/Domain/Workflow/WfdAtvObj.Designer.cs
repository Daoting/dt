#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-11 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.Mgr.Domain
{
    [Tbl("cm_wfd_atv")]
    public partial class WfdAtvObj : Entity
    {
        #region 构造方法
        WfdAtvObj() { }

        public WfdAtvObj(
            long ID,
            long PrcID = default,
            string Name = default,
            WfdAtvType Type = default,
            WfdAtvExecScope ExecScope = default,
            WfdAtvExecLimit ExecLimit = default,
            long? ExecAtvID = default,
            bool AutoAccept = default,
            bool CanDelete = default,
            bool CanTerminate = default,
            bool CanJumpInto = default,
            WfdAtvTransKind TransKind = default,
            WfdAtvJoinKind JoinKind = default,
            DateTime Ctime = default,
            DateTime Mtime = default)
        {
            AddCell("ID", ID);
            AddCell("PrcID", PrcID);
            AddCell("Name", Name);
            AddCell("Type", Type);
            AddCell("ExecScope", ExecScope);
            AddCell("ExecLimit", ExecLimit);
            AddCell("ExecAtvID", ExecAtvID);
            AddCell("AutoAccept", AutoAccept);
            AddCell("CanDelete", CanDelete);
            AddCell("CanTerminate", CanTerminate);
            AddCell("CanJumpInto", CanJumpInto);
            AddCell("TransKind", TransKind);
            AddCell("JoinKind", JoinKind);
            AddCell("Ctime", Ctime);
            AddCell("Mtime", Mtime);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 流程标识
        /// </summary>
        public long PrcID
        {
            get { return (long)this["PrcID"]; }
            set { this["PrcID"] = value; }
        }

        /// <summary>
        /// 活动名称，同时作为状态名称
        /// </summary>
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
        }

        /// <summary>
        /// 活动类别 0:普通活动 1:开始活动 2:同步活动 3:结束活动
        /// </summary>
        public WfdAtvType Type
        {
            get { return (WfdAtvType)this["Type"]; }
            set { this["Type"] = value; }
        }

        /// <summary>
        /// 执行者范围 0:一组用户 1:所有用户 2:单个用户  3:任一用户
        /// </summary>
        public WfdAtvExecScope ExecScope
        {
            get { return (WfdAtvExecScope)this["ExecScope"]; }
            set { this["ExecScope"] = value; }
        }

        /// <summary>
        /// 执行者限制 0无限制 1前一活动的执行者 2前一活动的同部门执行者 3已完成活动的执行者 4已完成活动的同部门执行者
        /// </summary>
        public WfdAtvExecLimit ExecLimit
        {
            get { return (WfdAtvExecLimit)this["ExecLimit"]; }
            set { this["ExecLimit"] = value; }
        }

        /// <summary>
        /// 在执行者限制为3或4时选择的活动
        /// </summary>
        public long? ExecAtvID
        {
            get { return (long?)this["ExecAtvID"]; }
            set { this["ExecAtvID"] = value; }
        }

        /// <summary>
        /// 是否自动签收，打开工作流视图时自动签收工作项
        /// </summary>
        public bool AutoAccept
        {
            get { return (bool)this["AutoAccept"]; }
            set { this["AutoAccept"] = value; }
        }

        /// <summary>
        /// 能否删除流程实例和业务数据，0否 1
        /// </summary>
        public bool CanDelete
        {
            get { return (bool)this["CanDelete"]; }
            set { this["CanDelete"] = value; }
        }

        /// <summary>
        /// 能否中止流程实例，中止一个流程是流程的一种非正常的结束，0否 1能
        /// </summary>
        public bool CanTerminate
        {
            get { return (bool)this["CanTerminate"]; }
            set { this["CanTerminate"] = value; }
        }

        /// <summary>
        /// 是否可作为跳转目标，0不可跳转 1可以
        /// </summary>
        public bool CanJumpInto
        {
            get { return (bool)this["CanJumpInto"]; }
            set { this["CanJumpInto"] = value; }
        }

        /// <summary>
        /// 当前活动的后续迁移方式 0:自由选择 1:并行 2:独占式选择
        /// </summary>
        public WfdAtvTransKind TransKind
        {
            get { return (WfdAtvTransKind)this["TransKind"]; }
            set { this["TransKind"] = value; }
        }

        /// <summary>
        /// 同步活动有效，聚合方式，0:全部任务 1:任一任务 2:即时同步
        /// </summary>
        public WfdAtvJoinKind JoinKind
        {
            get { return (WfdAtvJoinKind)this["JoinKind"]; }
            set { this["JoinKind"] = value; }
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
        /// 修改时间
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
        public static Task<WfdAtvObj> GetByID(string p_id)
        {
            return EntityEx.GetByID<WfdAtvObj>(p_id);
        }

        /// <summary>
        /// 根据主键获得实体对象(包含所有列值)，仅支持单主键，当启用实体缓存时：
        /// <para>1. 首先从缓存中获取，有则直接返回</para>
        /// <para>2. 无则查询数据库，并将查询结果添加到缓存以备下次使用</para>
        /// </summary>
        /// <param name="p_id">主键</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<WfdAtvObj> GetByID(long p_id)
        {
            return EntityEx.GetByID<WfdAtvObj>(p_id);
        }

        /// <summary>
        /// 根据主键或唯一索引列获得实体对象(包含所有列值)，仅支持单主键，当启用实体缓存时：
        /// <para>1. 首先从缓存中获取，有则直接返回</para>
        /// <para>2. 无则查询数据库，并将查询结果添加到缓存以备下次使用</para>
        /// </summary>
        /// <param name="p_keyName">主键或唯一索引列名</param>
        /// <param name="p_keyVal">键值</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<WfdAtvObj> GetByKey(string p_keyName, string p_keyVal)
        {
            return EntityEx.GetByKey<WfdAtvObj>(p_keyName, p_keyVal);
        }

        /// <summary>
        /// 根据主键删除实体对象，仅支持单主键，删除前先根据主键获取该实体对象，并非直接删除！！！
        /// <para>删除成功后：</para>
        /// <para>1. 若存在领域事件，则发布事件</para>
        /// <para>2. 若已设置服务端缓存，则删除缓存</para>
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_id">主键</param>
        /// <param name="p_isNotify">是否提示删除结果</param>
        /// <returns>true 删除成功</returns>
        public static Task<bool> DelByID(string p_id, bool p_isNotify = true)
        {
            return EntityEx.DelByID<WfdAtvObj>(p_id, p_isNotify);
        }

        /// <summary>
        /// 根据主键删除实体对象，仅支持单主键，删除前先根据主键获取该实体对象，并非直接删除！！！
        /// <para>删除成功后：</para>
        /// <para>1. 若存在领域事件，则发布事件</para>
        /// <para>2. 若已设置服务端缓存，则删除缓存</para>
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_id">主键</param>
        /// <param name="p_isNotify">是否提示删除结果</param>
        /// <returns>true 删除成功</returns>
        public static Task<bool> DelByID(long p_id, bool p_isNotify = true)
        {
            return EntityEx.DelByID<WfdAtvObj>(p_id, p_isNotify);
        }

        /// <summary>
        /// 根据单主键或唯一索引列删除实体，删除前先获取该实体对象，并非直接删除！！！
        /// <para>删除成功后：</para>
        /// <para>1. 若存在领域事件，则发布事件</para>
        /// <para>2. 若已设置服务端缓存，则删除缓存</para>
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_keyName">主键或唯一索引列名</param>
        /// <param name="p_keyVal">主键值</param>
        /// <param name="p_isNotify">是否提示删除结果，客户端有效</param>
        /// <returns>实际删除行数</returns>
        public static Task<bool> DelByKey(string p_keyName, string p_keyVal, bool p_isNotify = true)
        {
            return EntityEx.DelByKey<WfdAtvObj>(p_keyName, p_keyVal);
        }

        /// <summary>
        /// 查询实体列表，每个实体包含所有列值，过滤条件null或空时返回所有实体
        /// </summary>
        /// <param name="p_filter">过滤串，where后面的部分，null或空返回所有实体</param>
        /// <returns>返回实体列表</returns>
        public static Task<Table<WfdAtvObj>> Query(string p_filter = null)
        {
            return EntityEx.Query<WfdAtvObj>();
        }

        /// <summary>
        /// 按页查询实体列表，每个实体包含所有列值
        /// </summary>
        /// <param name="p_starRow">起始行号：mysql中第一行为0行</param>
        /// <param name="p_pageSize">每页显示行数</param>
        /// <param name="p_filter">过滤串，where后面的部分</param>
        /// <returns>返回实体列表</returns>
        public static Task<Table<WfdAtvObj>> Page(int p_starRow, int p_pageSize, string p_filter = null)
        {
            return EntityEx.Page<WfdAtvObj>(p_starRow, p_pageSize, p_filter);
        }

        /// <summary>
        /// 返回符合条件的第一个实体对象，每个实体包含所有列值，不存在时返回null
        /// </summary>
        /// <param name="p_filter">过滤串，where后面的部分，null或空返回所有中的第一行</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<WfdAtvObj> First(string p_filter)
        {
            return EntityEx.First<WfdAtvObj>(p_filter);
        }

        /// <summary>
        /// 获取新ID
        /// </summary>
        /// <returns></returns>
        public static Task<long> NewID()
        {
            return EntityEx.GetNewID<WfdAtvObj>();
        }

        /// <summary>
        /// 获取新序列值
        /// </summary>
        /// <param name="p_colName">字段名称，不可为空</param>
        /// <returns>新序列值</returns>
        public static Task<int> NewSeq(string p_colName)
        {
            return EntityEx.GetNewSeq<WfdAtvObj>(p_colName);
        }
        #endregion
    }
}