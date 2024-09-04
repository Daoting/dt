#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls.Chart;
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Represents an Excel Image item
    /// </summary>
    public class ExcelImage : IExcelImage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.ExcelImage" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="imageType">Type of the image.</param>
        /// <param name="sourceArray">The source array.</param>
        public ExcelImage(string name, Dt.Xls.ImageType imageType, byte[] sourceArray)
        {
            if ((sourceArray == null) || (sourceArray.Length <= 0))
            {
                throw new ArgumentNullException("sourceArray");
            }
            this.Name = name;
            this.ImageType = imageType;
            this.SourceArray = sourceArray;
            this.Hidden = false;
            this.Locked = true;
        }

        /// <summary>
        /// Gets or sets the anchor
        /// </summary>
        public IAnchor Anchor { get; set; }

        /// <summary>
        /// Represents whether the picture is visible or hidden
        /// </summary>
        public bool Hidden { get; set; }

        /// <summary>
        /// Gets or sets the type of the image.
        /// </summary>
        /// <value>
        /// The type of the image.
        /// </value>
        public Dt.Xls.ImageType ImageType { get; set; }

        /// <summary>
        /// Represents whether the chart is locked
        /// </summary>
        public bool Locked { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Represents the picture format
        /// </summary>
        public IExcelChartFormat PictureFormat { get; set; }

        /// <summary>
        /// Gets or sets the source array.
        /// </summary>
        /// <value>
        /// The source array.
        /// </value>
        public byte[] SourceArray { get; set; }
    }
}

