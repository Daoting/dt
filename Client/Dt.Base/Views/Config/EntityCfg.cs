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
#endregion

namespace Dt.Base
{
    public class EntityCfg
    {
        #region 变量
        string _cls;
        EntitySchema _model;
        Type _entityType;
        // EntityX<TEntity> 获取静态方法
        Type _genericType;
        #endregion
        
        public EntityCfg()
        {
            ListCfg = new EntityListCfg { Owner = this };
            FormCfg = new EntityFormCfg { Owner = this };
        }
        
        /// <summary>
        /// 实体类全名，包括程序集名称
        /// </summary>
        public string Cls
        {
            get => _cls;
            set
            {
                if (_cls != value)
                {
                    _cls = value;
                    if (!string.IsNullOrEmpty(_cls))
                    {
                        _entityType = Type.GetType(_cls);
                        var task = EntitySchema.Get(_entityType);
                        task.Wait();
                        _model = task.Result;
                        // EntityX<TEntity> 获取静态方法
                        _genericType = typeof(EntityX<>).MakeGenericType(_entityType);
                    }
                    else
                    {
                        _entityType = null;
                        _model = null;
                        _genericType = null;
                    }
                }
            }
        }

        /// <summary>
        /// 查询面板的xaml
        /// </summary>
        public string QueryFvXaml { get; set; }

        /// <summary>
        /// 列表配置
        /// </summary>
        public EntityListCfg ListCfg { get; set; }

        /// <summary>
        /// 表单配置
        /// </summary>
        public EntityFormCfg FormCfg { get; set; }

        /// <summary>
        /// 一对多时子实体的父表主键字段名
        /// </summary>
        public string ParentID { get; set; }

        /// <summary>
        /// 是否一对多的子表
        /// </summary>
        public bool IsChild { get; set; }

        /// <summary>
        /// 实体对应的表模型
        /// </summary>
        public TableSchema Table => _model?.Schema;

        /// <summary>
        /// 实体类型
        /// </summary>
        public Type EntityType => _entityType;

        /// <summary>
        /// 调用EntityX`TEntity`的Query方法
        /// </summary>
        /// <param name="p_whereOrSqlOrSp"></param>
        /// <param name="p_params"></param>
        /// <returns></returns>
        public async Task<Table> Query(string p_whereOrSqlOrSp, object p_params = null)
        {
            var fun = _genericType.GetMethod("Query", BindingFlags.Public | BindingFlags.Static);
            var task = (Task)fun.Invoke(null, new object[] { p_whereOrSqlOrSp, p_params });
            await task;
            return (Table)task.GetType().GetProperty("Result").GetValue(task);
        }

