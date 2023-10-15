﻿#region 文件描述
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
    /// 提供报表模板三种方式优先级：
    /// 1. 直接提供RptRoot对象，内部使用，如报表编辑时预览
    /// 2. 重写 ReadTemplate 方法，模板在其他位置时
    /// 3. 默认通过Name查询本地db数据加载模板
    /// </summary>
    public class RptInfo
    {
        #region 成员变量
        // 报表模板缓存
        static readonly Dictionary<string, RptRoot> _tempCache = new Dictionary<string, RptRoot>();
        readonly Dictionary<string, RptData> _dataSet = new Dictionary<string, RptData>(StringComparer.OrdinalIgnoreCase);
        bool _inited;
        #endregion

        #region 属性
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
        #endregion

        #region 外部方法
        /// <summary>
        /// 读取模板内容，重写可自定义读取模板过程
        /// </summary>
        /// <returns></returns>
        public virtual Task<string> ReadTemplate()
        {
            return Kit.GetRequiredService<IModelCallback>().GetReportTemplate(Name);
        }

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
        /// 根据初始参数值生成Row，常用来给查询面板提供数据源
        /// </summary>
        /// <returns></returns>
        public Core.Row BuildParamsRow()
        {
            Throw.IfNull(Root, "未初始化模板，无法生成初始参数值");
            return Root.Params.BuildInitRow();
        }

        /// <summary>
        /// 根据初始参数值生成查询参数字典
        /// </summary>
        /// <returns></returns>
        public Dict BuildParamsDict()
        {
            Throw.IfNull(Root, "未初始化模板，无法生成查询参数");
            return Root.Params.BuildInitDict();
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
        #endregion

        #region 内部属性
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
        /// 脚本对象
        /// </summary>
        internal RptScript ScriptObj { get; private set; }

        /// <summary>
        /// 当前报表预览的工具栏菜单
        /// </summary>
        internal Menu ToolbarMenu { get; set; }

        /// <summary>
        /// 当前报表预览的上下文菜单
        /// </summary>
        internal Menu ContextMenu { get; set; }

        /// <summary>
        /// 当前报表预览的选中区域上下文菜单
        /// </summary>
        internal Menu SelectionMenu { get; set; }
        #endregion

        #region 内部方法
        /// <summary>
        /// 初始化模板、脚本、参数默认值
        /// </summary>
        /// <returns></returns>
        internal async Task<bool> Init()
        {
            if (_inited)
                return Root != null;

            // 加载模板
            _inited = true;
            if (Root == null)
            {
                if (string.IsNullOrEmpty(Name))
                {
                    Kit.Warn("未提供报表模板名称！");
                }
                else if (CacheTemplate && _tempCache.TryGetValue(Name, out var temp))
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
                            _tempCache[Name] = Root;
                        }
                    }
                    catch (Exception ex)
                    {
                        Kit.Warn("加载报表模板时异常！\r\n" + ex.Message);
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

            // 根据参数默认值创建初始查询参数（自动查询时用）
            if (Root.ViewSetting.AutoQuery
                && Params == null
                && Root.Params.Data.Count > 0)
            {
                Params = Root.Params.BuildInitDict();
            }
            return true;
        }

        /// <summary>
        /// 查询参数是否完备有效
        /// </summary>
        /// <returns></returns>
        internal bool IsParamsValid()
        {
            if (Root == null)
                return false;

            int count = Root.Params.Data.Count;
            Dict dt = Params;

            // 未提供查询参数
            if (dt == null || dt.Count == 0)
                return count == 0;

            // 参数个数不够
            if (dt.Count < count)
                return false;

            // 确保每个参数都包含
            foreach (var row in Root.Params.Data)
            {
                if (!dt.ContainsKey(row.Str("name")))
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

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
