#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-02-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.MgrDemo.Crud;
using Microsoft.UI.Xaml;
using System.Text;
#endregion

namespace Dt.MgrDemo
{
    public partial class AccessDemo : Win
    {
        public AccessDemo()
        {
            InitializeComponent();
        }

        async void OnInsert(object sender, RoutedEventArgs e)
        {
            var obj = await CrudX.New(new Random().Next(10000).ToString());
            await obj.Save();
        }

        async void OnUpdate(object sender, RoutedEventArgs e)
        {

        }
    }
}