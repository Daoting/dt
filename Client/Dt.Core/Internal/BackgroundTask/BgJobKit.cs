#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-12-31 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Rpc;
using System.Diagnostics;
using Windows.Storage;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 后台任务相关工具方法
    /// </summary>
    public static class BgJobKit
    {
        /// <summary>
        /// 未启用 Serilogger，使用该方法在 .doc/BgJob.log 记录日志
        /// </summary>
        /// <param name="p_msg"></param>
        public static void Log(string p_msg)
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
        
        /// <summary>
        /// 调用服务API
        /// </summary>
        /// <typeparam name="T">结果对象的类型</typeparam>
        /// <param name="p_serviceName">服务名称</param>
        /// <param name="p_methodName">方法名</param>
        /// <param name="p_params">参数列表</param>
        /// <returns>返回远程调用结果</returns>
        public static Task<T> Rpc<T>(string p_serviceName, string p_methodName, params object[] p_params)
        {
            return new UnaryRpc(
                p_serviceName,
                p_methodName,
                p_params
            ).Call<T>();
        }

        /// <summary>
        /// 显示系统通知，iOS只有app在后台或关闭时才显示！其他平台始终显示
        /// </summary>
        /// <param name="p_title">标题</param>
        /// <param name="p_content">内容</param>
        /// <param name="p_startInfo">点击通知的启动参数</param>
        public static void Toast(string p_title, string p_content, AutoStartInfo p_startInfo = null)
        {
#if WIN || ANDROID || IOS
            BgJob.Toast(p_title, p_content, p_startInfo);
#endif
        }

        static StreamWriter _logWriter;
    }
}