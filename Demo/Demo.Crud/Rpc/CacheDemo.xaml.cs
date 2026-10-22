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

public partial class CacheDemo : Win
{
    const string _prefix = "CacheDemo:";
    Random _rnd = new Random();
    
    public CacheDemo()
    {
        InitializeComponent();
    }

    async void OnValStr(object sender, RoutedEventArgs e)
    {
        await GetSetVal("值为字符串", "字符串" + _rnd.Next(10000));
    }

    async void OnValInt(object sender, RoutedEventArgs e)
    {
        await GetSetVal("值为整数", _rnd.Next(10000));
    }

    async void OnValDate(object sender, RoutedEventArgs e)
    {
        await GetSetVal("值为时间", DateTime.Now);
    }

    async void OnValBool(object sender, RoutedEventArgs e)
    {
        await GetSetVal("值为bool", _rnd.Next(10000) % 2 == 0);
    }
       
    async void OnValTable(object sender, RoutedEventArgs e)
    {
        await GetSetVal("值为Table", CreateTable());
    }

    async void OnValTableList(object sender, RoutedEventArgs e)
    {
        var tbl = CreateTable();
        List<Table> tbls = new List<Table>();
        tbls.Add(tbl);
        tbls.Add(tbl.Copy());
        await GetSetVal("值为TableList", tbls);
    }

    static Table CreateTable()
    {
        Table tbl = new Table
        {
            { "col1" },
            { "col2", typeof(bool) },
            { "col3", typeof(long) },
            { "col4", typeof(DateTime) },
            { "col5", typeof(Gender) },
            { "col6", typeof(byte) },
            { "col7", typeof(byte[]) }
        };
        tbl.AddRow(new { col1 = "原始值", col2 = true, col3 = 100L, col4 = DateTime.Now, col5 = Gender.男, col6 = 23, col7 = new byte[] { 10, 20, 30, 40 } });
        tbl.AddRow(new { col1 = "列值21", col4 = DateTime.Now });
        tbl[0]["col1"] = "当前值";
        return tbl;
    }
    
    async void OnValDict(object sender, RoutedEventArgs e)
    {
        await GetSetVal("值为Dict", CreateDict());
    }

    async void OnValDictList(object sender, RoutedEventArgs e)
    {
        await GetSetVal("值为DictList", new List<Dict>() { CreateDict(), CreateDict() });
    }

    static Dict CreateDict()
    {
        Dict dt = new Dict();

        Dict cls = new Dict();
        cls["string"] = "string value";
        cls["bool"] = true;
        cls["int"] = 100;
        cls["long"] = 123234L;
        cls["double"] = 1234.456d;
        cls["datetime"] = DateTime.Now;
        cls["bytearray"] = new byte[] { 10, 12, 14, 16 };
        cls["null"] = null;
        dt["基本数据类型"] = cls;

        dt["DataTable"] = CreateTable();

        dt["字符串列表"] = new List<string>() { "first", "second" };
        dt["bool列表"] = new List<bool>() { true, false, false, true };
        dt["int列表"] = new List<int>() { 100, 123, 432, 78 };
        return dt;
    }
    
    async Task GetSetVal<T>(object p_key, T p_value)
    {
        var suc = await At.SetCache(_prefix + p_key, p_value, _persist.IsChecked == true);
        _tbInfo.Text = $"设置缓存成功：{suc}";
        var val = await At.GetCache<T>(_prefix + p_key);
        _tbInfo.Text += $"\r\n获取缓存值：{val}";
    }
    
    async void OnKeyInt(object sender, RoutedEventArgs e)
    {
        var suc = await At.SetCache(12345, "键为整数", _persist.IsChecked == true);
        _tbInfo.Text = $"设置缓存成功：{suc}";
        var val = await At.GetCache<string>(12345);
        _tbInfo.Text += $"\r\n获取缓存值：{val}";
    }

    async void OnKeyDate(object sender, RoutedEventArgs e)
    {
        var suc = await At.SetCache(DateTime.MaxValue, "键为时间", _persist.IsChecked == true);
        _tbInfo.Text = $"设置缓存成功：{suc}";
        var val = await At.GetCache<string>(DateTime.MaxValue);
        _tbInfo.Text += $"\r\n获取缓存值：{val}";
    }

    async void OnDelStr(object sender, RoutedEventArgs e)
    {
        var suc = await At.DeleteCache(_prefix + "值为字符串", _persist.IsChecked == true);
        _tbInfo.Text = $"删除缓存成功：{suc}";
    }

    async void OnBatchDel(object sender, RoutedEventArgs e)
    {
        List<object> keys = new List<object>()
        {
            _prefix + "值为字符串",
            _prefix + "值为整数",
            _prefix + "值为时间",
            _prefix + "值为bool",
            _prefix + "值为Table",
            _prefix + "值为TableList",
            _prefix + "值为Dict",
            _prefix + "值为DictList",
            _prefix + "值为非内置对象",
            _prefix + "值为非内置对象列表",
            12345,
            DateTime.MaxValue
        };
        var suc = await At.BatchDeleteCache(keys, _persist.IsChecked == true);
        _tbInfo.Text = $"批量删除缓存成功：{suc}";
    }

    async void OnBatchStarDel(object sender, RoutedEventArgs e)
    {
        var suc = await At.DelCacheByPattern(_prefix, 0, _persist.IsChecked == true);
        _tbInfo.Text = $"批量删除缓存成功：{suc}";
    }

    async void OnGetKeys(object sender, RoutedEventArgs e)
    {
        var keys = await At.GetCacheKeys(null, 0);
        _tbInfo.Text = $"所有缓存键：\n{string.Join(",\n", keys)}";
    }
}