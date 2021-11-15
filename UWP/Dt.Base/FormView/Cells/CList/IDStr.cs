#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-03-05 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.UI.Xaml.Data;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 描述显示和实际值类
    /// </summary>
    public class IDStr
    {
        /// <summary>
        /// 获取设置实际值
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 获取设置要显示的字符串
        /// </summary>
        public string Str { get; set; }

        public override string ToString()
        {
            return Str;
        }
    }
}