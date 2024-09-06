#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-02-01 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Demo.Base
{
    /// <summary>
    /// lob服务的数据访问
    /// </summary>
    public partial class AtSvc : AccessAgent<AtSvc.Info>
    {
        public static Task<bool> Insert()
        {
            return Kit.Rpc<bool>(
                "lob",
                "CrudDs.Insert"
            );
        }

        public static Task<bool> Update()
        {
            return Kit.Rpc<bool>(
                "lob",
                "CrudDs.Update"
            );
        }

        public static Task<bool> Delete()
        {
            return Kit.Rpc<bool>(
                "lob",
                "CrudDs.Delete"
            );
        }

        public static Task<bool> BatchInsert()
        {
            return Kit.Rpc<bool>(
                "lob",
                "CrudDs.BatchInsert"
            );
        }

        public static Task<bool> Batch()
        {
            return Kit.Rpc<bool>(
                "lob",
                "CrudDs.Batch"
            );
        }

        public static Task<bool> SaveTable()
        {
            return Kit.Rpc<bool>(
                "lob",
                "CrudDs.SaveTable"
            );
        }

        public static Task<bool> BatchDel()
        {
            return Kit.Rpc<bool>(
                "lob",
                "CrudDs.BatchDel"
            );
        }

        public static Task<bool> DirectDel()
        {
            return Kit.Rpc<bool>(
                "lob",
                "CrudDs.DirectDel"
            );
        }

        public static Task<bool> DelByID()
        {
            return Kit.Rpc<bool>(
                "lob",
                "CrudDs.DelByID"
            );
        }

        public static Task<bool> InsertEvent()
        {
            return Kit.Rpc<bool>(
                "lob",
                "CrudDs.InsertEvent"
            );
        }

        public static Task<bool> UpdateEvent()
        {
            return Kit.Rpc<bool>(
                "lob",
                "CrudDs.UpdateEvent"
            );
        }

        public static Task<bool> DelEvent()
        {
            return Kit.Rpc<bool>(
                "lob",
                "CrudDs.DelEvent"
            );
        }

        public static Task<bool> InsertVir()
        {
            return Kit.Rpc<bool>(
                "lob",
                "CrudDs.InsertVir"
            );
        }

        public static Task<bool> UpdateVir()
        {
            return Kit.Rpc<bool>(
                "lob",
                "CrudDs.UpdateVir"
            );
        }

        public static Task<bool> DelVir()
        {
            return Kit.Rpc<bool>(
                "lob",
                "CrudDs.DelVir"
            );
        }

        public static Task<bool> SaveVir()
        {
            return Kit.Rpc<bool>(
                "lob",
                "CrudDs.SaveVir"
            );
        }

        public static Task<bool> DirectDelVir()
        {
            return Kit.Rpc<bool>(
                "lob",
                "CrudDs.DirectDelVir"
            );
        }

        public static Task<bool> DelByIDVir()
        {
            return Kit.Rpc<bool>(
                "lob",
                "CrudDs.DelByIDVir"
            );
        }

        public static Task<bool> InsertWithChild()
        {
            return Kit.Rpc<bool>(
                "lob",
                "CrudDs.InsertWithChild"
            );
        }

        public static Task<bool> UpdateWithChild()
        {
            return Kit.Rpc<bool>(
                "lob",
                "CrudDs.UpdateWithChild"
            );
        }

        public static Task<string> QueryWithChild()
        {
            return Kit.Rpc<string>(
                "lob",
                "CrudDs.QueryWithChild"
            );
        }

        public static Task<bool> InsertCache()
        {
            return Kit.Rpc<bool>(
                "lob",
                "CrudDs.InsertCache"
            );
        }

        public static Task<bool> UpdateCache()
        {
            return Kit.Rpc<bool>(
                "lob",
                "CrudDs.UpdateCache"
            );
        }

        public static Task<bool> DelCache()
        {
            return Kit.Rpc<bool>(
                "lob",
                "CrudDs.DelCache"
            );
        }

        public static Task<string> CacheByID()
        {
            return Kit.Rpc<string>(
                "lob",
                "CrudDs.CacheByID"
            );
        }

        public static Task<string> CacheByKey()
        {
            return Kit.Rpc<string>(
                "lob",
                "CrudDs.CacheByKey"
            );
        }
        
        public class Info : AgentInfo
        {
            public override string Name => "lob";
        }
    }
}