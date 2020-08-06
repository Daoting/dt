#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.ObjectModel;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a format.
    /// </summary>
    internal abstract class FormatPartBase
    {
        /// <summary>
        /// 
        /// </summary>
        string originalToken;
        /// <summary>
        /// the supported part format.
        /// </summary>
        static Collection<Type> supportedPartFormat;

        /// <summary>
        /// Creates a new format.
        /// </summary>
        /// <param name="token">The token.</param>
        protected FormatPartBase(string token)
        {
            this.originalToken = token;
        }

        /// <summary>
        /// Creates the specified token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>Returns the format part object.</returns>
        public static FormatPartBase Create(string token)
        {
            if (ConditionFormatPart.EvaluateFormat(token))
            {
                return new ConditionFormatPart(token);
            }
            if (DBNumberFormatPart.EvaluateFormat(token))
            {
                return new DBNumberFormatPart(token);
            }
            if (LocaleIDFormatPart.EvaluateFormat(token))
            {
                return new LocaleIDFormatPart(token);
            }
            if (ABSTimeFormatPart.EvaluateFormat(token))
            {
                return new ABSTimeFormatPart(token);
            }
            if (ColorFormatPart.EvaluateFormat(token))
            {
                return new ColorFormatPart(token);
            }
            return null;
        }

        internal string OriginalToken
        {
            get { return  this.originalToken; }
        }

        /// <summary>
        /// Gets the supported part format.
        /// </summary>
        /// <value>The supported part format.</value>
        public static Collection<Type> SupportedPartFormat
        {
            get
            {
                if (supportedPartFormat == null)
                {
                    supportedPartFormat = new Collection<Type>();
                    supportedPartFormat.Add(typeof(ConditionFormatPart));
                    supportedPartFormat.Add(typeof(ColorFormatPart));
                    supportedPartFormat.Add(typeof(LocaleIDFormatPart));
                }
                return supportedPartFormat;
            }
        }
    }
}

