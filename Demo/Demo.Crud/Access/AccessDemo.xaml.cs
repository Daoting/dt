#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-02-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Mgr.Rbac;
using Microsoft.UI.Xaml;
#endregion

namespace Demo.Crud
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
            var x = await 基础X.New("单个" + _rnd.Next(10000).ToString());
            await x.Save();

            //await EntityEx.Save(default(基础X));
        }

        async void OnUpdate(object sender, RoutedEventArgs e)
        {
            var x = await 基础X.First(null);
            if (x != null)
            {
                x.名称 = _rnd.Next(1000).ToString();
                await x.Save();
            }
        }

        async void OnChangeSvc(object sender, RoutedEventArgs e)
        {
            At.Svc = "lob";
            
            var x = await UserX.First(null);
            x = await At.First<UserX>("select * from cm_user");

            At.Reset();

            x = await UserX.First(null);
            x = await At.First<UserX>("select * from cm_user");
        }

        async void OnDelete(object sender, RoutedEventArgs e)
        {
            var x = await 基础X.First(null);
            if (x != null)
            {
                await x.Delete();
            }
        }

        async void OnBatchInsert(object sender, RoutedEventArgs e)
        {
            //var ls = new List<基础X>();
            //for (int i = 0; i < 3; i++)
            //{
            //    ls.Add(await 基础X.New("批量" + rnd.Next(1000)));
            //}
            //await ls.Save();

            var tbl = new Table<基础X>();
            for (int i = 0; i < 3; i++)
            {
                tbl.Add(await 基础X.New("批量" + _rnd.Next(1000)));
            }
            await tbl.Save();
        }

        async void OnBatch(object sender, RoutedEventArgs e)
        {
            var tbl = await 基础X.Page(0, 2, null);
            // 更
            if (tbl.Count > 0)
                tbl[0].名称 = "批增更" + _rnd.Next(1000);
            // 增
            tbl.Add(await 基础X.New("名称" + _rnd.Next(1000)));
            await tbl.Save();
        }

        async void OnSaveTable(object sender, RoutedEventArgs e)
        {
            var tbl = await 基础X.Page(0, 4, null);
            if (tbl.Count > 1)
            {
                tbl.LockCollection();
                // 删
                tbl.RemoveAt(0);
                // 更
                tbl[0].名称 = "批增更删" + _rnd.Next(1000);
            }
            // 增
            tbl.Add(await 基础X.New("批增更删" + _rnd.Next(1000)));
            await tbl.Save();
        }

        async void OnBatchDel(object sender, RoutedEventArgs e)
        {
            var tbl = await 基础X.Page(0, 2, null);
            await tbl.Delete();

            //var ls = new List<基础X>();
            //ls.AddRange(tbl);
            //await ls.Delete();
        }

        async void OnDirectDel(object sender, RoutedEventArgs e)
        {
            var x = await 基础X.First(null);
            if (x != null)
            {
                await 基础X.DelByID(x.ID);
            }
        }

        async void OnDelByID(object sender, RoutedEventArgs e)
        {
            var x = await 基础X.First(null);
            if (x != null)
            {
                x.禁止删除 = false;
                // 先保存
                await x.Save(false);
                await 基础X.DelByID(x.ID, false);
            }
        }
        #endregion

        #region 领域事件
        async void OnInsertEvent(object sender, RoutedEventArgs e)
        {
            var x = await 基础X.New("新增事件" + _rnd.Next(10000).ToString());
            x.发布插入事件 = true;
            await x.Save();
        }

        async void OnUpdateEvent(object sender, RoutedEventArgs e)
        {
            var x = await 基础X.First(null);
            if (x != null)
            {
                x.值变事件 = _rnd.Next(1000).ToString();
                await x.Save();
            }
        }

        async void OnDelEvent(object sender, RoutedEventArgs e)
        {
            var x = await 基础X.First(null);
            if (x != null)
            {
                x.发布删除事件 = true;
                await x.Delete();
            }
        }
        #endregion

        #region 虚拟实体
        async void OnInsertVir(object sender, RoutedEventArgs e)
        {
            var x = await VirX<主表X, 扩展1X, 扩展2X>.New();
            x.E1.主表名称 = "新1";
            x.E2.扩展1名称 = "新2";
            x.E3.扩展2名称 = "新3";
            await x.Save();
        }

        async void OnUpdateVir(object sender, RoutedEventArgs e)
        {
            var x = await VirX<主表X, 扩展1X, 扩展2X>.First(null);
            if (x != null)
            {
                var name = "更" + _rnd.Next(1000);
                x.E1.主表名称 = name;
                x.E2.扩展1名称 = name;
                x.E3.扩展2名称 = name;
                await x.Save();
            }
        }

        async void OnDelVir(object sender, RoutedEventArgs e)
        {
            var x = await VirX<主表X, 扩展1X, 扩展2X>.First(null);
            if (x != null)
                await x.Delete();
        }

        async void OnSaveVir(object sender, RoutedEventArgs e)
        {
            var tbl = await VirX<主表X, 扩展1X, 扩展2X>.Page(0, 4, null);
            if (tbl.Count > 1)
            {
                tbl.LockCollection();
                // 删
                tbl.RemoveAt(0);
                // 更
                var name = "批更" + _rnd.Next(1000);
                tbl[0].E1.主表名称 = name;
                tbl[0].E2.扩展1名称 = name;
                tbl[0].E3.扩展2名称 = name;
            }
            // 增
            var x = await VirX<主表X, 扩展1X, 扩展2X>.New();
            x.E1.主表名称 = "批增1";
            x.E2.扩展1名称 = "批增2";
            x.E3.扩展2名称 = "批增3";
            tbl.Add(x);
            await tbl.Save();
        }

        async void OnDirectDelVir(object sender, RoutedEventArgs e)
        {
            var x = await VirX<主表X, 扩展1X, 扩展2X>.First(null);
            if (x != null)
                await VirX<主表X, 扩展1X, 扩展2X>.DelByID(x.ID);
        }

        async void OnDelByIDVir(object sender, RoutedEventArgs e)
        {
            var x = await VirX<主表X, 扩展1X, 扩展2X>.First(null);
            if (x != null)
                await VirX<主表X, 扩展1X, 扩展2X>.DelByID(x.ID, false);
        }
        #endregion

        #region 父子实体
        async void OnInsertWithChild(object sender, RoutedEventArgs e)
        {
            var x = await 父表X.New("新增");
            for (int i = 0; i < 2; i++)
            {
                x.Tbl1.Add(await 大儿X.New(x.ID, "新增" + i));
                x.Tbl2.Add(await 小儿X.New(x.ID, "新增" + i));
            }
            await x.SaveWithChild();
        }

        async void OnUpdateWithChild(object sender, RoutedEventArgs e)
        {
            var x = await 父表X.First(null);
            if (x != null)
            {
                x = await 父表X.GetByIDWithChild(x.ID);
                var name = "修改" + _rnd.Next(1000);
                x.父名 = name;

                if (x.Tbl1 != null)
                {
                    foreach (var item in x.Tbl1)
                    {
                        item.大儿名 = name;
                    }
                }

                if (x.Tbl2 != null && x.Tbl2.Count > 1)
                {
                    x.Tbl2.LockCollection();
                    x.Tbl2.RemoveAt(0);
                    foreach (var item in x.Tbl2)
                    {
                        item.小儿名 = name;
                    }
                }
                await x.SaveWithChild();
            }
        }

        async void OnQueryWithChild(object sender, RoutedEventArgs e)
        {
            var x = await 父表X.First(null);
            string msg = "";
            if (x != null)
            {
                x = await 父表X.GetByIDWithChild(x.ID);
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
            var x = await 缓存表X.New(Kit.NewGuid.Substring(0, 11), _rnd.Next(10000).ToString());
            await x.Save();
        }

        async void OnUpdateCache(object sender, RoutedEventArgs e)
        {
            var x = await 缓存表X.First(null);
            if (x != null)
            {
                x.姓名 = _rnd.Next(1000).ToString();
                await x.Save();
            }
        }

        async void OnDelCache(object sender, RoutedEventArgs e)
        {
            var x = await 缓存表X.First(null);
            if (x != null)
            {
                await x.Delete();
            }
        }

        async void OnCacheByKey(object sender, RoutedEventArgs e)
        {
            var x = await 缓存表X.First(null);
            if (x != null)
            {
                x = await 缓存表X.GetFromCacheFirst("手机号", x.手机号);
            }
        }
        #endregion

        #region 领域服务
        async void OnSaveDs(object sender, RoutedEventArgs e)
        {
            await UsageDs.BatchSave();
        }
        #endregion

        #region 存储过程
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
            Kit.Msg(msg);
        }
        #endregion
    }
}