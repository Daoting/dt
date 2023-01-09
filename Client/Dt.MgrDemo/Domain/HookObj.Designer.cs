#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-04 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.MgrDemo.Domain
{
    [Tbl("demo_hook")]
    public partial class HookObj : Entity
    {
        #region 构造方法
        HookObj() { }

        public HookObj(
            long ID,
            string MaxLength = default,
            string NotNull = default,
            string Src = default,
            string Tgt = default,
            bool IsCheck = default,
            int NoBinding = default,
            int NoHook = default,
            bool NoDelete = default)
        {
            AddCell("ID", ID);
            AddCell("MaxLength", MaxLength);
            AddCell("NotNull", NotNull);
            AddCell("Src", Src);
            AddCell("Tgt", Tgt);
            AddCell("IsCheck", IsCheck);
            AddCell("NoBinding", NoBinding);
            AddCell("NoHook", NoHook);
            AddCell("NoDelete", NoDelete);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 限制最大长度
        /// </summary>
        public string MaxLength
        {
            get { return (string)this["MaxLength"]; }
            set { this["MaxLength"] = value; }
        }

        /// <summary>
        /// 不为空
        /// </summary>
        public string NotNull
        {
            get { return (string)this["NotNull"]; }
            set { this["NotNull"] = value; }
        }

        /// <summary>
        /// 联动源
        /// </summary>
        public string Src
        {
            get { return (string)this["Src"]; }
            set { this["Src"] = value; }
        }

        /// <summary>
        /// 联动目标
        /// </summary>
        public string Tgt
        {
            get { return (string)this["Tgt"]; }
            set { this["Tgt"] = value; }
        }

        /// <summary>
        /// 联动源不为空可选中
        /// </summary>
        public bool IsCheck
        {
            get { return (bool)this["IsCheck"]; }
            set { this["IsCheck"] = value; }
        }

        /// <summary>
        /// 未和UI绑定
        /// </summary>
        public int NoBinding
        {
            get { return (int)this["NoBinding"]; }
            set { this["NoBinding"] = value; }
        }

        /// <summary>
        /// 无值变化Hook
        /// </summary>
        public int NoHook
        {
            get { return (int)this["NoHook"]; }
            set { this["NoHook"] = value; }
        }

        /// <summary>
        /// 禁止删除
        /// </summary>
        public bool NoDelete
        {
            get { return (bool)this["NoDelete"]; }
            set { this["NoDelete"] = value; }
        }

        #region 静态方法
        /// <summary>
        /// 根据主键获得实体对象(包含所有列值)，仅支持单主键，当启用实体缓存时：
        /// <para>1. 首先从缓存中获取，有则直接返回</para>
        /// <para>2. 无则查询数据库，并将查询结果添加到缓存以备下次使用</para>
        /// </summary>
        /// <param name="p_id">主键</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<HookObj> GetByID(string p_id)
        {
            return EntityEx.GetByID<HookObj>(p_id);
        }

        /// <summary>
        /// 根据主键获得实体对象(包含所有列值)，仅支持单主键，当启用实体缓存时：
        /// <para>1. 首先从缓存中获取，有则直接返回</para>
        /// <para>2. 无则查询数据库，并将查询结果添加到缓存以备下次使用</para>
        /// </summary>
        /// <param name="p_id">主键</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<HookObj> GetByID(long p_id)
        {
            return EntityEx.GetByID<HookObj>(p_id);
        }

        /// <summary>
        /// 根据主键或唯一索引列获得实体对象(包含所有列值)，仅支持单主键，当启用实体缓存时：
        /// <para>1. 首先从缓存中获取，有则直接返回</para>
        /// <para>2. 无则查询数据库，并将查询结果添加到缓存以备下次使用</para>
        /// </summary>
        /// <param name="p_keyName">主键或唯一索引列名</param>
        /// <param name="p_keyVal">键值</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<HookObj> GetByKey(string p_keyName, string p_keyVal)
        {
            return EntityEx.GetByKey<HookObj>(p_keyName, p_keyVal);
        }

        /// <summary>
        /// 返回所有实体的列表，每个实体包含所有列值
        /// </summary>
        /// <returns></returns>
        public static Task<Table<HookObj>> GetAll()
        {
            return EntityEx.GetAll<HookObj>();
        }

        /// <summary>
        /// 获取新ID
        /// </summary>
        /// <returns></returns>
        public static Task<long> NewID()
        {
            return EntityEx.GetNewID<HookObj>();
        }

        /// <summary>
        /// 获取新序列值
        /// </summary>
        /// <param name="p_colName">字段名称，不可为空</param>
        /// <returns>新序列值</returns>
        public static Task<int> NewSeq(string p_colName)
        {
            return EntityEx.GetNewSeq<HookObj>(p_colName);
        }
        #endregion
    }
}