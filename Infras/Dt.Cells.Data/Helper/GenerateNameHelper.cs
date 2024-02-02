#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Cells.Data
{
    internal class GenerateNameHelper
    {
        public static string GenerteName(Worksheet sheet, GenerateType type)
        {
            if (sheet == null)
            {
                return null;
            }
            while (true)
            {
                for (int i = 1; i < 0x7fffffff; i++)
                {
                    if (type == GenerateType.Chart)
                    {
                        string name = "Chart" + ((int) i);
                        if (sheet.FindChart(name) == null)
                        {
                            return name;
                        }
                    }
                    else if (type == GenerateType.Picture)
                    {
                        string str2 = "Picture" + ((int) i);
                        if (sheet.FindPicture(str2) == null)
                        {
                            return str2;
                        }
                    }
                    else if (type == GenerateType.FloatingObject)
                    {
                        string str3 = "FloatingObject" + ((int) i);
                        if (sheet.FindFloatingObject(str3) == null)
                        {
                            return str3;
                        }
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }
        }
    }
}

