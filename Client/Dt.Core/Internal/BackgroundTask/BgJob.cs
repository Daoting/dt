#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Rpc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 后台作业，公共部分
    /// </summary>
    public static partial class BgJob
    {
        const string _stubType = "StubType";
        static StreamWriter _logWriter;

        /// <summary>
        /// 后台任务运行入口
        /// 此方法不可使用任何UI和外部变量，保证可独立运行！！！
        /// </summary>
        /// <returns></returns>
        public static async Task Run()
        {
            WriteLog("进入Run");

            // 打开状态库
            AtState.OpenDbBackground();

            // 前端在运行或后台资源未释放，Stub实例存在
            Stub stub = Stub.Inst;
            if (stub == null)
            {
                // 因后台任务独立运行，存根类型需要从State库获取！
                string tpName = AtState.GetCookie(_stubType);
                if (!string.IsNullOrEmpty(tpName))
                {
                    Type tp = Type.GetType(tpName);
                    if (tp != null)
                    {
                        stub = Activator.CreateInstance(tp) as Stub;

                        // 前端没运行，完全后台启动，避免涉及UI！
                        stub.LogSetting.TraceEnabled = false;
                        Serilogger.Init();
                    }
                }
            }

            if (stub == null)
            {
                Unregister();
                return;
            }

            var bgJob = stub.SvcProvider.GetService<IBackgroundJob>();
            if (bgJob != null)
            {
                string msg = "启动";
                try
                {
                    await bgJob.Run();
                    msg += " -> 结束";
                }
                catch (Exception ex)
                {
                    msg += $" -> 运行异常\r\n{ex.Message}";
                }
                WriteLog(msg);
            }
            else
            {
                Unregister();
                WriteLog("无处理内容，已注销！");
            }
        }

        static void WriteLog(string p_msg)
        {
            Debug.WriteLine(p_msg);

            if (_logWriter == null)
            {
                string logFileName = Path.Combine(ApplicationData.Current.LocalFolder.Path, ".doc", "BgJob.log");
                FileInfo fi = new FileInfo(logFileName);
                if (fi.Exists && fi.Length > 1024 * 1024)
                {
                    // 大于1MB，新文件
                    _logWriter = new StreamWriter(fi.Open(FileMode.Create, FileAccess.Write, FileShare.ReadWrite));
                }
                else
                {
                    // 定位文件末尾
                    var fs = fi.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
                    fs.Seek(0, SeekOrigin.End);
                    _logWriter = new StreamWriter(fs);
                }
            }
            _logWriter.WriteLine(DateTime.Now.ToString("MM-dd HH:mm:ss ") + p_msg);
            _logWriter.Flush();
        }
    }
}