#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-01-17 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Base.Tools
{
    public sealed partial class DbImport : Tab
    {
        DbInitInfo _info;

        public DbImport()
        {
            InitializeComponent();
        }

        protected override void OnFirstLoaded()
        {
            _info = (DbInitInfo)NaviParams;
            _info.Log = AppendMsg;
        }

        async void OnDt(object sender, RoutedEventArgs e)
        {
            if (await Kit.Confirm("导入时将删除旧的同名表、视图等！！！\r\n是否继续？"))
            {
                _tbInfo.Text = "开始导入初始表结构及数据...";
                await _info.Tools.ImportInit();
            }
        }

        async void OnOther(object sender, RoutedEventArgs e)
        {
            var picker = Kit.GetFileOpenPicker();
            picker.FileTypeFilter.Add(".sql");
            picker.FileTypeFilter.Add("*");
            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                _tbInfo.Text = "开始导入...";
                await _info.Tools.ImportFromFile(file);
            }
        }

        void AppendMsg(string p_msg)
        {
            Kit.RunAsync(() =>
            {
                _tbInfo.Text = _tbInfo.Text + "\r" + p_msg;
                _tbInfo.Focus(FocusState.Programmatic);
                _tbInfo.Select(_tbInfo.Text.Length, 0);
            });
        }
    }
}
