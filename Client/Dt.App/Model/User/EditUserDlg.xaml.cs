#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dt.Base;
using Dt.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.App.Model
{
    public sealed partial class EditUserDlg : Dlg
    {
        readonly CmDa _da = new CmDa("cm_user");

        public EditUserDlg()
        {
            InitializeComponent();
        }

        public async void Show(long p_userID)
        {
            if (p_userID > 0)
                _fv.Data = await _da.GetRow("用户-编辑", new { id = p_userID });
            else
                CreateUser();
            Show();
        }

        async void CreateUser()
        {
            _fv.Data = _da.NewRow(new { id = await _da.NewID() });
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
                && await _da.GetScalar<int>("用户-重复手机号", new { phone = phone}) > 0)
            {
                _fv["phone"].Warn("手机号码重复！");
                return;
            }

            // 初始密码为手机号后4位
            if (row.IsAdded)
            {
                row["pwd"] = AtKit.GetMD5(phone.Substring(phone.Length - 4));
                row["ctime"] = row["mtime"] = AtSys.Now;
            }

            if (await AtCm.SaveUser(row))
                CreateUser();
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
