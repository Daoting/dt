#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-02-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Mgr.Rbac;
using Dt.MgrDemo.Crud;
using Microsoft.UI.Xaml;
using System.Reflection;
using System.Text;
using Windows.UI.Core;
#endregion

namespace Dt.MgrDemo
{
    public partial class DbAccessDemo : Win
    {
        Random _rnd = new Random();
        
        public DbAccessDemo()
        {
            InitializeComponent();
        }

        #region 普通增删改查
        const string _sqlQuery = "select * from demo_crud";
        async void OnQueryMysql(object sender, RoutedEventArgs e)
        {
            var tbl = await AtMysql.Query(_sqlQuery);
            ShowTable(tbl);
        }

        async void OnQueryOrcl(object sender, RoutedEventArgs e)
        {
            var tbl = await AtOrcl.Query(_sqlQuery);
            ShowTable(tbl);
        }

        async void OnQueryPg(object sender, RoutedEventArgs e)
        {
            var tbl = await AtPg.Query(_sqlQuery);
            ShowTable(tbl);
        }

        async void OnQueryMssql(object sender, RoutedEventArgs e)
        {
            var tbl = await AtMssql.Query(_sqlQuery);
            ShowTable(tbl);
        }
        
        void ShowTable(Table tbl)
        {
            _tbInfo.Text = $"调用成功：\r\n共返回{tbl.Count}行";
        }

        async void OnInsertPg(object sender, RoutedEventArgs e)
        {
            var cnt = await AtPg.Exec("insert into demo_crud (id, name, dispidx, mtime, enable_insert_event, enable_name_changed_event, enable_del_event) values (@id, @name, @dispidx, @mtime, true, true, true)", 
                new { id = Kit.NewID, name = "新数据", dispidx = 1000, mtime = Kit.Now });
            _tbInfo.Text = cnt > 0 ? "插入成功" : "插入失败";
        }

        async void OnUpdatePg(object sender, RoutedEventArgs e)
        {
            long id = await AtPg.GetScalar<long>("select id from demo_crud order by id limit 1");
            var cnt = await AtPg.Exec("update demo_crud set name=name||'+' where id=@id", new { id = id });
            _tbInfo.Text = cnt > 0 ? "更新成功" : "更新失败";
        }

        async void OnDelPg(object sender, RoutedEventArgs e)
        {
            long id = await AtPg.GetScalar<long>("select id from demo_crud order by id desc limit 1");
            var cnt = await AtPg.Exec("delete from demo_crud where id=@id", new { id = id });
            _tbInfo.Text = cnt > 0 ? "删除成功" : "删除失败";
        }
        #endregion

        #region 增删改
        async void OnInsert(object sender, RoutedEventArgs e)
        {
            At.SetDb("pgdt");
            var x = await CrudX.New("单个" + _rnd.Next(10000).ToString());
            bool suc = await x.Save();
            _tbInfo.Text = suc ? "pg插入成功" : "pg插入失败";

            At.SetDb("mydt");
            x = await CrudX.New("单个" + _rnd.Next(10000).ToString());
            suc = await x.Save();
            _tbInfo.Text += suc ? "\r\nmysql插入成功" : "\r\nmysql插入失败";

            At.SetDb("orcldt");
            x = await CrudX.New("单个" + _rnd.Next(10000).ToString());
            suc = await x.Save();
            _tbInfo.Text += suc ? "\r\noracle插入成功" : "\r\noracle插入失败";

            At.SetDb("sqldt");
            x = await CrudX.New("单个" + _rnd.Next(10000).ToString());
            suc = await x.Save();
            _tbInfo.Text += suc ? "\r\nmssql插入成功" : "\r\nmssql插入失败";
            
            At.Reset();
        }

        const string _sqlFirst = "where 1=1 order by id";
        async void OnUpdate(object sender, RoutedEventArgs e)
        {
            At.SetDb("pgdt");
            var x = await CrudX.First(_sqlFirst);
            if (x != null)
            {
                x.Name += "+";
                _tbInfo.Text = await x.Save() ? "pg更新成功" : "pg更新失败";
            }

            At.SetDb("mydt");
            x = await CrudX.First(_sqlFirst);
            if (x != null)
            {
                x.Name += "+";
                _tbInfo.Text += await x.Save() ? "\r\nmysql更新成功" : "\r\nmysql更新失败";
            }

            At.SetDb("orcldt");
            x = await CrudX.First(_sqlFirst);
            if (x != null)
            {
                x.Name += "+";
                _tbInfo.Text += await x.Save() ? "\r\noracle更新成功" : "\r\noracle更新失败";
            }

            At.SetDb("sqldt");
            x = await CrudX.First(_sqlFirst);
            if (x != null)
            {
                x.Name += "+";
                _tbInfo.Text += await x.Save() ? "\r\nmssql更新成功" : "\r\nmssql更新失败";
            }

            At.Reset();
        }
        
