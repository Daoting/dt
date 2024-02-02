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
    /// 数据访问代理的描述信息
    /// </summary>
    public class AgentInfo
    {
        /// <summary>
        /// 数据访问的种类：通过服务访问、直连数据库访问、本地sqlite库访问
        /// </summary>
        public virtual AccessType Type { get; }

        /// <summary>
        /// <para>1. 通过服务访问时为服务名</para>
        /// <para>2. 直连数据库时为库的键名</para>
        /// <para>3. 本地sqlite库访问时为库名</para>
        /// </summary>
        public virtual string Name { get; }

        /// <summary>
        /// 根据描述信息获取数据访问信息
        /// </summary>
        /// <returns></returns>
        public IAccessInfo GetAccessInfo()
        {
            return At.GetAccessInfo(Type, Name);
        }
    }
}
