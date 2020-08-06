#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名

using System;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a built-in table style collection.
    /// </summary>
    public static class TableStyles
    {
        static List<TableStyle> customStyles = null;
        static TableStyle[] darks = new TableStyle[11];
        static TableStyle[] lights = new TableStyle[0x15];
        static TableStyle[] mediums = new TableStyle[0x1c];

        /// <summary>
        /// Adds the specified custom style.
        /// </summary>
        /// <param name="style">The table style.</param>
        public static void AddCustomStyles(TableStyle style)
        {
            if (style == null)
            {
                throw new NullReferenceException("style");
            }
            if (customStyles == null)
            {
                customStyles = new List<TableStyle>();
            }
            using (List<TableStyle>.Enumerator enumerator = customStyles.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (string.Equals(enumerator.Current.Name, style.Name, (StringComparison) StringComparison.CurrentCultureIgnoreCase))
                    {
                        throw new NotSupportedException(string.Format(ResourceStrings.TableStyleAddCustomStyleError, (object[]) new object[] { style.Name }));
                    }
                }
            }
            customStyles.Add(style);
        }

        static TableStyle CreateDarkA(int factor)
        {
            string theme = GetTheme(factor);
            Windows.UI.Color color = Colors.Black;
            string str2 = (factor == 0) ? string.Format("{0} 25", (object[]) new object[] { theme }) : string.Format("{0} -25", (object[]) new object[] { theme });
            string str3 = (factor == 0) ? string.Format("{0} 50", (object[]) new object[] { theme }) : string.Format("{0}", (object[]) new object[] { theme });
            string str4 = (factor == 0) ? string.Format("{0}", (object[]) new object[] { theme }) : string.Format("{0} -50", (object[]) new object[] { theme });
            TableStyle style = new TableStyle {
                HeaderRowStyle = new TableStyleInfo()
            };
            style.HeaderRowStyle.Background = new SolidColorBrush(color);
            style.HeaderRowStyle.FontWeight = FontWeights.Bold;
            style.HeaderRowStyle.Foreground = new SolidColorBrush(Colors.White);
            style.HeaderRowStyle.BorderBottom = new BorderLine(Colors.White, BorderLineStyle.Medium);
            style.WholeTableStyle = new TableStyleInfo();
            style.WholeTableStyle.BackgroundThemeColor = str3;
            style.WholeTableStyle.Foreground = new SolidColorBrush(Colors.White);
            style.FirstRowStripStyle = new TableStyleInfo();
            style.FirstRowStripStyle.BackgroundThemeColor = str2;
            style.FirstColumnStripStyle = new TableStyleInfo();
            style.FirstColumnStripStyle.BackgroundThemeColor = str2;
            style.HighlightFirstColumnStyle = new TableStyleInfo();
            style.HighlightFirstColumnStyle.FontWeight = FontWeights.Bold;
            style.HighlightFirstColumnStyle.BorderRight = new BorderLine(Colors.White, BorderLineStyle.Medium);
            style.HighlightFirstColumnStyle.BackgroundThemeColor = str2;
            style.HighlightFirstColumnStyle.Foreground = new SolidColorBrush(Colors.White);
            style.HighlightLastColumnStyle = new TableStyleInfo();
            style.HighlightLastColumnStyle.FontWeight = FontWeights.Bold;
            style.HighlightLastColumnStyle.BorderLeft = new BorderLine(Colors.White, BorderLineStyle.Medium);
            style.HighlightLastColumnStyle.BackgroundThemeColor = str2;
            style.HighlightLastColumnStyle.Foreground = new SolidColorBrush(Colors.White);
            style.FooterRowStyle = new TableStyleInfo();
            style.FooterRowStyle.BackgroundThemeColor = str4;
            style.FooterRowStyle.FontWeight = FontWeights.Bold;
            style.FooterRowStyle.Foreground = new SolidColorBrush(Colors.White);
            style.FooterRowStyle.BorderTop = new BorderLine(Colors.White, BorderLineStyle.Medium);
            return style;
        }

        static TableStyle CreateDarkB(int factor)
        {
            GetTheme(factor);
            string headerColor = GetHeaderColor(factor);
            string stripColor = GetStripColor(factor);
            string tableBackground = GetTableBackground(factor);
            string str4 = tableBackground;
            TableStyle style = new TableStyle {
                HeaderRowStyle = new TableStyleInfo()
            };
            style.HeaderRowStyle.BackgroundThemeColor = headerColor;
            style.HeaderRowStyle.Foreground = new SolidColorBrush(Colors.White);
            style.WholeTableStyle = new TableStyleInfo();
            style.WholeTableStyle.BackgroundThemeColor = tableBackground;
            style.FirstRowStripStyle = new TableStyleInfo();
            style.FirstRowStripStyle.BackgroundThemeColor = stripColor;
            style.FirstColumnStripStyle = new TableStyleInfo();
            style.FirstColumnStripStyle.BackgroundThemeColor = stripColor;
            style.HighlightFirstColumnStyle = new TableStyleInfo();
            style.HighlightFirstColumnStyle.FontWeight = FontWeights.Bold;
            style.HighlightLastColumnStyle = new TableStyleInfo();
            style.HighlightLastColumnStyle.FontWeight = FontWeights.Bold;
            style.FooterRowStyle = new TableStyleInfo();
            style.FooterRowStyle.BackgroundThemeColor = str4;
            style.FooterRowStyle.FontWeight = FontWeights.Bold;
            style.FooterRowStyle.BorderTop = new BorderLine(Colors.Black, BorderLineStyle.Double);
            return style;
        }

        static TableStyle CreateLightA(int factor)
        {
            string theme = GetTheme(factor);
            string str2 = string.Format("{0} 80", (object[]) new object[] { theme });
            string themeColor = string.Format("{0}", (object[]) new object[] { theme });
            TableStyle style = new TableStyle {
                HeaderRowStyle = new TableStyleInfo()
            };
            style.HeaderRowStyle.BorderBottom = new BorderLine(themeColor);
            style.HeaderRowStyle.FontWeight = FontWeights.Bold;
            style.HeaderRowStyle.ForegroundThemeColor = themeColor;
            style.WholeTableStyle = new TableStyleInfo();
            style.WholeTableStyle.BorderTop = new BorderLine(themeColor);
            style.WholeTableStyle.BorderBottom = new BorderLine(themeColor);
            style.WholeTableStyle.ForegroundThemeColor = themeColor;
            style.FirstRowStripStyle = new TableStyleInfo();
            style.FirstRowStripStyle.BackgroundThemeColor = str2;
            style.HighlightFirstColumnStyle = new TableStyleInfo();
            style.HighlightFirstColumnStyle.FontWeight = FontWeights.Bold;
            style.HighlightFirstColumnStyle.ForegroundThemeColor = themeColor;
            style.HighlightLastColumnStyle = new TableStyleInfo();
            style.HighlightLastColumnStyle.FontWeight = FontWeights.Bold;
            style.HighlightLastColumnStyle.ForegroundThemeColor = themeColor;
            style.FirstColumnStripStyle = new TableStyleInfo();
            style.FirstColumnStripStyle.BackgroundThemeColor = str2;
            style.FooterRowStyle = new TableStyleInfo();
            style.FooterRowStyle.BorderTop = new BorderLine(themeColor);
            style.FooterRowStyle.FontWeight = FontWeights.Bold;
            style.FooterRowStyle.ForegroundThemeColor = themeColor;
            return style;
        }

        static TableStyle CreateLightB(int factor)
        {
            string theme = GetTheme(factor);
            string themeColor = string.Format("{0}", (object[]) new object[] { theme });
            TableStyle style = new TableStyle {
                HeaderRowStyle = new TableStyleInfo()
            };
            style.HeaderRowStyle.BackgroundThemeColor = themeColor;
            style.HeaderRowStyle.FontWeight = FontWeights.Bold;
            style.HeaderRowStyle.Foreground = new SolidColorBrush(Colors.White);
            style.WholeTableStyle = new TableStyleInfo();
            style.WholeTableStyle.BorderLeft = new BorderLine(themeColor);
            style.WholeTableStyle.BorderTop = new BorderLine(themeColor);
            style.WholeTableStyle.BorderBottom = new BorderLine(themeColor);
            style.WholeTableStyle.BorderRight = new BorderLine(themeColor);
            style.FirstRowStripStyle = new TableStyleInfo();
            style.FirstRowStripStyle.BorderTop = new BorderLine(themeColor);
            style.SecondRowStripStyle = new TableStyleInfo();
            style.SecondRowStripStyle.BorderTop = new BorderLine(themeColor);
            style.FirstColumnStripStyle = new TableStyleInfo();
            style.FirstColumnStripStyle.BorderLeft = new BorderLine(themeColor);
            style.SecondColumnStripStyle = new TableStyleInfo();
            style.SecondColumnStripStyle.BorderLeft = new BorderLine(themeColor);
            style.HighlightFirstColumnStyle = new TableStyleInfo();
            style.HighlightFirstColumnStyle.FontWeight = FontWeights.Bold;
            style.HighlightLastColumnStyle = new TableStyleInfo();
            style.HighlightLastColumnStyle.FontWeight = FontWeights.Bold;
            style.FooterRowStyle = new TableStyleInfo();
            style.FooterRowStyle.BorderTop = new BorderLine(themeColor, BorderLineStyle.Double);
            style.FooterRowStyle.FontWeight = FontWeights.Bold;
            return style;
        }

        static TableStyle CreateLightC(int factor)
        {
            string theme = GetTheme(factor);
            string str2 = string.Format("{0} 80", (object[]) new object[] { theme });
            string themeColor = string.Format("{0}", (object[]) new object[] { theme });
            TableStyle style = new TableStyle {
                HeaderRowStyle = new TableStyleInfo()
            };
            style.HeaderRowStyle.BorderBottom = new BorderLine(themeColor, BorderLineStyle.Medium);
            style.HeaderRowStyle.FontWeight = FontWeights.Bold;
            style.WholeTableStyle = new TableStyleInfo();
            style.WholeTableStyle.BorderLeft = new BorderLine(themeColor);
            style.WholeTableStyle.BorderTop = new BorderLine(themeColor);
            style.WholeTableStyle.BorderRight = new BorderLine(themeColor);
            style.WholeTableStyle.BorderBottom = new BorderLine(themeColor);
            style.WholeTableStyle.BorderHorizontal = new BorderLine(themeColor);
            style.WholeTableStyle.BorderVertical = new BorderLine(themeColor);
            style.FirstRowStripStyle = new TableStyleInfo();
            style.FirstRowStripStyle.BackgroundThemeColor = str2;
            style.FirstColumnStripStyle = new TableStyleInfo();
            style.FirstColumnStripStyle.BackgroundThemeColor = str2;
            style.HighlightFirstColumnStyle = new TableStyleInfo();
            style.HighlightFirstColumnStyle.FontWeight = FontWeights.Bold;
            style.HighlightLastColumnStyle = new TableStyleInfo();
            style.HighlightLastColumnStyle.FontWeight = FontWeights.Bold;
            style.FooterRowStyle = new TableStyleInfo();
            style.FooterRowStyle.BorderTop = new BorderLine(themeColor, BorderLineStyle.Double);
            style.FooterRowStyle.FontWeight = FontWeights.Bold;
            return style;
        }

        static TableStyle CreateMediumA(int factor)
        {
            string theme = GetTheme(factor);
            string themeColor = string.Format("{0} 20", (object[]) new object[] { theme });
            string str3 = string.Format("{0}", (object[]) new object[] { theme });
            string str4 = string.Format("{0} 80", (object[]) new object[] { theme });
            TableStyle style = new TableStyle {
                HeaderRowStyle = new TableStyleInfo()
            };
            style.HeaderRowStyle.BackgroundThemeColor = str3;
            style.HeaderRowStyle.FontWeight = FontWeights.Bold;
            style.HeaderRowStyle.Foreground = new SolidColorBrush(Colors.White);
            style.WholeTableStyle = new TableStyleInfo();
            style.WholeTableStyle.BorderHorizontal = new BorderLine(themeColor);
            style.WholeTableStyle.BorderLeft = new BorderLine(themeColor);
            style.WholeTableStyle.BorderRight = new BorderLine(themeColor);
            style.WholeTableStyle.BorderBottom = new BorderLine(themeColor);
            style.WholeTableStyle.BorderTop = new BorderLine(themeColor);
            style.FirstRowStripStyle = new TableStyleInfo();
            style.FirstRowStripStyle.BackgroundThemeColor = str4;
            style.FirstColumnStripStyle = new TableStyleInfo();
            style.FirstColumnStripStyle.BackgroundThemeColor = str4;
            style.HighlightFirstColumnStyle = new TableStyleInfo();
            style.HighlightFirstColumnStyle.FontWeight = FontWeights.Bold;
            style.HighlightLastColumnStyle = new TableStyleInfo();
            style.HighlightLastColumnStyle.FontWeight = FontWeights.Bold;
            style.FooterRowStyle = new TableStyleInfo();
            style.FooterRowStyle.BorderTop = new BorderLine(themeColor, BorderLineStyle.Double);
            style.FooterRowStyle.FontWeight = FontWeights.Bold;
            style.FooterRowStyle.Foreground = new SolidColorBrush(Colors.Black);
            return style;
        }

        static TableStyle CreateMediumB(int factor)
        {
            string theme = GetTheme(factor);
            Windows.UI.Color color = Colors.White;
            string str2 = string.Format("{0}", (object[]) new object[] { theme });
            string str3 = string.Format("{0} 60", (object[]) new object[] { theme });
            string str4 = string.Format("{0} 80", (object[]) new object[] { theme });
            TableStyle style = new TableStyle {
                HeaderRowStyle = new TableStyleInfo()
            };
            style.HeaderRowStyle.BackgroundThemeColor = str2;
            style.HeaderRowStyle.BorderBottom = new BorderLine(color, BorderLineStyle.Medium);
            style.HeaderRowStyle.BorderVertical = new BorderLine(color);
            style.HeaderRowStyle.FontWeight = FontWeights.Bold;
            style.HeaderRowStyle.Foreground = new SolidColorBrush(Colors.White);
            style.WholeTableStyle = new TableStyleInfo();
            style.WholeTableStyle.BorderHorizontal = new BorderLine(color);
            style.WholeTableStyle.BorderVertical = new BorderLine(color);
            style.WholeTableStyle.BackgroundThemeColor = str4;
            style.FirstRowStripStyle = new TableStyleInfo();
            style.FirstRowStripStyle.BackgroundThemeColor = str3;
            style.FirstColumnStripStyle = new TableStyleInfo();
            style.FirstColumnStripStyle.BackgroundThemeColor = str3;
            style.HighlightFirstColumnStyle = new TableStyleInfo();
            style.HighlightFirstColumnStyle.BackgroundThemeColor = str2;
            style.HighlightFirstColumnStyle.Foreground = new SolidColorBrush(Colors.White);
            style.HighlightFirstColumnStyle.FontWeight = FontWeights.Bold;
            style.HighlightLastColumnStyle = new TableStyleInfo();
            style.HighlightLastColumnStyle.BackgroundThemeColor = str2;
            style.HighlightLastColumnStyle.Foreground = new SolidColorBrush(Colors.White);
            style.HighlightLastColumnStyle.FontWeight = FontWeights.Bold;
            style.FooterRowStyle = new TableStyleInfo();
            style.FooterRowStyle.BorderTop = new BorderLine(color, BorderLineStyle.Medium);
            style.FooterRowStyle.BorderVertical = new BorderLine(color);
            style.FooterRowStyle.BackgroundThemeColor = str2;
            style.FooterRowStyle.Foreground = new SolidColorBrush(Colors.White);
            style.FooterRowStyle.FontWeight = FontWeights.Bold;
            return style;
        }

        static TableStyle CreateMediumC(int factor)
        {
            string theme = GetTheme(factor);
            string str2 = string.Format("{0}", (object[]) new object[] { theme });
            Windows.UI.Color color = Colors.LightGray;
            Windows.UI.Color color2 = (factor == 0) ? Colors.Black : Colors.LightGray;
            TableStyle style = new TableStyle {
                HeaderRowStyle = new TableStyleInfo()
            };
            style.HeaderRowStyle.BackgroundThemeColor = str2;
            style.HeaderRowStyle.BorderTop = new BorderLine(Colors.Black, BorderLineStyle.Medium);
            style.HeaderRowStyle.BorderBottom = new BorderLine(Colors.Black, BorderLineStyle.Medium);
            style.HeaderRowStyle.FontWeight = FontWeights.Bold;
            style.HeaderRowStyle.Foreground = new SolidColorBrush(Colors.White);
            style.WholeTableStyle = new TableStyleInfo();
            style.WholeTableStyle.BorderLeft = new BorderLine(color2);
            style.WholeTableStyle.BorderRight = new BorderLine(color2);
            style.WholeTableStyle.BorderTop = new BorderLine(Colors.Black, BorderLineStyle.Medium);
            style.WholeTableStyle.BorderVertical = new BorderLine(color2);
            if (factor == 0)
            {
                style.WholeTableStyle.BorderHorizontal = new BorderLine(color2);
            }
            style.WholeTableStyle.BorderBottom = new BorderLine(Colors.Black, BorderLineStyle.Medium);
            style.FirstRowStripStyle = new TableStyleInfo();
            style.FirstRowStripStyle.Background = new SolidColorBrush(color);
            style.FirstColumnStripStyle = new TableStyleInfo();
            style.FirstColumnStripStyle.Background = new SolidColorBrush(color);
            style.HighlightFirstColumnStyle = new TableStyleInfo();
            style.HighlightFirstColumnStyle.BackgroundThemeColor = str2;
            style.HighlightFirstColumnStyle.Foreground = new SolidColorBrush(Colors.White);
            style.HighlightLastColumnStyle = new TableStyleInfo();
            style.HighlightLastColumnStyle.BackgroundThemeColor = str2;
            style.HighlightLastColumnStyle.Foreground = new SolidColorBrush(Colors.White);
            style.FirstFooterCellStyle = new TableStyleInfo();
            style.FirstFooterCellStyle.BackgroundThemeColor = str2;
            style.FirstFooterCellStyle.FontWeight = FontWeights.Bold;
            style.FirstFooterCellStyle.Foreground = new SolidColorBrush(Colors.White);
            style.LastFooterCellStyle = new TableStyleInfo();
            style.LastFooterCellStyle.BackgroundThemeColor = str2;
            style.LastFooterCellStyle.FontWeight = FontWeights.Bold;
            style.LastFooterCellStyle.Foreground = new SolidColorBrush(Colors.White);
            style.FooterRowStyle = new TableStyleInfo();
            style.FooterRowStyle.Foreground = new SolidColorBrush(Colors.Black);
            style.FooterRowStyle.BorderTop = new BorderLine(Colors.Black, BorderLineStyle.Double);
            return style;
        }

        static TableStyle CreateMediumD(int factor)
        {
            string theme = GetTheme(factor);
            string themeColor = string.Format("{0}", (object[]) new object[] { theme });
            string str3 = string.Format("{0} 40", (object[]) new object[] { theme });
            string str4 = string.Format("{0} 80", (object[]) new object[] { theme });
            string str5 = string.Format("{0} 60", (object[]) new object[] { theme });
            string str6 = string.Format("{0} 80", (object[]) new object[] { theme });
            TableStyle style = new TableStyle {
                HeaderRowStyle = new TableStyleInfo()
            };
            style.HeaderRowStyle.BackgroundThemeColor = str4;
            style.HeaderRowStyle.BorderVertical = new BorderLine(str3);
            style.HeaderRowStyle.BorderLeft = new BorderLine(str3);
            style.HeaderRowStyle.BorderTop = new BorderLine(str3);
            style.HeaderRowStyle.BorderRight = new BorderLine(str3);
            style.HeaderRowStyle.BorderBottom = new BorderLine(str3);
            style.HeaderRowStyle.FontWeight = FontWeights.Bold;
            style.HeaderRowStyle.Foreground = new SolidColorBrush(Colors.Black);
            style.WholeTableStyle = new TableStyleInfo();
            style.WholeTableStyle.BorderVertical = new BorderLine(str3);
            style.WholeTableStyle.BorderHorizontal = new BorderLine(str3);
            style.WholeTableStyle.BorderLeft = new BorderLine(str3);
            style.WholeTableStyle.BorderTop = new BorderLine(str3);
            style.WholeTableStyle.BorderRight = new BorderLine(str3);
            style.WholeTableStyle.BorderBottom = new BorderLine(str3);
            style.WholeTableStyle.BackgroundThemeColor = str4;
            style.FirstRowStripStyle = new TableStyleInfo();
            style.FirstRowStripStyle.BackgroundThemeColor = str5;
            style.FirstColumnStripStyle = new TableStyleInfo();
            style.FirstColumnStripStyle.BackgroundThemeColor = str5;
            style.HighlightFirstColumnStyle = new TableStyleInfo();
            style.HighlightFirstColumnStyle.FontWeight = FontWeights.Bold;
            style.HighlightLastColumnStyle = new TableStyleInfo();
            style.HighlightLastColumnStyle.FontWeight = FontWeights.Bold;
            style.FooterRowStyle = new TableStyleInfo();
            style.FooterRowStyle.BackgroundThemeColor = str6;
            style.FooterRowStyle.Foreground = new SolidColorBrush(Colors.Black);
            style.FooterRowStyle.FontWeight = FontWeights.Bold;
            style.FooterRowStyle.BorderLeft = new BorderLine(str3);
            style.FooterRowStyle.BorderTop = new BorderLine(themeColor, BorderLineStyle.Medium);
            style.FooterRowStyle.BorderRight = new BorderLine(str3);
            style.FooterRowStyle.BorderBottom = new BorderLine(str3);
            style.FooterRowStyle.BorderVertical = new BorderLine(str3);
            return style;
        }

        /// <summary>
        /// Gets the specified custom style.
        /// </summary>
        /// <param name="name">The table style name.</param>
        /// <returns>Return </returns>
        public static TableStyle GetCustomStyle(string name)
        {
            if (name == null)
            {
                throw new NullReferenceException("tableStyleName");
            }
            foreach (TableStyle style in customStyles)
            {
                if (string.Equals(style.Name, name, (StringComparison) StringComparison.CurrentCultureIgnoreCase))
                {
                    return style;
                }
            }
            return null;
        }

        static TableStyle GetDarkStyle(int id)
        {
            int index = id - 1;
            if (darks == null)
            {
                darks = new TableStyle[11];
            }
            if (darks[index] != null)
            {
                return darks[index];
            }
            TableStyle style = null;
            switch ((index / 7))
            {
                case 0:
                    style = CreateDarkA(index % 7);
                    break;

                case 1:
                    style = CreateDarkB(index % 7);
                    break;
            }
            if (style != null)
            {
                style.Name = "Dark" + ((int) id).ToString();
                darks[index] = style;
                return style;
            }
            return null;
        }

        static string GetHeaderColor(int index)
        {
            if (index == 0)
            {
                return "Text 1";
            }
            if (index == 1)
            {
                return "Accent 2";
            }
            if (index == 2)
            {
                return "Accent 4";
            }
            if (index == 3)
            {
                return "Accent 6";
            }
            return string.Empty;
        }

        static TableStyle GetLightStyle(int id)
        {
            int index = id - 1;
            if (lights == null)
            {
                lights = new TableStyle[0x15];
            }
            if (lights[index] != null)
            {
                return lights[index];
            }
            TableStyle style = null;
            switch ((index / 7))
            {
                case 0:
                    style = CreateLightA(index % 7);
                    break;

                case 1:
                    style = CreateLightB(index % 7);
                    break;

                case 2:
                    style = CreateLightC(index % 7);
                    break;
            }
            if (style != null)
            {
                style.Name = "Light" + ((int) id).ToString();
                lights[index] = style;
                return style;
            }
            return null;
        }

        static TableStyle GetMediumStyle(int id)
        {
            int index = id - 1;
            if (mediums == null)
            {
                mediums = new TableStyle[0x1c];
            }
            if (mediums[index] != null)
            {
                return mediums[index];
            }
            TableStyle style = null;
            switch ((index / 7))
            {
                case 0:
                    style = CreateMediumA(index % 7);
                    break;

                case 1:
                    style = CreateMediumB(index % 7);
                    break;

                case 2:
                    style = CreateMediumC(index % 7);
                    break;

                case 3:
                    style = CreateMediumD(index % 7);
                    break;
            }
            if (style != null)
            {
                style.Name = "Medium" + ((int) id).ToString();
                mediums[index] = style;
                return style;
            }
            return null;
        }

        static string GetStripColor(int index)
        {
            if (index == 0)
            {
                return "Background 1 -35";
            }
            if (index == 1)
            {
                return "Accent 1 60";
            }
            if (index == 2)
            {
                return "Accent 3 60";
            }
            if (index == 3)
            {
                return "Accent 5 60";
            }
            return string.Empty;
        }

        static string GetTableBackground(int index)
        {
            if (index == 0)
            {
                return "Background 1 -15";
            }
            if (index == 1)
            {
                return "Accent 1 80";
            }
            if (index == 2)
            {
                return "Accent 3 80";
            }
            if (index == 3)
            {
                return "Accent 5 80";
            }
            return string.Empty;
        }

        static string GetTheme(int factor)
        {
            if (factor == 0)
            {
                return "Text 1";
            }
            return string.Format("Accent {0}", (object[]) new object[] { ((int) factor) });
        }

        /// <summary>
        /// Removes all custom styles.
        /// </summary>
        public static void RemoveAllCustomStyles()
        {
            if (customStyles != null)
            {
                customStyles.Clear();
            }
        }

        /// <summary>
        /// Removes the specified custom style.
        /// </summary>
        /// <param name="style">The table style.</param>
        /// <returns>Returns true if the remove action succeeds; otherwise, returns false.</returns>
        public static bool RemoveCustomStyle(TableStyle style)
        {
            if (style == null)
            {
                throw new NullReferenceException("style");
            }
            if (customStyles != null)
            {
                return customStyles.Remove(style);
            }
            return true;
        }

        /// <summary>
        /// Removes the specified custom style.
        /// </summary>
        /// <param name="name">The table name.</param>
        /// <returns>Returns true if the remove action succeeds; otherwise, returns false.</returns>
        public static bool RemoveCustomStyle(string name)
        {
            if (name == null)
            {
                throw new NullReferenceException("name");
            }
            foreach (TableStyle style in customStyles)
            {
                if (string.Equals(style.Name, name, (StringComparison) StringComparison.CurrentCultureIgnoreCase))
                {
                    return customStyles.Remove(style);
                }
            }
            return true;
        }

        /// <summary>
        /// Gets the custom styles.
        /// </summary>
        public static TableStyle[] CustomStyles
        {
            get
            {
                if (customStyles != null)
                {
                    return customStyles.ToArray();
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the dark1 style.
        /// </summary>
        public static TableStyle Dark1
        {
            get { return  GetDarkStyle(1); }
        }

        /// <summary>
        /// Gets the dark10 style.
        /// </summary>
        public static TableStyle Dark10
        {
            get { return  GetDarkStyle(10); }
        }

        /// <summary>
        /// Gets the dark11 style.
        /// </summary>
        public static TableStyle Dark11
        {
            get { return  GetDarkStyle(11); }
        }

        /// <summary>
        /// Gets the dark2 style.
        /// </summary>
        public static TableStyle Dark2
        {
            get { return  GetDarkStyle(2); }
        }

        /// <summary>
        /// Gets the dark3 style.
        /// </summary>
        public static TableStyle Dark3
        {
            get { return  GetDarkStyle(3); }
        }

        /// <summary>
        /// Gets the dark4 style.
        /// </summary>
        public static TableStyle Dark4
        {
            get { return  GetDarkStyle(4); }
        }

        /// <summary>
        /// Gets the dark5 style.
        /// </summary>
        public static TableStyle Dark5
        {
            get { return  GetDarkStyle(5); }
        }

        /// <summary>
        /// Gets the dark6 style.
        /// </summary>
        public static TableStyle Dark6
        {
            get { return  GetDarkStyle(6); }
        }

        /// <summary>
        /// Gets the dark7 style.
        /// </summary>
        public static TableStyle Dark7
        {
            get { return  GetDarkStyle(7); }
        }

        /// <summary>
        /// Gets the dark8 style.
        /// </summary>
        public static TableStyle Dark8
        {
            get { return  GetDarkStyle(8); }
        }

        /// <summary>
        /// Gets the dark9 style.
        /// </summary>
        public static TableStyle Dark9
        {
            get { return  GetDarkStyle(9); }
        }

        /// <summary>
        /// Gets the light1 style.
        /// </summary>
        public static TableStyle Light1
        {
            get { return  GetLightStyle(1); }
        }

        /// <summary>
        /// Gets the light10 style.
        /// </summary>
        public static TableStyle Light10
        {
            get { return  GetLightStyle(10); }
        }

        /// <summary>
        /// Gets the light11 style.
        /// </summary>
        public static TableStyle Light11
        {
            get { return  GetLightStyle(11); }
        }

        /// <summary>
        /// Gets the light12 style.
        /// </summary>
        public static TableStyle Light12
        {
            get { return  GetLightStyle(12); }
        }

        /// <summary>
        /// Gets the light13 style.
        /// </summary>
        public static TableStyle Light13
        {
            get { return  GetLightStyle(13); }
        }

        /// <summary>
        /// Gets the light14 style.
        /// </summary>
        public static TableStyle Light14
        {
            get { return  GetLightStyle(14); }
        }

        /// <summary>
        /// Gets the light15 style.
        /// </summary>
        public static TableStyle Light15
        {
            get { return  GetLightStyle(15); }
        }

        /// <summary>
        /// Gets the light16 style.
        /// </summary>
        public static TableStyle Light16
        {
            get { return  GetLightStyle(0x10); }
        }

        /// <summary>
        /// Gets the light17 style.
        /// </summary>
        public static TableStyle Light17
        {
            get { return  GetLightStyle(0x11); }
        }

        /// <summary>
        /// Gets the light18 style.
        /// </summary>
        public static TableStyle Light18
        {
            get { return  GetLightStyle(0x12); }
        }

        /// <summary>
        /// Gets the light19  style.
        /// </summary>
        public static TableStyle Light19
        {
            get { return  GetLightStyle(0x13); }
        }

        /// <summary>
        /// Gets the light2 style.
        /// </summary>
        public static TableStyle Light2
        {
            get { return  GetLightStyle(2); }
        }

        /// <summary>
        /// Gets the light20 style.
        /// </summary>
        public static TableStyle Light20
        {
            get { return  GetLightStyle(20); }
        }

        /// <summary>
        /// Gets the light21 style.
        /// </summary>
        public static TableStyle Light21
        {
            get { return  GetLightStyle(0x15); }
        }

        /// <summary>
        /// Gets the light3 style.
        /// </summary>
        public static TableStyle Light3
        {
            get { return  GetLightStyle(3); }
        }

        /// <summary>
        /// Gets the light4 style.
        /// </summary>
        public static TableStyle Light4
        {
            get { return  GetLightStyle(4); }
        }

        /// <summary>
        /// Gets the light5 style.
        /// </summary>
        public static TableStyle Light5
        {
            get { return  GetLightStyle(5); }
        }

        /// <summary>
        /// Gets the light6 style.
        /// </summary>
        public static TableStyle Light6
        {
            get { return  GetLightStyle(6); }
        }

        /// <summary>
        /// Gets the light7 style.
        /// </summary>
        public static TableStyle Light7
        {
            get { return  GetLightStyle(7); }
        }

        /// <summary>
        /// Gets the light8 style.
        /// </summary>
        public static TableStyle Light8
        {
            get { return  GetLightStyle(8); }
        }

        /// <summary>
        /// Gets the light9 style.
        /// </summary>
        public static TableStyle Light9
        {
            get { return  GetLightStyle(9); }
        }

        /// <summary>
        /// Gets the medium1 style.
        /// </summary>
        public static TableStyle Medium1
        {
            get { return  GetMediumStyle(1); }
        }

        /// <summary>
        /// Gets the medium10 style.
        /// </summary>
        public static TableStyle Medium10
        {
            get { return  GetMediumStyle(10); }
        }

        /// <summary>
        /// Gets the medium11 style.
        /// </summary>
        public static TableStyle Medium11
        {
            get { return  GetMediumStyle(11); }
        }

        /// <summary>
        /// Gets the medium12 style.
        /// </summary>
        public static TableStyle Medium12
        {
            get { return  GetMediumStyle(12); }
        }

        /// <summary>
        /// Gets the medium13 style.
        /// </summary>
        public static TableStyle Medium13
        {
            get { return  GetMediumStyle(13); }
        }

        /// <summary>
        /// Gets the medium14 style.
        /// </summary>
        public static TableStyle Medium14
        {
            get { return  GetMediumStyle(14); }
        }

        /// <summary>
        /// Gets the medium15 style.
        /// </summary>
        public static TableStyle Medium15
        {
            get { return  GetMediumStyle(15); }
        }

        /// <summary>
        /// Gets the medium16 style.
        /// </summary>
        public static TableStyle Medium16
        {
            get { return  GetMediumStyle(0x10); }
        }

        /// <summary>
        /// Gets the medium17 style.
        /// </summary>
        public static TableStyle Medium17
        {
            get { return  GetMediumStyle(0x11); }
        }

        /// <summary>
        /// Gets the medium18 style.
        /// </summary>
        public static TableStyle Medium18
        {
            get { return  GetMediumStyle(0x12); }
        }

        /// <summary>
        /// Gets the medium19 style.
        /// </summary>
        public static TableStyle Medium19
        {
            get { return  GetMediumStyle(0x13); }
        }

        /// <summary>
        /// Gets the medium2 style.
        /// </summary>
        public static TableStyle Medium2
        {
            get { return  GetMediumStyle(2); }
        }

        /// <summary>
        /// Gets the medium20 style.
        /// </summary>
        public static TableStyle Medium20
        {
            get { return  GetMediumStyle(20); }
        }

        /// <summary>
        /// Gets the medium21 style.
        /// </summary>
        public static TableStyle Medium21
        {
            get { return  GetMediumStyle(0x15); }
        }

        /// <summary>
        /// Gets the medium22 style.
        /// </summary>
        public static TableStyle Medium22
        {
            get { return  GetMediumStyle(0x16); }
        }

        /// <summary>
        /// Gets the medium23 style.
        /// </summary>
        public static TableStyle Medium23
        {
            get { return  GetMediumStyle(0x17); }
        }

        /// <summary>
        /// Gets the medium24 style.
        /// </summary>
        public static TableStyle Medium24
        {
            get { return  GetMediumStyle(0x18); }
        }

        /// <summary>
        /// Gets the medium25 style.
        /// </summary>
        public static TableStyle Medium25
        {
            get { return  GetMediumStyle(0x19); }
        }

        /// <summary>
        /// Gets the medium26 style.
        /// </summary>
        public static TableStyle Medium26
        {
            get { return  GetMediumStyle(0x1a); }
        }

        /// <summary>
        /// Gets the medium27 style.
        /// </summary>
        public static TableStyle Medium27
        {
            get { return  GetMediumStyle(0x1b); }
        }

        /// <summary>
        /// Gets the medium28 style.
        /// </summary>
        public static TableStyle Medium28
        {
            get { return  GetMediumStyle(0x1c); }
        }

        /// <summary>
        /// Gets the medium3 style.
        /// </summary>
        public static TableStyle Medium3
        {
            get { return  GetMediumStyle(3); }
        }

        /// <summary>
        /// Gets the medium4 style.
        /// </summary>
        public static TableStyle Medium4
        {
            get { return  GetMediumStyle(4); }
        }

        /// <summary>
        /// Gets the medium5 style.
        /// </summary>
        public static TableStyle Medium5
        {
            get { return  GetMediumStyle(5); }
        }

        /// <summary>
        /// Gets the medium6 style.
        /// </summary>
        public static TableStyle Medium6
        {
            get { return  GetMediumStyle(6); }
        }

        /// <summary>
        /// Gets the medium7 style.
        /// </summary>
        public static TableStyle Medium7
        {
            get { return  GetMediumStyle(7); }
        }

        /// <summary>
        /// Gets the medium8 style.
        /// </summary>
        public static TableStyle Medium8
        {
            get { return  GetMediumStyle(8); }
        }

        /// <summary>
        /// Gets the medium9 style.
        /// </summary>
        public static TableStyle Medium9
        {
            get { return  GetMediumStyle(9); }
        }
    }
}

