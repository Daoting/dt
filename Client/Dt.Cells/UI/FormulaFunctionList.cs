#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine.Functions;
using Dt.Cells.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Windows.Storage;
#endregion

namespace Dt.Cells.UI
{
    internal class FormulaFunctionList
    {
        static Dictionary<string, FormulaFunction> _allList = new Dictionary<string, FormulaFunction>();

        static FormulaFunctionList()
        {
            foreach (TypeInfo info in typeof(CalcFunction).GetTypeInfo().Assembly.DefinedTypes)
            {
                if ((info.IsPublic && !info.IsAbstract) && IntrospectionExtensions.GetTypeInfo((Type) typeof(CalcFunction)).IsAssignableFrom(info))
                {
                    foreach (ConstructorInfo info2 in info.DeclaredConstructors)
                    {
                        if (Enumerable.Count<ParameterInfo>(info2.GetParameters()) == 0)
                        {
                            try
                            {
                                CalcFunction function = info2.Invoke(new object[0]) as CalcFunction;
                                _allList.Add(function.Name, new FormulaFunction(function));
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
            ReadFormulaInformation();
        }

        static void ReadFormulaInformation()
        {
            Stream manifestResourceStream = typeof(FormulaFunctionList).GetTypeInfo().Assembly.GetManifestResourceStream("Dt.Cells.Res.FunctionInformation.zh_CN.xml");
            StreamReader reader = new StreamReader(manifestResourceStream);
            string s = reader.ReadToEnd();
            reader.Dispose();
            manifestResourceStream.Dispose();

            string attribute = string.Empty;
            string str3 = string.Empty;
            string str4 = string.Empty;
            if (s != null)
            {
                using (StringReader reader2 = new StringReader(s))
                {
                    using (XmlReader reader3 = XmlReader.Create((TextReader) reader2))
                    {
                        while (reader3.Read())
                        {
                            if (reader3.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element)))
                            {
                                break;
                            }
                        }
                        while (reader3.Read())
                        {
                            if (reader3.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element)))
                            {
                                attribute = reader3.GetAttribute(0);
                                str3 = reader3.GetAttribute(1);
                                str4 = reader3.GetAttribute(2);
                                FormulaFunction function = _allList[attribute];
                                if (function != null)
                                {
                                    function.Description = str4;
                                    function.FullName = attribute + "(" + str3 + ")";
                                    function.Param = str3.Split(new char[] { ',' });
                                }
                            }
                        }
                    }
                }
            }
        }

        public static Dictionary<string, FormulaFunction> AllFunctions
        {
            get { return  _allList; }
        }
    }
}

