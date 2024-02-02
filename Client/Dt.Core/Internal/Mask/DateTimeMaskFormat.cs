#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2010-09-24 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Core.Mask
{
    /// <summary>
    /// 
    /// </summary>
    public class DateTimeMaskFormatElement_AmPm : DateTimeMaskFormatElementEditable
    {
        #region 外部方法
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mask">掩码表达式</param>
        /// <param name="dateTimeFormatInfo">格式信息</param>
        public DateTimeMaskFormatElement_AmPm(string mask, DateTimeFormatInfo dateTimeFormatInfo)
            : base(mask, dateTimeFormatInfo, DateTimePart.Time)
        { }
        #endregion

        #region 重写方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="editedDateTime"></param>
        /// <returns></returns>
        public override DateTime ApplyElement(int result, DateTime editedDateTime)
        {
            return editedDateTime.AddHours((double)(12 * (result - (editedDateTime.Hour / 12))));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="editedDateTime"></param>
        /// <returns></returns>
        public override DateTimeElementEditor CreateElementEditor(DateTime editedDateTime)
        {
            return new DateTimeElementEditorAmPm(base.Mask, editedDateTime.Hour / 12, base._DateTimeFormatInfo.AMDesignator, base._DateTimeFormatInfo.PMDesignator);
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class DateTimeElementEditorAmPm : DateTimeElementEditor
    {
        #region 静态内容
        /// <summary>
        /// 
        /// </summary>
        public const int AMValue = 0;
        
        /// <summary>
        /// 
        /// </summary>
        public const int PMValue = 1;
        #endregion

        #region 成员变量
        /// <summary>
        /// 
        /// </summary>
        protected readonly string _AMDesignator;

        string fMask;

        int fResult;

        /// <summary>
        /// 
        /// </summary>
        protected readonly string _PMDesignator;
        #endregion

        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="mask">掩码表达式</param>
        /// <param name="initialValue"></param>
        /// <param name="am"></param>
        /// <param name="pm"></param>
        public DateTimeElementEditorAmPm(string mask, int initialValue, string am, string pm)
        {
            this.fResult = initialValue;
            this.fMask = mask;
            this._AMDesignator = am;
            this._PMDesignator = pm;
            if (this._AMDesignator == this._PMDesignator)
            {
                this._AMDesignator = "<" + this._AMDesignator;
                this._PMDesignator = ">" + this._PMDesignator;
            }
        }
        #endregion

        #region 属性
        /// <summary>
        /// 显示内容
        /// </summary>
        public override string DisplayText
        {
            get
            {
                string str = (this.fResult == 0) ? this._AMDesignator : this._PMDesignator;
                if ((this.fMask.Length == 1) && (str.Length > 1))
                {
                    str = str.Substring(0, 1);
                }
                return str;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool FinalOperatorInsert
        {
            get { return true; }
        }
        #endregion

        #region 重写方法
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Delete()
        {
            if ((this._AMDesignator.Length != 0) && (this._PMDesignator.Length == 0))
            {
                if (this.fResult == 1)
                {
                    return false;
                }
                this.fResult = 1;
            }
            else
            {
                if (this.fResult == 0)
                {
                    return false;
                }
                this.fResult = 0;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetResult()
        {
            return this.fResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inserted"></param>
        /// <returns></returns>
        public override bool Insert(string inserted)
        {
            if (inserted.Length == 0)
            {
                return this.Delete();
            }
            if ((this._AMDesignator.Length > 0) && (this._PMDesignator.Length > 0))
            {
                if (char.ToUpper(this._AMDesignator[0]) == char.ToUpper(inserted[0]))
                {
                    this.fResult = 0;
                    return true;
                }
                if (char.ToUpper(this._PMDesignator[0]) == char.ToUpper(inserted[0]))
                {
                    this.fResult = 1;
                    return true;
                }
            }
            else
            {
                if (this._AMDesignator.Length > 0)
                {
                    this.fResult = 0;
                    return true;
                }
                if (this._PMDesignator.Length > 0)
                {
                    this.fResult = 1;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool SpinDown()
        {
            return this.SpinUp();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool SpinUp()
        {
            this.fResult = 1 - this.fResult;
            return true;
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class DateTimeMaskFormatElement_d : DateTimeNumericRangeFormatElementEditable
    {
        #region 成员变量
        DateTimeMaskFormatElementContext _context;
        #endregion

        #region 外部方法
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mask">掩码表达式</param>
        /// <param name="dateTimeFormatInfo">格式信息</param>
        /// <param name="context"></param>
        public DateTimeMaskFormatElement_d(string mask, DateTimeFormatInfo dateTimeFormatInfo, DateTimeMaskFormatElementContext context)
            : base(mask, dateTimeFormatInfo, DateTimePart.Date)
        {
            this._context = context;
        }
        #endregion 

        #region 重写方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="editedDateTime"></param>
        /// <returns></returns>
        public override DateTime ApplyElement(int result, DateTime editedDateTime)
        {
            DateTime time = editedDateTime.AddDays((double)(result - editedDateTime.Day));
            if (time.Day == result)
            {
                return time;
            }
            if (((result == 0x1d) && (editedDateTime.Month == 2)) && this._context._MonthProcessed)
            {
                time = editedDateTime;
                while (!DateTime.IsLeapYear(time.Year))
                {
                    time = time.AddYears(1);
                }
                return time.AddDays((double)(result - time.Day));
            }
            return editedDateTime.AddMonths(1 - editedDateTime.Month).AddDays((double)(result - editedDateTime.Day));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="editedDateTime"></param>
        /// <returns></returns>
        public override DateTimeElementEditor CreateElementEditor(DateTime editedDateTime)
        {
            int year = this._context._YearProcessed ? editedDateTime.Year : 0x7d4;
            int month = this._context._MonthProcessed ? editedDateTime.Month : 1;
            return new DateTimeNumericRangeElementEditor(editedDateTime.Day, 1, DateTime.DaysInMonth(year, month), (base.Mask.Length == 1) ? 1 : 2, 2);
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class DateTimeMaskFormatElement_h12 : DateTimeNumericRangeFormatElementEditable
    {
        #region 外部方法
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mask">掩码表达式</param>
        /// <param name="dateTimeFormatInfo">格式信息</param>
        public DateTimeMaskFormatElement_h12(string mask, DateTimeFormatInfo dateTimeFormatInfo)
            : base(mask, dateTimeFormatInfo, DateTimePart.Time)
        { }
        #endregion

        #region 重写方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="editedDateTime"></param>
        /// <returns></returns>
        public override DateTime ApplyElement(int result, DateTime editedDateTime)
        {
            int num = (result == 12) ? 0 : result;
            return editedDateTime.AddHours((double)(num - (editedDateTime.Hour % 12)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="editedDateTime"></param>
        /// <returns></returns>
        public override DateTimeElementEditor CreateElementEditor(DateTime editedDateTime)
        {
            int initialValue = editedDateTime.Hour % 12;
            if (initialValue == 0)
            {
                initialValue = 12;
            }
            return new DateTimeNumericRangeElementEditor(initialValue, 1, 12, (base.Mask.Length == 1) ? 1 : 2, 2);
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class DateTimeMaskFormatElement_H24 : DateTimeNumericRangeFormatElementEditable
    {
        #region 外部方法
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mask">掩码表达式</param>
        /// <param name="dateTimeFormatInfo">格式信息</param>
        public DateTimeMaskFormatElement_H24(string mask, DateTimeFormatInfo dateTimeFormatInfo)
            : base(mask, dateTimeFormatInfo, DateTimePart.Time)
        { }
        #endregion

        #region 重写方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="editedDateTime"></param>
        /// <returns></returns>
        public override DateTime ApplyElement(int result, DateTime editedDateTime)
        {
            return editedDateTime.AddHours((double)(result - editedDateTime.Hour));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="editedDateTime"></param>
        /// <returns></returns>
        public override DateTimeElementEditor CreateElementEditor(DateTime editedDateTime)
        {
            return new DateTimeNumericRangeElementEditor(editedDateTime.Hour, 0, 0x17, (base.Mask.Length == 1) ? 1 : 2, 2);
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class DateTimeMaskFormatElement_Millisecond : DateTimeNumericRangeFormatElementEditable
    {
        #region 属性
        int EditorCoeff
        {
            get
            {
                switch (base.Mask.Length)
                {
                    case 1:
                        return 100;

                    case 2:
                        return 10;
                }
                return 1;
            }
        }

        int EditorMaxValue
        {
            get
            {
                switch (base.Mask.Length)
                {
                    case 1:
                        return 9;

                    case 2:
                        return 0x63;
                }
                return 0x3e7;
            }
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mask">掩码表达式</param>
        /// <param name="dateTimeFormatInfo">格式信息</param>
        public DateTimeMaskFormatElement_Millisecond(string mask, DateTimeFormatInfo dateTimeFormatInfo)
            : base(mask, dateTimeFormatInfo, DateTimePart.Time)
        { }
        #endregion

        #region 重写方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="editedDateTime"></param>
        /// <returns></returns>
        public override DateTime ApplyElement(int result, DateTime editedDateTime)
        {
            return editedDateTime.AddMilliseconds((double)((result * this.EditorCoeff) - editedDateTime.Millisecond));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="editedDateTime"></param>
        /// <returns></returns>
        public override DateTimeElementEditor CreateElementEditor(DateTime editedDateTime)
        {
            return new DateTimeNumericRangeElementEditor(editedDateTime.Millisecond / this.EditorCoeff, 0, this.EditorMaxValue, base.Mask.Length, base.Mask.Length);
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class DateTimeMaskFormatElement_Min : DateTimeNumericRangeFormatElementEditable
    {
        #region 外部方法
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mask">掩码表达式</param>
        /// <param name="dateTimeFormatInfo">格式信息</param>
        public DateTimeMaskFormatElement_Min(string mask, DateTimeFormatInfo dateTimeFormatInfo)
            : base(mask, dateTimeFormatInfo, DateTimePart.Time)
        { }
        #endregion

        #region 重写方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="editedDateTime"></param>
        /// <returns></returns>
        public override DateTime ApplyElement(int result, DateTime editedDateTime)
        {
            return editedDateTime.AddMinutes((double)(result - editedDateTime.Minute));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="editedDateTime"></param>
        /// <returns></returns>
        public override DateTimeElementEditor CreateElementEditor(DateTime editedDateTime)
        {
            return new DateTimeNumericRangeElementEditor(editedDateTime.Minute, 0, 0x3b, (base.Mask.Length == 1) ? 1 : 2, 2);
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class DateTimeMaskFormatElement_Month : DateTimeNumericRangeFormatElementEditable
    {
        #region 成员变量
        string[] _MonthNames;
        readonly DateTimeMaskFormatElementContext _context;
        DateTimeMaskFormatGlobalContext _monthNamesDeterminator;
        #endregion

        #region 属性
        /// <summary>
        /// 
        /// </summary>
        protected string[] MonthNames
        {
            get
            {
                if (this._monthNamesDeterminator != null)
                {
                    if (base.Mask.Length == 3)
                    {
                        if (this._monthNamesDeterminator.Value._DayProcessed)
                        {
                            this._MonthNames = base._DateTimeFormatInfo.AbbreviatedMonthGenitiveNames;
                        }
                        else
                        {
                            this._MonthNames = base._DateTimeFormatInfo.AbbreviatedMonthNames;
                        }
                    }
                    else if (base.Mask.Length > 3)
                    {
                        if (this._monthNamesDeterminator.Value._DayProcessed)
                        {
                            this._MonthNames = base._DateTimeFormatInfo.MonthGenitiveNames;
                        }
                        else
                        {
                            this._MonthNames = base._DateTimeFormatInfo.MonthNames;
                        }
                    }
                    else
                    {
                        this._MonthNames = null;
                    }
                    this._monthNamesDeterminator = null;
                }
                return this._MonthNames;
            }
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mask">掩码表达式</param>
        /// <param name="dateTimeFormatInfo">格式信息</param>
        /// <param name="globalContext"></param>
        public DateTimeMaskFormatElement_Month(string mask, DateTimeFormatInfo dateTimeFormatInfo, DateTimeMaskFormatGlobalContext globalContext)
            : base(mask, dateTimeFormatInfo, DateTimePart.Date)
        {
            this._monthNamesDeterminator = globalContext;
            this._context = globalContext.Value;
        }
        #endregion

        #region 重写方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="editedDateTime"></param>
        /// <returns></returns>
        public override DateTime ApplyElement(int result, DateTime editedDateTime)
        {
            DateTime time = editedDateTime.AddMonths(result - editedDateTime.Month);
            if ((((result != 2) || (editedDateTime.Day != 0x1d)) || ((editedDateTime.Day == time.Day) || !this._context._DayProcessed)) || this._context._YearProcessed)
            {
                return time;
            }
            time = editedDateTime;
            while (!DateTime.IsLeapYear(time.Year))
            {
                time = time.AddYears(1);
            }
            return time.AddMonths(result - time.Month);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="editedDateTime"></param>
        /// <returns></returns>
        public override DateTimeElementEditor CreateElementEditor(DateTime editedDateTime)
        {
            return new DateTimeMonthElementEditor(editedDateTime.Month, (base.Mask.Length == 2) ? 2 : 1, this.MonthNames);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formattedDateTime"></param>
        /// <returns></returns>
        public override string Format(DateTime formattedDateTime)
        {
            if (this.MonthNames != null)
            {
                int month = formattedDateTime.Month;
                return this.MonthNames[month - 1];
            }
            return base.Format(formattedDateTime);
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class DateTimeMonthElementEditor : DateTimeNumericRangeElementEditor
    {
        #region 成员变量
        readonly string[] _monthsKeys;
        #endregion

        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public override string DisplayText
        {
            get
            {
                if (((this._monthsKeys != null) && (base.CurrentValue >= 1)) && (base.CurrentValue <= 12))
                {
                    return this._monthsKeys[base.CurrentValue - 1];
                }
                return base.DisplayText;
            }
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="initialValue"></param>
        /// <param name="minDigits"></param>
        /// <param name="monthsNames"></param>
        public DateTimeMonthElementEditor(int initialValue, int minDigits, string[] monthsNames)
            : base(initialValue, 1, 12, minDigits, 2)
        {
            this._monthsKeys = monthsNames;
            if (((this._monthsKeys != null) && (this._monthsKeys[12] != null)) && (this._monthsKeys[12].Length > 0))
            {
                this._monthsKeys = null;
            }
        }
        #endregion

        #region 重写方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inserted"></param>
        /// <returns></returns>
        public override bool Insert(string inserted)
        {
            if (this._monthsKeys != null)
            {
                string str = inserted.ToLower();
                if (inserted.Length > 0)
                {
                    for (int i = 0; i < 11; i++)
                    {
                        int newValue = ((base.CurrentValue + i) % 12) + 1;
                        if (this._monthsKeys[newValue - 1].ToLower() == str)
                        {
                            base.SetUntouchedValue(newValue);
                            return true;
                        }
                    }
                }
                if (str.Length == 1)
                {
                    for (int j = 0; j < 11; j++)
                    {
                        int num4 = ((base.CurrentValue + j) % 12) + 1;
                        string str2 = this._monthsKeys[num4 - 1];
                        if (str2.ToLower().Substring(0, 1) == str)
                        {
                            base.SetUntouchedValue(num4);
                            return true;
                        }
                    }
                }
            }
            return base.Insert(inserted);
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class DateTimeMaskFormatElement_s : DateTimeNumericRangeFormatElementEditable
    {
        #region 外部方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="dateTimeFormatInfo"></param>
        public DateTimeMaskFormatElement_s(string mask, DateTimeFormatInfo dateTimeFormatInfo)
            : base(mask, dateTimeFormatInfo, DateTimePart.Time)
        { }
        #endregion

        #region 重写方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="editedDateTime"></param>
        /// <returns></returns>
        public override DateTime ApplyElement(int result, DateTime editedDateTime)
        {
            return editedDateTime.AddSeconds((double)(result - editedDateTime.Second));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="editedDateTime"></param>
        /// <returns></returns>
        public override DateTimeElementEditor CreateElementEditor(DateTime editedDateTime)
        {
            return new DateTimeNumericRangeElementEditor(editedDateTime.Second, 0, 0x3b, (base.Mask.Length == 1) ? 1 : 2, 2);
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class DateTimeMaskFormatElement_Year : DateTimeNumericRangeFormatElementEditable
    {
        #region 外部方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="dateTimeFormatInfo"></param>
        public DateTimeMaskFormatElement_Year(string mask, DateTimeFormatInfo dateTimeFormatInfo)
            : base(mask, dateTimeFormatInfo, DateTimePart.Date)
        { }
        #endregion

        #region 重写方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="editedDateTime"></param>
        /// <returns></returns>
        public override DateTime ApplyElement(int result, DateTime editedDateTime)
        {
            return editedDateTime.AddYears(result - editedDateTime.Year);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="editedDateTime"></param>
        /// <returns></returns>
        public override DateTimeElementEditor CreateElementEditor(DateTime editedDateTime)
        {
            return new DateTimeYearElementEditor(editedDateTime.Year, base.Mask.Length, base._DateTimeFormatInfo);
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class DateTimeYearElementEditor : DateTimeNumericRangeElementEditor
    {
        #region 静态内容
        static int GetYearShift(Calendar calendar)
        {
            return (calendar.GetYear(new DateTime(0x7d1, 1, 1)) - 0x7d1);
        }
        #endregion

        #region 成员变量
        readonly DateTimeFormatInfo _dateTimeFormatInfo;
        readonly int _maskLength;
        #endregion 
                 
        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public override int MinDigits
        {
            get
            {
                if (this._maskLength != 4)
                {
                    return base.MinDigits;
                }
                if (base._digitsEntered < 1)
                {
                    return 1;
                }
                if (base._digitsEntered > 4)
                {
                    return 4;
                }
                return base._digitsEntered;
            }
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="initialYear"></param>
        /// <param name="maskLength"></param>
        /// <param name="dateTimeFormatInfo"></param>
        public DateTimeYearElementEditor(int initialYear, int maskLength, DateTimeFormatInfo dateTimeFormatInfo)
            : base((maskLength <= 4) ? 0 : DateTime.MinValue.Year, (maskLength < 4) ? 0x63 : DateTime.MaxValue.Year, (maskLength == 2) ? 2 : 1, (maskLength > 3) ? 4 : 2)
        {
            this._maskLength = maskLength;
            this._dateTimeFormatInfo = dateTimeFormatInfo;
            int num = initialYear + GetYearShift(dateTimeFormatInfo.Calendar);
            num = Math.Max(Math.Min(num, 0x270f), 1);
            num = (maskLength < 4) ? (num % 100) : num;
            base.SetUntouchedValue(num);
        }
        #endregion

        #region 重写方法
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetResult()
        {
            int result = base.GetResult();
            if (((result < DateTime.MinValue.Year) || ((this._maskLength < 4) && (result <= 0x63))) || (((this._maskLength == 4) && (result <= 0x63)) && (base._digitsEntered <= 2)))
            {
                try
                {
                    result = this._dateTimeFormatInfo.Calendar.ToFourDigitYear(result);
                }
                catch
                {
                }
            }
            result -= GetYearShift(this._dateTimeFormatInfo.Calendar);
            return Math.Max(Math.Min(result, this._dateTimeFormatInfo.Calendar.MaxSupportedDateTime.Year), this._dateTimeFormatInfo.Calendar.MinSupportedDateTime.Year);
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class DateTimeMaskFormatElementLiteral : DateTimeMaskFormatElement
    {
        #region 成员变量
        /// <summary>
        /// 
        /// </summary>
        protected string _fLiteral;
        #endregion
        
        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public string Literal
        {
            get { return this._fLiteral; }
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="dateTimeFormatInfo"></param>
        public DateTimeMaskFormatElementLiteral(string mask, DateTimeFormatInfo dateTimeFormatInfo)
            : base(dateTimeFormatInfo, DateTimePart.None)
        {
            this._fLiteral = mask;
        }
        #endregion

        #region 重写方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="formattedDateTime"></param>
        /// <returns></returns>
        public override string Format(DateTime formattedDateTime)
        {
            return this.Literal;
        }
        #endregion
    }
    
    /// <summary>
    /// 
    /// </summary>
    public class DateTimeNumericRangeElementEditor : DateTimeElementEditor
    {
        #region 成员变量
        /// <summary>
        /// 
        /// </summary>
        protected int _digitsEntered;
        int _fCurrentValue;
        int _fMaxDigits;
        int _fMaxValue;
        int _fMinDigits;
        int _fMinValue;
        #endregion 

        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public int CurrentValue
        {
            get { return this._fCurrentValue; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override string DisplayText
        {
            get { return this.CurrentValue.ToString("d" + this.MinDigits.ToString("d2", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture); }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool FinalOperatorInsert
        {
            get
            {
                if (!this.Touched)
                {
                    return false;
                }
                return (((this.CurrentValue > 0) && ((this.CurrentValue * 10) > this.MaxValue)) || ((this.MaxDigits > 0) && (this._digitsEntered >= this.MaxDigits)));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int MaxDigits
        {
            get { return this._fMaxDigits; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int MaxValue
        {
            get { return this._fMaxValue; }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual int MinDigits
        {
            get { return this._fMinDigits; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int MinValue
        {
            get { return this._fMinValue; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool Touched
        {
            get { return (this._digitsEntered > 0); }
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <param name="minDigits"></param>
        /// <param name="maxDigits"></param>
        public DateTimeNumericRangeElementEditor(int minValue, int maxValue, int minDigits, int maxDigits)
        {
            this._fMinValue = minValue;
            this._fMaxValue = maxValue;
            this._fMinDigits = minDigits;
            this._fMaxDigits = maxDigits;
            this.SetUntouchedValue(minValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="initialValue"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <param name="minDigits"></param>
        /// <param name="maxDigits"></param>
        public DateTimeNumericRangeElementEditor(int initialValue, int minValue, int maxValue, int minDigits, int maxDigits)
            : this(minValue, maxValue, minDigits, maxDigits)
        {
            this.SetUntouchedValue(initialValue);
        }
        #endregion

        #region 重写方法
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Delete()
        {
            if ((this.CurrentValue == this.MinValue) && !this.Touched)
            {
                return false;
            }
            this.SetUntouchedValue(this.MinValue);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetResult()
        {
            if ((this.CurrentValue >= this.MinValue) && (this.CurrentValue <= this.MaxValue))
            {
                return this.CurrentValue;
            }
            return this.MinValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inserted"></param>
        /// <returns></returns>
        public override bool Insert(string inserted)
        {
            if (inserted.Length == 0)
            {
                return this.Delete();
            }
            string s = this.Touched ? this._fCurrentValue.ToString("d", CultureInfo.InvariantCulture) : string.Empty;
            int num = 0;
            foreach (char ch in inserted)
            {
                if ((ch >= '0') && (ch <= '9'))
                {
                    s = s + ch;
                    num++;
                }
                else
                {
                    return false;
                }
            }
            if (s.Length > this.MaxDigits)
            {
                s = s.Substring(s.Length - this.MaxDigits);
            }
            while (int.Parse(s, CultureInfo.InvariantCulture) > this.MaxValue)
            {
                s = s.Substring(1);
            }
            int num2 = int.Parse(s, CultureInfo.InvariantCulture);
            this._fCurrentValue = num2;
            this._digitsEntered += num;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool SpinDown()
        {
            int newValue = this.CurrentValue - 1;
            if (newValue < this.MinValue)
            {
                newValue = this.MaxValue;
            }
            if ((newValue == this.CurrentValue) && !this.Touched)
            {
                return false;
            }
            this.SetUntouchedValue(newValue);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool SpinUp()
        {
            int newValue = this.CurrentValue + 1;
            if (newValue > this.MaxValue)
            {
                newValue = this.MinValue;
            }
            if ((newValue == this.CurrentValue) && !this.Touched)
            {
                return false;
            }
            this.SetUntouchedValue(newValue);
            return true;
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="newValue"></param>
        protected void SetUntouchedValue(int newValue)
        {
            this._fCurrentValue = newValue;
            this._digitsEntered = 0;
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class DateTimeNumericRangeFormatElementEditable : DateTimeMaskFormatElementEditable
    {
        #region 内部方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="dateTimeFormatInfo"></param>
        /// <param name="dateTimePart"></param>
        protected DateTimeNumericRangeFormatElementEditable(string mask, DateTimeFormatInfo dateTimeFormatInfo, DateTimePart dateTimePart)
            : base(mask, dateTimeFormatInfo, dateTimePart)
        { }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class DateTimeMaskFormatElement
    {
        #region 成员变量
        /// <summary>
        /// 
        /// </summary>
        protected readonly DateTimeFormatInfo _DateTimeFormatInfo;

        /// <summary>
        /// 
        /// </summary>
        public readonly DateTimePart _DateTimePart;
        #endregion 

        #region 构造方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTimeFormatInfo"></param>
        /// <param name="dateTimePart"></param>
        protected DateTimeMaskFormatElement(System.Globalization.DateTimeFormatInfo dateTimeFormatInfo, DateTimePart dateTimePart)
        {
            this._DateTimeFormatInfo = dateTimeFormatInfo;
            this._DateTimePart = dateTimePart;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public bool Editable
        {
            get { return (this is DateTimeMaskFormatElementEditable); }
        }
        #endregion

        #region 抽象方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="formattedDateTime"></param>
        /// <returns></returns>
        public abstract string Format(DateTime formattedDateTime);
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class DateTimeMaskFormatElementNonEditable : DateTimeMaskFormatElement
    {
        #region 成员变量
        string _fMask;
        #endregion

        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public string Mask
        {
            get { return this._fMask; }
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="dateTimeFormatInfo"></param>
        /// <param name="dateTimePart"></param>
        public DateTimeMaskFormatElementNonEditable(string mask, DateTimeFormatInfo dateTimeFormatInfo, DateTimePart dateTimePart)
            : base(dateTimeFormatInfo, dateTimePart)
        {
            this._fMask = mask;
        }
        #endregion

        #region 重写方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="formattedDateTime"></param>
        /// <returns></returns>
        public override string Format(DateTime formattedDateTime)
        {
            return formattedDateTime.ToString((this._fMask.Length == 1) ? ('%' + this._fMask) : this._fMask, base._DateTimeFormatInfo);
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class DateTimeMaskFormatElementEditable : DateTimeMaskFormatElementNonEditable
    {
        #region 内部方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="dateTimeFormatInfo"></param>
        /// <param name="dateTimePart"></param>
        protected DateTimeMaskFormatElementEditable(string mask, DateTimeFormatInfo dateTimeFormatInfo, DateTimePart dateTimePart)
            : base(mask, dateTimeFormatInfo, dateTimePart)
        { }
        #endregion

        #region 抽象方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="editedDateTime"></param>
        /// <returns></returns>
        public abstract DateTime ApplyElement(int result, DateTime editedDateTime);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="editedDateTime"></param>
        /// <returns></returns>
        public abstract DateTimeElementEditor CreateElementEditor(DateTime editedDateTime);
        #endregion
    }


    /// <summary>
    /// 
    /// </summary>
    public class DateTimeMaskFormatInfo : IEnumerable<DateTimeMaskFormatElement>, IEnumerable
    {
        #region 静态内容
        static string ExpandFormat(string format, DateTimeFormatInfo info)
        {
            if ((format == null) || (format.Length == 0))
            {
                format = "G";
            }
            if (format.Length == 1)
            {
                switch (format[0])
                {
                    case 'D':
                        return info.LongDatePattern;

                    case 'F':
                        return info.FullDateTimePattern;

                    case 'G':
                        return (info.ShortDatePattern + ' ' + info.LongTimePattern);

                    case 'M':
                    case 'm':
                        return info.MonthDayPattern;

                    case 'R':
                    case 'r':
                        return info.RFC1123Pattern;

                    case 'T':
                        return info.LongTimePattern;

                    case 's':
                        return info.SortableDateTimePattern;

                    case 't':
                        return info.ShortTimePattern;

                    case 'u':
                        return info.UniversalSortableDateTimePattern;

                    case 'y':
                    case 'Y':
                        return info.YearMonthPattern;

                    case 'd':
                        return info.ShortDatePattern;

                    case 'f':
                        return (info.LongDatePattern + ' ' + info.ShortTimePattern);

                    case 'g':
                        return (info.ShortDatePattern + ' ' + info.ShortTimePattern);
                }
            }
            if ((format.Length == 2) && (format[0] == '%'))
            {
                format = format.Substring(1);
            }
            return format;
        }

        static int GetGroupLength(string mask)
        {
            for (int i = 1; i < mask.Length; i++)
            {
                if (mask[i] != mask[0])
                {
                    return i;
                }
            }
            return mask.Length;
        }

        static IList<DateTimeMaskFormatElement> ParseFormatString(string mask, DateTimeFormatInfo dateTimeFormatInfo)
        {
            List<DateTimeMaskFormatElement> list = new List<DateTimeMaskFormatElement>();
            string str = mask;
            DateTimeMaskFormatGlobalContext globalContext = new DateTimeMaskFormatGlobalContext();
            while (str.Length > 0)
            {
                DateTimeMaskFormatElement element;
                int groupLength = GetGroupLength(str);
                switch (str[0])
                {
                    case '/':
                        groupLength = 1;
                        element = new DateTimeMaskFormatElementNonEditable(str.Substring(0, 1), dateTimeFormatInfo, DateTimePart.Date);
                        goto Label_02C8;

                    case ':':
                        groupLength = 1;
                        element = new DateTimeMaskFormatElementNonEditable(str.Substring(0, 1), dateTimeFormatInfo, DateTimePart.Time);
                        goto Label_02C8;

                    case 'H':
                        element = new DateTimeMaskFormatElement_H24(str.Substring(0, groupLength), dateTimeFormatInfo);
                        goto Label_02C8;

                    case '"':
                    case '\'':
                        {
                            int index = str.IndexOf(str[0], 1);
                            if (index <= 0)
                            {
                                throw new ArgumentException("Incorrect mask: closing quote expected");
                            }
                            element = new DateTimeMaskFormatElementLiteral(str.Substring(1, index - 1), dateTimeFormatInfo);
                            groupLength = index + 1;
                            goto Label_02C8;
                        }
                    case 'd':
                        if (groupLength <= 2)
                        {
                            break;
                        }
                        element = new DateTimeMaskFormatElementNonEditable(str.Substring(0, groupLength), dateTimeFormatInfo, DateTimePart.Date);
                        goto Label_02C8;

                    case 'f':
                        if (groupLength > 7)
                        {
                            groupLength = 7;
                        }
                        if (groupLength > 3)
                        {
                            element = new DateTimeMaskFormatElementNonEditable(str.Substring(0, groupLength), dateTimeFormatInfo, DateTimePart.Time);
                        }
                        else
                        {
                            element = new DateTimeMaskFormatElement_Millisecond(str.Substring(0, groupLength), dateTimeFormatInfo);
                        }
                        goto Label_02C8;

                    case 'g':
                        element = new DateTimeMaskFormatElementNonEditable(str.Substring(0, groupLength), dateTimeFormatInfo, DateTimePart.Date);
                        goto Label_02C8;

                    case 'h':
                        element = new DateTimeMaskFormatElement_h12(str.Substring(0, groupLength), dateTimeFormatInfo);
                        goto Label_02C8;

                    case '\\':
                        if (str.Length < 2)
                        {
                            throw new ArgumentException(@"Incorrect mask: character expected after '\'");
                        }
                        element = new DateTimeMaskFormatElementLiteral(str.Substring(1, 1), dateTimeFormatInfo);
                        groupLength = 2;
                        goto Label_02C8;

                    case 'M':
                        if (groupLength > 4)
                        {
                            groupLength = 4;
                        }
                        element = new DateTimeMaskFormatElement_Month(str.Substring(0, groupLength), dateTimeFormatInfo, globalContext);
                        globalContext.Value._MonthProcessed = true;
                        goto Label_02C8;

                    case 's':
                        element = new DateTimeMaskFormatElement_s(str.Substring(0, groupLength), dateTimeFormatInfo);
                        goto Label_02C8;

                    case 't':
                        element = new DateTimeMaskFormatElement_AmPm(str.Substring(0, groupLength), dateTimeFormatInfo);
                        goto Label_02C8;

                    case 'm':
                        element = new DateTimeMaskFormatElement_Min(str.Substring(0, groupLength), dateTimeFormatInfo);
                        goto Label_02C8;

                    case 'y':
                        element = new DateTimeMaskFormatElement_Year(str.Substring(0, groupLength), dateTimeFormatInfo);
                        globalContext.Value._YearProcessed = true;
                        goto Label_02C8;

                    case 'z':
                        element = new DateTimeMaskFormatElementNonEditable(str.Substring(0, groupLength), dateTimeFormatInfo, DateTimePart.Both);
                        goto Label_02C8;

                    default:
                        groupLength = 1;
                        element = new DateTimeMaskFormatElementLiteral(str.Substring(0, 1), dateTimeFormatInfo);
                        goto Label_02C8;
                }
                element = new DateTimeMaskFormatElement_d(str.Substring(0, groupLength), dateTimeFormatInfo, globalContext.Value);
                globalContext.Value._DayProcessed = true;
            Label_02C8:
                list.Add(element);
                str = str.Substring(groupLength);
            }
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="patchedMask"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public static string RemoveTimePartFromTheMask(string patchedMask, IFormatProvider formatProvider)
        {
            if (formatProvider == null)
            {
                formatProvider = DateTimeFormatInfo.CurrentInfo;
            }
            DateTimeFormatInfo format = (DateTimeFormatInfo)formatProvider.GetFormat(typeof(DateTimeFormatInfo));
            if (format == null)
            {
                format = DateTimeFormatInfo.CurrentInfo;
            }
            string mask = ExpandFormat(patchedMask, format);
            IList<DateTimeMaskFormatElement> list = ParseFormatString(mask, format);
            string str2 = string.Empty;
            foreach (DateTimeMaskFormatElement element in list)
            {
                if (element is DateTimeMaskFormatElementLiteral)
                {
                    string literal = ((DateTimeMaskFormatElementLiteral)element).Literal;
                    switch (literal)
                    {
                        case " ":
                        case ",":
                        case ";":
                        case ".":
                        case "-":
                            {
                                str2 = str2 + literal;
                                continue;
                            }
                    }
                    if ((literal.Length > 1) && (literal.IndexOf('\'') < 0))
                    {
                        str2 = str2 + "'" + literal + "'";
                    }
                    else if ((literal.Length > 1) && (literal.IndexOf('"') < 0))
                    {
                        str2 = str2 + "\"" + literal + "\"";
                    }
                    else
                    {
                        foreach (char ch in literal)
                        {
                            str2 = str2 + @"\" + ch;
                        }
                    }
                    continue;
                }
                DateTimeMaskFormatElementNonEditable editable = (DateTimeMaskFormatElementNonEditable)element;
                switch (editable.Mask[0])
                {
                    case 'f':
                    case 'h':
                    case 'H':
                    case ':':
                    case 's':
                    case 't':
                    case 'z':
                    case 'm':
                        {
                            continue;
                        }
                }
                str2 = str2 + editable.Mask;
            }
            str2 = str2.Trim();
            if (str2 == mask)
            {
                return patchedMask;
            }
            return str2;
        }
        #endregion

        #region 成员变量
        /// <summary>
        /// 
        /// </summary>
        protected readonly IList<DateTimeMaskFormatElement>_innerList;
        #endregion

        #region 构造方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="dateTimeFormatInfo"></param>
        public DateTimeMaskFormatInfo(string mask, DateTimeFormatInfo dateTimeFormatInfo)
        {
            string str = ExpandFormat(mask, dateTimeFormatInfo);
            this._innerList = ParseFormatString(str, dateTimeFormatInfo);
        }
        #endregion

        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get { return this._innerList.Count; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTimePart DateTimeParts
        {
            get
            {
                DateTimePart none = DateTimePart.None;
                foreach (DateTimeMaskFormatElement element in (IEnumerable<DateTimeMaskFormatElement>)this)
                {
                    none |= element._DateTimePart;
                }
                return none;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DateTimeMaskFormatElement this[int index]
        {
            get { return this._innerList[index]; }
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="formatted"></param>
        /// <returns></returns>
        public string Format(DateTime formatted)
        {
            return this.Format(formatted, 0, this.Count - 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formatted"></param>
        /// <param name="startFormatIndex"></param>
        /// <param name="endFormatIndex"></param>
        /// <returns></returns>
        public string Format(DateTime formatted, int startFormatIndex, int endFormatIndex)
        {
            string str = string.Empty;
            for (int i = startFormatIndex; i <= endFormatIndex; i++)
            {
                str = str + this[i].Format(formatted);
            }
            return str;
        }

        IEnumerator<DateTimeMaskFormatElement> IEnumerable<DateTimeMaskFormatElement>.GetEnumerator()
        {
            return this._innerList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._innerList.GetEnumerator();
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DateTimeMaskFormatElementContext
    {
        #region 成员变量
        /// <summary>
        /// 
        /// </summary>
        public bool  _YearProcessed;

        /// <summary>
        /// 
        /// </summary>
        public bool _MonthProcessed;

        /// <summary>
        /// 
        /// </summary>
        public bool _DayProcessed;
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class DateTimeMaskFormatGlobalContext
    {
        #region 外部方法
        /// <summary>
        /// 
        /// </summary>
        public DateTimeMaskFormatElementContext Value = new DateTimeMaskFormatElementContext();
        #endregion
    }

    /// <summary>
    /// 显示格式
    /// </summary>
    [Flags]
    public enum DateTimePart
    {
        #region 枚举成员
        /// <summary>
        /// 
        /// </summary>
        None,

        /// <summary>
        /// 
        /// </summary>
        Date,

        /// <summary>
        /// 
        /// </summary>
        Time,

        /// <summary>
        /// 
        /// </summary>
        Both
        #endregion
    }
}

