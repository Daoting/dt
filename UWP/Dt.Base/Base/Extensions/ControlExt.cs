#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// Control扩展类
    /// </summary>
    public static class ControlExt
    {
        /// <summary>
        /// 连续状态迁移
        /// </summary>
        /// <param name="source"></param>
        /// <param name="p_stateNames"></param>
        public static void GoToState(this Control source, params string[] p_stateNames)
        {
            if (p_stateNames != null)
            {
                foreach (string str in p_stateNames)
                {
                    if (VisualStateManager.GoToState(source, str, true))
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 连续状态迁移
        /// </summary>
        /// <param name="source"></param>
        /// <param name="p_useTransitions"></param>
        /// <param name="p_stateNames"></param>
        public static void GoToState(this Control source, bool p_useTransitions, params string[] p_stateNames)
        {
            if (p_stateNames != null)
            {
                foreach (string str in p_stateNames)
                {
                    if (VisualStateManager.GoToState(source, str, p_useTransitions))
                    {
                        break;
                    }
                }
            }
        }
    }
}
