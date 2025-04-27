#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-09-25 创建
**************************************************************************/
#endregion

#region 命名空间
using System.Reflection;
using Windows.Storage;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 报表描述信息
    /// 提供报表模板三种方式优先级：
    /// 1. 直接提供RptRoot对象，内部使用，如报表编辑时预览
    /// 2. 重写 ReadTemplate 方法，模板在其他位置时
    /// 3. 默认通过Uri查询模板，支持4种格式：rpt local ms-appx embedded
    /// </summary>
    public abstract class RptInfoBase
    {
        protected string _uri;
        protected RptUriType _uriType;

        /// <summary>
        /// 获取设置报表模板路径，作为唯一标识，支持4种格式：
        /// <para>cm_rpt表：rpt://模板名称</para>
        /// <para>本地sqlite库：local://库名/模板名称</para>
        /// <para>内容文件：ms-appx:///Dt.UIDemo/Report/模板名称.rpt，android不支持路径中文</para>
        /// <para>嵌入资源：embedded://程序集名/完整路径.模板名称.rpt</para>
        /// </summary>
        public string Uri
        {
            get { return _uri; }
            set
            {
                if (_uri == value || string.IsNullOrEmpty(value))
                    return;

                _uri = value;
                if (_uri.StartsWith("rpt://", StringComparison.OrdinalIgnoreCase))
                {
                    Name = _uri.Substring(6);
                    _uriType = RptUriType.Default;
                }
                else if (_uri.StartsWith("ms-appx:///", StringComparison.OrdinalIgnoreCase))
                {
                    int index = _uri.LastIndexOf('/');
                    if (index > -1)
                    {
                        Name = _uri.Substring(index + 1);
                        index = Name.LastIndexOf('.');
                        if (index > -1)
                            Name = Name.Substring(0, index);
                    }
                    _uriType = RptUriType.Content;
                }
                else if (_uri.StartsWith("embedded://", StringComparison.OrdinalIgnoreCase))
                {
                    int index = _uri.LastIndexOf('/');
                    if (index > -1)
                    {
                        var arr = _uri.Substring(index + 1).Split('.');
                        if (arr.Length > 2)
                        {
                            Name = arr[arr.Length - 2];
                        }
                        else
                        {
                            Throw.Msg("报表模板路径格式错误：" + _uri);
                        }
                    }
                    _uriType = RptUriType.Embedded;
                }
                else if (_uri.StartsWith("local://", StringComparison.OrdinalIgnoreCase))
                {
                    var arr = _uri.Substring(8).Split('/');
                    if (arr.Length != 2)
                        Throw.Msg("本地库报表模板路径格式错误：" + _uri);
                    Name = arr[1];
                    _uriType = RptUriType.Local;
                }
                else
                {
                    Throw.Msg("不支持报表模板路径格式：" + _uri);
                }
            }
        }

        /// <summary>
        /// 获取设置报表名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 读取模板内容，重写可自定义读取模板过程
        /// </summary>
        /// <returns></returns>
        public virtual async Task<string> ReadTemplate()
        {
            // 默认report本地库
            if (_uriType == RptUriType.Default)
            {
                var da = new AgentInfo(AccessType.Local, "report").GetAccessInfo().GetDa();
                string define = await da.GetScalar<string>($"select define from OmReport where name='{Name}'");
                if (string.IsNullOrEmpty(define))
                    throw new Exception("未找到报表模板：" + _uri);
                return define;
            }

            // 内容文件
            if (_uriType == RptUriType.Content)
            {
                var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(_uri));
                if (file == null)
                    throw new Exception("未找到报表模板：" + _uri);

                using (var stream = await file.OpenStreamForReadAsync())
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }

            // 嵌入资源
            if (_uriType == RptUriType.Embedded)
            {
                var arr = _uri.Substring(11).Split('/');
                if (arr.Length != 2)
                    throw new Exception("报表模板路径格式错误：" + _uri);

                var asm = Assembly.Load(arr[0]);
                if (asm == null)
                    throw new Exception("报表模板路径格式错误：" + _uri);

                try
                {
                    using (var stream = asm.GetManifestResourceStream(arr[1]))
                    using (var reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
                catch
                {
                    throw new Exception("未找到报表模板：" + _uri);
                }
            }

            // 本地库
            if (_uriType == RptUriType.Local)
            {
                var ls = _uri.Substring(8).Split('/');
                if (ls.Length != 2)
                    throw new Exception("本地库报表模板路径格式错误：" + _uri);

                var da = new AgentInfo(AccessType.Local, ls[0]).GetAccessInfo().GetDa();
                string define = await da.GetScalar<string>($"select define from OmReport where name='{Name}'");
                if (string.IsNullOrEmpty(define))
                    throw new Exception("未找到报表模板：" + _uri);
                return define;
            }

            throw new Exception("未找到报表模板：" + _uri);
        }

        #region 比较
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is RptInfoBase))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            // 只比较标识，识别窗口用
            return Uri == ((RptInfoBase)obj).Uri;
        }

        public override int GetHashCode()
        {
            if (string.IsNullOrEmpty(Uri))
                return 0;
            return Uri.GetHashCode();
        }
        #endregion
    }

    public enum RptUriType
    {
        Default,
        Local,
        Content,
        Embedded
    }
}
