#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-04-26 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 通用实体访问Api，全称EntityAccess
    /// </summary>
    [Api(AgentMode = AgentMode.Generic)]
    public class Ea : BaseApi
    {
        /// <summary>
        /// 根据主键或唯一索引列获得实体对象(包含所有列值)，仅支持单主键
        /// 不存在时返回null，启用缓存时首先从缓存中获取
        /// </summary>
        /// <param name="p_keyName">主键列名</param>
        /// <param name="p_keyVal">主键值</param>
        /// <param name="p_tblName"></param>
        /// <returns>返回实体对象或null</returns>
        public async Task<Row> GetByKey(string p_keyName, string p_keyVal, string p_tblName)
        {
            Check.NotNullOrEmpty(p_keyName);
            return (Row)await InvokeRepo(p_tblName, "GetByKey", (type) => new object[] { p_keyName, p_keyVal, false });
        }

        /// <summary>
        /// 调用服务端Repo保存实体，不支持含子实体
        /// </summary>
        /// <param name="p_row">实体</param>
        /// <param name="p_tblName">表名</param>
        /// <returns>true 成功</returns>
        public async Task<bool> SaveRow(Row p_row, string p_tblName)
        {
            Check.NotNull(p_row);
            return (bool)await InvokeRepo(p_tblName, "Save", (type) => new object[] { p_row.CloneTo(type) });
        }

        /// <summary>
        /// 调用服务端Repo批量保存实体
        /// </summary>
        /// <param name="p_entities">实体列表</param>
        /// <param name="p_tblName">表名</param>
        /// <returns>true 成功</returns>
        [Transaction]
        public async Task<bool> SaveRows(Table p_entities, string p_tblName)
        {
            Check.NotNull(p_entities);
            return (bool)await InvokeRepo(p_tblName, "BatchSave", (type) => new object[] { p_entities.CloneTo(type) });
        }

        /// <summary>
        /// 调用服务端Repo删除实体
        /// </summary>
        /// <param name="p_row">实体</param>
        /// <param name="p_tblName">表名</param>
        /// <returns>删除行数</returns>
        public async Task<int> DelRow(Row p_row, string p_tblName)
        {
            Check.NotNull(p_row);
            return (int)await InvokeRepo(p_tblName, "Delete", (type) => new object[] { p_row.CloneTo(type) });
        }

        /// <summary>
        /// 调用服务端Repo批量删除实体
        /// </summary>
        /// <param name="p_entities">实体列表</param>
        /// <param name="p_tblName">表名</param>
        /// <returns>删除行数</returns>
        [Transaction]
        public async Task<int> DelRows(Table p_entities, string p_tblName)
        {
            Check.NotNull(p_entities);
            return (int)await InvokeRepo(p_tblName, "BatchDelete", (type) => new object[] { p_entities.CloneTo(type) });
        }

        /// <summary>
        /// 调用服务端Repo删除实体
        /// </summary>
        /// <param name="p_id">实体主键</param>
        /// <param name="p_tblName">表名</param>
        /// <returns>删除行数</returns>
        public async Task<int> DelRowByKey(string p_id, string p_tblName)
        {
            Check.NotNullOrEmpty(p_id);
            return (int)await InvokeRepo(p_tblName, "DelByID", (type) => new object[] { p_id });
        }

        /// <summary>
        /// 调用Repo方法
        /// </summary>
        /// <param name="p_tblName"></param>
        /// <param name="p_methodName"></param>
        /// <param name="p_params"></param>
        /// <returns></returns>
        async Task<object> InvokeRepo(string p_tblName, string p_methodName, Func<Type, object[]> p_params)
        {
            Check.NotNullOrEmpty(p_tblName);

            Type type = Silo.GetEntityType(p_tblName);
            Type repoType = typeof(Repo<>).MakeGenericType(type);

            var repo = Activator.CreateInstance(repoType);
            Task task = repoType.GetMethod(p_methodName).Invoke(repo, p_params(type)) as Task;
            await task;
            return task.GetType().GetProperty("Result").GetValue(task);
        }
    }
}
