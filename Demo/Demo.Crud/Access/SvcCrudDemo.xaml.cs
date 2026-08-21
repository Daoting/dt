using Dt.Mgr.Rbac;
using Microsoft.UI.Xaml;

namespace Demo.Crud;

public partial class SvcCrudDemo : Win
{
    Random _rnd = new Random();

    public SvcCrudDemo()
    {
        InitializeComponent();
    }

    async void OnInsert(object sender, RoutedEventArgs e)
    {
        //var tbl = await CreateTable();
        //var r = tbl.AddRow(new { id = Kit.NewID, 名称 = "save" + _rnd.Next(10000).ToString(), 序列 = 1, 禁止选中 = true, 禁止保存 = false, 禁止删除 = false, 发布插入事件 = true, 发布删除事件 = false, 创建时间 = DateTime.Now, 修改时间 = DateTime.Now });
        //var suc  = await At.Save(tbl);
        //_tbInfo.Text = suc.ToString();
    }

    async void OnUpdate(object sender, RoutedEventArgs e)
    {
        //var tbl = await At.Query("select * from crud_基础 limit 1");
        //if (tbl.Count > 0)
        //{
        //    tbl.SetTblName("crud_基础");
        //    tbl[0]["名称"] = tbl[0].Str("名称") + "+";
        //    var suc = await At.Save(tbl);
        //    _tbInfo.Text = suc.ToString();
        //}
    }

    async void OnBatchMix(object sender, RoutedEventArgs e)
    {
        
    }

    async void OnDelete(object sender, RoutedEventArgs e)
    {
        //var tbl = await At.Query("select * from crud_基础 limit 1");
        //if (tbl.Count > 0)
        //{
        //    tbl.SetTblName("crud_基础");
        //    tbl.LockCollection();
        //    tbl.RemoveAt(0);
        //    var suc = await At.Save(tbl);
        //    _tbInfo.Text = suc.ToString();
        //}
    }

    async void OnInsertSqlite(object sender, RoutedEventArgs e)
    {
        var cx = new CookieX(Key: _rnd.Next(10000).ToString(), Val: "abc");
        Table tbl = Table.Clone(cx);
        var r = tbl.AddRow(new { key = "" + _rnd.Next(10000).ToString(), val = "a1" });
        var suc = await AtState.Save(new SaveItem { Table = "cookie", Data = tbl});
        _tbInfo.Text = suc.ToString();
    }
    
    async Task<Table> CreateTable()
    {
        //var x = new 基础X(ID: Kit.NewID, 名称: _rnd.Next(10000).ToString());
        //Table tbl = Table.Clone(x);
        //tbl.SetTblName("crud_基础");
        //return tbl;
        return null;
    }
}