#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-02-01 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.MgrDemo
{
    /// <summary>
    /// 
    /// </summary>
    public partial class AtSvc
    {
        public static Task<bool> Insert()
        {
            return Kit.Rpc<bool>(
                "demo",
                "CrudDs.Insert"
            );
        }

        public static Task<bool> Update()
        {
            return Kit.Rpc<bool>(
                "demo",
                "CrudDs.Update"
            );
        }

        public static Task<bool> Delete()
        {
            return Kit.Rpc<bool>(
                "demo",
                "CrudDs.Delete"
            );
        }

        public static Task<bool> BatchInsert()
        {
            return Kit.Rpc<bool>(
                "demo",
                "CrudDs.BatchInsert"
            );
        }

        public static Task<bool> Batch()
        {
            return Kit.Rpc<bool>(
                "demo",
                "CrudDs.Batch"
            );
        }

        public static Task<bool> SaveTable()
        {
            return Kit.Rpc<bool>(
                "demo",
                "CrudDs.SaveTable"
            );
        }

        public static Task<bool> BatchDel()
        {
            return Kit.Rpc<bool>(
                "demo",
                "CrudDs.BatchDel"
            );
        }

        public static Task<bool> DirectDel()
        {
            return Kit.Rpc<bool>(
                "demo",
                "CrudDs.DirectDel"
            );
        }

        public static Task<bool> DelByID()
        {
            return Kit.Rpc<bool>(
                "demo",
                "CrudDs.DelByID"
            );
        }

        public static Task<bool> InsertEvent()
        {
            return Kit.Rpc<bool>(
                "demo",
                "CrudDs.InsertEvent"
            );
        }

        public static Task<bool> UpdateEvent()
        {
            return Kit.Rpc<bool>(
                "demo",
                "CrudDs.UpdateEvent"
            );
        }

        public static Task<bool> DelEvent()
        {
            return Kit.Rpc<bool>(
                "demo",
                "CrudDs.DelEvent"
            );
        }

        public static Task<bool> InsertVir()
        {
            return Kit.Rpc<bool>(
                "demo",
                "CrudDs.InsertVir"
            );
        }

        public static Task<bool> UpdateVir()
        {
            return Kit.Rpc<bool>(
                "demo",
                "CrudDs.UpdateVir"
            );
        }

        public static Task<bool> DelVir()
        {
            return Kit.Rpc<bool>(
                "demo",
                "CrudDs.DelVir"
            );
        }

        public static Task<bool> SaveVir()
        {
            return Kit.Rpc<bool>(
                "demo",
                "CrudDs.SaveVir"
            );
        }

        public static Task<bool> DirectDelVir()
        {
            return Kit.Rpc<bool>(
                "demo",
                "CrudDs.DirectDelVir"
            );
        }

        public static Task<bool> DelByIDVir()
        {
            return Kit.Rpc<bool>(
                "demo",
                "CrudDs.DelByIDVir"
            );
        }

        public static Task<bool> InsertWithChild()
        {
            return Kit.Rpc<bool>(
                "demo",
                "CrudDs.InsertWithChild"
            );
        }

        public static Task<bool> UpdateWithChild()
        {
            return Kit.Rpc<bool>(
                "demo",
                "CrudDs.UpdateWithChild"
            );
        }

        public static Task<string> QueryWithChild()
        {
            return Kit.Rpc<string>(
                "demo",
                "CrudDs.QueryWithChild"
            );
        }

        public static Task<bool> InsertCache()
        {
            return Kit.Rpc<bool>(
                "demo",
                "CrudDs.InsertCache"
            );
        }

        public static Task<bool> UpdateCache()
        {
            return Kit.Rpc<bool>(
                "demo",
                "CrudDs.UpdateCache"
            );
        }

        public static Task<bool> DelCache()
        {
            return Kit.Rpc<bool>(
                "demo",
                "CrudDs.DelCache"
            );
        }

        public static Task<string> CacheByID()
        {
            return Kit.Rpc<string>(
                "demo",
                "CrudDs.CacheByID"
            );
        }

        public static Task<string> CacheByKey()
        {
            return Kit.Rpc<string>(
                "demo",
                "CrudDs.CacheByKey"
            );
        }
    }
}