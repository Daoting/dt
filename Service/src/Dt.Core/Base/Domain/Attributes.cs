#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-10-17 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 实体类映射表标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TblAttribute : Attribute
    {
        string _svc;

        public TblAttribute(string p_tblName, string p_svc = null)
        {
            Name = p_tblName;
            _svc = p_svc;
        }

        /// <summary>
        /// 实体类对应的表名
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 实体类所使用的服务名
        /// </summary>
        public string Svc
        {
            get
            {
                if (_svc == null)
                {
                    if (string.IsNullOrEmpty(Name))
                        throw new Exception("实体类未设置对应的表名和服务名！");

                    int index = Name.IndexOf("_");
                    // 缺省使用cm服务
                    _svc = index == -1 ? "cm" : Name.Substring(0, index).ToLower();
                }
                return _svc;
            }
        }
    }

    /// <summary>
    /// 子实体类标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ChildXAttribute : Attribute
    {
        public ChildXAttribute(string p_parentID)
        {
            ParentID = p_parentID;
        }

        /// <summary>
        /// 父表外键字段名
        /// </summary>
        public string ParentID { get; }
    }

    /// <summary>
    /// 实体缓存配置标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CacheAttribute : Attribute
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="p_keys">除主键以外其它作为缓存键的属性名(字段名)，该列通常为唯一索引</param>
        public CacheAttribute(params string[] p_keys)
        {
            Keys = p_keys;
        }

        /// <summary>
        /// 除主键以外其它作为缓存键的属性名，该列通常为唯一索引，确保该列数据不重复
        /// </summary>
        public string[] Keys { get; }
    }
}
