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
    public partial class AccessDemo : Win
    {
        Random _rnd = new Random();

        public AccessDemo()
        {
            InitializeComponent();
        }

        #region 增删改
        async void OnInsert(object sender, RoutedEventArgs e)
        {
            var x = await CrudX.New("单个" + _rnd.Next(10000).ToString());
            await x.Save();

            //await EntityEx.Save(default(CrudX));
        }

        async void OnUpdate(object sender, RoutedEventArgs e)
        {
            Kit.SvcName = "demo";
            //var x = await CrudX.First(null);
            //if (x != null)
            //{
            //    x.Name = _rnd.Next(1000).ToString();
            //    await x.Save();
            //}
            var x = await UserX.First(null);
            x = await At.First<UserX>("select * from cm_user");
            Kit.ResetSvcName();
            x = await UserX.First(null);
            x = await At.First<UserX>("select * from cm_user");
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
                tbl.Add(await CrudX.New("批量" + _rnd.Next(1000)));
            }
            await tbl.Save();
        }

        async void OnBatch(object sender, RoutedEventArgs e)
        {
            var tbl = await CrudX.Page(0, 2, null);
            // 更
            if (tbl.Count > 0)
                tbl[0].Name = "批增更" + _rnd.Next(1000);
            // 增
            tbl.Add(await CrudX.New("批增更" + _rnd.Next(1000)));
            await tbl.Save();
        }

        async void OnSaveTable(object sender, RoutedEventArgs e)
        {
            var tbl = await CrudX.Page(0, 4, null);
            if (tbl.Count > 1)
            {
                tbl.RecordDeleted();
                // 删
                tbl.RemoveAt(0);
                // 更
                tbl[0].Name = "批增更删" + _rnd.Next(1000);
            }
            // 增
            tbl.Add(await CrudX.New("批增更删" + _rnd.Next(1000)));
            await tbl.Save();
        }

        async void OnBatchDel(object sender, RoutedEventArgs e)
        {
            var tbl = await CrudX.Page(0, 2, null);
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
        #endregion

        #region 领域事件
        async void OnInsertEvent(object sender, RoutedEventArgs e)
        {
            var x = await CrudX.New("新增事件" + _rnd.Next(10000).ToString());
            x.EnableInsertEvent = true;
            await x.Save();
        }

        async void OnUpdateEvent(object sender, RoutedEventArgs e)
        {
            var x = await CrudX.First(null);
            if (x != null)
            {
                x.EnableNameChangedEvent = true;
                x.Name = "Name变化事件" + _rnd.Next(1000).ToString();
                await x.Save();
            }
        }

        async void OnDelEvent(object sender, RoutedEventArgs e)
        {
            var x = await CrudX.First(null);
            if (x != null)
            {
                x.EnableDelEvent = true;
                await x.Delete();
            }
        }
        #endregion

        #region 虚拟实体
        async void OnInsertVir(object sender, RoutedEventArgs e)
        {
            var x = await VirX<Virtbl1X, Virtbl2X, Virtbl3X>.New();
            x.E1.Name1 = "新1";
            x.E2.Name2 = "新2";
            x.E3.Name3 = "新3";
            await x.Save();
        }

        async void OnUpdateVir(object sender, RoutedEventArgs e)
        {
            var x = await VirX<Virtbl1X, Virtbl2X, Virtbl3X>.First(null);
            if (x != null)
            {
                var name = "更" + _rnd.Next(1000);
                x.E1.Name1 = name;
                x.E2.Name2 = name;
                x.E3.Name3 = name;
                await x.Save();
            }
        }

        async void OnDelVir(object sender, RoutedEventArgs e)
        {
            var x = await VirX<Virtbl1X, Virtbl2X, Virtbl3X>.First(null);
            if (x != null)
                await x.Delete();
        }

        async void OnSaveVir(object sender, RoutedEventArgs e)
        {
            var tbl = await VirX<Virtbl1X, Virtbl2X, Virtbl3X>.Page(0, 4, null);
            if (tbl.Count > 1)
            {
                tbl.RecordDeleted();
                // 删
                tbl.RemoveAt(0);
                // 更
                var name = "批更" + _rnd.Next(1000);
                tbl[0].E1.Name1 = name;
                tbl[0].E2.Name2 = name;
                tbl[0].E3.Name3 = name;
            }
            // 增
            var x = await VirX<Virtbl1X, Virtbl2X, Virtbl3X>.New();
            x.E1.Name1 = "批增1";
            x.E2.Name2 = "批增2";
            x.E3.Name3 = "批增3";
            tbl.Add(x);
            await tbl.Save();
        }

        async void OnDirectDelVir(object sender, RoutedEventArgs e)
        {
            var x = await VirX<Virtbl1X, Virtbl2X, Virtbl3X>.First(null);
            if (x != null)
                await VirX<Virtbl1X, Virtbl2X, Virtbl3X>.DelByID(x.ID);
        }

        async void OnDelByIDVir(object sender, RoutedEventArgs e)
        {
            var x = await VirX<Virtbl1X, Virtbl2X, Virtbl3X>.First(null);
            if (x != null)
                await VirX<Virtbl1X, Virtbl2X, Virtbl3X>.DelByID(x.ID, false);
        }
        #endregion

        #region 父子实体
        async void OnInsertWithChild(object sender, RoutedEventArgs e)
        {
            var x = await ParTblX.New("新增");
            for (int i = 0; i < 2; i++)
            {
                x.Tbl1.Add(await ChildTbl1X.New(x.ID, "新增" + i));
                x.Tbl2.Add(await ChildTbl2X.New(x.ID, "新增" + i));
            }
            await x.SaveWithChild();
        }

        async void OnUpdateWithChild(object sender, RoutedEventArgs e)
        {
            var x = await ParTblX.First(null);
            if (x != null)
            {
                x = await ParTblX.GetByIDWithChild(x.ID);
                var name = "修改" + _rnd.Next(1000);
                x.Name = name;

                if (x.Tbl1 != null)
                {
                    foreach (var item in x.Tbl1)
                    {
                        item.ItemName = name;
                    }
                }

                if (x.Tbl2 != null && x.Tbl2.Count > 1)
                {
                    x.Tbl2.RecordDeleted();
                    x.Tbl2.RemoveAt(0);
                    foreach (var item in x.Tbl2)
                    {
                        item.ItemName = name;
                    }
                }
                await x.SaveWithChild();
            }
        }

        async void OnQueryWithChild(object sender, RoutedEventArgs e)
        {
            var x = await ParTblX.First(null);
            string msg = "";
            if (x != null)
            {
                x = await ParTblX.GetByIDWithChild(x.ID);
                if (x.Tbl1 != null)
                    msg = $"子表1：{x.Tbl1.Count}行";
                else
                    msg = "子表1无数据";

                if (x.Tbl2 != null)
                    msg += $"\r\n子表2：{x.Tbl2.Count}行";
                else
                    msg += $"\r\n子表2无数据";
            }
            else
            {
                msg = "无数据！";
            }
            Kit.Msg(msg);
        }

        #endregion

        #region 缓存
        async void OnInsertCache(object sender, RoutedEventArgs e)
        {
            var x = await CacheTbl1X.New(Kit.NewGuid, _rnd.Next(10000).ToString());
            await x.Save();
        }

        async void OnUpdateCache(object sender, RoutedEventArgs e)
        {
            var x = await CacheTbl1X.First(null);
            if (x != null)
            {
                x.Name = _rnd.Next(1000).ToString();
                await x.Save();
            }
        }

        async void OnDelCache(object sender, RoutedEventArgs e)
        {
            var x = await CacheTbl1X.First(null);
            if (x != null)
            {
                await x.Delete();
            }
        }

        async void OnCacheByKey(object sender, RoutedEventArgs e)
        {
            var x = await CacheTbl1X.First(null);
            if (x != null)
            {
                x = await CacheTbl1X.GetFromCacheFirst("phone", x.Phone);
            }
        }
        #endregion

        #region 领域服务
        async void OnSaveDs(object sender, RoutedEventArgs e)
        {
            await UsageDs.BatchSave();
        }
        #endregion

        async void OnQueryBySp(object sender, RoutedEventArgs e)
        {
            var db = await At.GetDbType();
            if (db == DatabaseType.PostgreSql)
            {
                Kit.Warn("PostgreSql的存储过程不支持返回数据集");
                return;
            }

            var tbl = await At.Query("demo_用户可访问的菜单", new { p_userid = Kit.UserID });
            string msg = "可访问的菜单：\r\n";
            foreach (var r in tbl)
            {
                msg += r.Str(1);
                msg += "、";
            }
            Kit.Msg(msg, 0);
        }

    }
}