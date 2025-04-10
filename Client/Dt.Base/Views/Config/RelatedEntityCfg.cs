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
    /// <summary>
    /// 多对多关联实体配置
    /// </summary>
    public class RelatedEntityCfg
    {
        #region 变量
        string _relatedCls;
        EntitySchema _relatedModel;
        Type _relatedType;
        // EntityX<TEntity> 获取静态方法
        Type _genericRelatedType;

        string _middleCls;
        EntitySchema _middleModel;
        Type _middleType;
        Type _genericMiddleType;
        string _tabTitle;
        string _selectDlgTitle;
        #endregion

        /// <summary>
        /// 关联实体类全名，包括程序集名称
        /// </summary>
        public string RelatedCls
        {
            get => _relatedCls;
            set
            {
                if (_relatedCls != value)
                {
                    _relatedCls = value;
                    if (!string.IsNullOrEmpty(_relatedCls))
                    {
                        _relatedType = Type.GetType(_relatedCls);
                        var task = EntitySchema.Get(_relatedType);
                        task.Wait();
                        _relatedModel = task.Result;
                        // EntityX<TEntity> 获取静态方法
                        _genericRelatedType = typeof(EntityX<>).MakeGenericType(_relatedType);
                    }
                    else
                    {
                        _relatedType = null;
                        _relatedModel = null;
                        _genericRelatedType = null;
                    }
                }
            }
        }

        /// <summary>
        /// 中间实体类全名，包括程序集名称
        /// </summary>
        public string MiddleCls
        {
            get => _middleCls;
            set
            {
                if (_middleCls != value)
                {
                    _middleCls = value;
                    if (!string.IsNullOrEmpty(_middleCls))
                    {
                        _middleType = Type.GetType(_middleCls);
                        var task = EntitySchema.Get(_middleType);
                        task.Wait();
                        _middleModel = task.Result;
                        // EntityX<TEntity> 获取静态方法
                        _genericMiddleType = typeof(EntityX<>).MakeGenericType(_middleType);
                    }
                    else
                    {
                        _relatedType = null;
                        _relatedModel = null;
                        _genericRelatedType = null;
                    }
                }
            }
        }

        /// <summary>
        /// 中间表对主表的外键字段名
        /// </summary>
        public string MainFk { get; set; }

        /// <summary>
        /// 中间表对关联表的外键字段名
        /// </summary>
        public string RelatedFk { get; set; }

        /// <summary>
        /// Tab标题
        /// </summary>
        public string TabTitle
        {
            get
            {
                if (!string.IsNullOrEmpty(_tabTitle))
                    return _tabTitle;
                return GetTabTitle();
            }
            set
            {
                var def = GetTabTitle();
                if (value == def)
                {
                    _tabTitle = null;
                }
                else if (_tabTitle != value)
                {
                    _tabTitle = value;
                }
            }
        }

        /// <summary>
        /// 选择对话框标题
        /// </summary>
        public string SelectDlgTitle
        {
            get
            {
                if (!string.IsNullOrEmpty(_selectDlgTitle))
                    return _selectDlgTitle;
                return GetSelectDlgTitle();
            }
            set
            {
                var def = GetSelectDlgTitle();
                if (value == def)
                {
                    _selectDlgTitle = null;
                }
                else if (_selectDlgTitle != value)
                {
                    _selectDlgTitle = value;
                }
            }
        }

        /// <summary>
        /// 关联或未关联列表的XAML
        /// </summary>
        public string ListXaml { get; set; }

        /// <summary>
        /// 是否显示添加菜单
        /// </summary>
        public bool ShowAddMi { get; set; } = true;

        /// <summary>
        /// 是否显示删除菜单
        /// </summary>
        public bool ShowDelMi { get; set; } = true;

        /// <summary>
        /// 是否显示多选菜单
        /// </summary>
        public bool ShowMultiSelMi { get; set; } = true;

        public TableSchema Table => _relatedModel?.Schema;

        public TableSchema MiddleTable => _middleModel?.Schema;
        
        /// <summary>
        /// 查询已关联数据
        /// </summary>
        /// <param name="p_mainID"></param>
        /// <returns></returns>
        public Task<Table> QueryRelated(long p_mainID)
        {
            var sql = $"where exists ( select {MainFk} from {_middleModel.Schema.Name} b where a.ID = b.{RelatedFk} and {MainFk}={p_mainID} )";
            return Query(sql);
        }

        /// <summary>
        /// 查询未关联数据
        /// </summary>
        /// <param name="p_mainID"></param>
        /// <returns></returns>
        public Task<Table> QueryUnrelated(long p_mainID)
        {
            var sql = $"where not exists ( select {MainFk} from {_middleModel.Schema.Name} b where a.ID = b.{RelatedFk} and {MainFk}={p_mainID} )";
            return Query(sql);
        }

        public async Task<Table> Query(string p_whereOrSqlOrSp, object p_params = null)
        {
            var fun = _genericRelatedType.GetMethod("Query", BindingFlags.Public | BindingFlags.Static);
            var task = (Task)fun.Invoke(null, new object[] { p_whereOrSqlOrSp, p_params });
            await task;
            return (Table)task.GetType().GetProperty("Result").GetValue(task);
        }

        /// <summary>
        /// 添加关联
        /// </summary>
        /// <param name="p_relatedFks"></param>
        /// <param name="p_mainFk"></param>
        /// <returns></returns>
        public Task<bool> AddRelation(List<long> p_relatedFks, long p_mainFk)
        {
            var tblType = typeof(Table<>).MakeGenericType(_middleType);
            var tbl = Activator.CreateInstance(tblType) as Table;

            var defVal = CreateParams(_middleType);
            foreach (var fk in p_relatedFks)
            {
                var en = Activator.CreateInstance(_middleType, defVal) as Row;
                en[MainFk] = p_mainFk;
                en[RelatedFk] = fk;
                tbl.Add(en);
            }
            return tbl.Save();
        }

        /// <summary>
        /// 删除关联
        /// </summary>
        /// <param name="p_relatedFks"></param>
        /// <param name="p_mainFk"></param>
        /// <returns></returns>
        public Task<bool> DelRelation(List<long> p_relatedFks, long p_mainFk)
        {
            var tblType = typeof(Table<>).MakeGenericType(_middleType);
            var tbl = Activator.CreateInstance(tblType) as Table;

            var defVal = CreateParams(_middleType);
            foreach (var fk in p_relatedFks)
            {
                var en = Activator.CreateInstance(_middleType, defVal) as Row;
                en[MainFk] = p_mainFk;
                en[RelatedFk] = fk;
                en.AcceptChanges();
                tbl.Add(en);
            }
            return tbl.Delete();
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
            if (!string.IsNullOrEmpty(_relatedCls))
                writer.WriteString("RelatedCls", _relatedCls);
            if (!string.IsNullOrEmpty(_middleCls))
                writer.WriteString("MiddleCls", _middleCls);
            if (!string.IsNullOrEmpty(MainFk))
                writer.WriteString("MainFk", MainFk);
            if (!string.IsNullOrEmpty(RelatedFk))
                writer.WriteString("RelatedFk", RelatedFk);
            if (!string.IsNullOrEmpty(_tabTitle) && _tabTitle != GetTabTitle())
                writer.WriteString("TabTitle", _tabTitle);
            if (!string.IsNullOrEmpty(_selectDlgTitle) && _selectDlgTitle != GetSelectDlgTitle())
                writer.WriteString("SelectDlgTitle", _selectDlgTitle);
            if (!string.IsNullOrEmpty(ListXaml))
                writer.WriteString("ListXaml", ListXaml);

            if (!ShowAddMi)
                writer.WriteBoolean("ShowAddMi", ShowAddMi);
            if (!ShowDelMi)
                writer.WriteBoolean("ShowDelMi", ShowDelMi);
            if (!ShowMultiSelMi)
                writer.WriteBoolean("ShowMultiSelMi", ShowMultiSelMi);
        }

        /// <summary>
        /// 调用Entity的构造方法，创建默认参数值
        /// </summary>
        /// <param name="p_entity"></param>
        /// <returns></returns>
        static object[] CreateParams(Type p_entity)
        {
            // 无静态方法，调用构造函数
            var cos = p_entity.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            object[] tgtParams = null;
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
            return tgtParams;
        }

        string GetTabTitle()
        {
            if (_relatedType != null)
                return "关联" + _relatedType.Name.TrimEnd('X');
            return "列表";
        }

        string GetSelectDlgTitle()
        {
            if (_relatedType != null)
                return "选择" + _relatedType.Name.TrimEnd('X');
            return "选择";
        }
    }
}