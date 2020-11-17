#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-04-26 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Reflection;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 通用实体访问Api
    /// </summary>
    [Api(AgentMode = AgentMode.Generic)]
    public class EntityAccess : BaseApi
    {
        static MethodInfo _save = typeof(DataProvider).GetMethod("Save", BindingFlags.Instance | BindingFlags.Public);
        static MethodInfo _delete = typeof(DataProvider).GetMethod("Delete", BindingFlags.Instance | BindingFlags.Public);

        /// <summary>
        /// 调用服务端DataProvider保存实体
        /// </summary>
        /// <param name="p_row">实体</param>
        /// <param name="p_tblName">表名</param>
        /// <returns>true 成功</returns>
        public async Task<bool> Save(Row p_row, string p_tblName)
        {
            Throw.If(p_row == null || string.IsNullOrEmpty(p_tblName));

            // 调用DataProvider.Save<T>方法
            Type type = Silo.GetEntityType(p_tblName);
            Task task = _save.MakeGenericMethod(type).Invoke(_dp, new object[] { p_row.CloneTo(type) }) as Task;
            await task;
            return (bool)task.GetType().GetProperty("Result").GetValue(task);
        }

        /// <summary>
        /// 调用服务端DataProvider删除实体
        /// </summary>
        /// <param name="p_row">实体</param>
        /// <param name="p_tblName">表名</param>
        /// <returns>删除行数</returns>
        public async Task<bool> Delete(Row p_row, string p_tblName)
        {
            Throw.If(p_row == null || string.IsNullOrEmpty(p_tblName));

            // 调用DataProvider.Delete<T>方法
            Type type = Silo.GetEntityType(p_tblName);
            Task task = _delete.MakeGenericMethod(type).Invoke(_dp, new object[] { p_row.CloneTo(type) }) as Task;
            await task;
            return (bool)task.GetType().GetProperty("Result").GetValue(task);
        }
    }
}
