#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-03-08 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Rbac
{
    [View(LobViews.基础权限)]
    public partial class PerWin : Win
    {
        readonly PerModuleForm _moduleForm;
        readonly PerFuncForm _funcForm;
        readonly PerForm _perForm;
        
        public PerWin()
        {
            InitializeComponent();
            _moduleForm = new PerModuleForm { OwnWin = this };
            _funcForm = new PerFuncForm { OwnWin = this };
            _perForm = new PerForm {  OwnWin = this };
            Attach();
        }

        void Attach()
        {
            _mouleList.Msg += e => _ = _moduleForm.Query(e);
            _mouleList.Navi += () => NaviTo(_funcList.Title);

            _moduleForm.UpdateList += e => _ = _mouleList.Refresh(e.ID);
            _moduleForm.UpdateRelated += e => _funcList.Query(e.ID);

            _funcList.Msg += e => _ = _funcForm.Query(e);
            _funcList.Navi += () => NaviTo(_perList.Title);

            _funcForm.UpdateList += e => _ = _funcList.Refresh(e.ID);
            _funcForm.UpdateRelated += e => _perList.Query(e.ID);

            _perList.Msg += e => _ = _perForm.Query(e);
            _perList.Navi += () => NaviTo(_perRoleList.Title);

            _perForm.UpdateList += e => _ = _perList.Refresh(e.ID);
            _perForm.UpdateRelated += e => _perRoleList.Query(e.ID);
        }
    }
}