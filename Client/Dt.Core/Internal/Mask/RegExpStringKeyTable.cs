#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2010-09-24 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
#endregion

namespace Dt.Core.Mask
{
    internal sealed class RegExpStringKeyTable
    {
        #region 成员变量
        readonly Dictionary<object, RegExpStringKey> inner = new Dictionary<object, RegExpStringKey>(new Comparer());
        #endregion

        #region 构造方法
        public RegExpStringKey this[string index]
        {
            get
            {
                RegExpStringKey key;
                this.inner.TryGetValue(index, out key);
                return key;
            }
        }
        #endregion

        #region 外部方法
        public void Add(RegExpStringKey key)
        {
            this.inner.Add(key, key);
        }
        #endregion

        class Comparer : IEqualityComparer<object>
        {
            #region 静态内容
            static bool IsEqual(RegExpStringKey item, RegExpStringKey key)
            {
                if (item._Length != key._Length)
                {
                    return false;
                }
                RegExpStringKey next = key;
                for (RegExpStringKey key3 = item; next._Next != null; key3 = key3._Next)
                {
                    if (key3._Symbol != next._Symbol)
                    {
                        return false;
                    }
                    next = next._Next;
                }
                return true;
            }

            static bool IsEqual(string str, RegExpStringKey key)
            {
                int length = str.Length;
                if (key._Length != length)
                {
                    return false;
                }
                for (RegExpStringKey key2 = key; key2._Next != null; key2 = key2._Next)
                {
                    length--;
                    if (str[length] != key2._Symbol)
                    {
                        return false;
                    }
                }
                return true;
            }
            #endregion
            
            #region 外部方法
            bool IEqualityComparer<object>.Equals(object x, object y)
            {
                RegExpStringKey item = x as RegExpStringKey;
                if (item == null)
                {
                    return IsEqual((string)x, (RegExpStringKey)y);
                }
                RegExpStringKey key = y as RegExpStringKey;
                if (key != null)
                {
                    return IsEqual(item, key);
                }
                return IsEqual((string)y, item);
            }

            int IEqualityComparer<object>.GetHashCode(object obj)
            {
                return obj.GetHashCode();
            }
            #endregion
        }
    }
}

