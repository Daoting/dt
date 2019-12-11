#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-12-10 创建
******************************************************************************/
#endregion

#region 引用命名
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 启动内容
    /// </summary>
    public class StartupInfo
    {
        /// <summary>
        /// 设置提示信息
        /// </summary>
        /// <param name="p_msg"></param>
        public void SetMessage(string p_msg)
        {
            if (SysVisual.RootContent is TextBlock tb && p_msg != null)
                tb.Text = p_msg;
        }

        /// <summary>
        /// 加载根元素UI
        /// </summary>
        /// <param name="p_content"></param>
        public void LoadRootUI(UIElement p_content)
        {
            SysVisual.RootContent = p_content;
        }
    }
}
