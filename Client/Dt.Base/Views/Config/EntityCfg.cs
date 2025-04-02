#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2025-02-26 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
#endregion

namespace Dt.Base
{
    public class EntityCfg
    {
        /// <summary>
        /// 实体类全名，包括程序集名称
        /// </summary>
        public string Cls { get; set; }

        /// <summary>
        /// 查询面板的xaml
        /// </summary>
        public string QueryFvXaml { get; set; }

        /// <summary>
        /// 列表配置
        /// </summary>
        public EntityListCfg ListCfg { get; set; } = new EntityListCfg();

        /// <summary>
        /// 表单配置
        /// </summary>
        public EntityFormCfg FormCfg { get; set; } = new EntityFormCfg();

        /// <summary>
        /// 实体对应的表模型
        /// </summary>
        public TableSchema Table => _model.Schema;

        /// <summary>
        /// 实体类型
        /// </summary>
        public Type EntityType => _entityType;

        internal async Task Init()
        {
            _entityType = Type.GetType(Cls);
            _model = await EntitySchema.Get(_entityType);
            // EntityX<TEntity> 获取静态方法
            _genericType = typeof(EntityX<>).MakeGenericType(_entityType);
        }

        EntitySchema _model;
        Type _entityType;
        // EntityX<TEntity> 获取静态方法
        Type _genericType;
    }
}