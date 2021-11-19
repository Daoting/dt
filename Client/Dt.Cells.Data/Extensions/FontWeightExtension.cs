#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
using Windows.UI.Text;
#endregion

namespace Dt.Cells.Data
{
    internal static class FontWeightExtension
    {
        public static bool Equals(this FontWeight fw1, FontWeight fw2)
        {
            return (fw1.Weight == fw2.Weight);
        }
    }
}

