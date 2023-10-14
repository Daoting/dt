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
    /// <summary>
    /// 为库项目生成 Sqlite表结构字典 和 类型别名字典
    /// </summary>
    class DictSource
    {
        StubGenerator _gen;

        internal void Generate(StubGenerator gen)
        {
            _gen = gen;
            try
            {
                _gen.Context.AddSource("DtDictionaryResource", BuildSource());
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                var desc = new DiagnosticDescriptor(
                    "Dt0001",
                    "生成 DtDictionaryResource 失败",
                    "{0}",
                    "Dictionary",
                    DiagnosticSeverity.Error,
                    true,
                    "");
                var diagnostic = Diagnostic.Create(
                    desc,
                    null,
                    e.Message + " 生成 DtDictionaryResource 失败！");

                _gen.Context.ReportDiagnostic(diagnostic);
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

            using (sb.Block("namespace {0}", _gen.Context.GetMSBuildPropertyValue("RootNamespace")))
            using (sb.Block("public class DtDictionaryResource : DictionaryResourceBase"))
            {
                BuildSqliteDbs(sb);
                BuildTypeAlias(sb);
                BuildPublicTypes(sb);
            }

            return sb.ToString();
        }

        void BuildSqliteDbs(IndentedStringBuilder sb)
        {
            using (sb.Block("protected override void MergeSqliteDbs(Dictionary<string, SqliteTblsInfo> p_dict)"))
            {
                if (_gen.SqliteTypes.Count > 0)
                {
                    foreach (var item in _gen.SqliteTypes)
                    {
                        _gen.Context.CancellationToken.ThrowIfCancellationRequested();

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
            using (sb.Block("protected override void MergeTypeAlias()"))
            {
                if (_gen.AliasTypes.Count > 0)
                {
                    foreach (var item in _gen.AliasTypes)
                    {
                        _gen.Context.CancellationToken.ThrowIfCancellationRequested();
                        foreach (var tp in item.Value)
                        {
                            sb.AppendLine($"DoMergeTypeAlias(\"{item.Key}\", typeof({tp}));");
                        }
                    }
                }
            }
        }

        void BuildPublicTypes(IndentedStringBuilder sb)
        {
            using (sb.Block("protected override void MergePublicTypes(Dictionary<string, Type> p_publicTypes)"))
            {
                if (_gen.PublicTypes.Count > 0)
                {
                    foreach (var item in _gen.PublicTypes)
                    {
                        _gen.Context.CancellationToken.ThrowIfCancellationRequested();
                        sb.AppendLine($"p_publicTypes[\"{item}\"] = typeof({item});");
                    }
                }
            }
        }
    }
}