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
                _fv.Data = await Repo.Get<User>("用户-编辑", new { id = p_userID });
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
                ID: await AtCm.NewFlagID(0),
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
            if (_fv.ExistNull("name", "phone"))
                return;

            var usr = _fv.Data.To<User>();
            if (!Regex.IsMatch(usr.Phone, "^1[34578]\\d{9}$"))
            {
                _fv["phone"].Warn("手机号码错误！");
                return;
            }

            if ((usr.IsAdded || usr.Cells["phone"].IsChanged)
                && await AtCm.GetScalar<int>("用户-重复手机号", new { phone = usr.Phone }) > 0)
            {
                _fv["phone"].Warn("手机号码重复！");
                return;
            }

            if (usr.IsAdded)
            {
                // 初始密码为手机号后4位
                usr.Pwd = AtKit.GetMD5(usr.Phone.Substring(usr.Phone.Length - 4));
                usr.Ctime = usr.Mtime = AtSys.Now;
            }
            else
            {
                usr.Mtime = AtSys.Now;
            }

            if (await Repo.Save(usr))
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
