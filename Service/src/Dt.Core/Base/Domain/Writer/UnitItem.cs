#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-10 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections;
#endregion

namespace Dt.Core
{
    class UnitItem
    {
        public UnitItem(EntitySchema p_schema, IList p_data, List<Dict> p_exec)
        {
            Schema = p_schema;
            Data = p_data;
            Exec = p_exec;
        }

        public EntitySchema Schema { get; }

        public IList Data { get; }

        public List<Dict> Exec { get; }

        public bool IsDelete { get; set; }

        /// <summary>
        /// 提交成功后的处理，对于每个实体：
        /// <para>1. 调用保存后或删除后回调</para>
        /// <para>2. 对于新增或修改的实体进行状态复位</para>
        /// <para>3. 若存在领域事件，则发布事件</para>
        /// </summary>
        public async Task OnCommited()
        {
            foreach (var en in Data.Cast<Entity>())
            {
                if (IsDelete)
                {
                    // 删除后的回调
                    if (en.GetDeletedHook() is Func<Task> hook)
                    {
                        try
                        {
                            await hook();
                        }
                        catch { }
                    }
                }
                else
                {
                    // 保存后的回调
                    if (en.GetSavedHook() is Func<Task> hook)
                    {
                        try
                        {
                            await hook();
                        }
                        catch { }
                    }

                    // 状态复位
                    en.AcceptChanges();
                    // 触发保存后事件
                    en.OnAfterSaved();
                }

                // 发布领域事件
                var ls = en.GetEvents();
                if (ls != null && ls.Count > 0)
                {
                    foreach (var ev in ls)
                    {
                        // 不等待
                        _ = Kit.PublishEvent(ev);
                    }
                    // 发布完毕，清空领域事件
                    en.ClearEvents();
                }
            }
        }
    }
}
