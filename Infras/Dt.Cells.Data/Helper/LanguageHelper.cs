#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Globalization;
#endregion

namespace Dt.Cells.Data
{
    internal class LanguageHelper
    {
        public static Language GetCurrentRuntimeLanguage()
        {
            switch (CultureInfo.CurrentCulture.Name)
            {
                case "ja-JP":
                    return Language.Ja_jp;

                case "zh-CN":
                    return Language.Zh_cn;
            }
            return Language.En_us;
        }
    }
}

