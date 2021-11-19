#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2010-09-24 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Runtime.InteropServices;
#endregion

namespace Dt.Core.Mask
{
    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RegExpBracketTransitionRange
    {
        #region 成员变量
        /// <summary>
        /// 
        /// </summary>
        public readonly char _From;

        /// <summary>
        /// 
        /// </summary>
        public readonly char _To;
        #endregion

        #region 外部方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public RegExpBracketTransitionRange(char from, char to)
        {
            this._From = from;
            this._To = to;
        }
        #endregion
    }
}

