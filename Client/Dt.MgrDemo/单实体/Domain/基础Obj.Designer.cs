#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-17 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.MgrDemo.单实体
{
    [Tbl("demo_基础")]
    public partial class 基础Obj : Entity
    {
        #region 构造方法
        基础Obj() { }

        public 基础Obj(
            long ID,
            int 序列 = default,
            string 限长4 = default,
            string 不重复 = default,
            bool 禁止选中 = default,
            int 校验后台列 = default,
            bool 禁止保存 = default,
            bool 禁止删除 = default,
            string 值变事件 = default,
            DateTime 创建时间 = default,
            DateTime 修改时间 = default)
        {
            AddCell("ID", ID);
            AddCell("序列", 序列);
            AddCell("限长4", 限长4);
            AddCell("不重复", 不重复);
            AddCell("禁止选中", 禁止选中);
            AddCell("校验后台列", 校验后台列);
            AddCell("禁止保存", 禁止保存);
            AddCell("禁止删除", 禁止删除);
            AddCell("值变事件", 值变事件);
            AddCell("创建时间", 创建时间);
            AddCell("修改时间", 修改时间);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 序列自动赋值
        /// </summary>
        public int 序列
        {
            get { return (int)this["序列"]; }
            set { this["序列"] = value; }
        }

        /// <summary>
        /// 限制最大长度4
        /// </summary>
        public string 限长4
        {
            get { return (string)this["限长4"]; }
            set { this["限长4"] = value; }
        }

        /// <summary>
        /// 列值无重复
        /// </summary>
        public string 不重复
        {
            get { return (string)this["不重复"]; }
            set { this["不重复"] = value; }
        }

        /// <summary>
        /// 始终为false
        /// </summary>
        public bool 禁止选中
        {
            get { return (bool)this["禁止选中"]; }
            set { this["禁止选中"] = value; }
        }

        /// <summary>
        /// 未和UI绑定，但校验
        /// </summary>
        public int 校验后台列
        {
            get { return (int)this["校验后台列"]; }
            set { this["校验后台列"] = value; }
        }

        /// <summary>
        /// true时保存前校验不通过
        /// </summary>
        public bool 禁止保存
        {
            get { return (bool)this["禁止保存"]; }
            set { this["禁止保存"] = value; }
        }

        /// <summary>
        /// true时删除前校验不通过
        /// </summary>
        public bool 禁止删除
        {
            get { return (bool)this["禁止删除"]; }
            set { this["禁止删除"] = value; }
        }

        /// <summary>
        /// 每次值变化时触发领域事件
        /// </summary>
        public string 值变事件
        {
            get { return (string)this["值变事件"]; }
            set { this["值变事件"] = value; }
        }

        /// <summary>
        /// 初次创建时间
        /// </summary>
        public DateTime 创建时间
        {
            get { return (DateTime)this["创建时间"]; }
            set { this["创建时间"] = value; }
        }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime 修改时间
        {
            get { return (DateTime)this["修改时间"]; }
            set { this["修改时间"] = value; }
        }

        #region 静态方法
        /// <summary>
        /// 根据主键获得实体对象(包含所有列值)，仅支持单主键，当启用实体缓存时：
        /// <para>1. 首先从缓存中获取，有则直接返回</para>
        /// <para>2. 无则查询数据库，并将查询结果添加到缓存以备下次使用</para>
        /// </summary>
        /// <param name="p_id">主键</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<基础Obj> GetByID(string p_id)
        {
            return EntityEx.GetByID<基础Obj>(p_id);
        }

        /// <summary>
        /// 根据主键获得实体对象(包含所有列值)，仅支持单主键，当启用实体缓存时：
        /// <para>1. 首先从缓存中获取，有则直接返回</para>
        /// <para>2. 无则查询数据库，并将查询结果添加到缓存以备下次使用</para>
        /// </summary>
        /// <param name="p_id">主键</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<基础Obj> GetByID(long p_id)
        {
            return EntityEx.GetByID<基础Obj>(p_id);
        }

        /// <summary>
        /// 根据主键或唯一索引列获得实体对象(包含所有列值)，仅支持单主键，当启用实体缓存时：
        /// <para>1. 首先从缓存中获取，有则直接返回</para>
        /// <para>2. 无则查询数据库，并将查询结果添加到缓存以备下次使用</para>
        /// </summary>
        /// <param name="p_keyName">主键或唯一索引列名</param>
        /// <param name="p_keyVal">键值</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<基础Obj> GetByKey(string p_keyName, string p_keyVal)
        {
            return EntityEx.GetByKey<基础Obj>(p_keyName, p_keyVal);
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
            return EntityEx.DelByID<基础Obj>(p_id, p_isNotify);
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
            return EntityEx.DelByID<基础Obj>(p_id, p_isNotify);
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
            return EntityEx.DelByKey<基础Obj>(p_keyName, p_keyVal);
        }

        /// <summary>
        /// 查询实体列表，每个实体包含所有列值，过滤条件null或空时返回所有实体
        /// </summary>
        /// <param name="p_filter">过滤串，where后面的部分，null或空返回所有实体</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回实体列表</returns>
        public static Task<Table<基础Obj>> Query(string p_filter = null, object p_params = null)
        {
            return EntityEx.Query<基础Obj>(p_filter, p_params);
        }

        /// <summary>
        /// 按页查询实体列表，每个实体包含所有列值
        /// </summary>
        /// <param name="p_starRow">起始行号：mysql中第一行为0行</param>
        /// <param name="p_pageSize">每页显示行数</param>
        /// <param name="p_filter">过滤串，where后面的部分</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回实体列表</returns>
        public static Task<Table<基础Obj>> Page(int p_starRow, int p_pageSize, string p_filter = null, object p_params = null)
        {
            return EntityEx.Page<基础Obj>(p_starRow, p_pageSize, p_filter, p_params);
        }

        /// <summary>
        /// 返回符合条件的第一个实体对象，每个实体包含所有列值，不存在时返回null
        /// </summary>
        /// <param name="p_filter">过滤串，where后面的部分，null或空返回所有中的第一行</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<基础Obj> First(string p_filter, object p_params = null)
        {
            return EntityEx.First<基础Obj>(p_filter, p_params);
        }

        /// <summary>
        /// 获取新ID
        /// </summary>
        /// <returns></returns>
        public static Task<long> NewID()
        {
            return EntityEx.GetNewID<基础Obj>();
        }

        /// <summary>
        /// 获取新序列值
        /// </summary>
        /// <param name="p_colName">字段名称，不可为空</param>
        /// <returns>新序列值</returns>
        public static Task<int> NewSeq(string p_colName)
        {
            return EntityEx.GetNewSeq<基础Obj>(p_colName);
        }
        #endregion
    }
}