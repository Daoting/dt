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
        var tbl = CreateTable();
        var r = tbl.AddRow(new { id = Kit.NewID, 名称 = "save" + _rnd.Next(10000).ToString(), 序列 = 1, 禁止选中 = true, 禁止保存 = false, 禁止删除 = false, 发布插入事件 = true, 发布删除事件 = false, 创建时间 = DateTime.Now, 修改时间 = DateTime.Now });
        var suc = await At.Save(new SaveItem { Table = "crud_基础", Data = tbl });
        _tbInfo.Text = suc.ToString();
    }

    async void OnUpdate(object sender, RoutedEventArgs e)
    {
        var tbl = await At.Query("select * from crud_基础 limit 1");
        if (tbl.Count > 0)
        {
            tbl[0]["名称"] = tbl[0].Str("名称") + "+";
            var suc = await At.Save(new SaveItem { Table = "crud_基础", Data = tbl });
            _tbInfo.Text = suc.ToString();
        }
    }

    async void OnBatchMix(object sender, RoutedEventArgs e)
    {
        List<SaveItem> ls = new List<SaveItem>();
        var tbl = await At.Query("select * from crud_基础 order by 创建时间 limit 1");
        if (tbl.Count > 0)
        {
            ls.Add(new SaveItem { Table = "crud_基础", IsDeleted = true, Data = tbl });
        }

        tbl = await At.Query("select * from crud_基础 order by 创建时间 desc limit 1");
        if (tbl.Count > 0)
        {
            tbl[0]["名称"] = tbl[0].Str("名称") + "#";
            ls.Add(new SaveItem { Table = "crud_基础", Data = tbl });
        }

        tbl = CreateTable();
        var r = tbl.AddRow(new { id = Kit.NewID, 名称 = "save" + _rnd.Next(10000).ToString(), 序列 = 1, 禁止选中 = true, 禁止保存 = false, 禁止删除 = false, 发布插入事件 = true, 发布删除事件 = false, 创建时间 = DateTime.Now, 修改时间 = DateTime.Now });
        ls.Add(new SaveItem { Table = "crud_基础", Data = tbl });

        var suc = await At.Save(ls);
        _tbInfo.Text = suc.ToString();
    }

    async void OnDelete(object sender, RoutedEventArgs e)
    {
        var tbl = await At.Query("select * from crud_基础 limit 1");
        if (tbl.Count > 0)
        {
            var suc = await At.Save(new SaveItem { Table = "crud_基础", IsDeleted = true, Data = tbl });
            _tbInfo.Text = suc.ToString();
        }
    }

    async void OnInsertSqlite(object sender, RoutedEventArgs e)
    {
        var cx = new CookieX(Key: _rnd.Next(10000).ToString(), Val: "abc");
        Table tbl = Table.Clone(cx);
        var r = tbl.AddRow(new { key = "" + _rnd.Next(10000).ToString(), val = "a1" });
        var suc = await AtState.Save(new SaveItem { Table = "cookie", Data = tbl });
        _tbInfo.Text = suc.ToString();
    }

    Table CreateTable()
    {
        var x = new 基础X(ID: Kit.NewID, 名称: _rnd.Next(10000).ToString());
        Table tbl = Table.Clone(x);
        return tbl;
    }

    async void OnInsertRow(object sender, RoutedEventArgs e)
    {
        var row = CreateRow();
        row["名称"] = "row" + _rnd.Next(10000).ToString();
        var suc = await At.Save(new SaveItem { Table = "crud_基础", Data = row });
        _tbInfo.Text = suc.ToString();
    }

    async void OnUpdateRow(object sender, RoutedEventArgs e)
    {
        var r = await At.First("select * from crud_基础 limit 1");
        if (r != null)
        {
            r["名称"] = r.Str("名称") + "%";
            var suc = await At.Save(new SaveItem { Table = "crud_基础", Data = r });
            _tbInfo.Text = suc.ToString();
        }
    }

    async void OnDeleteRow(object sender, RoutedEventArgs e)
    {
        var r = await At.First("select * from crud_基础 limit 1");
        if (r != null)
        {
            var suc = await At.Save(new SaveItem { Table = "crud_基础", IsDeleted = true, Data = r });
            _tbInfo.Text = suc.ToString();
        }
    }

    async void OnBatchRow(object sender, RoutedEventArgs e)
    {
        List<SaveItem> ls = new List<SaveItem>();
        var r = await At.First("select * from crud_基础 order by 创建时间 limit 1");
        if (r != null)
        {
            ls.Add(new SaveItem { Table = "crud_基础", IsDeleted = true, Data = r });
        }

        r = await At.First("select * from crud_基础 order by 创建时间 desc limit 1");
        if (r != null)
        {
            r["名称"] = r.Str("名称") + "#";
            ls.Add(new SaveItem { Table = "crud_基础", Data = r });
        }

        r = CreateRow();
        r["名称"] = "row" + _rnd.Next(10000).ToString();
        ls.Add(new SaveItem { Table = "crud_基础", Data = r });

        var suc = await At.Save(ls);
        _tbInfo.Text = suc.ToString();
    }

    Row CreateRow()
    {
        var x = new Row
        {
            {"id", Kit.NewID },
            {"parent_id", default(long) },
            {"序列", 0 },
            {"名称", "名称" },
            {"限长4", default },
            {"不重复", default },
            {"禁止选中", false },
            {"禁止保存", false },
            {"禁止删除", false },
            {"值变事件", false },
            {"发布插入事件", false },
            {"发布删除事件", false },
            {"创建时间", default(DateTime) },
            {"修改时间", default(DateTime) },
        };
        x.IsAdded = true;
        return x;
    }
}