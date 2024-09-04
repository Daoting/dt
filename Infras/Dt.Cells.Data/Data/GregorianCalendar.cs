#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.Globalization;
#endregion

namespace Dt.Cells.Data
{
    internal class GregorianCalendar : CustomCalendar
    {
        public GregorianCalendar()
        {
            base.Calendar.ChangeCalendarSystem(CalendarIdentifiers.Gregorian);
        }
    }
}

