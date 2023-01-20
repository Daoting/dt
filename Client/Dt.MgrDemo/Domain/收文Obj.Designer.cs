#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-19 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.MgrDemo.Domain
{
    [Tbl("demo_收文")]
    public partial class 收文Obj : Entity
    {
        #region 构造方法
        收文Obj() { }

        public 收文Obj(CellList p_cells) : base(p_cells) { }

        public 收文Obj(
            long ID,
            string 来文单位 = default,
            DateTime 来文时间 = default,
            密级 密级 = default,
            string 文件标题 = default,
            string 文件附件 = default,
            string 市场部经理意见 = default,
            string 综合部经理意见 = default,
            DateTime 收文完成时间 = default)
        {
            AddCell("ID", ID);
            AddCell("来文单位", 来文单位);
            AddCell("来文时间", 来文时间);
            AddCell("密级", 密级);
            AddCell("文件标题", 文件标题);
            AddCell("文件附件", 文件附件);
            AddCell("市场部经理意见", 市场部经理意见);
            AddCell("综合部经理意见", 综合部经理意见);
            AddCell("收文完成时间", 收文完成时间);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public string 来文单位
        {
            get { return (string)this["来文单位"]; }
            set { this["来文单位"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime 来文时间
        {
            get { return (DateTime)this["来文时间"]; }
            set { this["来文时间"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public 密级 密级
        {
            get { return (密级)this["密级"]; }
            set { this["密级"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string 文件标题
        {
            get { return (string)this["文件标题"]; }
            set { this["文件标题"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string 文件附件
        {
            get { return (string)this["文件附件"]; }
            set { this["文件附件"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string 市场部经理意见
        {
            get { return (string)this["市场部经理意见"]; }
            set { this["市场部经理意见"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string 综合部经理意见
        {
            get { return (string)this["综合部经理意见"]; }
            set { this["综合部经理意见"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime 收文完成时间
        {
            get { return (DateTime)this["收文完成时间"]; }
            set { this["收文完成时间"] = value; }
        }

        #region 静态方法
        /// <summary>
        /// 根据主键获得实体对象(包含所有列值)，仅支持单主键，当启用实体缓存时：
        /// <para>1. 首先从缓存中获取，有则直接返回</para>
        /// <para>2. 无则查询数据库，并将查询结果添加到缓存以备下次使用</para>
        /// </summary>
        /// <param name="p_id">主键</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<收文Obj> GetByID(string p_id)
        {
            return EntityEx.GetByID<收文Obj>(p_id);
        }

        /// <summary>
        /// 根据主键获得实体对象(包含所有列值)，仅支持单主键，当启用实体缓存时：
        /// <para>1. 首先从缓存中获取，有则直接返回</para>
        /// <para>2. 无则查询数据库，并将查询结果添加到缓存以备下次使用</para>
        /// </summary>
        /// <param name="p_id">主键</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<收文Obj> GetByID(long p_id)
        {
            return EntityEx.GetByID<收文Obj>(p_id);
        }

        /// <summary>
        /// 根据主键或唯一索引列获得实体对象(包含所有列值)，仅支持单主键，当启用实体缓存时：
        /// <para>1. 首先从缓存中获取，有则直接返回</para>
        /// <para>2. 无则查询数据库，并将查询结果添加到缓存以备下次使用</para>
        /// </summary>
        /// <param name="p_keyName">主键或唯一索引列名</param>
        /// <param name="p_keyVal">键值</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<收文Obj> GetByKey(string p_keyName, string p_keyVal)
        {
            return EntityEx.GetByKey<收文Obj>(p_keyName, p_keyVal);
        }

        /// <summary>
        /// 根据主键删除实体对象，仅支持单主键，删除前先根据主键获取该实体对象，并非直接删除！！！
        /// <para>删除成功后：</para>
        /// <para>1. 若存在领域事件，则发布事件</para>
        /// <para>2. 若已设置服务端缓存，则删除缓存</para>
        /// </summary>
        /// <param name="p_id">主键</param>
        /// <param name="p_isNotify">是否提示删除结果</param>
        /// <returns>true 删除成功</returns>
        public static Task<bool> DelByID(string p_id, bool p_isNotify = true)
        {
            return EntityEx.DelByID<收文Obj>(p_id, p_isNotify);
        }

        /// <summary>
        /// 根据主键删除实体对象，仅支持单主键，删除前先根据主键获取该实体对象，并非直接删除！！！
        /// <para>删除成功后：</para>
        /// <para>1. 若存在领域事件，则发布事件</para>
        /// <para>2. 若已设置服务端缓存，则删除缓存</para>
        /// </summary>
        /// <param name="p_id">主键</param>
        /// <param name="p_isNotify">是否提示删除结果</param>
        /// <returns>true 删除成功</returns>
        public static Task<bool> DelByID(long p_id, bool p_isNotify = true)
        {
            return EntityEx.DelByID<收文Obj>(p_id, p_isNotify);
        }

        /// <summary>
        /// 查询实体列表，每个实体包含所有列值，过滤条件null或空时返回所有实体
        /// </summary>
        /// <param name="p_filter">过滤串，where后面的部分，null或空返回所有实体</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回实体列表</returns>
        public static Task<Table<收文Obj>> Query(string p_filter = null, object p_params = null)
        {
            return EntityEx.Query<收文Obj>(p_filter, p_params);
        }

        /// <summary>
        /// 按页查询实体列表，每个实体包含所有列值
        /// </summary>
        /// <param name="p_starRow">起始行号：mysql中第一行为0行</param>
        /// <param name="p_pageSize">每页显示行数</param>
        /// <param name="p_filter">过滤串，where后面的部分</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回实体列表</returns>
        public static Task<Table<收文Obj>> Page(int p_starRow, int p_pageSize, string p_filter = null, object p_params = null)
        {
            return EntityEx.Page<收文Obj>(p_starRow, p_pageSize, p_filter, p_params);
        }

        /// <summary>
        /// 返回符合条件的第一个实体对象，每个实体包含所有列值，不存在时返回null
        /// </summary>
        /// <param name="p_filter">过滤串，where后面的部分，null或空返回所有中的第一行</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<收文Obj> First(string p_filter, object p_params = null)
        {
            return EntityEx.First<收文Obj>(p_filter, p_params);
        }

        /// <summary>
        /// 获取新ID
        /// </summary>
        /// <returns></returns>
        public static Task<long> NewID()
        {
            return EntityEx.GetNewID<收文Obj>();
        }

        /// <summary>
        /// 获取新序列值
        /// </summary>
        /// <param name="p_colName">字段名称，不可为空</param>
        /// <returns>新序列值</returns>
        public static Task<int> NewSeq(string p_colName)
        {
            return EntityEx.GetNewSeq<收文Obj>(p_colName);
        }
        #endregion
    }
}