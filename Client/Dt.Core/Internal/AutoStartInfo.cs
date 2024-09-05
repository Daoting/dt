#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 自启动信息描述
    /// </summary>
    public class AutoStartInfo
    {
        /// <summary>
        /// 获取设置自启动的窗口类型
        /// </summary>
        public string WinType { get; set; }

        /// <summary>
        /// 获取设置初始参数
        /// </summary>
        public string Params { get; set; }

        /// <summary>
        /// 获取设置初始参数类型
        /// </summary>
        public string ParamsType { get; set; }

        /// <summary>
        /// 获取设置Win标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 获取设置图标名称
        /// </summary>
        public string Icon { get; set; }
    }
}
