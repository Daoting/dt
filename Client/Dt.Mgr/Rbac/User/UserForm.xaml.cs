#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-03-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Rbac
{
    public sealed partial class UserForm : Tab
    {
        #region 构造方法
        public UserForm()
        {
            InitializeComponent();
        }
        #endregion

        #region 公开
        public async Task Update(long p_id, bool p_enableAdd = true)
        {
            var d = Data;
            if (d != null && d.ID == p_id)
                return;

            if (!await _fv.DiscardChanges())
                return;

            if (p_id > 0)
            {
                Data = await UserX.GetByID(p_id);
                UpdateRelated(p_id);
            }
            else
            {
                Create();
            }

            Menu["增加"].Visibility = p_enableAdd ? Visibility.Visible : Visibility.Collapsed;
        }

        public void Clear()
        {
            Data = null;
            UpdateRelated(-1);
        }

        public UserX Data
        {
            get { return _fv.Data.To<UserX>(); }
            private set { _fv.Data = value; }
        }
        #endregion

        #region 内部
        void OnPhotoChanged(object sender, object e)
        {
            Save();
        }

        async void Create()
        {
            Data = await UserX.New();
            UpdateRelated(-1);
        }

        async void Save()
        {
            var d = Data;
            bool isNew = d.IsAdded;
            if (await d.Save())
            {
                _win?.MainList.Update();
                if (isNew)
                {
                    Kit.Msg("初始密码为4个1");
                    UpdateRelated(d.ID);
                }
            }
        }

        void UpdateRelated(long p_id)
        {
            _win?.GroupList.Update(p_id);
            _win?.RoleList.Update(p_id);
        }

        protected override Task<bool> OnClosing()
        {
            return _fv.DiscardChanges();
        }

        UserWin _win => OwnWin as UserWin;
        #endregion
    }
}
