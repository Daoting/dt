#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-10-25 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.CodeAnalysis;
#endregion

namespace Dt.BuildTools
{
	internal static class Kit
	{
        public static string GetMSBuildPropertyValue(
            this GeneratorExecutionContext context,
            string name,
            string defaultValue = "")
        {
            context.AnalyzerConfigOptions.GlobalOptions.TryGetValue($"build_property.{name}", out var value);
            return value ?? defaultValue;
        }

        public static IEnumerable<INamedTypeSymbol> GetNamespaceTypes(this INamespaceSymbol sym)
        {
            foreach (var child in sym.GetTypeMembers())
            {
                yield return child;
            }

            foreach (var ns in sym.GetNamespaceMembers())
            {
                foreach (var child2 in GetNamespaceTypes(ns))
                {
                    yield return child2;
                }
            }
        }

        public static bool IsDesignTime(GeneratorExecutionContext context)
        {
            var isBuildingProjectValue = context.GetMSBuildPropertyValue("BuildingProject"); // legacy projects
            var isDesignTimeBuildValue = context.GetMSBuildPropertyValue("DesignTimeBuild"); // sdk-style projects

            return string.Equals(isBuildingProjectValue, "false", StringComparison.OrdinalIgnoreCase)
                || string.Equals(isDesignTimeBuildValue, "true", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsUseStub(GeneratorExecutionContext context)
        {
            var useStub = context.GetMSBuildPropertyValue("UseStub");
            return string.Equals(useStub, "true", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsExePrj(GeneratorExecutionContext context)
        {
            var exe = context.GetMSBuildPropertyValue("OutputType");
            return exe.ToLower().Contains("exe");
        }

        public static string GetMD5(string p_str)
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(p_str);
                MD5 md = MD5.Create();
                byte[] data = md.ComputeHash(bytes);

                string[] hexDigits = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f" };
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    int num = data[i];
                    if (num < 0)
                    {
                        num = 0x100 + num;
                    }
                    int index = num / 0x10;
                    int num3 = num % 0x10;

                    builder.Append(hexDigits[index] + hexDigits[num3]);
                }
                return builder.ToString();
            }
            catch { }
            return "";
        }
    }
}
