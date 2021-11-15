#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a spread theme class.
    /// </summary>
    public class SpreadTheme : ICloneable, IXmlSerializable
    {
        FontFamily bodyFontFamily;
        string bodyFontName;
        ThemeColor colors;
        FontFamily headingFontFamily;
        string headingFontName;
        bool isDirty;
        string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.SpreadTheme" /> class.
        /// </summary>
        public SpreadTheme() : this("")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.SpreadTheme" /> class.
        /// </summary>
        /// <param name="name">The name of the spread theme.</param>
        public SpreadTheme(string name) : this(name, ThemeColors.Office, NameConstans.DEFAULT_FONT_FAMILY, NameConstans.DEFAULT_FONT_FAMILY)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.SpreadTheme" /> class.
        /// </summary>
        /// <param name="name">The name of the spread theme.</param>
        /// <param name="colors">The base colors of the theme color.</param>
        /// <param name="headingFontName">Name of the heading font.</param>
        /// <param name="bodyFontName">Name of the body font.</param>
        public SpreadTheme(string name, ThemeColor colors, string headingFontName, string bodyFontName)
        {
            this.isDirty = false;
            this.name = name;
            this.colors = colors;
            this.headingFontName = headingFontName;
            this.bodyFontName = bodyFontName;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            return new SpreadTheme(this.name, (this.colors == null) ? null : (this.colors.Clone() as ThemeColor), this.headingFontName, this.bodyFontName);
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            Serializer.InitReader(reader);
            this.name = null;
            this.colors = null;
            this.headingFontName = null;
            this.headingFontFamily = null;
            this.bodyFontName = null;
            this.bodyFontFamily = null;
            this.isDirty = false;
            while (reader.Read())
            {
                string str;
                if ((reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element))) && ((str = reader.Name) != null))
                {
                    if (str != "ThemeColors")
                    {
                        if (str == "Name")
                        {
                            goto Label_00B1;
                        }
                        if (str == "HeadingFont")
                        {
                            goto Label_00CE;
                        }
                        if (str == "BodyFont")
                        {
                            goto Label_00EB;
                        }
                    }
                    else
                    {
                        this.colors = new ThemeColor();
                        XmlReader reader2 = Serializer.ExtractNode(reader);
                        Serializer.DeserializePublicProperties(this.colors, reader2);
                        reader2.Close();
                    }
                }
                continue;
            Label_00B1:
                this.name = (string) ((string) Serializer.DeserializeObj(typeof(string), reader));
                continue;
            Label_00CE:
                this.headingFontName = (string) ((string) Serializer.DeserializeObj(typeof(string), reader));
                continue;
            Label_00EB:
                this.bodyFontName = (string) ((string) Serializer.DeserializeObj(typeof(string), reader));
            }
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            Serializer.InitWriter(writer);
            if (this.colors != null)
            {
                Serializer.WriteStartObj("ThemeColors", writer);
                Serializer.SerializePublicProperties(this.colors, writer);
                Serializer.WriteEndObj(writer);
            }
            if (this.name != null)
            {
                Serializer.SerializeObj(this.name, "Name", writer);
            }
            if (this.headingFontName != null)
            {
                Serializer.SerializeObj(this.headingFontName, "HeadingFont", writer);
            }
            if (this.bodyFontName != null)
            {
                Serializer.SerializeObj(this.bodyFontName, "BodyFont", writer);
            }
        }

        /// <summary>
        /// Gets the body font family.
        /// </summary>
        public FontFamily BodyFontFamily
        {
            get
            {
                if ((this.bodyFontFamily == null) && !string.IsNullOrEmpty(this.bodyFontName))
                {
                    bodyFontFamily = new FontFamily(this.bodyFontName);
                }
                return this.bodyFontFamily;
            }
        }

        /// <summary>
        /// Gets or sets the name of the body font.
        /// </summary>
        /// <value>
        /// The name of the body font.
        /// </value>
        public string BodyFontName
        {
            get { return  this.bodyFontName; }
            set
            {
                this.bodyFontName = value;
                this.bodyFontFamily = null;
                this.isDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the base colors of the theme.
        /// </summary>
        /// <value>
        /// The base colors of the theme.
        /// </value>
        public ThemeColor Colors
        {
            get { return  this.colors; }
            set
            {
                this.colors = value;
                this.isDirty = true;
            }
        }

        /// <summary>
        /// Gets the heading font family.
        /// </summary>
        public FontFamily HeadingFontFamily
        {
            get
            {
                if ((this.headingFontFamily == null) && !string.IsNullOrEmpty(this.headingFontName))
                {
                    try
                    {
                        this.headingFontFamily = new FontFamily(this.headingFontName);
                    }
                    catch
                    {
                    }
                }
                return this.headingFontFamily;
            }
        }

        /// <summary>
        /// Gets or sets the name of the heading font.
        /// </summary>
        /// <value>
        /// The name of the heading font.
        /// </value>
        public string HeadingFontName
        {
            get { return  this.headingFontName; }
            set
            {
                this.headingFontName = value;
                this.headingFontFamily = null;
                this.isDirty = true;
            }
        }

        internal bool IsDirty
        {
            get { return  (((this.colors != null) && this.colors.IsDirty) || this.isDirty); }
        }

        /// <summary>
        /// Gets or sets the name of the current theme.
        /// </summary>
        /// <value>
        /// The name of the current theme.
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
    }
}

