#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf;
using Dt.Pdf.BaseObject;
using Dt.Pdf.Exceptions;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Pdf.Drawing
{
    /// <summary>
    /// Image Object
    /// </summary>
    public abstract class Image
    {
        private static readonly System.Type[] _ImageTypes = new System.Type[] { typeof(GifImage), typeof(JpegImage), typeof(Jpeg2000Image), typeof(PngImage), typeof(BitmapImage) };
        /// <summary>
        /// Sets the bits-per-component(bpc) of the image.
        /// </summary>
        protected int bpc;
        /// <summary>
        /// Sets the colorspace of the image.
        /// </summary>
        protected int colorspace;
        private int dpiX;
        private int dpiY;
        /// <summary>
        /// Sets the height of the image.
        /// </summary>
        protected float height;
        private Image imageMask;
        private bool mask;
        /// <summary>
        /// Holds value of property originalData.
        /// </summary>
        protected internal byte[] originalData;
        /// <summary>
        /// Holds value of property originalType.
        /// </summary>
        protected internal ImageType originalType;
        /// <summary>
        /// Sets the data.
        /// </summary>
        protected byte[] rawData;
        private bool smask;
        /// <summary> 
        /// this is the transparency information of the raw image
        /// </summary>
        protected int[] transparency;
        /// <summary>
        /// Sets the type of the image.
        /// </summary>
        protected ImageType type;
        /// <summary>
        /// Sets the width of the image.
        /// </summary>
        protected float width;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Image" /> class.
        /// </summary>
        protected Image()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Image" /> class.
        /// </summary>
        /// <param name="image">The image.</param>
        protected Image(Image image)
        {
            this.rawData = image.rawData;
            this.type = image.type;
            this.width = image.width;
            this.colorspace = image.colorspace;
            this.bpc = image.bpc;
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static Image GetInstance(byte[] data)
        {
            if ((data == null) || (data.Length < 4))
            {
                throw new ArgumentNullException("data");
            }
            byte num = data[0];
            byte num2 = data[1];
            byte num3 = data[2];
            byte num4 = data[3];
            for (int i = 0; i < _ImageTypes.Length; i++)
            {
                System.Type type = _ImageTypes[i];
                // hdt
                Type ctParam = typeof(byte);
                if (!type.GetTypeInfo().IsAbstract 
                    && (((type == typeof(GifImage)) || (type == typeof(PngImage))) || ((type == typeof(BitmapImage)) || type.GetTypeInfo().IsSubclassOf(typeof(Image))))
                    && ((bool)type.GetRuntimeMethod("CheckType", new Type[] { ctParam, ctParam, ctParam, ctParam }).Invoke(null, new object[] { (byte)num, (byte)num2, (byte)num3, (byte)num4 })))
                {
                    if (type == typeof(GifImage))
                    {
                        GifImage image = (GifImage) Activator.CreateInstance(type, new object[] { data });
                        return image.GetImage(1);
                    }
                    if (type == typeof(PngImage))
                    {
                        return PngImage.GetImage(data);
                    }
                    if (type == typeof(BitmapImage))
                    {
                        return BitmapImage.GetImage(data);
                    }
                    return (Image) Activator.CreateInstance(type, new object[] { data });
                }
            }
            throw new PdfUnrecognizedImageFormatException("Not a recognized Image Format.");
        }

        /// <summary>
        /// Determines whether [is mask candidate].
        /// </summary>
        /// <returns>
        /// <c>true</c> if [is mask candidate]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsMaskCandidate()
        {
            return (((this is RawImage) && (this.bpc > 0xff)) || (this.colorspace == 1));
        }

        /// <summary>
        /// Makes the mask.
        /// </summary>
        public void MakeMask()
        {
            if (!this.IsMaskCandidate())
            {
                throw new PdfImageException("This image can not be an image mask.");
            }
            this.mask = true;
        }

        /// <summary>
        /// Sets the dpi.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public void SetDpi(int x, int y)
        {
            this.dpiX = x;
            this.dpiY = y;
        }

        /// <summary>
        /// Skips specific bytes from the memory stream.
        /// </summary>
        /// <remarks>
        /// This method is used by the Image Decoder Classes.
        /// </remarks>
        /// <param name="istr">Image stream</param>
        /// <param name="size">Number of bytes to skip</param>
        internal static void Skip(Stream istr, int size)
        {
            while (size > 0)
            {
                byte[] buffer = new byte[size];
                size -= istr.Read(buffer, 0, size);
            }
        }

        /// <summary>
        /// USRs the specified op1.
        /// </summary>
        /// <param name="op1">The op1.</param>
        /// <param name="op2">The op2.</param>
        /// <returns></returns>
        internal static int USR(int op1, int op2)
        {
            if (op2 < 1)
            {
                return op1;
            }
            return (((int) op1) >> op2);
        }

        /// <summary>
        /// Gets or sets the BPC.
        /// </summary>
        /// <value>The BPC.</value>
        public int Bpc
        {
            get { return  this.bpc; }
            set { this.bpc = value; }
        }

        /// <summary>
        /// Gets or sets the colorspace.
        /// </summary>
        /// <value>The colorspace.</value>
        public int Colorspace
        {
            get { return  this.colorspace; }
            set { this.colorspace = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Image" /> is deflated.
        /// </summary>
        /// <value><c>true</c> if deflated; otherwise, <c>false</c>.</value>
        public bool Deflated { get; set; }

        /// <summary>
        /// Gets the dpi X.
        /// </summary>
        /// <value>The dpi X.</value>
        public int DpiX
        {
            get { return  this.dpiX; }
        }

        /// <summary>
        /// Gets the dpi Y.
        /// </summary>
        /// <value>The dpi Y.</value>
        public int DpiY
        {
            get { return  this.dpiY; }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public float Height
        {
            get { return  this.height; }
            set { this.height = value; }
        }

        /// <summary>
        /// Gets or sets the image mask.
        /// </summary>
        /// <value>The image mask.</value>
        public Image ImageMask
        {
            get { return  this.imageMask; }
            set
            {
                if (this.mask)
                {
                    throw new PdfImageException("An image mask cannot contain another image mask.");
                }
                if (!value.mask)
                {
                    throw new PdfImageException("The image mask is false.");
                }
                this.imageMask = value;
                this.smask = (value.bpc > 1) && (value.bpc <= 8);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:Image" /> is mask.
        /// </summary>
        /// <value><c>true</c> if mask; otherwise, <c>false</c>.</value>
        public bool Mask
        {
            get { return  this.mask; }
        }

        /// <summary>
        /// Gets or sets the original data.
        /// </summary>
        /// <value>The original data.</value>
        public byte[] OriginalData
        {
            get { return  this.originalData; }
            internal set { this.originalData = value; }
        }

        /// <summary>
        /// Gets or sets the type of the original.
        /// </summary>
        /// <value>The type of the original.</value>
        internal ImageType OriginalType
        {
            get { return  this.originalType; }
            set { this.originalType = value; }
        }

        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        /// <value>The properties.</value>
        internal PdfDictionary Properties { get; set; }

        /// <summary>
        /// Gets the raw data.
        /// </summary>
        /// <value>The raw data.</value>
        public byte[] RawData
        {
            get { return  this.rawData; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Image" /> is smask.
        /// </summary>
        /// <value><c>true</c> if smask; otherwise, <c>false</c>.</value>
        public bool Smask
        {
            get { return  this.smask; }
            set { this.smask = value; }
        }

        /// <summary>
        /// Gets or sets the transparency.
        /// </summary>
        /// <value>The transparency.</value>
        public int[] Transparency
        {
            get { return  this.transparency; }
            set { this.transparency = value; }
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public ImageType Type
        {
            get { return  this.type; }
            set { this.type = value; }
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public float Width
        {
            get { return  this.width; }
            set { this.width = value; }
        }

        /// <summary>
        /// Gets or sets the XY ratio.
        /// </summary>
        /// <value>The XY ratio.</value>
        public float XYRatio { get; set; }
    }
}

