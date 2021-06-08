#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
#endregion

namespace Dt.App.Workflow
{
    public partial class WfFormWin : Win
    {
        readonly IWfForm _form;

        public WfFormWin(WfFormInfo p_info)
        {
            InitializeComponent();

            _form = (IWfForm)Activator.CreateInstance(p_info.FormType);
            _tab.Content = _form;
            _tab.Title = p_info.PrcDef.Name;
            LoadMenu(p_info);
        }

        public Task<bool> Save()
        {
            return _form.Save();
        }

        public Task<bool> Delete()
        {
            return _form.Delete();
        }

        public string GetPrcName()
        {
            return _form.GetPrcName();
        }

        /// <summary>
        /// 加载默认菜单，自动绑定命令
        /// </summary>
        async void LoadMenu(WfFormInfo p_info)
        {
            var fv = ((FrameworkElement)_form).FindChildByType<Fv>();
            if (fv == null)
            {
                Kit.Warn($"未找到流程表单【{p_info.PrcDef.Name}】！");
                return;
            }

            Menu m = new Menu();
            if (p_info.Usage == WfFormUsage.Edit)
            {
                m.Items.Add(new Mi { ID = "发送", Icon = Icons.发出, Cmd = p_info.CmdSend });

                if (await p_info.AllowRollback())
                {
                    m.Items.Add(new Mi { ID = "回退", Icon = Icons.追回, Cmd = p_info.CmdRollback });
                }

                if (!p_info.IsStartItem)
                {
                    Mi mi = new Mi { ID = "签收", Icon = Icons.锁卡, IsCheckable = true, Cmd = p_info.CmdAccept };
                    if (p_info.WorkItem.IsAccept)
                        mi.IsChecked = true;
                    m.Items.Add(mi);
                }

                // 合并IsDirty属性
                p_info.CmdSave.AllowExecute = fv.IsDirty;
                fv.Dirty += (s, b) => p_info.CmdSave.AllowExecute = b;
                m.Items.Add(new Mi { ID = "保存", Icon = Icons.保存, Cmd = p_info.CmdSave });
                m.Items.Add(new Mi { ID = "撤消", Icon = Icons.撤消, Cmd = fv.CmdUndo });

                if (p_info.AtvDef.CanDelete || p_info.AtvDef.Type == WfdAtvType.Start)
                    m.Items.Add(new Mi { ID = "删除", Icon = Icons.垃圾箱, Cmd = p_info.CmdDelete });
            }
            else if (p_info.Usage == WfFormUsage.Manage)
            {
                m.Items.Add(new Mi { ID = "删除", Icon = Icons.垃圾箱, Cmd = p_info.CmdDelete });
                fv.IsReadOnly = true;
            }
            else
            {
                fv.IsReadOnly = true;
            }
            m.Items.Add(new Mi { ID = "日志", Icon = Icons.审核, Cmd = p_info.CmdLog });

            p_info.Menu = m;
            _tab.Menu = m;

            _form.Init(p_info);
        }
    }
}