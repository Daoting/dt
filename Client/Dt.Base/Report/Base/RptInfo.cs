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
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 报表描述信息
    /// 报表模板三种方式优先级：
    /// 1. 直接提供RptRoot对象，内部使用，如报表编辑时预览
    /// 2. 重写 ReadTemplate 方法，模板在其他位置时
    /// 3. 默认通过Name查询本地db数据加载模板
    /// </summary>
    public class RptInfo
    {
        #region 成员变量
        // 报表模板缓存
        static readonly Dictionary<string, RptRoot> _tempCache = new Dictionary<string, RptRoot>();
        const string _paramsMsg = "报表查询参数不完整！";
        readonly Dictionary<string, RptData> _dataSet = new Dictionary<string, RptData>(StringComparer.OrdinalIgnoreCase);
        RptScript _scriptObj;
        #endregion

        /// <summary>
        /// 获取设置报表名称，作为唯一标识识别窗口用
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 获取设置报表数据的查询参数，初始化时做为预输入参数
        /// </summary>
        public Dict Params { get; set; }

        /// <summary>
        /// 是否缓存报表模板，默认true
        /// </summary>
        public bool CacheTemplate { get; set; } = true;

        /// <summary>
        /// 读取模板内容
        /// </summary>
        /// <returns></returns>
        public virtual Task<string> ReadTemplate()
        {
            return Task.Run(() =>
            {
                string define = AtLocal.GetModelScalar<string>("select define from OmReport where name=:name", new Dict { { "name", Name } });
                if (string.IsNullOrEmpty(define))
                    AtKit.Warn($"未找到报表模板【{Name}】！");
                return define;
            });
        }

        /// <summary>
        /// 获取报表要输出的Sheet
        /// </summary>
        internal Worksheet Sheet { get; set; }

        /// <summary>
        /// 获取设置报表模板根节点
        /// </summary>
        internal RptRoot Root { get; set; }

        /// <summary>
        /// 获取设置报表实例
        /// </summary>
        internal RptRootInst Inst { get; set; }

        /// <summary>
        /// 报表预览菜单
        /// </summary>
        internal Menu ViewMenu { get; set; }

        internal RptScript GetScriptObj()
        {
            if (_scriptObj != null)
                return _scriptObj;

            Type type;
            if (Root == null 
                || string.IsNullOrEmpty(Root.ViewSetting.Script)
                || (type = Type.GetType(Root.ViewSetting.Script)) == null)
                return null;

            _scriptObj = (RptScript)Activator.CreateInstance(type);
            return _scriptObj;
        }

        /// <summary>
        /// 初始化报表模板
        /// </summary>
        /// <returns></returns>
        internal async Task<bool> InitTemplate()
        {
            if (Root != null)
                return true;

            if (string.IsNullOrEmpty(Name))
            {
                AtKit.Warn("未提供报表模板名称！");
                return false;
            }

            // 允许缓存先查找缓存
            // 通过名称加载模板，代码中写名称可读性比ID高！！！
            if (CacheTemplate && _tempCache.TryGetValue(Name, out var temp))
            {
                Root = temp;
                return true;
            }

            try
            {
                string define = await ReadTemplate();
                Root = await AtRpt.DeserializeTemplate(define);

                if (CacheTemplate)
                {
                    // 允许缓存，名称作为键
                    _tempCache[Name] = Root;
                }
                return true;
            }
            catch (Exception ex)
            {
                AtKit.Warn("加载报表模板时异常！\r\n" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 获取数据表
        /// </summary>
        /// <param name="p_name">数据源名称</param>
        internal async Task<RptData> GetData(string p_name)
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
                var script = GetScriptObj();
                if (script != null)
                {
                    tbl = await script.GetData(p_name);
                }
                else
                {
                    AtKit.Warn($"未定义报表脚本，无法获取数据集 {p_name}");
                }
            }
            else
            {

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
        /// 查询参数是否完备有效
        /// </summary>
        /// <param name="p_info"></param>
        /// <returns></returns>
        internal bool IsParamsValid()
        {
            if (Root == null)
            {
                AtKit.Msg("未加载报表模板，无法验证查询参数！");
                return false;
            }

            int count = Root.Params.Data.Count;
            Dict dt = Params;

            // 未提供查询参数
            if (dt == null || dt.Count == 0)
            {
                if (count > 0)
                {
                    AtKit.Msg(_paramsMsg);
                    return false;
                }
                return true;
            }

            // 参数个数不够
            if (dt.Count < count)
            {
                AtKit.Msg(_paramsMsg);
                return false;
            }

            // 确保每个参数都包含
            foreach (var row in Root.Params.Data)
            {
                if (!dt.ContainsKey(row.Str("id")))
                {
                    AtKit.Msg(_paramsMsg);
                    return false;
                }
            }
            return true;
        }

        #region 比较
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is RptInfo))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            // 只比较标识，识别窗口用
            return Name == ((RptInfo)obj).Name;
        }

        public override int GetHashCode()
        {
            if (string.IsNullOrEmpty(Name))
                return 0;
            return Name.GetHashCode();
        }
        #endregion
    }
}
