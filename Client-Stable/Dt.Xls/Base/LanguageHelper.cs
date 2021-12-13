#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Globalization;
#endregion

namespace Dt.Xls
{
    internal class LanguageHelper
    {
        public static Language GetCurrentRuntimeLanguage()
        {
            if (CultureInfo.CurrentCulture.Name == "ja-JP")
            {
                return Language.Ja_jp;
            }
            if (CultureInfo.CurrentCulture.Name == "zh-CN")
            {
                return Language.Zh_cn;
            }
            return Language.En_us;
        }
    }
}

