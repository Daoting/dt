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
            var x = await CrudX.New("单个" + _rnd.Next(10000).ToString());
            return await x.Save();
        }

        public async Task<bool> Update()
        {
            var x = await CrudX.First(null);
            if (x != null)
            {
                x.Name = _rnd.Next(1000).ToString();
                x.EnableDelEvent = !x.EnableDelEvent;
                return await x.Save();
            }
            return false;
        }

        public async Task<bool> Delete()
        {
            var x = await CrudX.First(null);
            if (x != null)
            {
                return await x.Delete();
            }
            return false;
        }

        public async Task<bool> BatchInsert()
        {
            var tbl = new Table<CrudX>();
            for (int i = 0; i < 3; i++)
            {
                tbl.Add(await CrudX.New("批量" + _rnd.Next(1000)));
            }
            return await tbl.Save();
        }

        public async Task<bool> Batch()
        {
            var tbl = await CrudX.Page(0, 2, null);
            // 更
            if (tbl.Count > 0)
                tbl[0].Name = "批增更" + _rnd.Next(1000);
            // 增
            tbl.Add(await CrudX.New("批增更" + _rnd.Next(1000)));
            return await tbl.Save();
        }

        public async Task<bool> SaveTable()
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
            return await tbl.Save();
        }

        public async Task<bool> BatchDel()
        {
            var tbl = await CrudX.Page(0, 2, null);
            return await tbl.Delete();

            //var ls = new List<CrudX>();
            //ls.AddRange(tbl);
            //await ls.Delete();
        }

        public async Task<bool> DirectDel()
        {
            var x = await CrudX.First(null);
            if (x != null)
            {
                return await CrudX.DelByID(x.ID);
            }
            return false;
        }

        public async Task<bool> DelByID()
        {
            var x = await CrudX.First(null);
            if (x != null)
            {
                x.EnableDelEvent = true;
                // 先保存
                await x.Save(false);
                return await CrudX.DelByID(x.ID, false);
            }
            return false;
        }
        #endregion

        #region 领域事件
        public async Task<bool> InsertEvent()
        {
            var x = await CrudX.New("新增事件" + _rnd.Next(10000).ToString());
            x.EnableInsertEvent = true;
            return await x.Save();
        }

        public async Task<bool> UpdateEvent()
        {
            var x = await CrudX.First(null);
            if (x != null)
            {
                x.EnableNameChangedEvent = true;
                x.Name = "Name变化事件" + _rnd.Next(1000).ToString();
                return await x.Save();
            }
            return false;
        }

        public async Task<bool> DelEvent()
        {
            var x = await CrudX.First(null);
            if (x != null)
            {
                x.EnableDelEvent = true;
                return await x.Delete();
            }
            return false;
        }
        #endregion

        #region 虚拟实体
        public async Task<bool> InsertVir()
        {
            var x = await VirX<Virtbl1X, Virtbl2X, Virtbl3X>.New();
            x.E1.ID = await Virtbl1X.NewID();
            x.E1.Name1 = "新1";
            x.E2.Name2 = "新2";
            x.E3.Name3 = "新3";
            return await x.Save();
        }

        public async Task<bool> UpdateVir()
        {
            var x = await VirX<Virtbl1X, Virtbl2X, Virtbl3X>.First(null);
            if (x != null)
            {
                var name = "更" + _rnd.Next(1000);
                x.E1.Name1 = name;
                x.E2.Name2 = name;
                x.E3.Name3 = name;
                return await x.Save();
            }
            return false;
        }

        public async Task<bool> DelVir()
        {
            var x = await VirX<Virtbl1X, Virtbl2X, Virtbl3X>.First(null);
            if (x != null)
                return await x.Delete();
            return false;
        }

        public async Task<bool> SaveVir()
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
            x.E1.ID = await Virtbl1X.NewID();
            x.E1.Name1 = "批增1";
            x.E2.Name2 = "批增2";
            x.E3.Name3 = "批增3";
            tbl.Add(x);
            return await tbl.Save();
        }

        public async Task<bool> DirectDelVir()
        {
            var x = await VirX<Virtbl1X, Virtbl2X, Virtbl3X>.First(null);
            if (x != null)
                return await VirX<Virtbl1X, Virtbl2X, Virtbl3X>.DelByID(x.ID);
            return false;
        }

        public async Task<bool> DelByIDVir()
        {
            var x = await VirX<Virtbl1X, Virtbl2X, Virtbl3X>.First(null);
            if (x != null)
                return await VirX<Virtbl1X, Virtbl2X, Virtbl3X>.DelByID(x.ID, false);
            return false;
        }
        #endregion

        #region 父子实体
        public async Task<bool> InsertWithChild()
        {
            var x = await ParTblX.New("新增");
            for (int i = 0; i < 2; i++)
            {
                x.Tbl1.Add(await ChildTbl1X.New(x.ID, "新增" + i));
                x.Tbl2.Add(await ChildTbl2X.New(x.ID, "新增" + i));
            }
            return await x.SaveWithChild();
        }

        public async Task<bool> UpdateWithChild()
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
                return await x.SaveWithChild();
            }
            return false;
        }

        public async Task<string> QueryWithChild()
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
            return msg;
        }

        #endregion

        #region 缓存
        public async Task<bool> InsertCache()
        {
            var x = await CacheTbl1X.New(Kit.NewGuid, _rnd.Next(10000).ToString());
            return await x.Save();
        }

        public async Task<bool> UpdateCache()
        {
            var x = await CacheTbl1X.First(null);
            if (x != null)
            {
                x.Name = _rnd.Next(1000).ToString();
                return await x.Save();
            }
            return false;
        }

        public async Task<bool> DelCache()
        {
            var x = await CacheTbl1X.First(null);
            if (x != null)
            {
                return await x.Delete();
            }
            return false;
        }

        public async Task<string> CacheByKey()
        {
            var x = await CacheTbl1X.First(null);
            if (x != null)
            {
                var model = await EntitySchema.Get(typeof(CacheTbl1X));
                var key = $"{model.Schema.Name}:phone:{x.Phone}";
                await Kit.DeleteCache(key);

                await CacheTbl1X.GetFromCacheFirst("phone", x.Phone);
                var val = await Kit.StringGet<string>(key);
                return $"缓存键：{key}\r\n缓存值：{val}";
            }
            return "无数据";
        }
        #endregion
    }
}