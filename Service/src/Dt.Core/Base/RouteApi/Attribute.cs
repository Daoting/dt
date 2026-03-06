#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2026-03-04 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 路由处理标志
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RouteAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_path">路由路径</param>
        /// <param name="p_dbKey">数据源键名，空时为当前服务的默认数据源</param>
        public RouteAttribute(string p_path, string p_dbKey = null)
        {
            Path = p_path;
            DbKey = p_dbKey;
        }

        /// <summary>
        /// 路由路径
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// 数据源键名，空时为当前服务的默认数据源
        /// </summary>
        public string DbKey { get; }
    }
    
}
