using System;
using System.Collections.Generic;
using System.Linq;

namespace Dt.Toolkit.Sql
{
    class Utils
    {
        public static List<T> NullToEmpty<T>(List<T> list)
        {
            return list ?? new List<T>();
        }

        public static R FirstNotnull<R>(params Func<R>[] suppliers) where R : class
        {
            return suppliers.FirstOrDefault(supplier => supplier() != null)?.Invoke();
        }

        public static string Repeat(string s, int n)
        {
            return string.Concat(Enumerable.Repeat(s, n));
        }

        public static List<T> Concat<T>(List<T> l1, List<T> l2)
        {
            return l1.Concat(l2).ToList();
        }

        public static JSLikeList<string> SortByLengthDesc(JSLikeList<string> strings)
        {
            return new JSLikeList<string>(
                strings.ToList().OrderByDescending(s => s.Length).ToList());
        }
    }
}
