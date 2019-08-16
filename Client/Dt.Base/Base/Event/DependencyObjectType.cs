#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2011-03-04 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Reflection;
using Windows.UI.Xaml;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 依赖类型声明的管理类
    /// </summary>
    internal class DependencyObjectType
    {
        DependencyObjectType _baseDType;
        
        /// <summary>
        /// 构造方法，禁止外部实例化
        /// </summary>
        DependencyObjectType()
        {
        }

        /// <summary>
        /// 用索引作为当前对象的哈希码
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.ID;
        }

        /// <summary>
        /// 判断指定的依赖对象和当前依赖类型声明中的是否相同
        /// </summary>
        /// <param name="dependencyObject">指定的依赖对象</param>
        /// <returns></returns>
        public bool IsInstanceOfType(DependencyObject dependencyObject)
        {
            if (dependencyObject != null)
            {
                DependencyObjectType dependencyObjectType = Attached.GetDependencyObjectType(dependencyObject);
                do
                {
                    if (dependencyObjectType.ID == this.ID)
                    {
                        return true;
                    }
                    dependencyObjectType = dependencyObjectType._baseDType;
                }
                while (dependencyObjectType != null);
            }
            return false;
        }

        /// <summary>
        /// 是否为指定的依赖类型的子类型
        /// </summary>
        /// <param name="dependencyObjectType">指定的依赖类型声明</param>
        /// <returns></returns>
        public bool IsSubclassOf(DependencyObjectType dependencyObjectType)
        {
            if (dependencyObjectType != null)
            {
                for (DependencyObjectType type = this._baseDType; type != null; type = type._baseDType)
                {
                    if (type.ID == dependencyObjectType.ID)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 父类型声明
        /// </summary>
        public DependencyObjectType BaseType
        {
            get { return _baseDType; }
        }

        /// <summary>
        /// 类型描述ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 类型名称
        /// </summary>
        public string Name
        {
            get { return this.SystemType.Name; }
        }

        /// <summary>
        /// 对应的系统类型声明
        /// </summary>
        public Type SystemType { get; set; }

        #region 静态内容
        /// <summary>
        /// 计数器
        /// </summary>
        static int _dependencyTypeCount;

        /// <summary>
        /// 实现整个系统内部依赖对象类型的字典式管理
        /// </summary>
        static Dictionary<Type, DependencyObjectType> _dependencyTypeFromCLRType = new Dictionary<Type, DependencyObjectType>();
        static object _lockObject = new object();

        /// <summary>
        /// 获取指定系统类型对应的依赖类型声明
        /// </summary>
        /// <param name="systemType"></param>
        /// <returns></returns>
        internal static DependencyObjectType FromSystemType(Type systemType)
        {
            lock (_lockObject)
            {
                return FromSystemTypeRecursive(systemType);
            }
        }

        /// <summary>
        /// 返回给定的系统类型声明的依赖类型声明
        /// 从字典中取出或构造依赖类型声明
        /// </summary>
        /// <param name="systemType"></param>
        /// <returns></returns>
        static DependencyObjectType FromSystemTypeRecursive(Type systemType)
        {
            DependencyObjectType type = null;
            if (_dependencyTypeFromCLRType.ContainsKey(systemType))
            {
                type = _dependencyTypeFromCLRType[systemType];
            }
            if (type == null)
            {
                type = new DependencyObjectType();
                type.SystemType = systemType;
                _dependencyTypeFromCLRType[systemType] = type;
                if (systemType != typeof(DependencyObject))
                {
                    // 将递归注册父类型
                    type._baseDType = FromSystemTypeRecursive(systemType.GetTypeInfo().BaseType);
                }
                type.ID = _dependencyTypeCount++;
            }
            return type;
        }

        #endregion
    }
}
