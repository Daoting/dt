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

        /// <summary>
        /// 切换模板事件
        /// </summary>
        internal event EventHandler<TemplateChangedArgs> TemplateChanged;

        /// <summary>
        /// 模板修改变化事件
        /// </summary>
        internal event EventHandler<bool> DirtyChanged;

        /// <summary>
        /// 模板保存后事件
        /// </summary>
        internal event EventHandler Saved;

        /// <summary>
        /// 页面设置变化事件
        /// </summary>
        internal event EventHandler PageSettingChanged;

        internal RptRoot Root { get; private set; }

        internal bool IsDirty { get; private set; }

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
                AttachRootEvent();
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
                DetachRootEvent();

            Root = await AtRpt.DeserializeTemplate(p_define);
            AttachRootEvent();
            TemplateChanged?.Invoke(this, new TemplateChangedArgs { NewRoot = Root, OldRoot = old });
        }

        /// <summary>
        /// 保存报表模板
        /// </summary>
        internal void SaveTemplate()
        {
            if (Root != null && Root.IsValid())
            {
                SaveTemplate(AtRpt.SerializeTemplate(Root));
                History.Clear();
                Saved?.Invoke(this, EventArgs.Empty);
                OnCellValueChanged(this, EventArgs.Empty);
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
            OnCellValueChanged(p_cmd, null);
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
            OnCellValueChanged(null, null);
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
            OnCellValueChanged(null, null);
        }
        #endregion

        #region 事件
        void AttachRootEvent()
        {
            Root.ItemValueChanged += OnItemValueChanged;
            Root.CellValueChanged += OnCellValueChanged;
        }

        void DetachRootEvent()
        {
            Root.ItemValueChanged -= OnItemValueChanged;
            Root.CellValueChanged -= OnCellValueChanged;
        }

        internal void OnPageSettingChanged()
        {
            PageSettingChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 记录报表项属性值变化，提供可撤消和重做功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnItemValueChanged(object sender, Core.Cell e)
        {
            History.RecordAction(new HistoryCmdAction(RptCmds.ValueChanged, new ValueChangedArgs(e, sender as RptText)));
            OnCellValueChanged(sender, null);
        }

        void OnCellValueChanged(object sender, EventArgs e)
        {
            bool isDirty = History.CanUndo
                || Root.Params.Data.IsChanged
                || Root.Data.DataSet.IsChanged
                || Root.PageSetting.Data.IsChanged
                || Root.ViewSetting.Data.IsChanged;

            if (IsDirty != isDirty)
            {
                IsDirty = isDirty;
                DirtyChanged?.Invoke(this, IsDirty);
            }
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
