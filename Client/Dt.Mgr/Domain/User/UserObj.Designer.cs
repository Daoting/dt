#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-10 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.Mgr.Domain
{
    [Tbl("cm_user")]
    public partial class UserObj : Entity
    {
        #region 构造方法
        UserObj() { }

        public UserObj(
            long ID,
            string Phone = default,
            string Name = default,
            string Pwd = default,
            Gender Sex = (Gender)1,
            string Photo = default,
            bool Expired = false,
            DateTime Ctime = default,
            DateTime Mtime = default)
        {
            AddCell("ID", ID);
            AddCell("Phone", Phone);
            AddCell("Name", Name);
            AddCell("Pwd", Pwd);
            AddCell("Sex", Sex);
            AddCell("Photo", Photo);
            AddCell("Expired", Expired);
            AddCell("Ctime", Ctime);
            AddCell("Mtime", Mtime);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 手机号，唯一
        /// </summary>
        public string Phone
        {
            get { return (string)this["Phone"]; }
            set { this["Phone"] = value; }
        }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
        }

        /// <summary>
        /// 密码的md5
        /// </summary>
        public string Pwd
        {
            get { return (string)this["Pwd"]; }
            set { this["Pwd"] = value; }
        }

        /// <summary>
        /// 性别
        /// </summary>
        public Gender Sex
        {
            get { return (Gender)this["Sex"]; }
            set { this["Sex"] = value; }
        }

        /// <summary>
        /// 头像
        /// </summary>
        public string Photo
        {
            get { return (string)this["Photo"]; }
            set { this["Photo"] = value; }
        }

        /// <summary>
        /// 是否停用
        /// </summary>
        public bool Expired
        {
            get { return (bool)this["Expired"]; }
            set { this["Expired"] = value; }
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
        public static Task<UserObj> GetByID(string p_id)
        {
            return EntityEx.GetByID<UserObj>(p_id);
        }

        /// <summary>
        /// 根据主键获得实体对象(包含所有列值)，仅支持单主键，当启用实体缓存时：
        /// <para>1. 首先从缓存中获取，有则直接返回</para>
        /// <para>2. 无则查询数据库，并将查询结果添加到缓存以备下次使用</para>
        /// </summary>
        /// <param name="p_id">主键</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<UserObj> GetByID(long p_id)
        {
            return EntityEx.GetByID<UserObj>(p_id);
        }

        /// <summary>
        /// 根据主键或唯一索引列获得实体对象(包含所有列值)，仅支持单主键，当启用实体缓存时：
        /// <para>1. 首先从缓存中获取，有则直接返回</para>
        /// <para>2. 无则查询数据库，并将查询结果添加到缓存以备下次使用</para>
        /// </summary>
        /// <param name="p_keyName">主键或唯一索引列名</param>
        /// <param name="p_keyVal">键值</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<UserObj> GetByKey(string p_keyName, string p_keyVal)
        {
            return EntityEx.GetByKey<UserObj>(p_keyName, p_keyVal);
        }

        /// <summary>
        /// 查询实体列表，每个实体包含所有列值，过滤条件null或空时返回所有实体
        /// </summary>
        /// <param name="p_filter">过滤串，where后面的部分，null或空返回所有实体</param>
        /// <returns>返回实体列表</returns>
        public static Task<Table<UserObj>> Query(string p_filter = null)
        {
            return EntityEx.Query<UserObj>();
        }

        /// <summary>
        /// 按页查询实体列表，每个实体包含所有列值
        /// </summary>
        /// <param name="p_starRow">起始行号：mysql中第一行为0行</param>
        /// <param name="p_pageSize">每页显示行数</param>
        /// <param name="p_filter">过滤串，where后面的部分</param>
        /// <returns>返回实体列表</returns>
        public static Task<Table<UserObj>> Page(int p_starRow, int p_pageSize, string p_filter = null)
        {
            return EntityEx.Page<UserObj>(p_starRow, p_pageSize, p_filter);
        }

        /// <summary>
        /// 返回符合条件的第一个实体对象，每个实体包含所有列值，不存在时返回null
        /// </summary>
        /// <param name="p_filter">过滤串，where后面的部分，null或空返回所有中的第一行</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<UserObj> First(string p_filter)
        {
            return EntityEx.First<UserObj>(p_filter);
        }

        /// <summary>
        /// 获取新ID
        /// </summary>
        /// <returns></returns>
        public static Task<long> NewID()
        {
            return EntityEx.GetNewID<UserObj>();
        }

        /// <summary>
        /// 获取新序列值
        /// </summary>
        /// <param name="p_colName">字段名称，不可为空</param>
        /// <returns>新序列值</returns>
        public static Task<int> NewSeq(string p_colName)
        {
            return EntityEx.GetNewSeq<UserObj>(p_colName);
        }
        #endregion
    }
}