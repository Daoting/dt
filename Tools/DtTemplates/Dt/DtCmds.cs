using Dt.Editor;
using Dt.LocalTbl;
using Dt.ManyToMany;
using Dt.OnToMany;
using Dt.SingleTbl;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using System.Windows.Forms;
using Task = System.Threading.Tasks.Task;

namespace Dt
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class DtCmds
    {
        const int LvCommandId = 0x0100;
        const int DotCmdId = 0x0101;
        const int FvCommandId = 0x0102;
        const int CellCmdId = 0x0103;
        const int MenuCmdId = 0x0104;

        const int SingleTblCmdId = 0x3000;
        const int OnToManyCmdId = 0x3001;
        const int ManyToManyCmdId = 0x3002;
        const int LocalSingleTblCmdId = 0x3003;

        const int InsertMvCmdId = 0x4000;
        const int InsertWinCmdId = 0x4001;
        const int InsertDlgCmdId = 0x4002;

        const int EntityClassCmdId = 0x5000;
        const int DomainExClsCmdId = 0x5001;
        const int LvCallClsCmdId = 0x5002;
        const int FvCallClsCmdId = 0x5003;
        const int CListExClsCmdId = 0x5004;
        const int AgentClsCmdId = 0x5005;
        const int ApiClsCmdId = 0x5006;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("6ef30193-3795-49e0-b034-e320355b74ae");

        /// <summary>
        /// Initializes a new instance of the <see cref="DtCmds"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="cs">Command service to add command to, not null.</param>
        private DtCmds(AsyncPackage package, OleMenuCommandService cs)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            cs = cs ?? throw new ArgumentNullException(nameof(cs));

            ThreadHelper.ThrowIfNotOnUIThread();
            cs.AddCommand(CmdUserControl(LvCommandId, typeof(LvXaml)));
            cs.AddCommand(CmdDialog(DotCmdId, typeof(DotXaml)));
            cs.AddCommand(CmdUserControl(FvCommandId, typeof(FvXaml)));
            cs.AddCommand(CmdDialog(CellCmdId, typeof(CellXaml)));
            cs.AddCommand(CmdDialog(MenuCmdId, typeof(MenuXaml)));

            cs.AddCommand(CmdClient(SingleTblCmdId, typeof(SingleForm)));
            cs.AddCommand(CmdClient(OnToManyCmdId, typeof(OnToManyForm)));
            cs.AddCommand(CmdClient(ManyToManyCmdId, typeof(ManyToManyForm)));
            cs.AddCommand(CmdClient(LocalSingleTblCmdId, typeof(LocalTblForm)));

            cs.AddCommand(CmdClient(InsertMvCmdId, typeof(InsertMvForm)));
            cs.AddCommand(CmdClient(InsertWinCmdId, typeof(InsertWinForm)));
            cs.AddCommand(CmdClient(InsertDlgCmdId, typeof(InsertDlgForm)));

            cs.AddCommand(CmdDialog(EntityClassCmdId, typeof(InsertEntityClsForm)));
            cs.AddCommand(CmdInsertClass(DomainExClsCmdId, ClsType.DomainEx, null));
            cs.AddCommand(CmdInsertClass(LvCallClsCmdId, ClsType.LvCall, true));
            cs.AddCommand(CmdInsertClass(FvCallClsCmdId, ClsType.FvCall, true));
            cs.AddCommand(CmdInsertClass(CListExClsCmdId, ClsType.CListEx, true));
            cs.AddCommand(CmdInsertClass(AgentClsCmdId, ClsType.Agent, null));
            cs.AddCommand(CmdInsertClass(ApiClsCmdId, ClsType.Api, false));
        }

        /// <summary>
        /// 显示标准编辑窗口，窗口内部的UserControl是命令
        /// </summary>
        /// <param name="p_cmdID"></param>
        /// <param name="p_type"></param>
        /// <returns></returns>
        MenuCommand CmdUserControl(int p_cmdID, Type p_type)
        {
            return new MenuCommand(
                (s, e) =>
                {
                    ThreadHelper.ThrowIfNotOnUIThread();
                    // 显示编辑窗口
                    new CmdForm(p_type).ShowDialog();
                },
                new CommandID(CommandSet, p_cmdID));
        }

        /// <summary>
        /// 显示自定义对话框
        /// </summary>
        /// <param name="p_cmdID"></param>
        /// <param name="p_type"></param>
        /// <returns></returns>
        MenuCommand CmdDialog(int p_cmdID, Type p_type)
        {
            return new MenuCommand(
                (s, e) =>
                {
                    ThreadHelper.ThrowIfNotOnUIThread();
                    var dlg = Activator.CreateInstance(p_type) as Form;
                    dlg.ShowDialog();
                },
                new CommandID(CommandSet, p_cmdID));
        }

        /// <summary>
        /// 显示客户端自定义对话框
        /// </summary>
        /// <param name="p_cmdID"></param>
        /// <param name="p_type"></param>
        /// <returns></returns>
        MenuCommand CmdClient(int p_cmdID, Type p_type)
        {
            return new MenuCommand(
                (s, e) =>
                {
                    ThreadHelper.ThrowIfNotOnUIThread();
                    if (!Kit.IsClientPrj())
                    {
                        Kit.Output("客户端不支持");
                        return;
                    }

                    var dlg = Activator.CreateInstance(p_type) as Form;
                    dlg.ShowDialog();
                },
                new CommandID(CommandSet, p_cmdID));
        }

        
        MenuCommand CmdInsertClass(int p_cmdID, ClsType p_clsType, bool? p_isClient)
        {
            return new MenuCommand(
                (s, e) =>
                {
                    ThreadHelper.ThrowIfNotOnUIThread();
                    if (p_isClient.HasValue)
                    {
                        if ((bool)p_isClient && !Kit.IsClientPrj())
                        {
                            MessageBox.Show("服务端不支持该类型！");
                            return;
                        }

                        if (!(bool)p_isClient && !Kit.IsSvcPrj())
                        {
                            MessageBox.Show("客户端不支持该类型！");
                            return;
                        }
                    }
                    
                    new InsertClassForm(p_clsType).ShowDialog();
                },
                new CommandID(CommandSet, p_cmdID));
        }

        /// <summary>
        /// 直接粘贴文本
        /// </summary>
        /// <param name="p_cmdID"></param>
        /// <param name="p_txt"></param>
        /// <returns></returns>
        MenuCommand CmdPaste(int p_cmdID, string p_txt)
        {
            return new MenuCommand(
                (s, e) =>
                {
                    ThreadHelper.ThrowIfNotOnUIThread();
                    Kit.Paste(p_txt);
                },
                new CommandID(CommandSet, p_cmdID));
        }

        #region 生成
        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static DtCmds Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in LvCmd's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new DtCmds(package, commandService);
        }
        #endregion
    }
}
