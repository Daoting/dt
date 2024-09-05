#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-07-01 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Mgr.Workflow;
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dt.Mgr.Rbac;
using System.Reflection;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
#endregion

namespace Dt.Mgr
{
    /// <summary>
    /// 流程表单基类
    /// </summary>
    public abstract partial class WfForm : Form
    {
        #region 成员变量
        protected WfFormInfo _info;

        #endregion

        #region 保存
        /// <summary>
        /// 保存命令
        /// </summary>
        protected override void Save()
        {
            _locker.Call(async () => await WfiDs.SaveForm(_info));
        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除命令
        /// </summary>
        protected override void Delete()
        {
            if (MainFv.Data == null)
                return;

            _locker.Call(async () => await WfiDs.Delete(_info));
        }

        /// <summary>
        /// 删除表单数据，禁止删除或删除失败时可返回false
        /// </summary>
        /// <param name="w">实体写入器，所有需要增删改的实体在一个事务内保存到db</param>
        /// <returns></returns>
        protected virtual async Task<bool> OnDelete(IEntityWriter w)
        {
            if (IsEntityData())
            {
                // 先删子实体
                if (Items.Count > 0)
                {
                    foreach (var fi in Items)
                    {
                        if (fi.Lv.Data is Table tbl)
                            await w.Delete(tbl);
                    }
                }

                var entity = MainFv.Data as Entity;
                await w.Delete(entity);
                return true;
            }
            return false;
        }

        internal Task<bool> DeleteInternal(IEntityWriter w)
        {
            return OnDelete(w);
        }
        #endregion

        #region 发送
        /// <summary>
        /// 发送命令
        /// </summary>
        protected void Send()
        {
            _locker.Call(async () => await WfiDs.Send(_info));
        }

        /// <summary>
        /// 发送时可以处理：
        /// <para>1. 自动填写表单数据或特殊校验，如发送人、发送时间、当前状态等</para>
        /// <para>2. 自定义执行者范围，_info.NextRecvs</para>
        /// <para>3. 若下一活动为结束活动时，自定义提示信息等</para>
        /// </summary>
        /// <param name="w">实体写入器，所有需要增删改的实体在一个事务内保存到db</param>
        /// <returns></returns>
        protected abstract Task<bool> OnSend(IEntityWriter w);

        /// <summary>
        /// 调用 WfForm.OnSend 方法
        /// <para>1. 发送时外部自动填写表单数据或特殊校验，如发送人、发送时间、当前状态等</para>
        /// <para>2. 发送时外部自定义执行者范围，_info.NextRecvs</para>
        /// <para>3. 若下一活动为结束活动时，自定义提示信息等</para>
        /// </summary>
        /// <param name="w"></param>
        /// <returns></returns>
        internal Task<bool> SendInternal(IEntityWriter w)
        {
            return OnSend(w);
        }
        #endregion

        #region 回退
        /// <summary>
        /// 回退命令
        /// </summary>
        protected void Rollback()
        {
            _locker.Call(async () => await WfiDs.Rollback(_info));
        }
        #endregion

        #region 签收/取消
        /// <summary>
        /// 签收/取消签收命令
        /// </summary>
        protected void ToggleAccept()
        {
            _locker.Call(async () => await WfiDs.ToggleAccept(_info));
        }
        #endregion

        #region 日志
        /// <summary>
        /// 查看日志(流程图)命令
        /// </summary>
        protected void ShowLog()
        {
            AtWf.ShowLog(_info.PrcInst.ID, _info.PrcDef.ID);
        }
        #endregion

        #region 创建菜单
        /// <summary>
        /// 加载默认菜单，自动绑定命令
        /// </summary>
        protected async Task<Menu> CreateMenu(Menu p_menu = null)
        {
            Menu m = p_menu ?? new Menu();
            if (!_info.IsReadOnly)
            {
                if (_info.IsLastAtv)
                {
                    m.Items.Add(new Mi("完成", Icons.完成, call: Send));
                }
                else
                {
                    m.Items.Add(new Mi("发送", Icons.发出, call: Send));
                }

                if (await _info.AllowRollback())
                {
                    m.Items.Add(new Mi("回退", Icons.追回, call: Rollback));
                }

                Mi mi;
                if (!_info.IsStartItem)
                {
                    mi = new Mi("签收", Icons.锁卡, call: ToggleAccept, isCheckable: true);
                    if (_info.WorkItem.IsAccept)
                        mi.IsChecked = true;
                    m.Items.Add(mi);
                }

                mi = Mi.保存(call: Save);
                mi.SetBinding(Mi.IsEnabledProperty, new Binding { Path = new PropertyPath("IsDirty"), Source = this });
                ToolTipService.SetToolTip(mi, "Ctrl + S");
                m.Items.Add(mi);

                if (_info.AtvDef.CanDelete || _info.AtvDef.Type == WfdAtvType.Start)
                    m.Items.Add(Mi.删除(call: Delete));
            }
            m.Items.Add(new Mi("日志", Icons.日志, call: ShowLog));
            return m;
        }
        #endregion

        #region 抽象方法
        /// <summary>
        /// 任务单名称
        /// </summary>
        protected abstract string PrcName { get; }

        internal string GetPrcName() => PrcName;
        #endregion
    }
}
