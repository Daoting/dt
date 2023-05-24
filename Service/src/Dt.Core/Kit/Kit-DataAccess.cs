#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-05-17 创建
******************************************************************************/
#endregion

#region 引用命名
using Serilog;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 当前整个http请求期间的上下文
    /// </summary>
    public partial class Kit
    {
        /// <summary>
        /// 获取当前http请求上下文的数据访问对象，无http请求上下文时返回新对象！AutoClose为true
        /// <para>如：本地定时器调用或RabbitMQ消息产生的调用无http请求上下文</para>
        /// </summary>
        public static IDataAccess DataAccess
        {
            get
            {
                if (HttpContext != null)
                    return ((Bag)HttpContext.Items[ContextItemName]).DataAccess;
                return NewDataAccess();
            }
        }

        /// <summary>
        /// 获取新的数据访问对象
        /// </summary>
        /// <param name="p_dbKey">数据源键名，在global.json可根据键名获取连接串，null时使用默认数据源</param>
        /// <returns>返回数据访问对象</returns>
        public static IDataAccess NewDataAccess(string p_dbKey = null)
        {
            // 默认数据源
            if (string.IsNullOrEmpty(p_dbKey) || _dbInfo.Key.Equals(p_dbKey, StringComparison.OrdinalIgnoreCase))
                return _dbInfo.NewDataAccess();

            return DbInfo.ReadConfig(p_dbKey).NewDataAccess();
        }

        /// <summary>
        /// 默认数据库描述信息
        /// </summary>
        static DbInfo _dbInfo;

        static void InitDbInfo()
        {
            _dbInfo = DbInfo.ReadConfig(_config["DbConn"]);
        }
    }
}
