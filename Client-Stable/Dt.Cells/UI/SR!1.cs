#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents the basic functionality used to get string information from assembly resource.
    /// </summary>
    /// <typeparam name="T">
    /// The type to provide the assembly information.
    /// </typeparam>
    internal class SR<T>
    {
        /// <summary>
        /// SR will use it to reader string.
        /// </summary>
        static readonly ResourceManager _resources;

        /// <summary>
        /// Static constructor.
        /// </summary>
        static SR()
        {
            string[] manifestResourceNames = IntrospectionExtensions.GetTypeInfo((Type) typeof(T)).Assembly.GetManifestResourceNames();
            for (int i = 0; i < manifestResourceNames.Length; i++)
            {
                if (manifestResourceNames[i].StartsWith(typeof(T).FullName, (StringComparison) StringComparison.Ordinal))
                {
                    SR<T>._resources = new ResourceManager(typeof(T));
                    return;
                }
            }
            SR<T>._resources = new ResourceManager(typeof(T).Namespace, IntrospectionExtensions.GetTypeInfo((Type) typeof(T)).Assembly);
        }

        /// <summary>
        /// Returns the value of the resource with the specified name in the assembly indicated by the template type.
        /// </summary>
        /// <param name="name">
        /// Name of the resource to get.
        /// </param>
        /// <returns>
        /// The value of a resource.
        /// </returns>
        public static string GetString(string name)
        {
            return SR<T>.GetString(null, name);
        }

        /// <summary>
        /// Returns the value of the resource localized for the specified culture with the specified name in the assembly 
        /// indicated by the template type.
        /// </summary>
        /// <param name="uiCulture">
        /// The CultureInfo object that represents the culture for which the resource is localized.
        /// </param>
        /// <param name="name">
        /// Name of the resource to get.
        /// </param>
        /// <returns>
        /// The value of a resource.
        /// </returns>
        public static string GetString(CultureInfo uiCulture, string name)
        {
            if (uiCulture == null)
            {
                uiCulture = CultureInfo.CurrentUICulture;
            }
            try
            {
                string str2;
                if (((str2 = name) != null) && (str2 == "AutoFilterList.Blanks"))
                {
                    return ResourceStrings.Filter_Blanks;
                }
                return SR<T>._resources.GetString(name, uiCulture);
            }
            catch
            {
                return name;
            }
        }

        /// <summary>
        /// Returns the value of the resource with the specified name in the assembly indicated by the template type 
        /// and returns the formatted string that replaced the format specification in the String 
        /// with the textual equivalent of the value of a corresponding Object instance in a specified array.
        /// </summary>
        /// <param name="name">
        /// Name of the resource to get.
        /// </param>
        /// <param name="args">
        /// An Object array containing zero or more objects to be formatted. 
        /// </param>
        /// <returns>
        /// The formatted value of a resource.
        /// </returns>
        public static string GetString(string name, params object[] args)
        {
            return SR<T>.GetString(null, name, args);
        }

        /// <summary>
        /// Returns the value of the resource localized for the specified culture with the specified name in the assembly 
        /// indicated by the template type and returns the formatted string that replaced the format specification in the 
        /// String with the textual equivalent of the value of a corresponding Object instance in a specified array.
        /// </summary>
        /// <param name="uiCulture">
        /// The CultureInfo object that represents the culture for which the resource is localized.
        /// </param>
        /// <param name="name">
        /// Name of the resource to get.
        /// </param>
        /// <param name="args">
        /// An Object array containing zero or more objects to be formatted. 
        /// </param>
        /// <returns>
        /// The formatted value of a resource.
        /// </returns>
        public static string GetString(CultureInfo uiCulture, string name, params object[] args)
        {
            if (uiCulture == null)
            {
                uiCulture = CultureInfo.CurrentUICulture;
            }
            try
            {
                string format = SR<T>._resources.GetString(name, uiCulture);
                if ((args != null) && (args.Length > 0))
                {
                    return string.Format(format, args);
                }
                return format;
            }
            catch
            {
                return name;
            }
        }
    }
}

