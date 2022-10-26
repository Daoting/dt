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
            CancellationToken _cancellationToken;
            INamedTypeSymbol _tpSqliteAttr;
            INamedTypeSymbol _tpAliasAttr;
            INamedTypeSymbol _tpAttribute;
            INamedTypeSymbol _tpIgnore;
            INamedTypeSymbol _baseStub;
            INamedTypeSymbol _stub;
            Dictionary<string, SqliteDbTbls> _sqliteTypes;
            Dictionary<string, string> _aliasTypes;
            string _defaultNamespace;

            internal void Generate(GeneratorExecutionContext context)
            {
                Debugger.Launch();
                try
                {
                    // 项目csproj文件无 <UseStub>true</UseStub> 不处理
                    //if (!Kit.IsUseStub(context) || Kit.IsDesignTime(context))
                    //    return;

                    _tpSqliteAttr = context.Compilation.GetTypeByMetadataName("Dt.Core.SqliteAttribute");
                    _tpAliasAttr = context.Compilation.GetTypeByMetadataName("Dt.Core.TypeAliasAttribute");
                    _baseStub = context.Compilation.GetTypeByMetadataName("Dt.Core.Stub");
                    _tpIgnore = context.Compilation.GetTypeByMetadataName("Dt.Core.IgnoreAttribute");
                    _tpAttribute = context.Compilation.GetTypeByMetadataName("System.Attribute");

                    if (_tpSqliteAttr == null
                        || _tpAliasAttr == null
                        || _baseStub == null
                        || _tpIgnore == null)
                        return;

                    _cancellationToken = context.CancellationToken;

                    // 从当前项目过滤所有类型
                    var types = from type in context.Compilation.SourceModule.GlobalNamespace.GetNamespaceTypes()
                                where !type.IsAbstract
                                    && !type.IsGenericType
                                    && !type.IsStatic
                                    && type.DeclaredAccessibility == Accessibility.Public
                                select type;

                    _sqliteTypes = new Dictionary<string, SqliteDbTbls>();
                    _aliasTypes = new Dictionary<string, string>();

                    foreach (var type in types)
                    {
                        _cancellationToken.ThrowIfCancellationRequested();

                        if (IsStub(type))
                        {
                            _stub = type;
                        }
                        else
                        {
                            var attrs = type.GetAttributes();
                            if (attrs.Length == 0)
                                continue;

                            var attr = attrs[0];
                            if (SymbolEqualityComparer.Default.Equals(attr.AttributeClass, _tpSqliteAttr))
                            {
                                ExtractSqliteDb(type, attr);
                            }
                            else if (IsAliasAttr(attr))
                            {
                                var val = attr.ConstructorArguments.FirstOrDefault().Value;
                                var name = attr.AttributeClass.Name;
                                _aliasTypes[$"{name.Substring(0, name.Length - 9)}-{val}"] = type.ToString();
                            }
                        }
                    }

                    // 无存根，不生成代码
                    if (_stub != null)
                    {
                        _defaultNamespace = context.GetMSBuildPropertyValue("RootNamespace");
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
                        "搬运工自动生成字典代码失败",
                        "{0}",
                        "Dictionary",
                        DiagnosticSeverity.Error,
                        true,
                        "");
                    var diagnostic = Diagnostic.Create(
                        desc,
                        null,
                        $"自动生成字典代码失败！({e.Message})");

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

            string BuildSource()
            {
                var sb = new IndentedStringBuilder();

                sb.AppendLine("// auto-generated");
                sb.AppendLine();
                sb.AppendLine("#pragma warning disable 618  // Ignore obsolete members warnings");
                sb.AppendLine("#pragma warning disable 1591 // Ignore missing XML comment warnings");
                sb.AppendLine("using System;");
                sb.AppendLine("using System.Linq;");
                sb.AppendLine("using Dt.Core;");

                using (sb.Block("namespace {0}", _defaultNamespace))
                using (sb.Block("public partial class {0}", _stub.Name))
                {
                    BuildSqliteDbs(sb);
                    BuildTypeAlias(sb);
                }

                return sb.ToString();
            }

            void BuildSqliteDbs(IndentedStringBuilder sb)
            {
                using (sb.Block("protected override void MergeSqliteDbs(Dictionary<string, SqliteTblsInfo> p_dict)"))
                {
                    sb.AppendLine("base.MergeSqliteDbs(p_dict);");
                    if (_sqliteTypes.Count > 0)
                    {
                        foreach (var item in _sqliteTypes)
                        {
                            _cancellationToken.ThrowIfCancellationRequested();

                            using (sb.Block("p_dict[\"{0}\"] = new SqliteTblsInfo", item.Key))
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
                            sb.AppendLine(";");
                            sb.AppendLine();
                        }
                    }
                }
            }

            void BuildTypeAlias(IndentedStringBuilder sb)
            {

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