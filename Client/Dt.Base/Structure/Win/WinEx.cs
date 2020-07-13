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
        public static void Open(this Win p_win)
        {
            Throw.IfNull(p_win, "显示窗口不可为空！");

            if (!AtSys.IsPhoneUI && Desktop.Inst.ActiveWin(p_win))
                return;

            if (AtSys.IsPhoneUI)
                p_win.NaviToHome();
            else
                Desktop.Inst.ShowNewWin(p_win);
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="p_win"></param>
        /// <returns></returns>
        public static async void Close(this Win p_win)
        {
            if (!AtSys.IsPhoneUI)
            {
                await Desktop.Inst.CloseWin(p_win);
            }
        }
    }
}
