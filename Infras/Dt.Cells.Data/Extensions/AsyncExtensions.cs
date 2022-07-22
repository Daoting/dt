#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Foundation;
#endregion

namespace Dt.Cells.Data
{
    internal static class AsyncExtensions
    {
        public static T GetResultSynchronously<T>(this IAsyncOperation<T> op)
        {
            if (op == null)
            {
                throw new ArgumentNullException();
            }
            if (op.Status == AsyncStatus.Completed)
            {
                return op.GetResults();
            }
            Task<T> task = WindowsRuntimeSystemExtensions.AsTask<T>(op);
            task.Wait();
            return task.Result;
        }
    }
}

