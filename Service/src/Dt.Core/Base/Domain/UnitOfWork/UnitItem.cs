#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-10 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Reflection;
#endregion

namespace Dt.Core
{
    class UnitItem
    {
        static MethodInfo _commited = typeof(UnitItem).GetMethod("Commited", BindingFlags.Instance | BindingFlags.NonPublic);

        public UnitItem(EntitySchema p_schema, IList<Entity> p_data, List<Dict> p_exec)
        {
            Schema = p_schema;
            Data = p_data;
            Exec = p_exec;
        }

        public EntitySchema Schema { get; }

        public IList<Entity> Data { get; }

        public List<Dict> Exec { get; }

        public bool IsDelete { get; set; }

        /// <summary>
        /// 提交成功后的处理，对于每个实体：
        /// <para>1. 若存在领域事件，则发布事件</para>
        /// <para>2. 若已设置服务端缓存，则删除缓存</para>
        /// <para>3. 对于新增、修改的实体进行状态复位</para>
        /// </summary>
        public void OnCommited()
        {
            // 动态构造泛型方法，确保事件参数类型准确，不然发布事件时无法类型转换！
            _commited.MakeGenericMethod(Schema.EntityType).Invoke(this, new object[0]);
        }

        async Task Commited<TEntity>()
            where TEntity : Entity
        {
            var localEB = Kit.GetService<EventBus.LocalEventBus>();
            foreach (var en in Data)
            {
                // 发布领域事件
                if (Schema.CudEvent != CudEvent.None)
                {
                    // 触发增删改领域事件
                    if (IsDelete)
                    {
                        if ((Schema.CudEvent & CudEvent.Delete) == CudEvent.Delete)
                            localEB.Publish(new DeleteEvent<TEntity> { Entity = (TEntity)en });
                    }
                    else
                    {
                        if (en.IsAdded)
                        {
                            if ((Schema.CudEvent & CudEvent.Insert) == CudEvent.Insert)
                                localEB.Publish(new InsertEvent<TEntity> { Entity = (TEntity)en });
                        }
                        else if (en.IsChanged)
                        {
                            if ((Schema.CudEvent & CudEvent.Update) == CudEvent.Update)
                                localEB.Publish(new UpdateEvent<TEntity> { Entity = (TEntity)en });
                        }
                    }
                }

                // 触发自定义领域事件
                var ls = en.GetEvents();
                if (ls != null && ls.Count > 0)
                {
                    ls.ForEach(localEB.Publish);
                    // 发布完毕，清空领域事件
                    en.ClearEvents();
                }

                // 删除服务端缓存
                if (Schema.CacheHandler != null)
                {
                    await Schema.CacheHandler.Remove(en);
                }

                // 状态复位
                if (!IsDelete)
                    en.AcceptChanges();
            }
        }
    }
}
