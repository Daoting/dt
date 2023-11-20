#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Mgr;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
#endregion

namespace Dt.MgrDemo.Workflow
{
    [WfForm("收文样例")]
    public partial class 收文Form : Tab, IWfForm
    {
        WfFormInfo _info;

        public 收文Form()
        {
            InitializeComponent();
        }

        public async Task Init(WfFormInfo p_info)
        {
            _info = p_info;
            Title = p_info.PrcDef.Name;

            if (_info.IsNew)
            {
                _fv.Data = new 收文X(
                    ID: _info.ID,
                    来文时间: Kit.Now);
            }
            else
            {
                _fv.Data = await 收文X.GetByID(_info.ID);
            }

            switch (_info.State)
            {
                case "接收文件":
                    _fv.Hide("市场部经理意见", "综合部经理意见", "收文完成时间");
                    _fv.Data.To<收文X>().Cells["文件附件"].PropertyChanged += OnUploaded;
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

            Menu = await _info.CreateMenu(_fv);
        }

        void OnUploaded(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Val")
            {
                var fi = _fv["文件附件"].To<CFile>().FileList.Items.FirstOrDefault();
                if (fi != null)
                    _fv.Data.To<收文X>().文件标题 = fi.Title;
            }
        }

        public Task<bool> Save()
        {
            var data = _fv.Data.To<收文X>();
            if (data.IsAdded || data.IsChanged)
                return data.Save();

            return Task.FromResult(true);
        }

        public Task<bool> Delete()
        {
            var data = _fv.Data.To<收文X>();
            if (!data.IsAdded)
                return data.Delete();

            return Task.FromResult(true);
        }

        public string GetPrcName()
        {
            return _fv.Data.To<收文X>().文件标题;
        }
    }
}