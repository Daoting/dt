#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-10-25 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
#endregion

namespace Dt.BuildTools
{
    [Generator]
    public class StubGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            new Generator().Generate(context);
        }

        class Generator
        {
            GeneratorExecutionContext _context;
            INamedTypeSymbol _tpSqliteAttr;
            INamedTypeSymbol _tpAliasAttr;
            INamedTypeSymbol _tpAttribute;
            INamedTypeSymbol _tpIgnore;
            INamedTypeSymbol _baseStub;
            List<INamedTypeSymbol> _stubs;
            Dictionary<string, SqliteDbTbls> _sqliteTypes;
            Dictionary<string, List<string>> _aliasTypes;

            internal void Generate(GeneratorExecutionContext context)
            {
                try
                {
                    // 项目csproj文件无 <UseStub>true</UseStub> 不处理
                    if (!Kit.IsUseStub(context) || Kit.IsDesignTime(context))
                        return;

                    _tpSqliteAttr = context.Compilation.GetTypeByMetadataName("Dt.Core.SqliteAttribute");
                    _tpAliasAttr = context.Compilation.GetTypeByMetadataName("Dt.Core.TypeAliasAttribute");
                    _baseStub = context.Compilation.GetTypeByMetadataName("Dt.Core.Stub");
                    _tpIgnore = context.Compilation.GetTypeByMetadataName("Dt.Core.IgnoreAttribute");
                    _tpAttribute = context.Compilation.GetTypeByMetadataName("System.Attribute");

                    // 未定义以上类型不处理
                    if (_tpSqliteAttr == null
                        || _tpAliasAttr == null
                        || _baseStub == null
                        || _tpIgnore == null)
                        return;

                    //Debugger.Launch();
                    _context = context;

                    // 从当前项目过滤所有类型
                    var types = from type in context.Compilation.SourceModule.GlobalNamespace.GetNamespaceTypes()
                                where !type.IsAbstract
                                    && !type.IsGenericType
                                    && type.DeclaredAccessibility == Accessibility.Public
                                select type;

                    _sqliteTypes = new Dictionary<string, SqliteDbTbls>();
                    _aliasTypes = new Dictionary<string, List<string>>();

                    _stubs = new List<INamedTypeSymbol>();
                    foreach (var type in types)
                    {
                        context.CancellationToken.ThrowIfCancellationRequested();

                        if (IsStub(type))
                        {
                            // 支持多个存根
                            _stubs.Add(type);
                        }
                        else
                        {
                            var attrs = type.GetAttributes();
                            if (attrs.Length == 0)
                                continue;

                            var attr = attrs[0];
                            if (SymbolEqualityComparer.Default.Equals(attr.AttributeClass, _tpSqliteAttr))
                            {
                                // 有 SqliteAttribute 标签
                                ExtractSqliteDb(type, attr);
                            }
                            else if (IsAliasAttr(attr))
                            {
                                // 为继承 TypeAliasAttribute 的子标签
                                ExtractTypeAlias(type, attr);
                            }
                        }
                    }

                    // 无存根，不生成代码
                    if (_stubs.Count > 0)
                    {
                        context.AddSource("AutoGenerateStub", BuildSource());
                    }
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    var desc = new DiagnosticDescriptor(
                        "Dt0001",
                        "生成 AutoGenerateStub 失败",
                        "{0}",
                        "Dictionary",
                        DiagnosticSeverity.Error,
                        true,
                        "");
                    var diagnostic = Diagnostic.Create(
                        desc,
                        null,
                        $"生成 AutoGenerateStub 失败！({e.Message})");

                    context.ReportDiagnostic(diagnostic);
                }
            }

            void ExtractSqliteDb(INamedTypeSymbol type, AttributeData attr)
            {
                var val = attr.ConstructorArguments.FirstOrDefault().Value;
                if (val == null)
                    return;

                SqliteDbTbls tbls;
                if (!_sqliteTypes.TryGetValue(val.ToString(), out tbls))
                {
                    tbls = new SqliteDbTbls();
                    _sqliteTypes[val.ToString()] = tbls;
                }

                tbls.Tbls.Add(type);
                foreach (var mem in type.GetMembers())
                {
                    if (mem is IPropertySymbol prop
                        && prop.DeclaredAccessibility == Accessibility.Public
                        && prop.SetMethod != null)
                    {
                        var attrs = prop.GetAttributes();
                        if (attrs.Length == 0
                            || !SymbolEqualityComparer.Default.Equals(attrs[0].AttributeClass, _tpIgnore))
                        {
                            tbls.Cols.Append(prop.Name);
                        }
                    }
                }
            }

            void ExtractTypeAlias(INamedTypeSymbol type, AttributeData attr)
            {
                string alias;
                var args = attr.ConstructorArguments;
                if (args.Length == 0 || args[0].Value == null)
                {
                    // 标签的构造方法无参数，取被贴标签的类名为别名，如 ApiAttribute
                    alias = type.Name;
                }
                else
                {
                    if (args[0].Kind == TypedConstantKind.Enum)
                    {
                        // 枚举类型，取枚举成员的名称做别名
                        int index = (int)args[0].Value;
                        var mems = args[0].Type.GetMembers();
                        alias = index < mems.Length ? mems[index].Name : type.Name;
                    }
                    else
                    {
                        alias = args[0].Value.ToString();
                    }
                }

                // 键规则：类名去掉尾部的Attribute-别名
                var name = attr.AttributeClass.Name;
                var key = $"{name.Substring(0, name.Length - 9)}-{alias}";

                List<string> ls;
                if (!_aliasTypes.TryGetValue(key, out ls))
                {
                    ls = new List<string>();
                    ls.Add(type.ToString());
                    _aliasTypes[key] = ls;
                }
                else
                {
                    // 插入头部
                    ls.Insert(0, type.ToString());
                }
            }

            string BuildSource()
            {
                var sb = new IndentedStringBuilder();

                sb.AppendLine("// auto-generated 搬运工");
                sb.AppendLine("#pragma warning disable 618  // Ignore obsolete members warnings");
                sb.AppendLine("#pragma warning disable 1591 // Ignore missing XML comment warnings");
                sb.AppendLine("using System;");
                sb.AppendLine("using System.Linq;");
                sb.AppendLine("using System.Collections.Generic;");
                sb.AppendLine("using Dt.Core;");

                foreach (var stub in _stubs)
                {
                    sb.AppendLine();
                    using (sb.Block("namespace {0}", stub.ContainingNamespace))
                    using (sb.Block("public partial class {0}", stub.Name))
                    {
                        BuildSqliteDbs(sb);
                        BuildTypeAlias(sb);
                    }
                }

                return sb.ToString();
            }

            void BuildSqliteDbs(IndentedStringBuilder sb)
            {
                using (sb.Block("protected override void MergeSqliteDbs(Dictionary<string, SqliteTblsInfo> p_dict)"))
                {
                    // 先调用base.MergeSqliteDbs，不可覆盖上级的同名本地库
                    sb.AppendLine("base.MergeSqliteDbs(p_dict);");
                    if (_sqliteTypes.Count > 0)
                    {
                        foreach (var item in _sqliteTypes)
                        {
                            _context.CancellationToken.ThrowIfCancellationRequested();

                            using (sb.Block("p_dict.TryAdd(\"{0}\", new SqliteTblsInfo", item.Key))
                            {
                                sb.AppendLine($"Version = \"{item.Value.GetVer()}\",");
                                using (sb.Block("Tables = new List<Type>"))
                                {
                                    foreach (var tp in item.Value.Tbls)
                                    {
                                        sb.AppendLine($"typeof({tp}),");
                                    }
                                }
                            }
                            sb.AppendLine(");");
                        }
                    }
                }
                sb.AppendLine();
            }

            void BuildTypeAlias(IndentedStringBuilder sb)
            {
                using (sb.Block("protected override void MergeTypeAlias(Dictionary<string, List<Type>> p_dict)"))
                {
                    sb.AppendLine("base.MergeTypeAlias(p_dict);");
                    if (_aliasTypes.Count > 0)
                    {
                        foreach (var item in _aliasTypes)
                        {
                            _context.CancellationToken.ThrowIfCancellationRequested();
                            foreach (var tp in item.Value)
                            {
                                sb.AppendLine($"DoMergeTypeAlias(p_dict, \"{item.Key}\", typeof({tp}));");
                            }
                        }
                    }
                }
            }

            bool IsStub(INamedTypeSymbol p_type)
            {
                INamedTypeSymbol tp = p_type.BaseType;
                while (tp != null)
                {
                    if (SymbolEqualityComparer.Default.Equals(tp, _baseStub))
                        return true;

                    if (tp.BaseType == null || tp.BaseType.SpecialType == SpecialType.System_Object)
                        break;

                    tp = tp.BaseType;
                }
                return false;
            }

            bool IsAliasAttr(AttributeData p_attr)
            {
                INamedTypeSymbol tp = p_attr.AttributeClass.BaseType;
                while (tp != null)
                {
                    if (SymbolEqualityComparer.Default.Equals(tp, _tpAliasAttr))
                        return true;

                    if (tp.BaseType == null
                        || SymbolEqualityComparer.Default.Equals(tp.BaseType, _tpAttribute)
                        || tp.BaseType.SpecialType == SpecialType.System_Object)
                        break;

                    tp = tp.BaseType;
                }
                return false;
            }

        }

        class SqliteDbTbls
        {
            public List<INamedTypeSymbol> Tbls { get; } = new List<INamedTypeSymbol>();

            public StringBuilder Cols { get; } = new StringBuilder();

            public string GetVer()
            {
                return Kit.GetMD5(Cols.ToString());
            }
        }
    }
}