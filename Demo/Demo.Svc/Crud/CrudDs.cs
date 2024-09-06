#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-02-13 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
#endregion

namespace Demo.Crud
{
    /// <summary>
    /// 
    /// </summary>
    [Api]
    public class CrudDs : DomainSvc
    {
        Random _rnd = new Random();

        #region 增删改
        public async Task<bool> Insert()
        {
            var x = await 基础X.New("单个" + _rnd.Next(10000).ToString());
            return await x.Save();
        }

        public async Task<bool> Update()
        {
            var x = await 基础X.First(null);
            if (x != null)
            {
                x.名称 = _rnd.Next(1000).ToString();
                return await x.Save();
            }
            return false;
        }

        public async Task<bool> Delete()
        {
            var x = await 基础X.First(null);
            if (x != null)
            {
                return await x.Delete();
            }
            return false;
        }

        public async Task<bool> BatchInsert()
        {
            var tbl = new Table<基础X>();
            for (int i = 0; i < 3; i++)
            {
                tbl.Add(await 基础X.New("批量" + _rnd.Next(1000)));
            }
            return await tbl.Save();
        }

        public async Task<bool> Batch()
        {
            var tbl = await 基础X.Page(0, 2, null);
            // 更
            if (tbl.Count > 0)
                tbl[0].名称 = "批增更" + _rnd.Next(1000);
            // 增
            tbl.Add(await 基础X.New("批增更" + _rnd.Next(1000)));
            return await tbl.Save();
        }

        public async Task<bool> SaveTable()
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
            return await tbl.Save();
        }

        public async Task<bool> BatchDel()
        {
            var tbl = await 基础X.Page(0, 2, null);
            return await tbl.Delete();

            //var ls = new List<基础X>();
            //ls.AddRange(tbl);
            //await ls.Delete();
        }

        public async Task<bool> DirectDel()
        {
            var x = await 基础X.First(null);
            if (x != null)
            {
                return await 基础X.DelByID(x.ID);
            }
            return false;
        }

        public async Task<bool> DelByID()
        {
            var x = await 基础X.First(null);
            if (x != null)
            {
                x.禁止删除 = false;
                // 先保存
                await x.Save(false);
                return await 基础X.DelByID(x.ID, false);
            }
            return false;
        }
        #endregion

        #region 领域事件
        public async Task<bool> InsertEvent()
        {
            var x = await 基础X.New("新增事件" + _rnd.Next(10000).ToString());
            x.发布插入事件 = true;
            return await x.Save();
        }

        public async Task<bool> UpdateEvent()
        {
            var x = await 基础X.First(null);
            if (x != null)
            {
                x.值变事件 = _rnd.Next(1000).ToString();
                return await x.Save();
            }
            return false;
        }

        public async Task<bool> DelEvent()
        {
            var x = await 基础X.First(null);
            if (x != null)
            {
                x.发布删除事件 = true;
                return await x.Delete();
            }
            return false;
        }
        #endregion

        #region 虚拟实体
        public async Task<bool> InsertVir()
        {
            var x = await VirX<主表X, 扩展1X, 扩展2X>.New();
            x.E1.ID = await 主表X.NewID();
            x.E1.主表名称 = "新1";
            x.E2.扩展1名称 = "新2";
            x.E3.扩展2名称 = "新3";
            return await x.Save();
        }

        public async Task<bool> UpdateVir()
        {
            var x = await VirX<主表X, 扩展1X, 扩展2X>.First(null);
            if (x != null)
            {
                var name = "更" + _rnd.Next(1000);
                x.E1.主表名称 = name;
                x.E2.扩展1名称 = name;
                x.E3.扩展2名称 = name;
                return await x.Save();
            }
            return false;
        }

        public async Task<bool> DelVir()
        {
            var x = await VirX<主表X, 扩展1X, 扩展2X>.First(null);
            if (x != null)
                return await x.Delete();
            return false;
        }

        public async Task<bool> SaveVir()
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
            x.E1.ID = await 主表X.NewID();
            x.E1.主表名称 = "批增1";
            x.E2.扩展1名称 = "批增2";
            x.E3.扩展2名称 = "批增3";
            tbl.Add(x);
            return await tbl.Save();
        }

        public async Task<bool> DirectDelVir()
        {
            var x = await VirX<主表X, 扩展1X, 扩展2X>.First(null);
            if (x != null)
                return await VirX<主表X, 扩展1X, 扩展2X>.DelByID(x.ID);
            return false;
        }

        public async Task<bool> DelByIDVir()
        {
            var x = await VirX<主表X, 扩展1X, 扩展2X>.First(null);
            if (x != null)
                return await VirX<主表X, 扩展1X, 扩展2X>.DelByID(x.ID, false);
            return false;
        }
        #endregion

        #region 父子实体
        public async Task<bool> InsertWithChild()
        {
            var x = await 父表X.New("新增");
            for (int i = 0; i < 2; i++)
            {
                x.Tbl1.Add(await 大儿X.New(x.ID, "新增" + i));
                x.Tbl2.Add(await 小儿X.New(x.ID, "新增" + i));
            }
            return await x.SaveWithChild();
        }

        public async Task<bool> UpdateWithChild()
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
                return await x.SaveWithChild();
            }
            return false;
        }

        public async Task<string> QueryWithChild()
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
            return msg;
        }

        #endregion

        #region 缓存
        public async Task<bool> InsertCache()
        {
            var x = await 缓存表X.New(Kit.NewGuid.Substring(0, 11), _rnd.Next(10000).ToString());
            return await x.Save();
        }

        public async Task<bool> UpdateCache()
        {
            var x = await 缓存表X.First(null);
            if (x != null)
            {
                x.姓名 = _rnd.Next(1000).ToString();
                return await x.Save();
            }
            return false;
        }

        public async Task<bool> DelCache()
        {
            var x = await 缓存表X.First(null);
            if (x != null)
            {
                return await x.Delete();
            }
            return false;
        }

        public async Task<string> CacheByKey()
        {
            var x = await 缓存表X.First(null);
            if (x != null)
            {
                var model = await EntitySchema.Get(typeof(缓存表X));
                var key = $"{model.Schema.Name}:手机号:{x.手机号}";
                await Kit.DeleteCache(key);

                await 缓存表X.GetFromCacheFirst("手机号", x.手机号);
                var val = await Kit.StringGet<string>(key);
                return $"缓存键：{key}\r\n缓存值：{val}";
            }
            return "无数据";
        }
        #endregion
    }
}