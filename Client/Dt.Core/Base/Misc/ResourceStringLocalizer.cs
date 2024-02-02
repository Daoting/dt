#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-12-29 创建
******************************************************************************/
#endregion

#region 引用命名
#if WIN
using Microsoft.Windows.ApplicationModel.Resources;
#else
using Windows.ApplicationModel.Resources;
#endif
using Microsoft.Extensions.Localization;
using System.Globalization;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// This implementation of <see cref="IStringLocalizer"/> uses ResourceLoader on Uno and ResourceManager on WinAppSdk
    /// to get the string resources.
    /// </summary>
    public class ResourceStringLocalizer : IStringLocalizer
    {
        const string SearchLocation = "Resources";
#if WIN
        readonly ResourceMap _defaultResourceMap;
        readonly ResourceMap _appResourceMap;
#else
        readonly ResourceLoader _defaultResourceLoader;
        readonly ResourceLoader _appResourceLoader;

#endif
        readonly bool _treatEmptyAsNotFound;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceLoaderStringLocalizer"/> class.
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="treatEmptyAsNotFound">If empty strings should be treated as not found.</param>
        public ResourceStringLocalizer(string assemblyName, bool treatEmptyAsNotFound = true)
        {
            _treatEmptyAsNotFound = treatEmptyAsNotFound;
#if WIN
            var mainResourceMap = new ResourceManager().MainResourceMap;
            // TryGetSubtree can return null if no resources found, so defalut to main resource map if not found
            _defaultResourceMap = mainResourceMap.TryGetSubtree(SearchLocation) ?? mainResourceMap;
            _appResourceMap = mainResourceMap.TryGetSubtree(assemblyName)?.TryGetSubtree(SearchLocation) ?? _defaultResourceMap;
#else
            _defaultResourceLoader = ResourceLoader.GetForViewIndependentUse();
            try
            {
                _appResourceLoader = new ResourceLoader($"{assemblyName}/{SearchLocation}");
            }
            catch { }
#endif
        }

        /// <inheritdoc/>
        public LocalizedString this[string name] => GetLocalizedString(name);

        /// <inheritdoc/>
        public LocalizedString this[string name, params object[] arguments] => GetLocalizedString(name, arguments);

        /// <inheritdoc/>
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
            => throw new NotSupportedException("ResourceLoader doesn't support listing all strings.");

        LocalizedString GetLocalizedString(string name, params object[] arguments)
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            string resource = null;
            try
            {
#if WIN
                resource = _appResourceMap.GetValue(name)?.ValueAsString ??
                                _defaultResourceMap.GetValue(name)?.ValueAsString;
#else
                resource = _appResourceLoader?.GetString(name) ??
                    _defaultResourceLoader.GetString(name);
#endif

                if (_treatEmptyAsNotFound &&
                    string.IsNullOrEmpty(resource))
                {
                    resource = null;
                }
            }
            catch
            {
                resource = null;
            }

            var notFound = resource == null;

            if (notFound &&
                name.Contains("."))
            {
                return GetLocalizedString(name.Replace(".", "/"));
            }

            resource ??= name;

            var value = !notFound && arguments.Any()
                ? string.Format(CultureInfo.CurrentCulture, resource, arguments)
                : resource;

            return new LocalizedString(name, value, resourceNotFound: notFound, searchedLocation: SearchLocation);
        }
    }
}