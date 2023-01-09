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
    [Tbl("cm_pub_comment")]
    public partial class PubCommentObj : Entity
    {
        #region 构造方法
        PubCommentObj() { }

        public PubCommentObj(
            long ID,
            long PostID = default,
            string Content = default,
            long UserID = default,
            string UserName = default,
            DateTime Ctime = default,
            bool IsSpam = default,
            long? ParentID = default,
            int Support = default,
            int Oppose = default)
        {
            AddCell("ID", ID);
            AddCell("PostID", PostID);
            AddCell("Content", Content);
            AddCell("UserID", UserID);
            AddCell("UserName", UserName);
            AddCell("Ctime", Ctime);
            AddCell("IsSpam", IsSpam);
            AddCell("ParentID", ParentID);
            AddCell("Support", Support);
            AddCell("Oppose", Oppose);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 文章标识
        /// </summary>
        public long PostID
        {
            get { return (long)this["PostID"]; }
            set { this["PostID"] = value; }
        }

        /// <summary>
        /// 评论内容
        /// </summary>
        public string Content
        {
            get { return (string)this["Content"]; }
            set { this["Content"] = value; }
        }

        /// <summary>
        /// 评论人标识
        /// </summary>
        public long UserID
        {
            get { return (long)this["UserID"]; }
            set { this["UserID"] = value; }
        }

        /// <summary>
        /// 评论人
        /// </summary>
        public string UserName
        {
            get { return (string)this["UserName"]; }
            set { this["UserName"] = value; }
        }

        /// <summary>
        /// 评论时间
        /// </summary>
        public DateTime Ctime
        {
            get { return (DateTime)this["Ctime"]; }
            set { this["Ctime"] = value; }
        }

        /// <summary>
        /// 是否为垃圾评论
        /// </summary>
        public bool IsSpam
        {
            get { return (bool)this["IsSpam"]; }
            set { this["IsSpam"] = value; }
        }

        /// <summary>
        /// 上级评论标识
        /// </summary>
        public long? ParentID
        {
            get { return (long?)this["ParentID"]; }
            set { this["ParentID"] = value; }
        }

        /// <summary>
        /// 对该评论的支持数
        /// </summary>
        public int Support
        {
            get { return (int)this["Support"]; }
            set { this["Support"] = value; }
        }

        /// <summary>
        /// 对该评论的反对数
        /// </summary>
        public int Oppose
        {
            get { return (int)this["Oppose"]; }
            set { this["Oppose"] = value; }
        }

        #region 静态方法
        /// <summary>
        /// 根据主键获得实体对象(包含所有列值)，仅支持单主键，当启用实体缓存时：
        /// <para>1. 首先从缓存中获取，有则直接返回</para>
        /// <para>2. 无则查询数据库，并将查询结果添加到缓存以备下次使用</para>
        /// </summary>
        /// <param name="p_id">主键</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<PubCommentObj> GetByID(string p_id)
        {
            return EntityEx.GetByID<PubCommentObj>(p_id);
        }

        /// <summary>
        /// 根据主键获得实体对象(包含所有列值)，仅支持单主键，当启用实体缓存时：
        /// <para>1. 首先从缓存中获取，有则直接返回</para>
        /// <para>2. 无则查询数据库，并将查询结果添加到缓存以备下次使用</para>
        /// </summary>
        /// <param name="p_id">主键</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<PubCommentObj> GetByID(long p_id)
        {
            return EntityEx.GetByID<PubCommentObj>(p_id);
        }

        /// <summary>
        /// 根据主键或唯一索引列获得实体对象(包含所有列值)，仅支持单主键，当启用实体缓存时：
        /// <para>1. 首先从缓存中获取，有则直接返回</para>
        /// <para>2. 无则查询数据库，并将查询结果添加到缓存以备下次使用</para>
        /// </summary>
        /// <param name="p_keyName">主键或唯一索引列名</param>
        /// <param name="p_keyVal">键值</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<PubCommentObj> GetByKey(string p_keyName, string p_keyVal)
        {
            return EntityEx.GetByKey<PubCommentObj>(p_keyName, p_keyVal);
        }

        /// <summary>
        /// 返回所有实体的列表，每个实体包含所有列值
        /// </summary>
        /// <returns></returns>
        public static Task<Table<PubCommentObj>> GetAll()
        {
            return EntityEx.GetAll<PubCommentObj>();
        }

        /// <summary>
        /// 获取新ID
        /// </summary>
        /// <returns></returns>
        public static Task<long> NewID()
        {
            return EntityEx.GetNewID<PubCommentObj>();
        }

        /// <summary>
        /// 获取新序列值
        /// </summary>
        /// <param name="p_colName">字段名称，不可为空</param>
        /// <returns>新序列值</returns>
        public static Task<int> NewSeq(string p_colName)
        {
            return EntityEx.GetNewSeq<PubCommentObj>(p_colName);
        }
        #endregion
    }
}