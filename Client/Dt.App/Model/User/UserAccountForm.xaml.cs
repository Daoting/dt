#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
#endregion

namespace Dt.App.Model
{
    public sealed partial class UserAccountForm : Mv
    {
        public UserAccountForm()
        {
            InitializeComponent();
            Menu["保存"].Bind(IsEnabledProperty, _fv, "IsDirty");
        }

        public async void Update(long p_userID, bool p_enableAdd = true)
        {
            if (!await _fv.DiscardChanges())
                return;

            if (p_userID > 0)
            {
                _fv.Data = await AtCm.First<UserObj>("用户-编辑", new { id = p_userID });
                _win?.RoleList.Update(p_userID);
            }
            else
            {
                Create();
            }

            Menu["增加"].Visibility = p_enableAdd ? Visibility.Visible : Visibility.Collapsed;
        }

        public void Clear()
        {
            _fv.Data = null;
            _win.RoleList.Clear();
        }

        public UserObj User => _fv.Data.To<UserObj>();

        async void Create()
        {
            _fv.Data = new UserObj(
                ID: await AtCm.NewID(),
                Name: "新用户");
            _win?.RoleList.Clear();
        }

        void OnSave(object sender, Mi e)
        {
            Save();
        }

        void OnAdd(object sender, Mi e)
        {
            Create();
        }

        void OnPhotoChanged(object sender, object e)
        {
            Save();
        }

        protected override Task<bool> OnClosing()
        {
            return _fv.DiscardChanges();
        }

        async void Save()
        {
            var user = _fv.Data.To<UserObj>();
            bool isNew = user.IsAdded;
            if (await AtCm.SaveBySvc(user))
            {
                Result = true;
                _win?.List.Update();
                if (isNew)
                    _win.RoleList.Update(user.ID);
            }
        }

        UserAccountWin _win => (UserAccountWin)_tab.OwnWin;
    }
}
