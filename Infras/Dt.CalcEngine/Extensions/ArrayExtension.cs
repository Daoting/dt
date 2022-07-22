#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.CalcEngine
{
    internal static class ArrayExtension
    {
        public static Array Clone(this Array This)
        {
            if (!object.ReferenceEquals(This, null))
            {
                Type objA = DetectArrayItemType(This);
                if (!object.ReferenceEquals(objA, null))
                {
                    Array destinationArray = Array.CreateInstance(objA, new int[] { This.Length });
                    Array.ConstrainedCopy(This, 0, destinationArray, 0, This.Length);
                    return destinationArray;
                }
            }
            return null;
        }

        private static Type DetectArrayItemType(Array array)
        {
            IEnumerator enumerator = array.GetEnumerator();
            while (enumerator.MoveNext())
            {
                object current = enumerator.Current;
                if (!object.ReferenceEquals(current, null))
                {
                    return current.GetType();
                }
            }
            return null;
        }
    }
}

