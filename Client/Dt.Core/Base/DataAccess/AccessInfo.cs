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
    public class AccessInfo
    {
        public AccessInfo() { }

        public AccessInfo(AccessType p_type, string p_name)
        {
            Type = p_type;
            Name = p_name;
        }

        /// <summary>
        /// 数据访问的种类，远程数据访问 或 本地sqlite库访问
        /// </summary>
        public virtual AccessType Type { get; }

        /// <summary>
        /// 远程数据访问时为服务名，本地sqlite库访问时为库名
        /// </summary>
        public virtual string Name { get; }

        /// <summary>
        /// 根据描述信息获取数据访问对象
        /// </summary>
        /// <returns></returns>
        public IDataAccess GetDataAccess()
        {
            return Type == AccessType.Remote ? GetRemoteAccess(Name) : GetSqliteAccess(Name);
        }

        #region 静态内容
        static readonly Dictionary<string, IDataAccess> _remotes = new Dictionary<string, IDataAccess>(StringComparer.OrdinalIgnoreCase);
        static readonly Dictionary<string, IDataAccess> _sqlites = new Dictionary<string, IDataAccess>(StringComparer.OrdinalIgnoreCase);

        static IDataAccess GetRemoteAccess(string p_name)
        {
            if (_remotes.TryGetValue(p_name, out var m))
                return m;

            var da = new RemoteAccess(p_name);
            _remotes[p_name] = da;
            return da;
        }

        internal static IDataAccess GetSqliteAccess(string p_name)
        {
            if (_sqlites.TryGetValue(p_name, out var m))
                return m;

            var da = new SqliteAccess(p_name);
            _sqlites[p_name] = da;
            return da;
        }
        #endregion

        #region 判断相同
        /// <summary>
        /// 判断两实体是否相同
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null || obj is not AccessInfo ai)
                return false;

            // 相同实例
            if (ReferenceEquals(this, obj))
                return true;

            return Type == ai.Type && string.Equals(Name, ai.Name, StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(AccessInfo left, AccessInfo right)
        {
            if (Equals(left, null))
                return Equals(right, null);
            return left.Equals(right);
        }

        public static bool operator !=(AccessInfo left, AccessInfo right)
        {
            return !(left == right);
        }
        #endregion
    }

    /// <summary>
    /// 数据访问的种类，远程数据访问 或 本地sqlite库访问
    /// </summary>
    public enum AccessType
    {
        /// <summary>
        /// 远程数据访问
        /// </summary>
        Remote,

        /// <summary>
        /// 本地sqlite库访问
        /// </summary>
        Local
    }
}
