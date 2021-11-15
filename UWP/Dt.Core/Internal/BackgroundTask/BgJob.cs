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
        /// <param name="p_stub">存根</param>
        /// <returns></returns>
        public static async Task Run(IStub p_stub)
        {
            //Toast("后台", DateTime.Now.ToString(), null);
            OpenStateDb();

            // android ios已传stub
            if (p_stub == null)
            {
                Type tp;
                string tpName = AtState.GetCookie(_stubType);
                if (!string.IsNullOrEmpty(tpName) && (tp = Type.GetType(tpName)) != null)
                    p_stub = Activator.CreateInstance(tp) as IStub;
            }

            if (p_stub != null && p_stub.BgTaskType != null)
            {
                var bgTask = Activator.CreateInstance(p_stub.BgTaskType) as BgTask;
                if (bgTask != null)
                {
                    if (Kit.Stub == null)
                    {
                        // 避免涉及UI
                        Kit.StopTrace = true;
                        Kit.Stub = p_stub;
                    }
                    await bgTask.Run();
                }
            }
        }

        static void OpenStateDb()
        {
            if (AtState.IsOpened)
                return;

#if WASM
            // .net5.0 只能引用 SQLite3Provider_sqlite3，DllImport("sqlite3")
            // 默认为 SQLite3Provider_e_sqlite3 引用时出错！
            SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_sqlite3());
#else
            // 初始化不同平台的包绑定！V2支持类型和属性的绑定
            // 内部调用 SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());
            SQLitePCL.Batteries_V2.Init();
#endif
            // 打开状态库
            AtState.OpenDbBackground();
        }
    }
}