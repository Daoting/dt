#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.InteropServices;
#endregion

namespace Dt.CalcEngine
{
    /// <summary>
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct GoalSeekData
    {
        public double xmin;
        public double xmax;
        public double precision;
        public bool havexpos;
        public double xpos;
        public double ypos;
        public bool havexneg;
        public double xneg;
        public double yneg;
        public double root;
    }
}

