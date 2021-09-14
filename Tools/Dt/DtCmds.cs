using Dt.Editor;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Design;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Task = System.Threading.Tasks.Task;
using Window = System.Windows;

namespace Dt
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class DtCmds
    {
        const string _fvXaml = "<a:Fv x:Name=\"_fv\">\r\n\r\n</a:Fv>";
        const string _cellExCls = "\r\n#region ViewEx\r\nclass ViewEx1\r\n{\r\npublic static void SetStyle(ViewItem p_item)\r\n{}\r\n\r\npublic static TextBlock xb(ViewItem p_item)\r\n{}\r\n}\r\n#endregion\r\n";
        const string _tabMenu = "<a:Tab.Menu>\r\n<a:Menu>\r\n<a:Mi ID=\"保存\" Icon=\"保存\" />\r\n</a:Menu>\r\n</a:Tab.Menu>";
        const string _contextMenu = "<a:Ex.Menu>\r\n<a:Menu>\r\n<a:Mi ID=\"保存\" Icon=\"保存\" />\r\n</a:Menu>\r\n</a:Ex.Menu>";
        const string _dot = "<a:Dot ID=\"xx\" />";
        const string _dotSmall = "<a:Dot ID=\"xx\" Font=\"小灰\" />";

        const int LvCommandId = 0x0100;
        const int FvCommandId = 0x0101;
        const int LvCellExClsCmdId = 0x2000;
        const int CellCmdId = 0x0102;
        const int TabMenuCmdId = 0x0103;
        const int ContextMenuCmdId = 0x0104;
        const int DotCmdId = 0x0105;
        const int DotSmallCmdId = 0x0106;

        const int SingleTblCmdId = 0x3000;
        const int OnToManyCmdId = 0x3001;
        const int ManyToManyCmdId = 0x3002;

        const int InsertMvCmdId = 0x4000;
        const int InsertWinCmdId = 0x4001;
        const int InsertDlgCmdId = 0x4002;

        const int InsertEntityCmdId = 0x5000;
        const int InsertAgentCmdId = 0x5001;
        const int InsertApiCmdId = 0x5002;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("6ef30193-3795-49e0-b034-e320355b74ae");

        /// <summary>
        /// Initializes a new instance of the <see cref="DtCmds"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private DtCmds(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            ThreadHelper.ThrowIfNotOnUIThread();
            commandService.AddCommand(CmdForm(LvCommandId, typeof(LvXaml)));
            commandService.AddCommand(CmdPaste(FvCommandId, _fvXaml));
            commandService.AddCommand(CmdCustom(CellCmdId, typeof(CellXaml)));
            commandService.AddCommand(CmdPaste(LvCellExClsCmdId, _cellExCls));
            commandService.AddCommand(CmdPaste(TabMenuCmdId, _tabMenu));
            commandService.AddCommand(CmdPaste(ContextMenuCmdId, _contextMenu));
            commandService.AddCommand(CmdPaste(DotCmdId, _dot));
            commandService.AddCommand(CmdPaste(DotSmallCmdId, _dotSmall));

            commandService.AddCommand(CmdCustom(SingleTblCmdId, typeof(SingleTblForm)));
            commandService.AddCommand(CmdCustom(OnToManyCmdId, typeof(OnToManyForm)));
            commandService.AddCommand(CmdCustom(ManyToManyCmdId, typeof(ManyToManyForm)));

            commandService.AddCommand(CmdCustom(InsertWinCmdId, typeof(InsertWinForm)));
            commandService.AddCommand(CmdCustom(InsertDlgCmdId, typeof(InsertDlgForm)));

            commandService.AddCommand(CmdCustom(InsertEntityCmdId, typeof(InsertEntityForm)));
            commandService.AddCommand(CmdCustom(InsertAgentCmdId, typeof(InsertAgentForm)));
            commandService.AddCommand(CmdCustom(InsertApiCmdId, typeof(InsertApiForm)));
        }

        /// <summary>
        /// 显示标准编辑窗口
        /// </summary>
        /// <param name="p_cmdID"></param>
        /// <param name="p_type"></param>
        /// <returns></returns>
        MenuCommand CmdForm(int p_cmdID, Type p_type)
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
        /// 显示自定义编辑窗口
        /// </summary>
        /// <param name="p_cmdID"></param>
        /// <param name="p_type"></param>
        /// <returns></returns>
        MenuCommand CmdCustom(int p_cmdID, Type p_type)
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
