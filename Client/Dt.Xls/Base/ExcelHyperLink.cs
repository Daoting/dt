#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Represents the hyperlink in excel. Every hyperlink can contain a text mark and a description that 
    /// is show in the sheet instead of the read link.
    /// </summary>
    public class ExcelHyperLink : IExcelHyperLink
    {
        private string _address;
        private HyperLinkType _type = HyperLinkType.URL;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.ExcelHyperLink" /> class.
        /// </summary>
        /// <param name="description">The description text that is show in the excel instead of the real link</param>
        /// <param name="uri">The real address of the link location</param>
        /// <exception cref="T:System.ArgumentException">throws when the uri is not a valid uri</exception>
        /// <remarks>If the passed description is <see langword="null" />, it will use the uri as the description.</remarks>
        public ExcelHyperLink(string description, string uri)
        {
            if (IsValidUri(uri))
            {
                this._address = uri;
                if (string.IsNullOrWhiteSpace(description))
                {
                    this.Description = uri;
                }
                else
                {
                    this.Description = description;
                }
            }
            else
            {
                if (!IsValidUri(UrlHelper.Decode(uri)))
                {
                    throw new ArgumentException(ResourceHelper.GetResourceString("invalidUri"));
                }
                uri = UrlHelper.Decode(uri);
                this._address = uri;
                if (string.IsNullOrWhiteSpace(description))
                {
                    this.Description = uri;
                }
                else
                {
                    this.Description = description;
                }
            }
        }

        internal static bool IsValidUri(string uri)
        {
            if (string.IsNullOrWhiteSpace(uri))
            {
                return false;
            }
            try
            {
                new Uri(uri);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// The address of the real link location.
        /// </summary>
        public string Address
        {
            get { return  this._address; }
            set
            {
                if (!IsValidUri(value))
                {
                    throw new ArgumentException(ResourceHelper.GetResourceString("invalidUri"));
                }
                this._address = value;
            }
        }

        /// <summary>
        /// The description text that is show in the excel instead of the real link.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the hyperlink type
        /// </summary>
        public HyperLinkType Type
        {
            get { return  this._type; }
        }
    }
}

