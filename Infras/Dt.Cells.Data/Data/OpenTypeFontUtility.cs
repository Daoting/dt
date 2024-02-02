#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf.Text;
using System;
using System.Collections.Generic;
using System.Threading;
using Windows.Foundation;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Internal only.
    /// OpenTypeFontUtility
    /// </summary>
    internal class OpenTypeFontUtility : IMeasureable
    {
        Font defaultFont;
        int dpi;
        readonly Dictionary<string, BaseFont> fontCaches = new Dictionary<string, BaseFont>();
        UnitType unit = UnitType.CentileInch;

        /// <summary>
        /// Occurs when not using a built-in Silverlight font.
        /// </summary>
        public event EventHandler<ExternalFontEventArgs> ExternalFont;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.OpenTypeFontUtility" /> class.
        /// </summary>
        /// <param name="defaultFont">The default font.</param>
        /// <param name="dpi">The dpi.</param>
        public OpenTypeFontUtility(Font defaultFont, int dpi)
        {
            this.defaultFont = defaultFont;
            this.dpi = dpi;
        }

        /// <summary>
        /// Gets the font.
        /// </summary>
        /// <param name="font">The font</param>
        /// <returns></returns>
        internal BaseFont GetFont(Font font)
        {
            font = font ?? this.defaultFont;
            if (string.IsNullOrEmpty(font.FontFamilyName))
            {
                font = (Font) font.Clone();
                font.FontFamily = this.defaultFont.FontFamily;
            }
            string str = string.Format("{0},{1}", (object[]) new object[] { font.FontFamilyName, font.FontStyleType });
            if (this.fontCaches.ContainsKey(str))
            {
                return this.fontCaches[str];
            }
            BaseFont arialMT = null;
            byte[] data = null;
            switch (font.FontFamilyName)
            {
                case "Arial":
                    arialMT = SimpleTrueTypeFont.ArialMT;
                    break;

                case "Arial Black":
                    arialMT = SimpleTrueTypeFont.ArialBlack;
                    break;

                case "Arial Narrow":
                    arialMT = SimpleTrueTypeFont.ArialNarrow;
                    break;

                case "Comic Sans MS":
                    arialMT = SimpleTrueTypeFont.ComicSansMS;
                    break;

                case "Courier New":
                    arialMT = SimpleTrueTypeFont.CourierNewPSMT;
                    break;

                case "Georgia":
                    arialMT = SimpleTrueTypeFont.Georgia;
                    break;

                case "Lucida Sans Unicode":
                    arialMT = SimpleTrueTypeFont.LucidaSansUnicode;
                    break;

                case "Times New Roman":
                    arialMT = SimpleTrueTypeFont.TimesNewRomanPSMT;
                    break;

                case "Trebuchet MS":
                    arialMT = SimpleTrueTypeFont.TrebuchetMS;
                    break;

                case "Verdana":
                    arialMT = SimpleTrueTypeFont.Verdana;
                    break;

                case "Webdings":
                    arialMT = SimpleTrueTypeFont.Webdings;
                    break;

                case "Portable User Interface":
                    arialMT = SimpleTrueTypeFont.Verdana;
                    break;

                case "Microsoft Sans Serif":
                    arialMT = SimpleTrueTypeFont.MicrosoftSansSerif;
                    break;

                case "SimSun":
                    arialMT = SimpleTrueTypeFont.SimSun;
                    break;

                case "MS Mincho":
                    arialMT = SimpleTrueTypeFont.MSMincho;
                    break;

                case "Batang":
                    arialMT = SimpleTrueTypeFont.Batang;
                    break;
            }
            if ((arialMT == null) && (this.ExternalFont != null))
            {
                ExternalFontEventArgs args = new ExternalFontEventArgs(font.FontFamilyName);
                this.ExternalFont(this, args);
                data = args.FontData;
                if (args.SimpleTrueTypeFont != null)
                {
                    arialMT = args.SimpleTrueTypeFont;
                }
                else if ((data != null) && (data.Length > 0))
                {
                    arialMT = OpenTypeFont.Load(new OpenTypeFontReader(data));
                    arialMT.Encoding = "Identity-H";
                    arialMT.IsEmbedded = true;
                }
            }
            // hdt 原来Verdana
            if (arialMT == null)
                arialMT = SimpleTrueTypeFont.SimSun;
            this.fontCaches.Add(str, arialMT);
            return arialMT;
        }

        /// <summary>
        /// Measures the string with no wrapping.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="font">The font.</param>
        /// <returns></returns>
        public Windows.Foundation.Size MeasureNoWrapString(string str, Font font)
        {
            if (string.IsNullOrEmpty(str))
            {
                return new Windows.Foundation.Size(0.0, 0.0);
            }
            font = font ?? this.DefaultFont;
            BaseFont font2 = this.GetFont(font);
            float fontSize = font.GetFontSize(this.unit, this.dpi);
            List<string> list = Utilities.GetLines(str, font, false, 0.0, this);
            float num2 = (font2.GetFontHeight() / 1000f) * fontSize;
            float num3 = 0f;
            foreach (string str2 in list)
            {
                num3 = Math.Max(num3, font2.GetStringWidth(str2));
            }
            num3 *= fontSize / 1000f;
            if (font.Italic)
            {
                num3 += num2 / 9f;
            }
            return new Windows.Foundation.Size((double) num3, (double) (num2 * list.Count));
        }

        /// <summary>
        /// Measures the string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="font">The font.</param>
        /// <param name="allowWrap">if set to <c>true</c> [allow wrap].</param>
        /// <param name="width">The width.</param>
        /// <returns></returns>
        public Windows.Foundation.Size MeasureString(string str, Font font, bool allowWrap, int width)
        {
            if (string.IsNullOrEmpty(str))
            {
                return new Windows.Foundation.Size(0.0, 0.0);
            }
            font = font ?? this.DefaultFont;
            BaseFont font2 = this.GetFont(font);
            float fontSize = font.GetFontSize(this.unit, this.dpi);
            float num2 = (font2.GetFontHeight() / 1000f) * fontSize;
            if (!allowWrap)
            {
                return this.MeasureNoWrapString(str, font);
            }
            List<string> list = Utilities.GetLines(str, font, true, (double) width, this);
            if (list.Count == 0)
            {
                width = 0;
            }
            if (list.Count == 1)
            {
                return this.MeasureNoWrapString(str, font);
            }
            return new Windows.Foundation.Size((double) width, (double) (list.Count * num2));
        }

        /// <summary>
        /// Gets the default font.
        /// </summary>
        /// <value>The default font.</value>
        public Font DefaultFont
        {
            get { return  this.defaultFont; }
        }

        /// <summary>
        /// Gets or sets the dpi.
        /// </summary>
        /// <value>The dpi.</value>
        public int Dpi
        {
            get { return  this.dpi; }
            set { this.dpi = value; }
        }

        /// <summary>
        /// Gets or sets the unit.
        /// </summary>
        /// <value>The unit.</value>
        public UnitType Unit
        {
            get { return  this.unit; }
            set { this.unit = value; }
        }
    }
}

