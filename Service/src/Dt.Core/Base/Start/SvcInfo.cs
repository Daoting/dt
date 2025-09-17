#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2025-09-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.Extensions.Configuration;
using System.Collections.ObjectModel;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 服务描述类
    /// </summary>
    public class SvcInfo
    {
        public SvcInfo(string p_svcName, Stub p_stub)
        {
            SvcName = p_svcName;
            Stub = p_stub;
        }

        /// <summary>
        /// 服务名称，小写
        /// </summary>
        public string SvcName { get; }
        
        /// <summary>
        /// 服务存根
        /// </summary>
        public Stub Stub { get; }

        /// <summary>
        /// 当前服务的默认数据库描述信息
        /// </summary>
        public DbAccessInfo DbInfo { get; set; }

        /// <summary>
        /// 是否为空服务
        /// </summary>
        public bool IsEmptySvc => Stub is DefaultStub;
    }
    
    /// <summary>
    /// 服务列表
    /// </summary>
    public class SvcList : KeyedCollection<string, SvcInfo>
    {
        /// <summary>
        /// 构造方法，键比较时忽略大小写
        /// </summary>
        public SvcList()
            : base(StringComparer.OrdinalIgnoreCase)
        { }
        
        protected override string GetKeyForItem(SvcInfo item)
        {
            return item.SvcName;
        }
    }
}