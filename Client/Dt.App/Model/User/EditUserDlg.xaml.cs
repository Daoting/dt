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
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml;
#endregion

namespace Dt.App.Model
{
    public sealed partial class EditUserDlg : Dlg
    {
        bool _needRefresh;

        public EditUserDlg()
        {
            InitializeComponent();
        }

        public async Task<bool> Show(long p_userID, bool p_enableAdd = true)
        {
            if (p_userID > 0)
            {
                _fv.Data = await AtCm.Get<User>("用户-编辑", new { id = p_userID });
            }
            else
            {
                CreateUser();
            }

            if (!p_enableAdd)
                _miAdd.Visibility = Visibility.Collapsed;
            await ShowAsync();
            return _needRefresh;
        }

        public Row Info
        {
            get { return _fv.Row; }
        }

        async void CreateUser()
        {
            _fv.Data = new User(
                // 3位标志用来识别用户类型，如管理者、消费者
                ID: await AtCm.NewID(0),
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

        protected override Task<bool> OnClosing()
        {
            if (_fv.Row.IsChanged)
                return AtKit.Confirm("数据未保存，要放弃修改吗？");
            return Task.FromResult(true);
        }

        void OnPhotoChanged(object sender, object e)
        {
            Save();
        }

        async void Save()
        {
            if (await AtCm.SaveBySvc(_fv.Data.To<User>()))
            {
                _needRefresh = true;
                if (_miAdd.Visibility == Visibility.Visible)
                {
                    CreateUser();
                    _fv.GotoFirstCell();
                }
                else
                {
                    Close();
                }
            }
        }
    }
}