        async void OnDelete(object sender, RoutedEventArgs e)
        {
            At.SetDb("pgdt");
            var x = await CrudX.First(_sqlFirst);
            if (x != null)
                _tbInfo.Text = await x.Delete() ? "pg删除成功" : "pg删除失败";

            At.SetDb("mydt");
            x = await CrudX.First(_sqlFirst);
            if (x != null)
                _tbInfo.Text += await x.Delete() ? "\r\nmysql删除成功" : "\r\nmysql删除失败";

            At.SetDb("orcldt");
            x = await CrudX.First(_sqlFirst);
            if (x != null)
                _tbInfo.Text += await x.Delete() ? "\r\noracle删除成功" : "\r\noracle删除失败";

            At.SetDb("sqldt");
            x = await CrudX.First(_sqlFirst);
            if (x != null)
                _tbInfo.Text += await x.Delete() ? "\r\nmssql删除成功" : "\r\nmssql删除失败";

            At.Reset();
        }

        async void OnBatchInsert(object sender, RoutedEventArgs e)
        {
            At.SetDb("pgdt");
            var tbl = new Table<CrudX>();
            for (int i = 0; i < 3; i++)
            {
                tbl.Add(await CrudX.New("批量" + _rnd.Next(1000)));
            }
            _tbInfo.Text = await tbl.Save() ? "pg批量插入成功" : "pg批量插入失败";

            At.SetDb("mydt");
            tbl = new Table<CrudX>();
            for (int i = 0; i < 3; i++)
            {
                tbl.Add(await CrudX.New("批量" + _rnd.Next(1000)));
            }
            _tbInfo.Text += await tbl.Save() ? "\r\nmysql批量插入成功" : "\r\nmysql批量插入失败";

            At.SetDb("orcldt");
            tbl = new Table<CrudX>();
            for (int i = 0; i < 3; i++)
            {
                tbl.Add(await CrudX.New("批量" + _rnd.Next(1000)));
            }
            _tbInfo.Text += await tbl.Save() ? "\r\noracle批量插入成功" : "\r\noracle批量插入失败";

            At.SetDb("sqldt");
            tbl = new Table<CrudX>();
            for (int i = 0; i < 3; i++)
            {
                tbl.Add(await CrudX.New("批量" + _rnd.Next(1000)));
            }
            _tbInfo.Text += await tbl.Save() ? "\r\nmssql批量插入成功" : "\r\nmssql批量插入失败";

            At.Reset();
        }

        async void OnBatch(object sender, RoutedEventArgs e)
        {
            At.SetDb("pgdt");
            var tbl = await GetBatchTbl();
            _tbInfo.Text = await tbl.Save() ? "pg批量增更成功" : "pg批量增更失败";

            At.SetDb("mydt");
            tbl = await GetBatchTbl();
            _tbInfo.Text += await tbl.Save() ? "\r\nmysql批量增更成功" : "\r\nmysql批量增更失败";

            At.SetDb("orcldt");
            tbl = await GetBatchTbl();
            _tbInfo.Text += await tbl.Save() ? "\r\noracle批量增更成功" : "\r\noracle批量增更失败";

            At.SetDb("sqldt");
            tbl = await GetBatchTbl();
            _tbInfo.Text += await tbl.Save() ? "\r\nmssql批量增更成功" : "\r\nmssql批量增更失败";
            At.Reset();
        }

        async Task<Table<CrudX>> GetBatchTbl()
        {
            var tbl = await CrudX.Page(0, 2, null);
            // 更
            if (tbl.Count > 0)
                tbl[0].Name = "*批增更" + _rnd.Next(1000);
            // 增
            tbl.Add(await CrudX.New("+批增更" + _rnd.Next(1000)));
            return tbl;
        }
        
        async void OnBatchDel(object sender, RoutedEventArgs e)
        {
            At.SetDb("pgdt");
            var tbl = await CrudX.Page(0, 2, null);
            _tbInfo.Text = await tbl.Delete() ? "pg批量删成功" : "pg批量删失败";

            At.SetDb("mydt");
            tbl = await CrudX.Page(0, 2, null);
            _tbInfo.Text += await tbl.Delete() ? "\r\nmysql批量删成功" : "\r\nmysql批量删失败";

            At.SetDb("orcldt");
            tbl = await CrudX.Page(0, 2, null);
            _tbInfo.Text += await tbl.Delete() ? "\r\noracle批量删成功" : "\r\noracle批量删失败";

            At.SetDb("sqldt");
            tbl = await CrudX.Page(0, 2, null);
            _tbInfo.Text += await tbl.Delete() ? "\r\nmssql批量删成功" : "\r\nmssql批量删失败";
            At.Reset();
        }
        #endregion
    }

    class AtMysql : AccessAgent<AtMysql.Info>
    {
        public class Info : AgentInfo
        {
            public override AccessType Type => AccessType.Database;

            public override string Name => "mydt";
        }
    }

    class AtOrcl : AccessAgent<AtOrcl.Info>
    {
        public class Info : AgentInfo
        {
            public override AccessType Type => AccessType.Database;

            public override string Name => "orcldt";
        }
    }

    class AtPg : AccessAgent<AtPg.Info>
    {
        public class Info : AgentInfo
        {
            public override AccessType Type => AccessType.Database;

            public override string Name => "pgdt";
        }
    }

    class AtMssql : AccessAgent<AtMssql.Info>
    {
        public class Info : AgentInfo
        {
            public override AccessType Type => AccessType.Database;

            public override string Name => "sqldt";
        }
    }
}