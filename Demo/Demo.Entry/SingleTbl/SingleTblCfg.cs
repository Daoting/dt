#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2025-02-26 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Reflection;
using System.Text.Json.Serialization;
#endregion

namespace Dt.Base
{
    public class SingleTblCfg
    {
        public string EntityCls { get; set; }

        public SingleTblQueryCfg QueryCfg { get; set; }

        public SingleTblListCfg ListCfg { get; set; }

        public SingleTblFormCfg FormCfg { get; set; }

        [JsonIgnore]
        public TableSchema Table => _model.Schema;

        [JsonIgnore]
        public Type EntityType => _entityType;

        public async Task Init()
        {
            _entityType = Type.GetType(EntityCls);
            _model = await EntitySchema.Get(_entityType);
            // EntityX<TEntity> 获取静态方法
            _genericType = typeof(EntityX<>).MakeGenericType(_entityType);
        }

        public async Task<Table> Query(string p_whereOrSqlOrSp, object p_params = null)
        {
            var fun = _genericType.GetMethod("Query", BindingFlags.Public | BindingFlags.Static);
            var task = (Task)fun.Invoke(null, new object[] { p_whereOrSqlOrSp, p_params });
            await task;
            return (Table)task.GetType().GetProperty("Result").GetValue(task);
        }

        public async Task<object> New()
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
                    return task.GetType().GetProperty("Result").GetValue(task);
                }
                return fun.Invoke(null, tgtParams);
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
                return Activator.CreateInstance(_entityType, tgtParams);
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
        
        EntitySchema _model;
        Type _entityType;
        // EntityX<TEntity> 获取静态方法
        Type _genericType;
    }

    public class SingleTblQueryCfg
    {
        public bool AutoXaml { get; set; } = true;

        public string Xaml { get; set; }
    }

    public class SingleTblListCfg
    {
        public bool AutoXaml { get; set; } = true;

        public string Xaml { get; set; }


        public bool ShowMenu { get; set; } = true;

        public bool ShowAddMi { get; set; } = true;

        public bool ShowDelMi { get; set; } = true;
    }

    public class SingleTblFormCfg
    {
        public bool AutoXaml { get; set; } = true;

        public string Xaml { get; set; }

        public bool ShowMenu { get; set; } = true;

        public bool ShowAddMi { get; set; } = true;

        public bool ShowDelMi { get; set; } = true;
    }
}