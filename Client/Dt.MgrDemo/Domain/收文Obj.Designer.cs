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

namespace Dt.MgrDemo.Domain
{
    [Tbl("demo_收文")]
    public partial class 收文Obj : Entity
    {
        #region 构造方法
        收文Obj() { }

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
        /// 返回所有实体的列表，每个实体包含所有列值
        /// </summary>
        /// <returns></returns>
        public static Task<Table<收文Obj>> GetAll()
        {
            return EntityEx.GetAll<收文Obj>();
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