#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Globalization;
#endregion

namespace Dt.Charts
{
    internal class Keywords
    {
        Dictionary<string, object> _words = new Dictionary<string, object>();

        internal string Replace(string s)
        {
            string str = s;
            int index = 0;
            foreach (string str2 in _words.Keys)
            {
                if (str.IndexOf(str2, (StringComparison)StringComparison.Ordinal) != -1)
                {
                    index++;
                }
            }
            if (index > 0)
            {
                object[] objArray = new object[index];
                index = 0;
                foreach (string str3 in _words.Keys)
                {
                    if (str.IndexOf(str3, (StringComparison)StringComparison.Ordinal) != -1)
                    {
                        objArray[index] = _words[str3];
                        str = str.Replace(str3, ((int)index).ToString((IFormatProvider)CultureInfo.CurrentCulture));
                        index++;
                    }
                }
                try
                {
                    str = string.Format((IFormatProvider)CultureInfo.CurrentCulture, str, objArray);
                }
                catch
                {
                    throw new FormatException(C1Localizer.GetString("Bad format of keyword(s)") + ": " + s);
                }
            }
            return str;
        }

        internal object this[string name]
        {
            get { return _words[name]; }
            set { _words[name] = value; }
        }
    }
}

