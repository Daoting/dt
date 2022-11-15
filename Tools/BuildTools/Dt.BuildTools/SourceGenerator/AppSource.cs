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
    class AppSource
    {
        StubGenerator _gen;
        List<INamedTypeSymbol> _dicts;

        internal void Generate(StubGenerator gen)
        {
            _gen = gen;
            try
            {
                ExtractDictionary();
                _gen.Context.AddSource("AutoGenerateApp", BuildSource());
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                var desc = new DiagnosticDescriptor(
                    "Dt0002",
                    "生成 AutoGenerateApp 失败",
                    "{0}",
                    "Dictionary",
                    DiagnosticSeverity.Error,
                    true,
                    "");

                var diagnostic = Diagnostic.Create(
                    desc,
                    null,
                    e.Message + "生成 AutoGenerateApp 失败！");

                _gen.Context.ReportDiagnostic(diagnostic);
            }
        }

        void ExtractDictionary()
        {
            // 过滤引用 Dt.Core 的程序集
            var asms = (from ext in _gen.Context.Compilation.ExternalReferences
                        let sym = _gen.Context.Compilation.GetAssemblyOrModuleSymbol(ext) as IAssemblySymbol
                        where sym != null
                        from module in sym.Modules
                        where module.ReferencedAssemblies.Any(r => r.Name == "Dt.Core")
                        select sym).ToList();

            // 按照依赖关系重新排序
            var ordered = new List<IAssemblySymbol>();
            while (asms.Count > 0)
            {
                // 取出第一个和其它比较，若有依赖其它则将其放在最后，无依赖放入ordered
                var first = asms[0];
                bool existParent = false;
                foreach (var module in first.Modules)
                {
                    for (int i = 1; i < asms.Count; i++)
                    {
                        var compare = asms[i];
                        if (module.ReferencedAssemblies.Any(r => r.Name == compare.Name))
                        {
                            existParent = true;
                            break;
                        }
                    }
                }

                asms.RemoveAt(0);
                if (existParent)
                {
                    asms.Add(first);
                }
                else
                {
                    ordered.Add(first);
                }
            }

            _dicts = new List<INamedTypeSymbol>();
            foreach (var sym in ordered)
            {
                bool find = false;
                foreach (var tp in sym.GlobalNamespace.GetNamespaceTypes())
                {
                    if (tp.Name == "DtDictionaryResource")
                    {
                        find = true;
                        _dicts.Add(tp);
                        break;
                    }
                }

                if (!find)
                    throw new Exception(sym.Name + " 缺少自动生成类 DtDictionaryResource 的定义！");
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
            sb.AppendLine();

            using (sb.Block("namespace {0}", _gen.App.ContainingNamespace))
            using (sb.Block("public partial class {0}", _gen.App.Name))
            using (sb.Block("public void MergeDictionaryResource()"))
            {
                foreach (var dict in _dicts)
                {
                    sb.AppendLine($"new {dict.ContainingNamespace}.{dict.Name}().Merge();");
                }

                // App所在项目的 DtDictionaryResource
                sb.AppendLine($"new {_gen.Context.GetMSBuildPropertyValue("RootNamespace")}.DtDictionaryResource().Merge();");
            }

            return sb.ToString();
        }
    }
}