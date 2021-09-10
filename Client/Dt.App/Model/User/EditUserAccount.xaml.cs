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
    public sealed partial class EditUserAccount : Mv
    {
        public EditUserAccount()
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
                _fv.Data = await AtCm.First<User>("用户-编辑", new { id = p_userID });
            }
            else
            {
                CreateUser();
            }

            Menu["增加"].Visibility = p_enableAdd ? Visibility.Visible : Visibility.Collapsed;
        }

        public void Clear()
        {
            _fv.Data = null;
        }

        public User User => _fv.Data.To<User>();

        async void CreateUser()
        {
            _fv.Data = new User(
                ID: await AtCm.NewID(),
                Name: "新用户");
        }

        void OnSave(object sender, Mi e)
        {
            Save();
        }

        void OnAdd(object sender, Mi e)
        {
            CreateUser();
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
            var user = _fv.Data.To<User>();
            bool isNew = user.IsAdded;
            if (await AtCm.SaveBySvc(user))
            {
                Result = true;
                _win?.List.Update();
                if (Menu["增加"].Visibility == Visibility.Visible && isNew)
                {
                    CreateUser();
                    _fv.GotoFirstCell();
                }
            }
        }

        UserAccountWin _win => (UserAccountWin)_tab.OwnWin;
    }
}
