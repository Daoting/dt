#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-09-25 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Base;
using Dt.Base.Report;
using Dt.Cells.Data;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 报表设计的描述信息
    /// </summary>
    public abstract partial class RptDesignInfo
    {
        #region 成员变量
        internal readonly RptCmdHistory History = new RptCmdHistory();

        #endregion

        internal event EventHandler<TemplateChangedArgs> TemplateChanged;

        internal RptRoot Root { get; private set; }

        #region 报表模板
        /// <summary>
        /// 初始化报表模板
        /// </summary>
        /// <returns></returns>
        internal async Task<bool> InitTemplate()
        {
            try
            {
                // 报表设计时始终不缓存模板！
                string define = await ReadTemplate();
                Root = await AtRpt.DeserializeTemplate(define);
                Root.ValueChanged += OnItemValueChanged;
                return true;
            }
            catch { }

            return false;
        }

        /// <summary>
        /// 导入报表模板
        /// </summary>
        /// <param name="p_define"></param>
        /// <returns></returns>
        internal async Task ImportTemplate(string p_define)
        {
            var old = Root;
            if (old != null)
                old.ValueChanged -= OnItemValueChanged;

            Root = await AtRpt.DeserializeTemplate(p_define);
            Root.ValueChanged += OnItemValueChanged;
            TemplateChanged?.Invoke(this, new TemplateChangedArgs { NewRoot = Root, OldRoot = old });
        }

        /// <summary>
        /// 保存报表模板
        /// </summary>
        internal void SaveTemplate()
        {
            if (Root != null)
            {
                SaveTemplate(AtRpt.SerializeTemplate(Root));
                History.Clear();
            }
        }
        #endregion

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

        #region 比较
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is RptDesignInfo))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

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

    internal class TemplateChangedArgs
    {
        public RptRoot NewRoot { get; set; }

        public RptRoot OldRoot { get; set; }
    }
}
