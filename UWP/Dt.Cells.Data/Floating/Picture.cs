#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a picture in spread. 
    /// </summary>
    public class Picture : SpreadChartShapeBase
    {
        ImageSource _imageSource;
        PictureSerializationMode _serializationMode;
        Stretch _stretch;
        Uri _uriSource;
        internal string ImageByteArrayBase64String;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.Picture" /> class.
        /// </summary>
        public Picture()
        {
            _stretch = Stretch.Uniform;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.Picture" /> class.
        /// </summary>
        /// <param name="name">The picture name.</param>
        /// <param name="imageStream">The image stream of the picture.</param>
        public Picture(string name, Stream imageStream) : base(name)
        {
            _stretch = Stretch.Uniform;
            InitImageSource(imageStream);
            UpdateToPreferredSize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.Picture" /> class.
        /// </summary>
        /// <param name="name">The picture name.</param>
        /// <param name="source">The uri source of the picture.</param>
        public Picture(string name, Uri source) : base(name)
        {
            _stretch = Stretch.Uniform;
            _uriSource = source;
            UpdateToPreferredSize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.Picture" /> class.
        /// </summary>
        /// <param name="name">The picture name.</param>
        /// <param name="source">The image source of the picture.</param>
        public Picture(string name, ImageSource source) : base(name)
        {
            _stretch = Stretch.Uniform;
            _imageSource = source;
            UpdateToPreferredSize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.Picture" /> class.
        /// </summary>
        /// <param name="name">The picture name.</param>
        /// <param name="imageStream">The image stream of the picture.</param>
        /// <param name="x">The x location of the picture.</param>
        /// <param name="y">The y location of the picture.</param>
        public Picture(string name, Stream imageStream, double x, double y) : base(name, x, y)
        {
            _stretch = Stretch.Uniform;
            InitImageSource(imageStream);
            UpdateToPreferredSize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.Picture" /> class.
        /// </summary>
        /// <param name="name">The picture name.</param>
        /// <param name="source">The uri source of the picture.</param>
        /// <param name="x">The x location of the picture.</param>
        /// <param name="y">The y location of the picture.</param>
        public Picture(string name, Uri source, double x, double y) : base(name, x, y)
        {
            _stretch = Stretch.Uniform;
            _uriSource = source;
            UpdateToPreferredSize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.Picture" /> class.
        /// </summary>
        /// <param name="name">The picture name.</param>
        /// <param name="source">The image source of the picture.</param>
        /// <param name="x">The x location of the picture.</param>
        /// <param name="y">The y location of the picture.</param>
        public Picture(string name, ImageSource source, double x, double y) : base(name, x, y)
        {
            _stretch = Stretch.Uniform;
            _imageSource = source;
            UpdateToPreferredSize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.Picture" /> class.
        /// </summary>
        /// <param name="name">The picture name.</param>
        /// <param name="imageStream">The image stream of the picture.</param>
        /// <param name="x">The x location of the picture.</param>
        /// <param name="y">The y location of the picture.</param>
        /// <param name="width">The picture width.</param>
        /// <param name="height">The picture height.</param>
        public Picture(string name, Stream imageStream, double x, double y, double width, double height) : base(name, x, y, width, height)
        {
            _stretch = Stretch.Uniform;
            InitImageSource(imageStream);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.Picture" /> class.
        /// </summary>
        /// <param name="name">The picture name.</param>
        /// <param name="source">The uri source of the picture.</param>
        /// <param name="x">The x location of the picture.</param>
        /// <param name="y">The y location of the picture.</param>
        /// <param name="width">The picture width.</param>
        /// <param name="height">The picture height.</param>
        public Picture(string name, Uri source, double x, double y, double width, double height) : base(name, x, y, width, height)
        {
            _stretch = Stretch.Uniform;
            _uriSource = source;
            _imageSource = GetActualImageSource();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.Picture" /> class.
        /// </summary>
        /// <param name="name">The picture name.</param>
        /// <param name="source">The image source of the picture.</param>
        /// <param name="x">The x location of the picture.</param>
        /// <param name="y">The y location of the picture.</param>
        /// <param name="width">The picture width.</param>
        /// <param name="height">The picture height.</param>
        public Picture(string name, ImageSource source, double x, double y, double width, double height) : base(name, x, y, width, height)
        {
            _stretch = Stretch.Uniform;
            _imageSource = source;
        }

        internal ImageSource GetActualImageSource()
        {
            if (_imageSource != null)
                return _imageSource;

            if (_uriSource != null)
            {
                // hdt
                BitmapImage imageSource = new BitmapImage();
                StorageFile.GetFileFromApplicationUriAsync(_uriSource).AsTask().ContinueWith((fr) =>
                {
                    if ((fr.Result != null) && !fr.IsFaulted)
                    {
                        Action<Task<IRandomAccessStreamWithContentType>> func = delegate (Task<IRandomAccessStreamWithContentType> r)
                        {
                            using (Stream stream = WindowsRuntimeStreamExtensions.AsStreamForRead(r.Result))
                            {
                                // Utility.InitImageSource(imageSource, stream);
                                //导出RptText ImageUri时图片不出问题 李雪修改
                                InitImageSource(stream);
                            }
                        };
                        WindowsRuntimeSystemExtensions.AsTask<IRandomAccessStreamWithContentType>(fr.Result.OpenReadAsync()).ContinueWith(func);
                    }
                });
                return imageSource;
            }
            return null;
        }

        Size? GetPicturPreferredSize()
        {
            ImageSource imageSource = GetActualImageSource();
            if ((imageSource != null) && (imageSource is BitmapSource))
            {
                BitmapSource source = imageSource as BitmapSource;
                return new Windows.Foundation.Size((double)source.PixelWidth, (double)source.PixelHeight);
            }
            return null;
        }

        internal override IFloatingObjectSheet GetSheetFromOwner()
        {
            if (base.Owner != null)
            {
                return (base.Owner as SpreadPictures).Sheet;
            }
            return null;
        }

        void InitImageSource(Stream imageStream)
        {
            byte[] buffer = new byte[imageStream.Length];
            imageStream.Seek(0L, SeekOrigin.Begin);
            imageStream.Read(buffer, 0, (int)imageStream.Length);
            ImageByteArrayBase64String = Convert.ToBase64String(buffer);
            BitmapImage image = new BitmapImage();
            Utility.InitImageSource(image, imageStream);
            _imageSource = image;
        }

        internal override void OnPropertyChanged(string propertyName)
        {
            if (base.Worksheet != null)
            {
                base.Worksheet.RaisePictureChanged(this, propertyName);
                if (propertyName == "IsSelected")
                {
                    base.Worksheet.RaisePictureSelectionChangedEvent(this);
                }
            }
        }

        internal override void ReadXmlInternal(XmlReader reader)
        {
            string str;
            base.ReadXmlInternal(reader);
            if ((reader.NodeType == ((XmlNodeType)((int)XmlNodeType.Element))) && ((str = reader.Name) != null))
            {
                if (str != "SerializationMode")
                {
                    if (str != "ImageSource")
                    {
                        if (str == "Stretch")
                        {
                            _stretch = (Stretch)Serializer.DeserializeObj(typeof(Stretch), reader);
                        }
                        return;
                    }
                }
                else
                {
                    _serializationMode = (PictureSerializationMode)Serializer.DeserializeObj(typeof(PictureSerializationMode), reader);
                    return;
                }
                _imageSource = Serializer.DeserializeImage(reader, out ImageByteArrayBase64String);
            }
        }

        void UpdateToPreferredSize()
        {
            ImageSource imageSource = GetActualImageSource();
            if (imageSource is BitmapImage)
            {
                BitmapImage bmpImage = imageSource as BitmapImage;
                bmpImage.ImageOpened += (sender, e) => { Size = new Size(bmpImage.PixelWidth, bmpImage.PixelHeight); };
            }
        }
        
        internal override void WriteXmlInternal(XmlWriter writer)
        {
            base.WriteXmlInternal(writer);
            ImageSource imageSource = GetActualImageSource();
            Serializer.SerializeObj(_serializationMode, "SerializationMode", writer);
            if (!string.IsNullOrEmpty(ImageByteArrayBase64String))
            {
                Serializer.SerializeObj(ImageByteArrayBase64String, "ImageSource", writer);
            }
            else if (imageSource != null)
            {
                Stream stream = null;
                RenderTargetBitmap rtb = imageSource as RenderTargetBitmap;
                if (rtb != null)
                {
                    // hdt 新增，打印时保存成xml时用
                    stream = Utility.GetBmpStream(rtb);
                }
                else if(imageSource is BitmapSource)
                {
                    ImageFormat imageFormat = Utility.GetImageFormat(_uriSource);
                    stream = Utility.GetImageStream(imageSource, imageFormat, SerializationMode);
                }

                if (stream != null)
                {
                    byte[] buffer = new byte[stream.Length];
                    stream.Seek(0L, SeekOrigin.Begin);
                    stream.Read(buffer, 0, (int)stream.Length);
                    Serializer.SerializeObj(Convert.ToBase64String(buffer), "ImageSource", writer);
                }
            }
            if (_stretch != Stretch.Uniform)
                Serializer.SerializeObj(_stretch, "Stretch", writer);
        }

        async Task GetPixelsBuffer(RenderTargetBitmap p_rtb)
        {
            var buffer = await p_rtb.GetPixelsAsync();
        }

        /// <summary>
        /// Gets or sets a <see cref="T:Brush" /> object that describes the automatic background for a chart.
        /// </summary>
        /// <value>
        /// The <see cref="T:Brush" /> object that describes the automatic background for a chart.
        /// </value>
        public override Brush AutomaticFill
        {
            get { return null; }
        }

        /// <summary>
        /// Gets or sets a <see cref="T:Brush" /> object that describes the automatic outline for a chart.
        /// </summary>
        /// <value>
        /// The <see cref="T:Brush" /> object that describes the automatic outline for a chart.
        /// </value>
        public override Brush AutomaticStroke
        {
            get { return null; }
        }

        /// <summary>
        /// Gets or sets the image source of the picture.
        /// </summary>
        /// <value>
        /// The image source of the picture.
        /// </value>
        public ImageSource ImageSource
        {
            get { return _imageSource; }
            set
            {
                _uriSource = null;
                ImageByteArrayBase64String = null;
                if (value != ImageSource)
                {
                    _imageSource = value;
                    UpdateToPreferredSize();
                    base.RaisePropertyChanged("ImageSource");
                }
            }
        }

        /// <summary>
        /// Gets or sets the picture stretch.
        /// </summary>
        /// <value>
        /// The stretch of the picture.
        /// </value>
        public Stretch PictureStretch
        {
            get { return _stretch; }
            set
            {
                if (value != PictureStretch)
                {
                    _stretch = value;
                    base.RaisePropertyChanged("Stretch");
                }
            }
        }

        /// <summary>
        /// Gets or sets the serialization mode used for picture serialization.
        /// </summary>
        /// <value>
        /// The serialization mode used for picture serialization.
        /// </value>
        public PictureSerializationMode SerializationMode
        {
            get { return _serializationMode; }
            set
            {
                if (value != SerializationMode)
                {
                    _serializationMode = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the URI source of the picture.
        /// </summary>
        /// <value>
        /// The URI source of the picture.
        /// </value>
        public Uri UriSource
        {
            get { return _uriSource; }
            set
            {
                _imageSource = null;
                ImageByteArrayBase64String = null;
                if (value != UriSource)
                {
                    _uriSource = value;
                    UpdateToPreferredSize();
                    base.RaisePropertyChanged("Uri");
                }
            }
        }
    }
}

