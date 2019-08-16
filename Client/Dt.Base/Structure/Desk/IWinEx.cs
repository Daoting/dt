#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-28 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 基础窗口的扩展方法
    /// </summary>
    public static class WinExt
    {
        /// <summary>
        /// 激活旧窗口或打开新窗口
        /// </summary>
        /// <param name="p_win">窗口对象</param>
        public static void Open(this IWin p_win)
        {
            if (p_win == null)
                AtKit.Throw("显示窗口不可为空！");

            if (!AtSys.IsPhoneUI && Desktop.Inst.ActiveWin(p_win))
            {
                Taskbar.Inst.ActiveTaskItem(p_win);
                return;
            }

            if (AtSys.IsPhoneUI)
            {
                p_win.NaviToHome();
            }
            else
            {
                Taskbar.LoadTaskItem(p_win);
                Desktop.Inst.ShowNewWin(p_win);
            }
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="p_win"></param>
        /// <returns></returns>
        public static async void Close(this IWin p_win)
        {
            if (!AtSys.IsPhoneUI)
            {
                IWin nextWin = Taskbar.Inst.GetNextActiveItem(p_win);
                if (await Desktop.Inst.CloseWin(p_win, nextWin))
                {
                    Taskbar.Inst.RemoveTaskItem(p_win);
                    if (nextWin != null)
                        Taskbar.Inst.ActiveTaskItem(nextWin);
                }
            }
        }
    }
}
