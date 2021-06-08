#if IOS
#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-29 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System.Threading.Tasks;
using UIKit;
#endregion

namespace Dt.Base
{
    public sealed class CaptureController : UIImagePickerController
    {
        internal CaptureController(CaptureDelegate mpDelegate)
        {
            base.Delegate = mpDelegate;
        }

        public Task<FileData> GetResultAsync()
        { 
            return ((CaptureDelegate)Delegate).Task;
        }
            
        bool disposed;
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing && !disposed)
            {
                disposed = true;
                InvokeOnMainThread(() =>
                {
                    try
                    {
                        Delegate?.Dispose();
                        Delegate = null;
                    }
                    catch { }
                });
            }
        }
    }
}
#endif
