#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-02-04 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Rbac
{
    public sealed partial class UserForm : Form
    {
        public UserForm()
        {
            InitializeComponent();
            Menu = CreateMenu();
            Menu.Items.Remove(Menu["删除"]);
        }

        public async Task<bool> Open(long p_id, bool p_enableAdd)
        {
            Menu["增加"].Visibility = p_enableAdd ? Visibility.Visible : Visibility.Collapsed;
            
            if (_taskSrc == null || _taskSrc.Task.IsCompleted)
                _taskSrc = new TaskCompletionSource<bool>();
            await Query(p_id);
            await _taskSrc.Task;
            
            return IsModified;
        }

        public UserX Data => _fv.Data.To<UserX>();

        protected override async Task OnAdd()
        {
            _fv.Data = await UserX.New();
        }

        protected override async Task OnGet()
        {
            _fv.Data = await UserX.GetByID(_args.ID);
        }

        protected override async Task<bool> OnSave(IEntityWriter w)
        {
            var d = _fv.Data.To<UserX>();
            bool isNew = d.IsAdded;
            if (await d.Save())
            {
                if (isNew)
                    Kit.Msg("初始密码为4个1");
            }
            return false;
        }
        
        void OnPhotoChanged(FvCell arg1, object e)
        {
            Save();
        }
    }
}