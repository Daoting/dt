#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-02-24 创建
**************************************************************************/
#endregion

#region 命名空间
using System;
using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
#endregion

namespace Dt.Tasks
{
    /// <summary>
    /// 锁屏后定时启动后台任务，后台任务必须在*.winmd项目中，最小时间间隔15分钟
    /// </summary>
    public sealed class TimeTriggeredTask : IBackgroundTask
    {
        /// <summary>
        /// 后台处理入口
        /// </summary>
        /// <param name="taskInstance"></param>
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();
            try
            {
                Type tp = Type.GetType("Dt.Core.BgJob,Dt.Core");
                if (tp != null)
                {
                    var run = tp.GetMethod("Run", BindingFlags.Public | BindingFlags.Static);
                    var task = run.Invoke(null, new object[] { null }) as Task;
                    await task;
                }
            }
            catch { }
            deferral.Complete();
        }
    }
}