        /// <summary>
        /// 创建实体对象，优先调用静态方法New()，若无则调用构造函数
        /// </summary>
        /// <returns></returns>
        public async Task<Entity> New()
        {
            object[] tgtParams = null;

            // 调用静态方法New()
            var fun = _entityType.GetMethod("New", BindingFlags.Public | BindingFlags.Static);
            if (fun != null)
            {
                var pars = fun.GetParameters();
                if (pars.Length > 0)
                {
                    tgtParams = new object[pars.Length];
                    for (int i = 0; i < pars.Length; i++)
                    {
                        var par = pars[i];
                        if (par.HasDefaultValue)
                        {
                            tgtParams[i] = par.DefaultValue;
                        }
                        else if (par.ParameterType.IsValueType)
                        {
                            tgtParams[i] = Activator.CreateInstance(par.ParameterType);
                        }
                        else
                        {
                            tgtParams[i] = null;
                        }
                    }
                }

                if (typeof(Task).IsAssignableFrom(fun.ReturnType))
                {
                    var task = (Task)fun.Invoke(null, tgtParams);
                    await task;
                    return task.GetType().GetProperty("Result").GetValue(task) as Entity;
                }
                return fun.Invoke(null, tgtParams) as Entity;
            }

            // 无静态方法，调用构造函数
            var cos = _entityType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            foreach (var co in cos)
            {
                var pars = co.GetParameters();
                if (pars.Length > 1)
                {
                    tgtParams = new object[pars.Length];
                    for (int i = 0; i < pars.Length; i++)
                    {
                        var par = pars[i];
                        if (par.HasDefaultValue)
                        {
                            tgtParams[i] = par.DefaultValue;
                        }
                        else if (par.ParameterType == typeof(long)
                            && par.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
                        {
                            var idFun = _genericType.GetMethod("NewID", BindingFlags.Public | BindingFlags.Static);
                            var task = (Task<long>)idFun.Invoke(null, null);
                            tgtParams[i] = await task;
                        }
                        else if (par.ParameterType.IsValueType)
                        {
                            tgtParams[i] = Activator.CreateInstance(par.ParameterType);
                        }
                        else
                        {
                            tgtParams[i] = null;
                        }
                    }
                    break;
                }
            }

            if (tgtParams != null)
                return Activator.CreateInstance(_entityType, tgtParams) as Entity;
            return null;
        }

        /// <summary>
        /// 根据主键获得实体对象(包含所有列值)，仅支持单主键
        /// </summary>
        /// <param name="p_id">主键值</param>
        /// <returns>返回实体对象或null</returns>
        public async Task<object> GetByID(object p_id)
        {
            var fun = _genericType.GetMethod("GetByID", BindingFlags.Public | BindingFlags.Static);
            var task = (Task)fun.Invoke(null, new object[] { p_id });
            await task;
            return task.GetType().GetProperty("Result").GetValue(task);
        }

        /// <summary>
        /// 序列化为json字符串
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            using (var stream = new MemoryStream())
            using (var writer = new Utf8JsonWriter(stream, JsonOptions.UnsafeWriter))
            {
                writer.WriteStartObject();
                DoSerialize(writer);
                writer.WriteEndObject();
                writer.Flush();
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        /// <summary>
        /// 序列化为json
        /// </summary>
        /// <param name="writer"></param>
        public void DoSerialize(Utf8JsonWriter writer)
        {
            if (!string.IsNullOrEmpty(Cls))
                writer.WriteString("Cls", Cls);
            if (!string.IsNullOrEmpty(QueryFvXaml))
                writer.WriteString("QueryFvXaml", QueryFvXaml);
            if (!string.IsNullOrEmpty(ParentID))
                writer.WriteString("ParentID", ParentID);
            if (IsChild)
                writer.WriteBoolean("IsChild", true);

            if (ListCfg.IsChanged)
            {
                writer.WriteStartObject("ListCfg");
                if (!string.IsNullOrEmpty(ListCfg.Xaml))
                    writer.WriteString("Xaml", ListCfg.Xaml);
                if (!ListCfg.ShowAddMi)
                    writer.WriteBoolean("ShowAddMi", ListCfg.ShowAddMi);
                if (!ListCfg.ShowDelMi)
                    writer.WriteBoolean("ShowDelMi", ListCfg.ShowDelMi);
                if (!ListCfg.ShowMultiSelMi)
                    writer.WriteBoolean("ShowMultiSelMi", ListCfg.ShowMultiSelMi);
                if (!ListCfg.IsCustomTitle)
                    writer.WriteString("Title", ListCfg.Title);
                writer.WriteEndObject();
            }

            if (FormCfg.IsChanged)
            {
                writer.WriteStartObject("FormCfg");
                if (!string.IsNullOrEmpty(FormCfg.Xaml))
                    writer.WriteString("Xaml", FormCfg.Xaml);
                if (!FormCfg.ShowAddMi)
                    writer.WriteBoolean("ShowAddMi", FormCfg.ShowAddMi);
                if (!FormCfg.ShowDelMi)
                    writer.WriteBoolean("ShowDelMi", FormCfg.ShowDelMi);
                if (!FormCfg.ShowSaveMi)
                    writer.WriteBoolean("ShowSaveMi", FormCfg.ShowSaveMi);
                if (!FormCfg.IsCustomTitle)
                    writer.WriteString("Title", ListCfg.Title);
                writer.WriteEndObject();
            }
        }

        /// <summary>
        /// 反序列化json字符串为对象
        /// </summary>
        /// <param name="p_json"></param>
        /// <returns></returns>
        public static EntityCfg Deserialize(string p_json)
        {
            if (string.IsNullOrEmpty(p_json))
                return new EntityCfg();

            var cfg = JsonSerializer.Deserialize<EntityCfg>(p_json);
            if (cfg.ListCfg != null)
                cfg.ListCfg.Owner = cfg;
            if (cfg.FormCfg != null)
                cfg.FormCfg.Owner = cfg;
            return cfg;
        }
    }
}