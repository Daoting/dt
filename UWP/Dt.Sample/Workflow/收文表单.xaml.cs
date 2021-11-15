#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.App;
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public partial class 收文表单 : UserControl, IWfForm
    {
        WfFormInfo _info;

        public 收文表单()
        {
            InitializeComponent();
        }

        public async void Init(WfFormInfo p_info)
        {
            _info = p_info;

            if (_info.IsNew)
            {
                _fv.Data = new 收文(
                    ID: _info.ID,
                    来文时间: Kit.Now);
            }
            else
            {
                _fv.Data = await AtCm.GetByID<收文>(_info.ID);
            }

            switch (_info.State)
            {
                case "接收文件":
                    _fv.Hide("市场部经理意见", "综合部经理意见", "收文完成时间");
                    _fv.Data.To<收文>().Cells["文件附件"].PropertyChanged += OnUploaded;
                    break;
                case "市场部":
                    _fv.Hide("综合部经理意见", "收文完成时间");
                    break;
                case "综合部":
                    _fv.Hide("市场部经理意见", "收文完成时间");
                    break;
                case "返回收文人":
                    _fv.Hide("收文完成时间");
                    break;
            }
        }

        void OnUploaded(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Val")
            {
                var fi = _fv["文件附件"].To<CFile>().FileList.Items.FirstOrDefault();
                if (fi != null)
                    _fv.Data.To<收文>().文件标题 = fi.Title;
            }
        }

        public Task<bool> Save()
        {
            var data = _fv.Data.To<收文>();
            if (data.IsAdded || data.IsChanged)
                return AtCm.Save(data);

            return Task.FromResult(true);
        }

        public Task<bool> Delete()
        {
            var data = _fv.Data.To<收文>();
            if (!data.IsAdded)
                return AtCm.Delete(data);

            return Task.FromResult(true);
        }

        public string GetPrcName()
        {
            return _fv.Data.To<收文>().文件标题;
        }
    }
}