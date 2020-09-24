#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-11 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Base;
using Dt.Cells.Data;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 报表设计的描述信息
    /// </summary>
    public abstract class RptDesignInfo
    {
        internal readonly RptCmdHistory History = new RptCmdHistory();
        RptRoot _root;

        #region 外部方法
        /// <summary>
        /// 获取设置报表名称，作为唯一标识识别窗口用
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 读取模板内容
        /// </summary>
        /// <returns></returns>
        public abstract Task<string> ReadTemplate();

        /// <summary>
        /// 保存模板内容
        /// </summary>
        /// <param name="p_xml"></param>
        public abstract void SaveTemplate(string p_xml);
        #endregion

        internal event EventHandler<RptRoot> TemplateChanged;


        internal async Task<RptRoot> GetTemplate()
        {
            // 报表设计时始终不缓存模板！
            if (_root == null)
            {
                // 初次加载模板
                string define = await ReadTemplate();
                _root = await RptKit.DeserializeTemplate(define);
                _root.ValueChanged += OnItemValueChanged;
            }
            return _root;
        }

        internal async Task ImportTemplate(string p_define)
        {
            if (_root != null)
                _root.ValueChanged -= OnItemValueChanged;

            _root = await RptKit.DeserializeTemplate(p_define);
            _root.ValueChanged += OnItemValueChanged;
            TemplateChanged?.Invoke(this, _root);
        }

        internal void SaveTemplate()
        {
            if (_root != null)
            {
                SaveTemplate(RptKit.SerializeTemplate(_root));
                History.Clear();
            }
        }

        #region 命令
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="p_cmd"></param>
        /// <param name="p_args"></param>
        /// <returns></returns>
        internal object ExecuteCmd(RptCmdBase p_cmd, object p_args)
        {
            ValueChangedCmd cmd = RptCmds.ValueChanged;
            cmd.IsSetting = true;
            object result = p_cmd.Execute(p_args);
            History.RecordAction(new HistoryCmdAction(p_cmd, p_args));
            cmd.IsSetting = false;
            return result;
        }

        /// <summary>
        /// 撤消命令
        /// </summary>
        internal void Undo()
        {
            ValueChangedCmd cmd = RptCmds.ValueChanged;
            cmd.IsSetting = true;
            History.Undo();
            cmd.IsSetting = false;
        }

        /// <summary>
        /// 重做
        /// </summary>
        internal void Redo()
        {
            ValueChangedCmd cmd = RptCmds.ValueChanged;
            cmd.IsSetting = true;
            History.Redo();
            cmd.IsSetting = false;
        }

        /// <summary>
        /// 记录报表项属性值变化，提供可撤消和重做功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnItemValueChanged(object sender, Core.Cell e)
        {
            ValueChangedCmd cmd = RptCmds.ValueChanged;
            if (!cmd.IsSetting)
                History.RecordAction(new HistoryCmdAction(cmd, new ValueChangedArgs(e, sender as RptText)));
        }
        #endregion

        #region 内部方法

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is RptDesignInfo))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (GetType() != obj.GetType())
                return false;

            // 只比较标识，识别窗口用
            return Name == ((RptDesignInfo)obj).Name;
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
