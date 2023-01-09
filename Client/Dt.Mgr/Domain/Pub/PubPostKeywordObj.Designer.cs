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
    [Tbl("cm_pub_post_keyword")]
    public partial class PubPostKeywordObj : Entity
    {
        #region 构造方法
        PubPostKeywordObj() { }

        public PubPostKeywordObj(
            long PostID,
            string Keyword)
        {
            AddCell("PostID", PostID);
            AddCell("Keyword", Keyword);
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
        /// 关键字
        /// </summary>
        public string Keyword
        {
            get { return (string)this["Keyword"]; }
            set { this["Keyword"] = value; }
        }

        new public long ID { get { return -1; } }

        #region 静态方法
        /// <summary>
        /// 返回所有实体的列表，每个实体包含所有列值
        /// </summary>
        /// <returns></returns>
        public static Task<Table<PubPostKeywordObj>> GetAll()
        {
            return EntityEx.GetAll<PubPostKeywordObj>();
        }

        /// <summary>
        /// 获取新ID
        /// </summary>
        /// <returns></returns>
        public static Task<long> NewID()
        {
            return EntityEx.GetNewID<PubPostKeywordObj>();
        }

        /// <summary>
        /// 获取新序列值
        /// </summary>
        /// <param name="p_colName">字段名称，不可为空</param>
        /// <returns>新序列值</returns>
        public static Task<int> NewSeq(string p_colName)
        {
            return EntityEx.GetNewSeq<PubPostKeywordObj>(p_colName);
        }
        #endregion
    }
}