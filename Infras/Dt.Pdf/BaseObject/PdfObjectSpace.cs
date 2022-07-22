#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf.Exceptions;
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Pdf.BaseObject
{
    /// <summary>
    /// Pdf object space
    /// </summary>
    public class PdfObjectSpace
    {
        private readonly List<PdfArray> arrays = new List<PdfArray>();
        private readonly List<PdfBool> bools = new List<PdfBool>();
        private static PdfObjectSpace currentSpace;
        public static readonly string DEFAULTSPACENAME = "Level 1 multiverse";
        private readonly List<PdfDictionary> dictionaries = new List<PdfDictionary>();
        private readonly CompressLevel level;
        private readonly List<PdfName> names = new List<PdfName>();
        private readonly List<PdfNumber> numbers = new List<PdfNumber>();
        private static readonly Dictionary<string, PdfObjectSpace> spaces = new Dictionary<string, PdfObjectSpace>();
        private readonly List<PdfStream> streams = new List<PdfStream>();
        private readonly List<PdfString> strings = new List<PdfString>();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.BaseObject.PdfObjectSpace" /> class.
        /// </summary>
        /// <param name="cLevel">The c level.</param>
        public PdfObjectSpace(CompressLevel cLevel)
        {
            this.level = cLevel;
        }

        /// <summary>
        /// Adds the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        public void Add(PdfObjectBase obj)
        {
            if (obj is PdfStream)
            {
                this.streams.Add((PdfStream) obj);
                if (this.level != CompressLevel.Stream)
                {
                }
            }
            else if (obj is PdfDictionary)
            {
                this.dictionaries.Add((PdfDictionary) obj);
                if (this.level != CompressLevel.Dictionary)
                {
                }
            }
            else if (obj is PdfArray)
            {
                this.arrays.Add((PdfArray) obj);
                if (this.level != CompressLevel.Array)
                {
                }
            }
            else if (obj is PdfString)
            {
                this.strings.Add((PdfString) obj);
                if (this.level != CompressLevel.String)
                {
                }
            }
            else if (obj is PdfName)
            {
                this.names.Add((PdfName) obj);
            }
            else if (obj is PdfBool)
            {
                this.bools.Add((PdfBool) obj);
            }
            else if (obj is PdfNumber)
            {
                this.numbers.Add((PdfNumber) obj);
            }
        }

        /// <summary>
        /// Creates the space.
        /// </summary>
        /// <param name="spaceName">Name of the space.</param>
        /// <param name="cLevel">The c level.</param>
        public static void CreateSpace(string spaceName, CompressLevel cLevel)
        {
            if (string.IsNullOrEmpty(spaceName))
            {
                spaceName = DEFAULTSPACENAME;
            }
            if (spaces.ContainsKey(spaceName))
            {
                spaces[spaceName] = new PdfObjectSpace(cLevel);
            }
            else
            {
                spaces.Add(spaceName, new PdfObjectSpace(cLevel));
            }
        }

        /// <summary>
        /// Switches the space.
        /// </summary>
        /// <param name="objectSpace">The object space.</param>
        public static void SwitchSpace(PdfObjectSpace objectSpace)
        {
            if (objectSpace == null)
            {
                throw new PdfArgumentNullException("objectSpace");
            }
            currentSpace = objectSpace;
        }

        /// <summary>
        /// Switches the space.
        /// </summary>
        /// <param name="spaceName">Name of the space.</param>
        public static void SwitchSpace(string spaceName)
        {
            if (string.IsNullOrEmpty(spaceName))
            {
                spaceName = DEFAULTSPACENAME;
            }
            currentSpace = spaces.ContainsKey(spaceName) ? spaces[spaceName] : null;
        }

        /// <summary>
        /// Gets the current object space.
        /// </summary>
        /// <value>The current object space.</value>
        public static PdfObjectSpace Current
        {
            get
            {
                if (currentSpace == null)
                {
                    CreateSpace(DEFAULTSPACENAME, CompressLevel.Stream);
                    SwitchSpace(DEFAULTSPACENAME);
                }
                return currentSpace;
            }
        }

        /// <summary>
        /// Gets the level of object space.
        /// </summary>
        /// <value>The level.</value>
        public CompressLevel Level
        {
            get { return  this.level; }
        }

        /// <summary>
        /// Compress level of PdfObjectSpace.
        /// 0, is default value. Stream object always need compress.
        /// 1~3 allow more objects to be compressed
        /// 99 will compress all object types. need more time and not useful. just for logic.
        /// </summary>
        public enum CompressLevel
        {
            All = 0x63,
            Array = 2,
            Dictionary = 1,
            Stream = 0,
            String = 3
        }
    }
}

