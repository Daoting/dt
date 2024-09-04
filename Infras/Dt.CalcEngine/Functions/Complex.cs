#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Globalization;
using System.Text;
#endregion

namespace Dt.CalcEngine.Functions
{
    internal class Complex
    {
        private double imag;
        private double real;

        /// <summary>
        /// Constructs are complex number.
        /// </summary>
        public Complex(double real, double imag)
        {
            this.real = real;
            this.imag = imag;
        }

        /// <summary>
        /// Returns the complex number represented by the specified string.
        /// </summary>
        public static Complex Parse(string s)
        {
            double real = 0.0;
            double imag = 0.0;
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            int length = 0;
            int num4 = 0;
            int num5 = 0;
            if (s == null)
            {
                throw new ArgumentNullException();
            }
            if (s.Length == 0)
            {
                throw new FormatException();
            }
            if ((num5 < s.Length) && ((s[num5] == '+') || (s[num5] == '-')))
            {
                num5++;
            }
            while ((num5 < s.Length) && char.IsDigit(s[num5]))
            {
                num5++;
                flag = true;
            }
            if ((num5 < s.Length) && (s[num5] == '.'))
            {
                num5++;
            }
            while ((num5 < s.Length) && char.IsDigit(s[num5]))
            {
                num5++;
                flag = true;
            }
            if ((num5 < s.Length) && ((s[num5] == 'E') || (s[num5] == 'e')))
            {
                num5++;
                flag = false;
                if ((num5 < s.Length) && ((s[num5] == '+') || (s[num5] == '-')))
                {
                    num5++;
                }
                while ((num5 < s.Length) && char.IsDigit(s[num5]))
                {
                    num5++;
                    flag = true;
                }
            }
            if ((num5 < s.Length) && ((s[num5] == '+') || (s[num5] == '-')))
            {
                length = num5;
                num5++;
                while ((num5 < s.Length) && char.IsDigit(s[num5]))
                {
                    num5++;
                    flag2 = true;
                }
                if ((num5 < s.Length) && (s[num5] == '.'))
                {
                    num5++;
                }
                while ((num5 < s.Length) && char.IsDigit(s[num5]))
                {
                    num5++;
                    flag2 = true;
                }
                if ((num5 < s.Length) && ((s[num5] == 'E') || (s[num5] == 'e')))
                {
                    num5++;
                    flag2 = false;
                    if ((num5 < s.Length) && ((s[num5] == '+') || (s[num5] == '-')))
                    {
                        num5++;
                    }
                    while ((num5 < s.Length) && char.IsDigit(s[num5]))
                    {
                        num5++;
                        flag2 = true;
                    }
                }
                if ((num5 < s.Length) && ((s[num5] == 'i') || (s[num5] == 'j')))
                {
                    num5++;
                    flag3 = true;
                }
                num4 = num5 - length;
            }
            else if ((num5 < s.Length) && ((s[num5] == 'i') || (s[num5] == 'j')))
            {
                num5++;
                num4 = num5;
                flag2 = flag;
                flag3 = true;
                flag = false;
            }
            else
            {
                length = num5;
            }
            if (num5 < s.Length)
            {
                throw new FormatException();
            }
            if (length > 0)
            {
                if (!flag)
                {
                    throw new FormatException();
                }
                real = double.Parse(s.Substring(0, length), CultureInfo.InvariantCulture);
            }
            if (num4 > 0)
            {
                if (!flag3)
                {
                    throw new FormatException();
                }
                if (num4 != 1)
                {
                    if ((num4 != 2) || (s[length] != '+'))
                    {
                        if ((num4 != 2) || (s[length] != '-'))
                        {
                            if (!flag2)
                            {
                                throw new FormatException();
                            }
                            imag = double.Parse(s.Substring(length, num4 - 1), CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            imag = -1.0;
                        }
                    }
                    else
                    {
                        imag = 1.0;
                    }
                }
                else
                {
                    imag = 1.0;
                }
            }
            return new Complex(real, imag);
        }

        /// <summary>
        /// Returns the string representation of this complex number.
        /// </summary>
        public override string ToString()
        {
            return this.ToString("i");
        }

        /// <summary>
        /// Returns the string representation of this complex number.
        /// </summary>
        public string ToString(string suffix)
        {
            StringBuilder builder = new StringBuilder();
            if ((this.real != 0.0) || (this.imag == 0.0))
            {
                builder.Append(((double) this.real).ToString(CultureInfo.InvariantCulture));
            }
            if (this.imag == -1.0)
            {
                builder.Append("-");
            }
            else if ((this.real != 0.0) && (this.imag > 0.0))
            {
                builder.Append("+");
            }
            if (((this.imag != -1.0) && (this.imag != 0.0)) && (this.imag != 1.0))
            {
                builder.Append(((double) this.imag).ToString(CultureInfo.InvariantCulture));
            }
            if (this.imag != 0.0)
            {
                builder.Append(suffix);
            }
            return builder.ToString();
        }

        /// <summary>
        /// Gets the imaginary part of the complex number.
        /// </summary>
        public double Imag
        {
            get
            {
                return this.imag;
            }
        }

        /// <summary>
        /// Gets the real part of the complex number.
        /// </summary>
        public double Real
        {
            get
            {
                return this.real;
            }
        }
    }
}

