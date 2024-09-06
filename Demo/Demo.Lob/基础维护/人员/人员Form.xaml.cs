#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-14 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.Lob
{
    using A = 人员X;

    public sealed partial class 人员Form : Form
    {
        public 人员Form()
        {
            InitializeComponent();
            Menu = CreateMenu();
            Menu.Add(new Mi("绑定账号", Icons.多人, call: OnBindingUser));
        }

        protected override async Task OnAdd()
        {
            _fv.Data = await A.New();
        }

        protected override async Task OnGet()
        {
            _fv.Data = await A.View1.GetByID(_args.ID);
        }

        protected override void OnFvDataChanged()
        {
            var x = _fv.Data as A;
            Mi mi = Menu[3];
            if (x != null && !x.IsAdded)
            {
                mi.IsEnabled = true;
                mi.ID = x.UserID == null ? "绑定账号" : "解除账号";
            }
            else
            {
                mi.ID = "绑定账号";
                mi.IsEnabled = false;
            }
        }

        async void OnBindingUser()
        {
            await 人员List.DoBindUser(_fv.Data as A);
        }
    }
}