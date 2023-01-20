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
#endregion

namespace Dt.BuildTools
{
    [Generator]
    public class StubGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            // 设计时不处理，不再判断 <UseStub>true</UseStub>，只要引用Dt.BuildTools就处理
            if (Kit.IsDesignTime(context))
                return;

            //Debugger.Launch();
            Context = context;

            try
            {
                // 从当前项目过滤所有类型
                var types = from type in context.Compilation.SourceModule.GlobalNamespace.GetNamespaceTypes()
                            where !type.IsAbstract
                                && !type.IsGenericType
                                && type.DeclaredAccessibility == Accessibility.Public
                            select type;

                SqliteTypes = new Dictionary<string, SqliteDbTbls>();
                AliasTypes = new Dictionary<string, List<string>>();

                var baseApp = context.Compilation.GetTypeByMetadataName("Microsoft.UI.Xaml.Application");
                var tpSqliteAttr = context.Compilation.GetTypeByMetadataName("Dt.Core.SqliteAttribute");
                _tpAliasAttr = context.Compilation.GetTypeByMetadataName("Dt.Core.TypeAliasAttribute");
                _tpIgnore = context.Compilation.GetTypeByMetadataName("Dt.Core.IgnoreAttribute");
                _tpAttribute = context.Compilation.GetTypeByMetadataName("System.Attribute");
                _tpEventHandlerAttr = context.Compilation.GetTypeByMetadataName("Dt.Core.EventHandlerAttribute");

                foreach (var type in types)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();

                    if (SymbolEqualityComparer.Default.Equals(type.BaseType, baseApp))
                    {
                        // 只查直接继承 Application 的类，多层继承时uno有bug
                        App = type;
                        continue;
                    }

                    var attrs = type.GetAttributes();
                    if (attrs.Length == 0)
                        continue;

                    var attr = attrs[0];
                    if (SymbolEqualityComparer.Default.Equals(attr.AttributeClass, tpSqliteAttr))
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
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                var desc = new DiagnosticDescriptor(
                    "Dt0001",
                    "StubGenerator分析类型失败",
                    "{0}",
                    "Dictionary",
                    DiagnosticSeverity.Error,
                    true,
                    "");
                var diagnostic = Diagnostic.Create(
                    desc,
                    null,
                    e.Message + " StubGenerator分析类型时失败！");

                context.ReportDiagnostic(diagnostic);
            }

            new DictSource().Generate(this);
            if (App != null)
                new AppSource().Generate(this);
        }

        public GeneratorExecutionContext Context { get; private set; }

        public INamedTypeSymbol App { get; private set; }

        public Dictionary<string, SqliteDbTbls> SqliteTypes { get; private set; }

        public Dictionary<string, List<string>> AliasTypes { get; private set; }

        void ExtractSqliteDb(INamedTypeSymbol type, AttributeData attr)
        {
            var val = attr.ConstructorArguments.FirstOrDefault().Value;
            if (val == null)
                return;

            SqliteDbTbls tbls;
            if (!SqliteTypes.TryGetValue(val.ToString(), out tbls))
            {
                tbls = new SqliteDbTbls();
                SqliteTypes[val.ToString()] = tbls;
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
            string alias = "";
            var args = attr.ConstructorArguments;
            if (args.Length == 0 || args[0].Value == null)
            {
                // 标签的构造方法无参数

                // 标签为EventHandlerAttribute且基类是泛型接口，泛型参数为事件类型，取事件类型作为名称
                if (SymbolEqualityComparer.Default.Equals(attr.AttributeClass, _tpEventHandlerAttr)
                        && type.Interfaces.Length > 0
                        && type.Interfaces[0].IsGenericType
                        && !type.Interfaces[0].TypeArguments.IsEmpty)
                {
                    //Debugger.Launch();

                    // TypeParameters是接口定义中的泛型参数，TypeArguments是当前泛型接口的参数
                    alias = type.Interfaces[0].TypeArguments[0].Name;
                }
                else
                {
                    // 取被贴标签的类名为别名，如 ApiAttribute
                    alias = type.Name;
                }
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
            if (!AliasTypes.TryGetValue(key, out ls))
            {
                ls = new List<string>();
                ls.Add(type.ToString());
                AliasTypes[key] = ls;
            }
            else
            {
                // 插入头部
                ls.Insert(0, type.ToString());
            }
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

        public void Initialize(GeneratorInitializationContext context)
        {
        }

        INamedTypeSymbol _tpAliasAttr;
        INamedTypeSymbol _tpAttribute;
        INamedTypeSymbol _tpIgnore;
        INamedTypeSymbol _tpEventHandlerAttr;
    }

    public class SqliteDbTbls
    {
        public List<INamedTypeSymbol> Tbls { get; } = new List<INamedTypeSymbol>();

        public StringBuilder Cols { get; } = new StringBuilder();

        public string GetVer()
        {
            return Kit.GetMD5(Cols.ToString());
        }
    }
}