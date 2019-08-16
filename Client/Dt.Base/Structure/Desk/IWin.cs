#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-28 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 基础窗口的接口定义
    /// </summary>
    public interface IWin : IPhonePage
    {
        /// <summary>
        /// 获取设置标题文字
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// 获取设置图标
        /// </summary>
        Icons Icon { get; set; }

        /// <summary>
        /// 获取设置初始参数
        /// </summary>
        string Params { get; }

        /// <summary>
        /// 导航到窗口主页
        /// </summary>
        void NaviToHome();
    }
}
