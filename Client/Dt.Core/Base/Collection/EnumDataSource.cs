#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2011-03-04 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 提供枚举类型的集合数据源，主要适用于ComboBox中绑定，两种用法：
    /// 1. EnumDataSource.FromType()返回数据源
    /// 2. new EnumDataSource() { EnumType = typeof(XX) }
    /// </summary>
    public class EnumDataSource : IEnumerable, INotifyCollectionChanged
    {
        Type _enumType;
        readonly IList<EnumMember> _viewModels = new List<EnumMember>();

        /// <summary>
        /// 集合变化事件
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// 返回EnumMemberViewModel的集合，可以用来绑定到ComboBox的ItemsSource
        /// </summary>
        /// <typeparam name="TEnum">枚举类型</typeparam>
        /// <returns>EnumMemberViewModel的可枚举集合</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static IEnumerable<EnumMember> FromType<TEnum>() where TEnum : struct
        {
            return FromType(typeof(TEnum));
        }

        /// <summary>
        /// 返回EnumMemberViewModel的集合，可以用来绑定到ComboBox的ItemsSource
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static IEnumerable<EnumMember> FromType(Type enumType)
        {
            if (!enumType.GetTypeInfo().IsEnum)
            {
                throw new ArgumentException("应为枚举类型！");
            }
            return FromTypeCore(enumType);
        }

        /// <summary>
        /// 获取设置枚举类型
        /// </summary>
        public Type EnumType
        {
            get { return _enumType; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                if (!value.GetTypeInfo().IsEnum)
                {
                    throw new ArgumentException("应为枚举类型！");
                }
                if (value != _enumType)
                {
                    _enumType = value;
                    Refresh();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            return _viewModels.GetEnumerator();
        }

        static IEnumerable<EnumMember> FromTypeCore(Type enumType)
        {
            foreach (FieldInfo info in from f in enumType.GetRuntimeFields()
                                       where f.IsLiteral
                                       select f)
            {
                yield return new EnumMember(info.GetValue(enumType), info.Name);
            }
        }

        void Refresh()
        {
            _viewModels.Clear();
            foreach (var item in FromType(EnumType))
            {
                _viewModels.Add(item);
            }
            RaiseCollectionChanged();
        }

        void RaiseCollectionChanged()
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }

    /// <summary>
    /// 单个枚举成员的信息
    /// </summary>
    public class EnumMember
    {
        readonly string _name;
        readonly object _value;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="name">The name.</param>
        public EnumMember(object value, string name)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (!value.GetType().GetTypeInfo().IsEnum)
            {
                throw new ArgumentException("应为枚举类型！");
            }
            _value = value;

            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("name不可为空！");
            }
            _name = name;
        }

        /// <summary>
        /// 获取当前枚举成员的名称
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// 获取当前枚举成员的值
        /// </summary>
        /// <value>The value.</value>
        public object Value
        {
            get { return _value; }
        }

        /// <summary>
        /// 返回名称
        /// </summary>
        /// <returns>
        /// 名称
        /// </returns>
        public override string ToString()
        {
            return _name;
        }
    }
}