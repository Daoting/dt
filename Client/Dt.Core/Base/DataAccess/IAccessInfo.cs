#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-28 创建
******************************************************************************/
#endregion

#region 引用命名

#endregion

namespace Dt.Core
{
    /// <summary>
    /// 数据访问的描述信息
    /// </summary>
    public interface IAccessInfo
    {
        /// <summary>
        /// 数据访问的种类：通过服务访问、直连数据库访问、本地sqlite库访问
        /// </summary>
        AccessType Type { get; }

        /// <summary>
        /// <para>1. 通过服务访问时为'服务名' 或 '服务名+数据源键名'(实体)</para>
        /// <para>2. 直连数据库时为库的键名</para>
        /// <para>3. 本地sqlite库访问时为库名</para>
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 获取数据访问对象
        /// </summary>
        /// <returns></returns>
        IDataAccess GetDa();
    }

    /// <summary>
    /// 数据访问的种类：通过服务访问、直连数据库访问、本地sqlite库访问
    /// </summary>
    public enum AccessType
    {
        /// <summary>
        /// 通过服务进行数据访问
        /// </summary>
        Service,

        /// <summary>
        /// 直连数据库方式访问
        /// </summary>
        Database,

        /// <summary>
        /// 本地sqlite库访问
        /// </summary>
        Local
    }
}
