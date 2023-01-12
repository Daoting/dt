#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-12 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.Mgr.Domain
{
    [Tbl("cm_pub_album")]
    public partial class PubAlbumObj : Entity
    {
        #region 构造方法
        PubAlbumObj() { }

        public PubAlbumObj(
            long ID,
            string Name = default,
            string Creator = default,
            DateTime Ctime = default)
        {
            AddCell("ID", ID);
            AddCell("Name", Name);
            AddCell("Creator", Creator);
            AddCell("Ctime", Ctime);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
        }

        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator
        {
            get { return (string)this["Creator"]; }
            set { this["Creator"] = value; }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Ctime
        {
            get { return (DateTime)this["Ctime"]; }
            set { this["Ctime"] = value; }
        }

        #region 静态方法
        /// <summary>
        /// 根据主键获得实体对象(包含所有列值)，仅支持单主键，当启用实体缓存时：
        /// <para>1. 首先从缓存中获取，有则直接返回</para>
        /// <para>2. 无则查询数据库，并将查询结果添加到缓存以备下次使用</para>
        /// </summary>
        /// <param name="p_id">主键</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<PubAlbumObj> GetByID(string p_id)
        {
            return EntityEx.GetByID<PubAlbumObj>(p_id);
        }

        /// <summary>
        /// 根据主键获得实体对象(包含所有列值)，仅支持单主键，当启用实体缓存时：
        /// <para>1. 首先从缓存中获取，有则直接返回</para>
        /// <para>2. 无则查询数据库，并将查询结果添加到缓存以备下次使用</para>
        /// </summary>
        /// <param name="p_id">主键</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<PubAlbumObj> GetByID(long p_id)
        {
            return EntityEx.GetByID<PubAlbumObj>(p_id);
        }

        /// <summary>
        /// 根据主键或唯一索引列获得实体对象(包含所有列值)，仅支持单主键，当启用实体缓存时：
        /// <para>1. 首先从缓存中获取，有则直接返回</para>
        /// <para>2. 无则查询数据库，并将查询结果添加到缓存以备下次使用</para>
        /// </summary>
        /// <param name="p_keyName">主键或唯一索引列名</param>
        /// <param name="p_keyVal">键值</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<PubAlbumObj> GetByKey(string p_keyName, string p_keyVal)
        {
            return EntityEx.GetByKey<PubAlbumObj>(p_keyName, p_keyVal);
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
            return EntityEx.DelByID<PubAlbumObj>(p_id, p_isNotify);
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
            return EntityEx.DelByID<PubAlbumObj>(p_id, p_isNotify);
        }

        /// <summary>
        /// 根据单主键或唯一索引列删除实体，删除前先获取该实体对象，并非直接删除！！！
        /// <para>删除成功后：</para>
        /// <para>1. 若存在领域事件，则发布事件</para>
        /// <para>2. 若已设置服务端缓存，则删除缓存</para>
        /// </summary>
        /// <param name="p_keyName">主键或唯一索引列名</param>
        /// <param name="p_keyVal">主键值</param>
        /// <param name="p_isNotify">是否提示删除结果，客户端有效</param>
        /// <returns>实际删除行数</returns>
        public static Task<bool> DelByKey(string p_keyName, string p_keyVal, bool p_isNotify = true)
        {
            return EntityEx.DelByKey<PubAlbumObj>(p_keyName, p_keyVal);
        }

        /// <summary>
        /// 查询实体列表，每个实体包含所有列值，过滤条件null或空时返回所有实体
        /// </summary>
        /// <param name="p_filter">过滤串，where后面的部分，null或空返回所有实体</param>
        /// <returns>返回实体列表</returns>
        public static Task<Table<PubAlbumObj>> Query(string p_filter = null)
        {
            return EntityEx.Query<PubAlbumObj>();
        }

        /// <summary>
        /// 按页查询实体列表，每个实体包含所有列值
        /// </summary>
        /// <param name="p_starRow">起始行号：mysql中第一行为0行</param>
        /// <param name="p_pageSize">每页显示行数</param>
        /// <param name="p_filter">过滤串，where后面的部分</param>
        /// <returns>返回实体列表</returns>
        public static Task<Table<PubAlbumObj>> Page(int p_starRow, int p_pageSize, string p_filter = null)
        {
            return EntityEx.Page<PubAlbumObj>(p_starRow, p_pageSize, p_filter);
        }

        /// <summary>
        /// 返回符合条件的第一个实体对象，每个实体包含所有列值，不存在时返回null
        /// </summary>
        /// <param name="p_filter">过滤串，where后面的部分，null或空返回所有中的第一行</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<PubAlbumObj> First(string p_filter)
        {
            return EntityEx.First<PubAlbumObj>(p_filter);
        }

        /// <summary>
        /// 获取新ID
        /// </summary>
        /// <returns></returns>
        public static Task<long> NewID()
        {
            return EntityEx.GetNewID<PubAlbumObj>();
        }

        /// <summary>
        /// 获取新序列值
        /// </summary>
        /// <param name="p_colName">字段名称，不可为空</param>
        /// <returns>新序列值</returns>
        public static Task<int> NewSeq(string p_colName)
        {
            return EntityEx.GetNewSeq<PubAlbumObj>(p_colName);
        }
        #endregion
    }
}