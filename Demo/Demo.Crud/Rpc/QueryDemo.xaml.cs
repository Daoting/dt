#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-02-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
#endregion

namespace Demo.Crud;

public partial class QueryDemo : Win
{
    public QueryDemo()
    {
        InitializeComponent();
    }

    async void OnCommonTbl(object sender, RoutedEventArgs e)
    {
        var tbl = await At.Query("#", new { f = "crud_基础" });
        //var tbl = await At.Query("#", new { f = ("crud_基础", "") });
        ShowTable(tbl);
    }

    async void OnCommonWhere(object sender, RoutedEventArgs e)
    {
        var tbl = await At.Query("#", new { f = ("crud_基础", "名称 like '%批量%'") });
        //var tbl = await At.Query("#", new { f = (object[])["crud_基础", "名称 like '%批量%'"] });
        //var tbl = await At.Query("#", new { f = new List<string> { "crud_基础", "名称 like '%批量%'" } });
        ShowTable(tbl);
    }

    async void OnCommonField(object sender, RoutedEventArgs e)
    {
        var tbl = await At.Query("#", new { f = ("crud_基础", "名称 like '%批量%'", "id,名称") });
        ShowTable(tbl);
    }

    async void OnCommonOrder(object sender, RoutedEventArgs e)
    {
        var tbl = await At.Query("#", new { f = ("crud_基础", "名称 like '%批量%' order by 序列 desc") });
        ShowTable(tbl);
    }

    async void OnSql(object sender, RoutedEventArgs e)
    {
        var tbl = await At.Query("#sql无参数");
        ShowTable(tbl);
    }

    async void OnSqlParams(object sender, RoutedEventArgs e)
    {
        var tbl = await At.Query("#含参数sql", new { id = 1 });
        ShowTable(tbl);
    }

    async void OnSqlArgs(object sender, RoutedEventArgs e)
    {
        var tbl = await At.Query("#含格式化串sql", new { f = "名称 like '%批量%' order by 序列 desc" });
        ShowTable(tbl);
    }

    async void OnSqlArgsParams(object sender, RoutedEventArgs e)
    {
        var tbl = await At.Query("#含格式化串含参数sql", new { f = "'%批量%'", 序列 = 100 });
        ShowTable(tbl);
    }
    
    void ShowTable(Table tbl)
    {
        _tbInfo.Text = $"调用成功：\r\n共返回{tbl.Count}行";
    }

}