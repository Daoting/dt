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
using System.Text;
#endregion

namespace Dt.Charts
{
    internal class ConverterHelper : Dictionary<string, object>
    {
        const char delim = ';';
        const char pair_delim = '=';

        public string ConvertToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<string, object> pair in this)
            {
                if (builder.Length > 0)
                {
                    builder.Append(';');
                }
                builder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}={1}", new object[] { pair.Key, pair.Value });
            }
            return builder.ToString();
        }

        public static ConverterHelper ParseString(string s)
        {
            ConverterHelper helper = null;
            if (s != null)
            {
                string[] strArray = s.Split(new char[] { ';' });
                if ((strArray == null) || (strArray.Length <= 0))
                {
                    return helper;
                }
                helper = new ConverterHelper();
                int length = strArray.Length;
                for (int i = 0; i < length; i++)
                {
                    string[] strArray2 = strArray[i].Split(new char[] { '=' });
                    if ((strArray2 != null) && (strArray2.Length == 2))
                    {
                        helper.Add(strArray2[0], strArray2[1]);
                    }
                }
            }
            return helper;
        }
    }
}

