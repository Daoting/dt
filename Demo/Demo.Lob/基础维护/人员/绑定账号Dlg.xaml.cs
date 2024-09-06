#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-11-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Mgr.Rbac;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.Lob
{
    public partial class 绑定账号Dlg : Dlg
    {
        public 绑定账号Dlg()
        {
            InitializeComponent();
            Menu = Menu.New(Mi.确定(OnOK));
        }

        public Row SelectedRow => _lv.SelectedRow;

        public async Task<bool> Show(long p_releatedID)
        {
            _lv.Data = await UserX.Query($"where not exists ( select user_id from 人员 b where a.id=b.user_id and user_id is not null )");
            if (!Kit.IsPhoneUI)
            {
                Height = Kit.ViewHeight / 2;
            }
            return await ShowAsync();
        }

        void OnItemDoubleClick(object obj)
        {
            OnOK(null);
        }
    }
}