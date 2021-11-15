#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf.Drawing;
using Dt.Pdf.Object;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Pdf.Text
{
    /// <summary>
    /// The base font of Pdf Font.
    /// </summary>
    public abstract class BaseFont : IDisposable
    {
        public const int CNS_CodePage = 950;
        protected string encoding = "WinAnsiEncoding";
        public const int GB_CodePage = 0x3a8;
        public const string Identity_H = "Identity-H";
        public const string Identity_V = "Identity-V";
        protected bool isEmbedded;
        protected bool isSubSet = true;
        protected bool isSymbolic;
        public const int JIS_CodePage = 0x3a4;
        public const int KS_Johab_CodePage = 0x551;
        public const int KS_Wansung_CodePage = 0x3b5;
        private static readonly Random random = new Random();
        private readonly string subSetPrefix = CreateSubsetPrefix();
        public const string UniCNS_UCS2_H = "UniCNS-UCS2-H";
        public const string UniGB_UCS2_H = "UniGB-UCS2-H";
        public const string UniJIS_UCS2_H = "UniJIS-UCS2-H";
        public const string UniKS_UCS2_H = "UniKS-UCS2-H";
        public const string WinAnsiEncoding = "WinAnsiEncoding";

        protected BaseFont()
        {
        }

        /// <summary>
        /// Creates the subset prefix.
        /// </summary>
        /// <returns></returns>
        internal static string CreateSubsetPrefix()
        {
            char[] chArray = new char[7];
            lock (random)
            {
                for (int i = 0; i < 6; i++)
                {
                    chArray[i] = (char) random.Next(0x41, 0x5b);
                }
            }
            chArray[6] = '+';
            return (string) new string(chArray);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
        }

        /// <summary>
        /// Gets the ascent.
        /// </summary>
        /// <returns></returns>
        public virtual float GetAscent()
        {
            return 0f;
        }

        /// <summary>
        /// Gets the height of the capital letter.
        /// </summary>
        /// <returns></returns>
        public virtual float GetCapitalLetterHeight()
        {
            return 500f;
        }

        /// <summary>
        /// Gets the code pages.
        /// </summary>
        /// <returns></returns>
        public abstract int[] GetCodePages();
        /// <summary>
        /// Gets the default width.
        /// </summary>
        /// <returns></returns>
        public virtual float GetDefaultWidth()
        {
            return 1000f;
        }

        /// <summary>
        /// Gets the descent.
        /// </summary>
        /// <returns></returns>
        public virtual float GetDescent()
        {
            return 0f;
        }

        /// <summary>
        /// Gets the flag.
        /// </summary>
        /// <returns></returns>
        public virtual PdfFontDescriptor.Flag GetFlag()
        {
            return PdfFontDescriptor.Flag.Nonsymbolic;
        }

        /// <summary>
        /// Gets the font bounding box.
        /// </summary>
        /// <returns></returns>
        public virtual PdfRectangle GetFontBoundingBox()
        {
            return new PdfRectangle(0f, 0f, 1000f, 1000f);
        }

        public float GetFontHeight()
        {
            float height = this.GetFontBoundingBox().Height;
            if (height <= 0f)
            {
                height = this.GetAscent() - this.GetDescent();
            }
            if (height <= 0f)
            {
                height = this.GetCapitalLetterHeight();
            }
            return height;
        }

        /// <summary>
        /// Gets the index of the glyph.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns></returns>
        public abstract int GetGlyphIndex(char c);
        /// <summary>
        /// Gets the italic angle.
        /// </summary>
        /// <returns></returns>
        public virtual float GetItalicAngle()
        {
            return 0f;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            if (this.isSubSet && this.isEmbedded)
            {
                return (this.subSetPrefix + this.GetPostScriptName());
            }
            return this.GetPostScriptName();
        }

        /// <summary>
        /// Gets the name of the post script.
        /// </summary>
        /// <returns></returns>
        protected abstract string GetPostScriptName();
        public float GetStringWidth(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return 0f;
            }
            float num = 0f;
            foreach (char ch in str)
            {
                num += this.GetWidth(this.GetGlyphIndex(ch));
            }
            return num;
        }

        /// <summary>
        /// Gets the sub font.
        /// </summary>
        /// <param name="chars">The chars.</param>
        /// <param name="isCFF">if set to <c>true</c> [is CFF].</param>
        /// <returns></returns>
        public abstract byte[] GetSubFont(Dictionary<char, int> chars, out bool isCFF);
        /// <summary>
        /// Gets the vertical stem.
        /// </summary>
        /// <returns></returns>
        public virtual float GetVerticalStem()
        {
            return 0f;
        }

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public abstract float GetWidth(int index);
        /// <summary>
        /// Supports the encoding.
        /// </summary>
        /// <param name="enc">The enc.</param>
        /// <returns></returns>
        public bool SupportEncoding(string enc)
        {
            int[] codePages = this.GetCodePages();
            if (codePages != null)
            {
                List<int> list = new List<int>(codePages);
                switch (enc)
                {
                    case "UniGB-UCS2-H":
                        return list.Contains(0x3a8);

                    case "UniCNS-UCS2-H":
                        return list.Contains(950);

                    case "UniJIS-UCS2-H":
                        return list.Contains(0x3a4);

                    case "UniKS-UCS2-H":
                        if (!list.Contains(0x3b5))
                        {
                            return list.Contains(0x551);
                        }
                        return true;

                    case "WinAnsiEncoding":
                        return list.Contains(0x4e4);
                }
            }
            return false;
        }

        /// <summary>
        /// Gets or sets the encoding.
        /// </summary>
        /// <value>The encoding.</value>
        public string Encoding
        {
            get { return  this.encoding; }
            set { this.encoding = value; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is adobe C map.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is adobe C map; otherwise, <c>false</c>.
        /// </value>
        internal bool IsAdobeCMap
        {
            get
            {
                if (((this.encoding != "UniGB-UCS2-H") && (this.encoding != "UniCNS-UCS2-H")) && (this.encoding != "UniJIS-UCS2-H"))
                {
                    return (this.encoding == "UniKS-UCS2-H");
                }
                return true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is embedded.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is embedded; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsEmbedded
        {
            get { return  this.isEmbedded; }
            set { this.isEmbedded = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is sub set.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is sub set; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsSubSet
        {
            get { return  this.isSubSet; }
            set { this.isSubSet = value; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is symbolic.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is symbolic; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsSymbolic
        {
            get { return  this.isSymbolic; }
        }
    }
}

