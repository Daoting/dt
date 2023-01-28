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
        public async void OnCommited()
        {
            foreach (var en in Data)
            {
                // 发布领域事件
                var ls = en.GetEvents();
                if (ls != null && ls.Count > 0)
                {
                    ls.ForEach(Kit.PublishEvent);
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
