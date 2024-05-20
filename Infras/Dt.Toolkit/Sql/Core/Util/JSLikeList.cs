using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Dt.Toolkit.Sql
{
    public class JSLikeList<T> : IEnumerable
    {
        private readonly List<T> tList;

        public JSLikeList(List<T> tList)
        {
            this.tList = tList ?? new List<T>();
        }

        public List<T> ToList()
        {
            return tList;
        }

        public JSLikeList<R> Map<R>(Func<T, R> mapper)
        {
            return new JSLikeList<R>(
                tList
                .Select(mapper.Invoke)
                .ToList());
        }

        public string Join(string delimiter)
        {
            return string.Join(delimiter, tList);
        }

        public JSLikeList<T> With(List<T> other)
        {
            return new JSLikeList<T>(
                tList
                .Concat(other)
                .ToList());
        }

        public string Join()
        {
            return Join(",");
        }

        public bool IsEmpty()
        {
            return tList == null || tList.Count == 0;
        }

        public T Get(int index)
        {
            if (index < 0 || tList.Count <= index)
            {
                return default;
            }

            return tList.ElementAt(index);
        }

        public IEnumerator GetEnumerator()
        {
            return tList.GetEnumerator();
        }

        public int Size()
        {
            return tList.Count;
        }
    }
}
