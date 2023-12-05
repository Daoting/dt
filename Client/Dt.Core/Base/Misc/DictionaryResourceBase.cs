#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-11-14 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 由 Dt.BuildTools 在编译前自动生成子类
    /// </summary>
    public class DictionaryResourceBase
    {
        public void Merge(Dictionary<string, SqliteTblsInfo> p_sqliteDbs, Dictionary<string, Type> p_aliasTypes, Dictionary<string, List<Type>> p_aliasTypeList)
        {
            MergeSqliteDbs(p_sqliteDbs);
            MergeTypeAlias(p_aliasTypes);
            MergeTypeListAlias(p_aliasTypeList);
        }

        // 以下方法内容由 Dt.BuildTools 在编译前自动生成

        /// <summary>
        /// 合并本地库的结构信息，键为小写的库文件名(不含扩展名)，值为该库信息，包括版本号和表结构的映射类型
        /// 先调用base.MergeSqliteDbs，不可覆盖上级的同名本地库
        /// </summary>
        /// <param name="p_dict"></param>
        protected virtual void MergeSqliteDbs(Dictionary<string, SqliteTblsInfo> p_dict) { }

        /// <summary>
        /// 合并类型别名字典，程序集中所有贴有 TypeAliasAttribute 子标签的类型都会收集到字典，如视图类型、工作流表单类型等
        /// 先调用 base.MergeTypeAlias，后添加的别名相同的项放在列表的头部
        /// 键规则：标签类名去掉尾部的Attribute-别名，如：View-主页
        /// </summary>
        /// <param name="p_dict"></param>
        protected virtual void MergeTypeAlias(Dictionary<string, Type> p_dict) { }

        /// <summary>
        /// 合并类型别名字典，程序集中所有贴有 TypeListAliasAttribute 子标签的类型都会收集到字典
        /// 键规则和以上相同
        /// </summary>
        /// <param name="p_dict"></param>
        protected virtual void MergeTypeListAlias(Dictionary<string, List<Type>> p_dict) { }

        /// <summary>
        /// 合并新类型，若存在相同别名的列表则插入头部，不存在则创建新列表
        /// </summary>
        /// <param name="p_key"></param>
        /// <param name="p_type"></param>
        /// <param name="p_dict"></param>
        protected void DoMergeTypeAlias(string p_key, Type p_type, Dictionary<string, List<Type>> p_dict)
        {
            List<Type> ls;
            if (!p_dict.TryGetValue(p_key, out ls))
            {
                ls = new List<Type>();
                ls.Add(p_type);
                p_dict[p_key] = ls;
            }
            else
            {
                // 插入头部
                ls.Insert(0, p_type);
            }
        }
    }
}