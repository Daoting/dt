#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-26 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 监视窗口输出
    /// </summary>
    public partial class Kit
    {
        #region 成员变量
        const int _maxTrace = 30;

        /// <summary>
        /// 创建Trace数据源表
        /// </summary>
        static readonly Table _traceList = new Table
        {
            { "type", typeof(TraceOutType) },
            { "title" },
            { "content" },
            { "time" },
            { "service" },
        };

        static bool _stopTrace;
        #endregion

        /// <summary>
        /// 向监视窗口输出信息
        /// </summary>
        /// <param name="p_title">输出内容标题</param>
        /// <param name="p_content">内容</param>
        public static void Trace(string p_title, string p_content = null)
        {
            Row row = _traceList.NewRow(new
            {
                type = TraceOutType.Normal,
                title = p_title,
                content = (p_content == null) ? string.Empty : p_content,
                time = DateTime.Now.ToString("HH:mm:ss")
            });
            Trace(row);
        }

        /// <summary>
        /// 向监视窗口输出信息
        /// </summary>
        /// <param name="p_type">输出类别</param>
        /// <param name="p_title">标题</param>
        /// <param name="p_content">内容</param>
        /// <param name="p_serviceName">服务名称</param>
        public static void Trace(TraceOutType p_type, string p_title, string p_content, string p_serviceName = null)
        {
            // 只控制系统级不输出
            if (_stopTrace)
                return;

            Row row = _traceList.NewRow(new
            {
                type = p_type,
                title = p_title,
                content = (p_content == null) ? string.Empty : p_content,
                time = DateTime.Now.ToString("HH:mm:ss"),
                service = p_serviceName
            });
            Trace(row);
        }

        /// <summary>
        /// 向监视窗口输出信息
        /// </summary>
        /// <param name="p_row">数据行</param>
        static void Trace(Row p_row)
        {
            RunAsync(() =>
            {
                using (_traceList.Defer())
                {
                    _traceList.Add(p_row);
                    if (_traceList.Count > _maxTrace)
                    {
                        // 确保输出行数不超过给定的最大行数
                        _traceList.RemoveAt(0);
                    }
                }
            });
        }

        /// <summary>
        /// 是否停止监视输出，内部用
        /// </summary>
        internal static bool StopTrace
        {
            get { return _stopTrace; }
            set { _stopTrace = value; }
        }

        /// <summary>
        /// 获取监视信息列表
        /// </summary>
        internal static Table TraceList
        {
            get { return _traceList; }
        }
    }
}