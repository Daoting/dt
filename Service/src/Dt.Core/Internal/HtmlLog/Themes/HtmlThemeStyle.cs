﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-01-22 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Core.HtmlLog
{
    /// <summary>
    /// Elements styled by a console theme.
    /// </summary>
    public enum HtmlThemeStyle
    {
        /// <summary>
        /// Prominent text, generally content within an event's message.
        /// </summary>
        Text,

        /// <summary>
        /// Boilerplate text, for example items specified in an output template.
        /// </summary>
        SecondaryText,

        /// <summary>
        /// De-emphasized text, for example literal text in output templates and
        /// punctuation used when writing structured data.
        /// </summary>
        TertiaryText,

        /// <summary>
        /// Output demonstrating some kind of configuration issue, e.g. an invalid
        /// message template token.
        /// </summary>
        Invalid,

        /// <summary>
        /// The built-in <see langword="null"/> value.
        /// </summary>
        Null,

        /// <summary>
        /// Property and type names.
        /// </summary>
        Name,

        /// <summary>
        /// Strings.
        /// </summary>
        String,

        /// <summary>
        /// Numbers.
        /// </summary>
        Number,

        /// <summary>
        /// <see cref="bool"/> values.
        /// </summary>
        Boolean,

        /// <summary>
        /// All other scalar values, e.g. <see cref="System.Guid"/> instances.
        /// </summary>
        Scalar,

        /// <summary>
        /// Level indicator.
        /// </summary>
        LevelVerbose,

        /// <summary>
        /// Level indicator.
        /// </summary>
        LevelDebug,

        /// <summary>
        /// Level indicator.
        /// </summary>
        LevelInformation,

        /// <summary>
        /// Level indicator.
        /// </summary>
        LevelWarning,

        /// <summary>
        /// Level indicator.
        /// </summary>
        LevelError,

        /// <summary>
        /// Level indicator.
        /// </summary>
        LevelFatal,
    }
}
