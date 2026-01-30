#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-09-25 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Base.Report;
using Dt.Cells.Data;
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
#if WIN
    [WinRT.GeneratedBindableCustomProperty]
#else
    [Microsoft.UI.Xaml.Data.Bindable]
#endif
    public partial class RptInfo : RptInfoBase
    {
        #region 成员变量
        // 报表模板缓存
        static readonly Dictionary<string, RptRoot> _tempCache = new Dictionary<string, RptRoot>();
        readonly Dictionary<string, RptData> _dataSet = new Dictionary<string, RptData>(StringComparer.OrdinalIgnoreCase);
        bool _inited;
        #endregion

        #region 属性
        /// <summary>
        /// 最终报表查询时的参数值，初始化后始终存在，查询框的参数值、报表视图的参数值最终存入此处
        /// </summary>
        public Dict Params { get; set; }

        /// <summary>
        /// 是否缓存报表模板，默认true
        /// </summary>
        public bool CacheTemplate { get; set; } = true;

        /// <summary>
        /// 获取设置报表模板根节点
        /// </summary>
        public RptRoot Root { get; set; }
        #endregion

        #region 外部方法
        /// <summary>
        /// 根据数据名称获取数据
        /// </summary>
        /// <param name="p_name">数据名称</param>
        /// <returns></returns>
        public async Task<RptData> GetData(string p_name)
        {
            if (_dataSet.TryGetValue(p_name, out var data))
                return data;

            RptDataSourceItem srcItem;
            if (Root == null || (srcItem = Root.Data.GetDataSourceItem(p_name)) == null)
                return null;

            Table tbl = null;
            if (srcItem.IsScritp)
            {
                // 通过脚本获取数据源
                if (ScriptObj != null)
                {
                    tbl = await ScriptObj.GetData(p_name);
                }
                else
                {
                    Kit.Warn($"未定义报表脚本，无法获取数据【{p_name}】");
                }
            }
            else
            {
                tbl = await Rpt.Query(srcItem.Srv, srcItem.Sql, Params);
            }

            if (tbl != null)
            {
                var rptData = new RptData(tbl);
                _dataSet[p_name] = rptData;
                return rptData;
            }
            return null;
        }
        
        /// <summary>
        /// 更新查询参数
        /// </summary>
        /// <param name="p_row"></param>
        public void UpdateParams(Core.Row p_row)
        {
            if (p_row != null)
                Params = p_row.ToDict();
        }

        /// <summary>
        /// 清除旧数据及旧Sheet
        /// </summary>
        public void ClearData()
        {
            _dataSet.Clear();
            Sheet = null;
        }
        
        /// <summary>
        /// 初始化模板、脚本、参数默认值
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Init()
        {
            if (_inited)
                return Root != null;

            // 加载模板
            _inited = true;
            if (Root == null)
            {
                if (string.IsNullOrEmpty(_uri))
                {
                    Throw.Msg("未提供报表模板路径！");
                }
                else if (CacheTemplate && _tempCache.TryGetValue(_uri, out var temp))
                {
                    // 允许缓存先查找缓存
                    // 通过名称加载模板，代码中写名称可读性比ID高！！！
                    Root = temp;
                }
                else
                {
                    try
                    {
                        string define = await ReadTemplate();
                        Root = await Rpt.DeserializeTemplate(define);

                        if (CacheTemplate)
                        {
                            // 允许缓存，名称作为键
                            _tempCache[_uri] = Root;
                        }
                    }
                    catch (Exception ex)
                    {
                        Throw.Msg("加载报表模板时异常！\r\n" + ex.Message);
                    }
                }

                if (Root == null)
                    return false;
            }

            // 脚本对象
            if (!string.IsNullOrEmpty(Root.ViewSetting.Script))
            {
                var tp = Kit.GetTypeByAlias(typeof(RptScriptAttribute), Root.ViewSetting.Script);
                if (tp != null && tp.IsSubclassOf(typeof(RptScript)))
                {
                    ScriptObj = Activator.CreateInstance(tp) as RptScript;
                }
                else
                {
                    Kit.Warn($"报表缺少继承自RptScript的脚本类型【{Root.ViewSetting.Script}】");
                }
            }

            // 根据参数默认值创建初始查询参数
            if (Root.Params.Data.Count > 0)
            {
                Params = await Root.Params.BuildInitDict();
                if (ScriptObj != null)
                    ScriptObj.InitParams(Params);
            }
            return true;
        }

        /// <summary>
        /// 查询参数是否完备有效
        /// </summary>
        /// <returns></returns>
        public string IsParamsValid()
        {
            if (Root == null)
                return "报表模板对象为空！";

            int count = Root.Params.Data.Count;
            Dict dt = Params;

            // 未提供查询参数
            if (dt == null || dt.Count == 0)
            {
                if (count == 0)
                    return null;
                return "未提供报表查询参数！";
            }

            // 参数个数不够
            if (dt.Count < count)
                return $"报表查询参数{count}个，实际提供{dt.Count}个！";

            // 确保每个参数都包含
            foreach (var row in Root.Params.Data)
            {
                if (!dt.ContainsKey(row.Str("name")))
                {
                    return $"未提供参数【{row.Str("name")}】的值！";
                }
            }
            return null;
        }
        #endregion
        
        #region 内部属性
        /// <summary>
        /// 获取报表要输出的Sheet
        /// </summary>
        internal protected Worksheet Sheet { get; set; }

        /// <summary>
        /// 获取设置报表实例
        /// </summary>
        internal protected RptRootInst Inst { get; set; }

        /// <summary>
        /// 脚本对象
        /// </summary>
        internal protected RptScript ScriptObj { get; set; }
        #endregion
    }
}
