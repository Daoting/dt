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
        Random rnd = new Random();

        public AccessDemo()
        {
            InitializeComponent();
        }

        async void OnInsert(object sender, RoutedEventArgs e)
        {
            var x = await CrudX.New("单个" + rnd.Next(10000).ToString());
            await x.Save();

            //await EntityEx.Save(default(CrudX));
        }

        async void OnUpdate(object sender, RoutedEventArgs e)
        {
            var x = await CrudX.First(null);
            if (x != null)
            {
                x.Name = rnd.Next(1000).ToString();
                await x.Save();
            }
        }

        async void OnDelete(object sender, RoutedEventArgs e)
        {
            var x = await CrudX.First(null);
            if (x != null)
            {
                await x.Delete();
            }
        }

        async void OnBatchInsert(object sender, RoutedEventArgs e)
        {
            //var ls = new List<CrudX>();
            //for (int i = 0; i < 3; i++)
            //{
            //    ls.Add(await CrudX.New("批量" + rnd.Next(1000)));
            //}
            //await ls.Save();

            var tbl = new Table<CrudX>();
            for (int i = 0; i < 3; i++)
            {
                tbl.Add(await CrudX.New("批量" + rnd.Next(1000)));
            }
            await tbl.Save();
        }

        async void OnBatch(object sender, RoutedEventArgs e)
        {
            var tbl = await CrudX.Query(" true limit 2");
            // 更
            if (tbl.Count > 0)
                tbl[0].Name = "批增更" + rnd.Next(1000);
            // 增
            tbl.Add(await CrudX.New("批增更" + rnd.Next(1000)));
            await tbl.Save();
        }

        async void OnSaveTable(object sender, RoutedEventArgs e)
        {
            var tbl = await CrudX.Query(" true limit 4");
            if (tbl.Count > 1)
            {
                tbl.RecordDeleted();
                // 删
                tbl.RemoveAt(0);
                // 更
                tbl[0].Name = "批增更删" + rnd.Next(1000);
            }
            // 增
            tbl.Add(await CrudX.New("批增更删" + rnd.Next(1000)));
            await tbl.Save();
        }

        async void OnBatchDel(object sender, RoutedEventArgs e)
        {
            var tbl = await CrudX.Query(" true limit 2");
            await tbl.Delete();

            //var ls = new List<CrudX>();
            //ls.AddRange(tbl);
            //await ls.Delete();
        }

        async void OnDirectDel(object sender, RoutedEventArgs e)
        {
            var x = await CrudX.First(null);
            if (x != null)
            {
                await CrudX.DelByID(x.ID);
            }
        }

        async void OnDelByID(object sender, RoutedEventArgs e)
        {
            var x = await CrudX.First(null);
            if (x != null)
            {
                x.EnableDelEvent = true;
                // 先保存
                await x.Save(false);
                await CrudX.DelByID(x.ID, false);
            }
        }
    }
}