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
        public 收文Form()
        {
            InitializeComponent();
        }

        public WfFormInfo Info { get; set; }

        protected override async void OnFirstLoaded()
        {
            Title = Info.PrcDef.Name;

            if (Info.IsNew)
            {
                _fv.Data = new 收文X(
                    ID: Info.ID,
                    来文时间: Kit.Now);
            }
            else
            {
                _fv.Data = await 收文X.GetByID(Info.ID);
            }

            switch (Info.State)
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

            Menu = await Info.CreateMenu();
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

        public Task<bool> Save(bool p_isSend)
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