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
        const string _tblName = "cm_user";
        bool _needRefresh;

        public EditUserDlg()
        {
            InitializeComponent();
        }

        public async Task<bool> Show(long p_userID, bool p_enableAdd = true)
        {
            if (p_userID > 0)
            {
                _fv.Data = await AtCm.GetRow("用户-编辑", new { id = p_userID });
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

        void CreateUser()
        {
            _fv.Data = Table.NewRow(_tblName);
        }

        async void OnSave(object sender, Mi e)
        {
            if (_fv.ExistNull("name", "phone"))
                return;

            Row row = _fv.Row;
            string phone = row.Str("phone");
            if (!Regex.IsMatch(phone, "^1[34578]\\d{9}$"))
            {
                _fv["phone"].Warn("手机号码错误！");
                return;
            }

            if ((row.IsAdded || row.Cells["phone"].IsChanged)
                && await AtCm.GetScalar<int>("用户-重复手机号", new { phone = phone }) > 0)
            {
                _fv["phone"].Warn("手机号码重复！");
                return;
            }

            if (row.IsAdded)
            {
                row["id"] = await AtCm.NewFlagID(0);
                // 初始密码为手机号后4位
                row["pwd"] = AtKit.GetMD5(phone.Substring(phone.Length - 4));
                row["ctime"] = row["mtime"] = AtSys.Now;
            }
            else
            {
                row["mtime"] = AtSys.Now;
            }
            if (await AtCm.SaveRow(row, _tblName))
            {
                _needRefresh = true;
                AtKit.Msg("保存成功！");

                if (_miAdd.Visibility == Visibility.Visible)
                {
                    CreateUser();
                    _fv.GotoFirstCell();
                }
                else
                {
                    row.AcceptChanges();
                    Close();
                }
            }
            else
            {
                AtKit.Warn("保存失败！");
            }
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
    }
}
