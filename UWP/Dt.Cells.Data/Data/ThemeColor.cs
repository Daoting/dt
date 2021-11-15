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
using Windows.UI;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents the theme color class.
    /// </summary>
    public class ThemeColor : ICloneable
    {
        const int ACCENT1 = 4;
        const int ACCENT2 = 5;
        const int ACCENT3 = 6;
        const int ACCENT4 = 7;
        const int ACCENT5 = 8;
        const int ACCENT6 = 9;
        const int BACKCOLOR1 = 0;
        const int BACKCOLOR2 = 1;
        const int colorCount = 12;
        Windows.UI.Color[] colorList;
        const int FHYPERLINK = 11;
        const int HYPERLINK = 10;
        bool isDirty;
        string name;
        const int TEXTCOLOR1 = 2;
        const int TEXTCOLOR2 = 3;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.ThemeColor" /> class.
        /// </summary>
        public ThemeColor() : this(string.Empty, Colors.White, Colors.White, Colors.Black, Colors.Black, Colors.Transparent, Colors.Transparent, Colors.Transparent, Colors.Transparent, Colors.Transparent, Colors.Transparent, Colors.Transparent, Colors.Transparent)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.ThemeColor" /> class.
        /// </summary>
        /// <param name="name">The name of the theme color.</param>
        /// <param name="text1">The text1 color.</param>
        /// <param name="text2">The text2 color.</param>
        /// <param name="background1">The background1.</param>
        /// <param name="background2">The background2.</param>
        /// <param name="accent1">The accent1.</param>
        /// <param name="accent2">The accent2.</param>
        /// <param name="accent3">The accent3.</param>
        /// <param name="accent4">The accent4.</param>
        /// <param name="accent5">The accent5.</param>
        /// <param name="accent6">The accent6.</param>
        /// <param name="link">The link.</param>
        /// <param name="followedLink">The followed link.</param>
        public ThemeColor(string name, Windows.UI.Color text1, Windows.UI.Color text2, Windows.UI.Color background1, Windows.UI.Color background2, Windows.UI.Color accent1, Windows.UI.Color accent2, Windows.UI.Color accent3, Windows.UI.Color accent4, Windows.UI.Color accent5, Windows.UI.Color accent6, Windows.UI.Color link, Windows.UI.Color followedLink)
        {
            this.isDirty = false;
            this.name = name;
            this.colorList = new Windows.UI.Color[] { background1, background2, text1, text2, accent1, accent2, accent3, accent4, accent5, accent6, link, followedLink };
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            ThemeColor color = new ThemeColor {
                name = this.name,
                colorList = new Windows.UI.Color[12]
            };
            Array.Copy(this.colorList, color.colorList, 12);
            return color;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Windows.UI.Color GetThemeColor(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return Colors.Transparent;
            }
            int index = -1;
            string[] strArray = name.Split(new char[] { ' ' });
            if (strArray != null)
            {
                CompareInfo info = CultureInfo.InvariantCulture.CompareInfo;
                if (strArray.Length > 1)
                {
                    if (info.Compare(strArray[0], "Background", (CompareOptions) CompareOptions.IgnoreCase) == 0)
                    {
                        index = int.Parse(strArray[1]) - 1;
                    }
                    else if (info.Compare(strArray[0], "Text", (CompareOptions) CompareOptions.IgnoreCase) == 0)
                    {
                        index = int.Parse(strArray[1]) + 1;
                    }
                    else if (info.Compare(strArray[0], "Accent", (CompareOptions) CompareOptions.IgnoreCase) == 0)
                    {
                        index = int.Parse(strArray[1]) + 3;
                    }
                }
                else if (strArray.Length == 1)
                {
                    if (info.Compare(strArray[0], "Hyperlink", (CompareOptions) CompareOptions.IgnoreCase) == 0)
                    {
                        index = 10;
                    }
                    else if (info.Compare(strArray[0], "FollowedHyperlink", (CompareOptions) CompareOptions.IgnoreCase) == 0)
                    {
                        index = 11;
                    }
                }
                int num5 = 0;
                if (strArray.Length > 2)
                {
                    num5 = int.Parse(strArray[2]);
                }
                if (index > -1)
                {
                    return Dt.Cells.Data.ColorHelper.UpdateTint(this.colorList[index], ((float) num5) / 100f);
                }
            }
            return Colors.Black;
        }

        /// <summary>
        /// Gets or sets the accent1.
        /// </summary>
        /// <value>
        /// The accent1.
        /// </value>
        public Windows.UI.Color Accent1
        {
            get { return  this.colorList[4]; }
            set
            {
                this.colorList[4] = value;
                this.isDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the accent2.
        /// </summary>
        /// <value>
        /// The accent2.
        /// </value>
        public Windows.UI.Color Accent2
        {
            get { return  this.colorList[5]; }
            set
            {
                this.colorList[5] = value;
                this.isDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the accent3.
        /// </summary>
        /// <value>
        /// The accent3.
        /// </value>
        public Windows.UI.Color Accent3
        {
            get { return  this.colorList[6]; }
            set
            {
                this.colorList[6] = value;
                this.isDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the accent4.
        /// </summary>
        /// <value>
        /// The accent4.
        /// </value>
        public Windows.UI.Color Accent4
        {
            get { return  this.colorList[7]; }
            set
            {
                this.colorList[7] = value;
                this.isDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the accent5.
        /// </summary>
        /// <value>
        /// The accent5.
        /// </value>
        public Windows.UI.Color Accent5
        {
            get { return  this.colorList[8]; }
            set
            {
                this.colorList[8] = value;
                this.isDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the accent6.
        /// </summary>
        /// <value>
        /// The accent6.
        /// </value>
        public Windows.UI.Color Accent6
        {
            get { return  this.colorList[9]; }
            set
            {
                this.colorList[9] = value;
                this.isDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the background color1.
        /// </summary>
        /// <value>
        /// The background color1.
        /// </value>
        public Windows.UI.Color BackgroundColor1
        {
            get { return  this.colorList[0]; }
            set
            {
                this.colorList[0] = value;
                this.isDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the background color2.
        /// </summary>
        /// <value>
        /// The background color2.
        /// </value>
        public Windows.UI.Color BackgroundColor2
        {
            get { return  this.colorList[1]; }
            set
            {
                this.colorList[1] = value;
                this.isDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the color of the followed hyperlink.
        /// </summary>
        /// <value>
        /// The color of the followed hyperlink.
        /// </value>
        public Windows.UI.Color FollowedHyperlink
        {
            get { return  this.colorList[11]; }
            set
            {
                this.colorList[11] = value;
                this.isDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the hyperlink color.
        /// </summary>
        /// <value>
        /// The color of the hyperlink.
        /// </value>
        public Windows.UI.Color Hyperlink
        {
            get { return  this.colorList[10]; }
            set
            {
                this.colorList[10] = value;
                this.isDirty = true;
            }
        }

        internal bool IsDirty
        {
            get { return  this.isDirty; }
        }

        /// <summary>
        /// Gets or sets the name of the theme color.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get { return  this.name; }
            set
            {
                this.name = value;
                this.isDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the text color1.
        /// </summary>
        /// <value>
        /// The text color1.
        /// </value>
        public Windows.UI.Color TextColor1
        {
            get { return  this.colorList[2]; }
            set
            {
                this.colorList[2] = value;
                this.isDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the text color2.
        /// </summary>
        /// <value>
        /// The text color2.
        /// </value>
        public Windows.UI.Color TextColor2
        {
            get { return  this.colorList[3]; }
            set
            {
                this.colorList[3] = value;
                this.isDirty = true;
            }
        }
    }
}

