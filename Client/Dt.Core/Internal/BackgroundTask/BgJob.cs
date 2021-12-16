#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 后台作业，公共部分
    /// </summary>
    public static partial class BgJob
    {
        const string _stubType = "StubType";

        /// <summary>
        /// 后台任务运行入口
        /// 此方法不可使用任何UI和外部变量，保证可独立运行！！！
        /// </summary>
        /// <returns></returns>
        public static async Task Run()
        {
            // 打开状态库
            AtState.OpenDbBackground();

            // 因后台任务独立运行，存根类型需要从State库获取！
            IStub stub = null;
            string tpName = AtState.GetCookie(_stubType);
            if (!string.IsNullOrEmpty(tpName))
            {
                Type tp = Type.GetType(tpName);
                if (tp != null)
                    stub = Activator.CreateInstance(tp) as IStub;
            }

            if (stub != null && stub.BgTaskType != null)
            {
                var bgTask = Activator.CreateInstance(stub.BgTaskType) as BgTask;
                if (bgTask != null)
                {
                    if (Kit.Stub == null)
                    {
                        // 避免涉及UI
                        Kit.StopTrace = true;
                        Kit.Stub = stub;
                    }
                    await bgTask.Run();
                }
            }
        }
    }
}