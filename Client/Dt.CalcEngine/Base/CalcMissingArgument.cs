#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.CalcEngine
{
    /// <summary>
    /// Represents the <see cref="T:Dt.CalcEngine.CalcMissingArgument" /> class.
    /// </summary>
    public class CalcMissingArgument
    {
        private static readonly CalcMissingArgument _instance = new CalcMissingArgument();

        private CalcMissingArgument()
        {
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static CalcMissingArgument Instance
        {
            get
            {
                return _instance;
            }
        }
    }
}

