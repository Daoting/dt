#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
#endregion

namespace Dt.Xls
{
    internal class FormulaProcess
    {
        private static string _alphabetTable = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static HashSet<string> _excel2007Functions = new HashSet<string>(new string[] { "AVERAGEIF", "AVERAGEIFS", "CUBEKPIMEMBER", "CUBEMEMBER", "CUBEMEMBERPROPERTY", "CUBERANKEDMEMBER", "CUBESET", "CUBESETCOUNT", "CUBEVALUE", "COUNTIFS", "IFERROR", "SUMIFS" });
        private static string[] valueFunctions = new string[] { "ISNUMBER", "ISBLANK", "ISERR", "ISERROR", "ISEVEN", "ISLOGICAL", "ISNA", "ISNONTEXT", "ISODD", "LEN" };
        private static string[] refFunctions = new string[] {
            "COLUMN", "ROW", "DCOUNT", "DCOUNTA", "DGET", "DMAX", "DMIN", "DAVERAGE", "DPRODUCT", "DSTDEV", "DSTDEVP", "DSUM", "DVAR", "DVARP", "OFFSET", "AVERAGEA",
            "COUNT", "COUNTA", "COUNTIF", "SUMIF", "FREQUENCY", "TREND", "STDEVA", "STDEVPA", "SUBTOTAL", "NETWORKDAYS", "MAXA", "MINA", "VARA", "VARPA", "WORKDAY", "XIRR",
            "AVERAGE", "COUNTBLANK", "FORECAST", "INDEX", "KURT", "STDEV", "STDEVP", "SERIESSUM", "SUMSQ", "SUM", "MAX", "MEDIAN", "MIN", "MODE", "PRODUCT", "RANK",
            "VARP", "ROWS", "XNPV", "AVERAGEIFS", "COUNTIFS", "IFERROR", "SUMIFS", "MATCH"
         };
        private static string[] arrayFunctions = new string[] { "LINEST", "LOGEST", "MDETERM", "MINVERSE", "MMULT", "SERIESSUM", "SLOPE", "SUMPRODUCT", "SUMX2MY2", "SUMX2PY2", "SUMXMY2", "SUMSQ" };
        private static string[] externalFunctions = new string[] {
            "ACCRINT", "ACCRINTM", "BESSELI", "BESSELJ", "BESSELY", "BESSELK", "DELTA", "DISC", "EDATE", "EFFECT", "EOMONTH", "ERF", "ERFC", "FACTDOUBLE", "FVSCHEDULE", "GCD",
            "GESTEP", "INTRATE", "ISEVEN", "ISODD", "LCM", "MROUND", "MULTINOMIAL", "NOMINAL", "PRICEDISC", "PRICEMAT", "QUOTIENT", "RANDBETWEEN", "RECEIVED", "SERIESSUM", "SQRTPI", "TBILLEQ",
            "TBILLPRICE", "TBILLYIELD", "WEEKNUM", "AMORDEGRC", "AMORLINC", "BIN2DEC", "BIN2HEX", "BIN2OCT", "COMPLEX", "CONVERT", "COUPDAYBS", "COUPDAYS", "COUPDAYSNC", "COUPNCD", "COUPNUM", "COUPPCD",
            "CUMIPMT", "CUMPRINC", "DEC2BIN", "DEC2HEX", "DEC2OCT", "DOLLARDE", "DOLLARFR", "DURATION", "EURO", "EUROCONVERT", "HEX2BIN", "HEX2DEC", "HEX2OCT", "IMABS", "IMAGINARY", "IMARGUMENT",
            "IMCONJUGATE", "IMCOS", "IMDIV", "IMEXP", "IMLN", "IMLOG10", "IMLOG2", "IMPOWER", "IMPRODUCT", "IMREAL", "IMSIN", "IMSQRT", "IMSUB", "IMSUM", "NETWORKDAYS", "OCT2BIN",
            "OCT2DEC", "OCT2HEX", "ODDFPRICE", "ODDFYIELD", "ODDLPRICE", "ODDLYIELD", "PRICE", "WORKDAY", "XIRR", "XNPV", "YEARFRAC", "YIELDDISC", "YIELD", "YIELDMAT", "AVERAGEIF", "AVERAGEIFS",
            "COUNTIFS", "IFERROR", "SUMIFS", "MDURATION"
         };
        private static HashSet<string> _SpecialFunctions = new HashSet<string>(new string[] { "IF", "CELL", "CHOOSE", "INFO", "OFFSET" });

        private static HashSet<string> refFunctionList = new HashSet<string>(refFunctions, (IEqualityComparer<string>)new InvariantCultureStringComparer(true));
        private static Dictionary<Xlf, int> _functionArgumentCount = new Dictionary<Xlf, int>();
        private static Dictionary<string, Xlf> _nameToXlfDict = new Dictionary<string, Xlf>();
        private static HashSet<string> _noArrayReferencenFunctions = new HashSet<string>();
        private static HashSet<string> _valueFunctionList = new HashSet<string>(valueFunctions, (IEqualityComparer<string>) new InvariantCultureStringComparer(true));
        private static Dictionary<Xlf, string> _xlfToString = new Dictionary<Xlf, string>();
        private static HashSet<string> arrayFunctionList = new HashSet<string>(arrayFunctions, (IEqualityComparer<string>) new InvariantCultureStringComparer(true));
        internal static HashSet<string> externalFunctionList = new HashSet<string>(externalFunctions, (IEqualityComparer<string>) new InvariantCultureStringComparer(true));

        internal int column;
        private int customNameIndex;
        private int definedNameIndex;
        internal int activeSheet;
        internal bool _isArrayFormula;
        internal bool _isConditionalFormatFormula;
        internal bool _isDataValidationFormula;
        private int externNameIndex;
        private int externSheetIndex;
        private MemoryStream extraBuff;
        private BinaryWriter extraWriter;
        private Dictionary<string, TokenClass[]> fnParamRefClassDefs;
        internal BinaryReader formulaExtraReader;
        internal BinaryReader formulaReader;
        private const string FUNCTION_PREFIX = "$Excel_2007_Function_Name";
        private bool hasExternNames;
        internal bool isA1RefStyle;
        internal bool isDefinedNameFormula;
        internal bool isNamedReference;
        internal string name;
        private bool namedExpression;
        
        internal int row;
        internal int sheet;
        internal List<string> sheetNames;
        internal List<Tuple<int, int>> SheetsSelectionList;
        
        static FormulaProcess()
        {
            _noArrayReferencenFunctions.Add("ROW");
            _noArrayReferencenFunctions.Add("COlUMN");
            _noArrayReferencenFunctions.Add("COUNTIF");
            _noArrayReferencenFunctions.Add("COUNTIFS");

            foreach (string name in Enum.GetNames(typeof(Xlf)))
            {
                Xlf key = (Xlf)Enum.Parse(typeof(Xlf), name, true);
                _nameToXlfDict.Add(name.ToUpperInvariant(), key);
                switch (key)
                {
                    case Xlf.SetName:
                        {
                            _xlfToString.Add(key, "SET.NAME");
                            continue;
                        }
                    case Xlf.ActiveCell:
                        {
                            _xlfToString.Add(key, "ACTIVE.CELL");
                            continue;
                        }
                    case Xlf.GetFormula:
                        {
                            _xlfToString.Add(key, "GET.FORMULA");
                            continue;
                        }
                    case Xlf.GetName:
                        {
                            _xlfToString.Add(key, "GET.NAME");
                            continue;
                        }
                    case Xlf.SetValue:
                        {
                            _xlfToString.Add(key, "SET.VALUE");
                            continue;
                        }
                    case Xlf.GetDef:
                        {
                            _xlfToString.Add(key, "GET.DEF");
                            continue;
                        }
                    case Xlf.AddBar:
                        {
                            _xlfToString.Add(key, "ADD.MENU");
                            continue;
                        }
                    case Xlf.AddMenu:
                        {
                            _xlfToString.Add(key, "ADD.MENU");
                            continue;
                        }
                    case Xlf.AddCommand:
                        {
                            _xlfToString.Add(key, "ADD.COMMAND");
                            continue;
                        }
                    case Xlf.EnableCommand:
                        {
                            _xlfToString.Add(key, "ENABLE.COMMAND");
                            continue;
                        }
                    case Xlf.CheckCommand:
                        {
                            _xlfToString.Add(key, "CHECK.COMMAND");
                            continue;
                        }
                    case Xlf.RenameCommand:
                        {
                            _xlfToString.Add(key, "RENAME.COMMAND");
                            continue;
                        }
                    case Xlf.ShowBar:
                        {
                            _xlfToString.Add(key, "SHOW.BAR");
                            continue;
                        }
                    case Xlf.DeleteMenu:
                        {
                            _xlfToString.Add(key, "DELETE.MENU");
                            continue;
                        }
                    case Xlf.DeleteCommand:
                        {
                            _xlfToString.Add(key, "DELETE.COMMAND");
                            continue;
                        }
                    case Xlf.GetChartItem:
                        {
                            _xlfToString.Add(key, "GET.CHAR.ITEM");
                            continue;
                        }
                    case Xlf.DialogBox:
                        {
                            _xlfToString.Add(key, "DIALOG.BOX");
                            continue;
                        }
                    case Xlf.CancelKey:
                        {
                            _xlfToString.Add(key, "CANCEL.KEY");
                            continue;
                        }
                    case Xlf.GetBar:
                        {
                            _xlfToString.Add(key, "GET.BAR");
                            continue;
                        }
                    case Xlf.GetCell:
                        {
                            _xlfToString.Add(key, "GET.CELL");
                            continue;
                        }
                    case Xlf.GetWorkspace:
                        {
                            _xlfToString.Add(key, "GET.WORKSPACE");
                            continue;
                        }
                    case Xlf.GetWindow:
                        {
                            _xlfToString.Add(key, "GET.WINDOW");
                            continue;
                        }
                    case Xlf.GetDocument:
                        {
                            _xlfToString.Add(key, "GET.DOCUMENT");
                            continue;
                        }
                    case Xlf.GetNote:
                        {
                            _xlfToString.Add(key, "GET.NOTE");
                            continue;
                        }
                    case Xlf.DeleteBar:
                        {
                            _xlfToString.Add(key, "DELETE.BAR");
                            continue;
                        }
                    case Xlf.ElseIf:
                        {
                            _xlfToString.Add(key, "ELSE.IF");
                            continue;
                        }
                    case Xlf.EndIf:
                        {
                            _xlfToString.Add(key, "END.IF");
                            continue;
                        }
                    case Xlf.ForCell:
                        {
                            _xlfToString.Add(key, "FOR.CELL");
                            continue;
                        }
                    case Xlf.LastError:
                        {
                            _xlfToString.Add(key, "LAST.ERROR");
                            continue;
                        }
                    case Xlf.CustomRepeat:
                        {
                            _xlfToString.Add(key, "CUSTOM.REPEAT");
                            continue;
                        }
                    case Xlf.FormulaConvert:
                        {
                            _xlfToString.Add(key, "FORMULA.CONVERT");
                            continue;
                        }
                    case Xlf.GetLinkInfo:
                        {
                            _xlfToString.Add(key, "GET.LINK.INFO");
                            continue;
                        }
                    case Xlf.TextBox:
                        {
                            _xlfToString.Add(key, "TEXT.BOX");
                            continue;
                        }
                    case Xlf.GetObject:
                        {
                            _xlfToString.Add(key, "GET.OBJECT");
                            continue;
                        }
                    case Xlf.AddToolbar:
                        {
                            _xlfToString.Add(key, "ADD.TOOLBAR");
                            continue;
                        }
                    case Xlf.DeleteToolbar:
                        {
                            _xlfToString.Add(key, "DELETE.TOOLBAR");
                            continue;
                        }
                    case Xlf.ResetToolbar:
                        {
                            _xlfToString.Add(key, "RESET.TOOLBAR");
                            continue;
                        }
                    case Xlf.GetToolbar:
                        {
                            _xlfToString.Add(key, "GET.TOOLBAR");
                            continue;
                        }
                    case Xlf.GetTool:
                        {
                            _xlfToString.Add(key, "GET.TOOL");
                            continue;
                        }
                    case Xlf.SpellingCheck:
                        {
                            _xlfToString.Add(key, "SPELLING.CHECK");
                            continue;
                        }
                    case Xlf.ErrorType:
                        {
                            _xlfToString.Add(key, "ERROR.TYPE");
                            continue;
                        }
                    case Xlf.AppTitle:
                        {
                            _xlfToString.Add(key, "APP.TITLE");
                            continue;
                        }
                    case Xlf.WindowTitle:
                        {
                            _xlfToString.Add(key, "WINDOW.TITLE");
                            continue;
                        }
                    case Xlf.SaveToolbar:
                        {
                            _xlfToString.Add(key, "SAVE.TOOLBAR");
                            continue;
                        }
                    case Xlf.EnableTool:
                        {
                            _xlfToString.Add(key, "ENABLE.TOOL");
                            continue;
                        }
                    case Xlf.PressTool:
                        {
                            _xlfToString.Add(key, "PRESS.TOOL");
                            continue;
                        }
                    case Xlf.RegisterId:
                        {
                            _xlfToString.Add(key, "REGISTER.ID");
                            continue;
                        }
                    case Xlf.GetWorkbook:
                        {
                            _xlfToString.Add(key, "GET.WORKBOOK");
                            continue;
                        }
                    case Xlf.MovieCommand:
                        {
                            _xlfToString.Add(key, "MOVIE.COMMAND");
                            continue;
                        }
                    case Xlf.GetMovie:
                        {
                            _xlfToString.Add(key, "GET.MOVIE");
                            continue;
                        }
                    case Xlf.PivotAddData:
                        {
                            _xlfToString.Add(key, "PIVOT.ADD.DATE");
                            continue;
                        }
                    case Xlf.GetPivotField:
                        {
                            _xlfToString.Add(key, "GET.PIVOT.FIELD");
                            continue;
                        }
                    case Xlf.GetPivotItem:
                        {
                            _xlfToString.Add(key, "GET.PIVOT.ITEM");
                            continue;
                        }
                    case Xlf.ScenarioGet:
                        {
                            _xlfToString.Add(key, "SCENARIO.GET");
                            continue;
                        }
                    case Xlf.OptionsListsGet:
                        {
                            _xlfToString.Add(key, "OPTIONS.LISTS.GET");
                            continue;
                        }
                    case Xlf.OpenDialog:
                        {
                            _xlfToString.Add(key, "OPEN.DIALOG");
                            continue;
                        }
                    case Xlf.SaveDialog:
                        {
                            _xlfToString.Add(key, "SAVE.DIALOG");
                            continue;
                        }
                    case Xlf.ViewGet:
                        {
                            _xlfToString.Add(key, "VIEW.GET");
                            continue;
                        }
                    case Xlf.GetPivotData:
                        {
                            _xlfToString.Add(key, "GET.PIVOT.DATE");
                            continue;
                        }
                }
                _xlfToString.Add(key, key.ToString().ToUpper());
            }
            InitFunctionArgumnentCount();
        }

        public FormulaProcess()
        {
            this.sheet = -1;
            this.row = -1;
            this.column = -1;
            this.activeSheet = -1;
            this.externSheetIndex = -1;
            this.externNameIndex = -1;
            this.customNameIndex = -1;
            this.definedNameIndex = -1;
            this.CreateFunctionParamRefClassDefs();
        }

        public FormulaProcess(BinaryReader r)
        {
            this.sheet = -1;
            this.row = -1;
            this.column = -1;
            this.activeSheet = -1;
            this.externSheetIndex = -1;
            this.externNameIndex = -1;
            this.customNameIndex = -1;
            this.definedNameIndex = -1;
            this.formulaReader = r;
            this.row = 0;
            this.column = 0;
        }

        public FormulaProcess(int sheet, int row, int column, List<INameSupport> externSources)
        {
            this.sheet = -1;
            this.row = -1;
            this.column = -1;
            this.activeSheet = -1;
            this.externSheetIndex = -1;
            this.externNameIndex = -1;
            this.customNameIndex = -1;
            this.definedNameIndex = -1;
            this.sheet = sheet;
            this.row = row;
            this.column = column;
            this.CreateFunctionParamRefClassDefs();
        }

        public FormulaProcess(BinaryReader r, BinaryReader rExtra, string name, bool isA1RefStyle)
        {
            this.sheet = -1;
            this.row = -1;
            this.column = -1;
            this.activeSheet = -1;
            this.externSheetIndex = -1;
            this.externNameIndex = -1;
            this.customNameIndex = -1;
            this.definedNameIndex = -1;
            this.formulaReader = r;
            this.formulaExtraReader = rExtra;
            this.name = name;
            this.isA1RefStyle = isA1RefStyle;
            this.row = 0;
            this.column = 0;
            this.CreateFunctionParamRefClassDefs();
        }

        public FormulaProcess(BinaryReader r, BinaryReader rExtra, int sheet, int row, int column, bool isA1RefStyle)
        {
            this.sheet = -1;
            this.row = -1;
            this.column = -1;
            this.activeSheet = -1;
            this.externSheetIndex = -1;
            this.externNameIndex = -1;
            this.customNameIndex = -1;
            this.definedNameIndex = -1;
            this.formulaReader = r;
            this.formulaExtraReader = rExtra;
            this.sheet = sheet;
            this.row = row;
            this.column = column;
            this.isA1RefStyle = isA1RefStyle;
            this.CreateFunctionParamRefClassDefs();
        }

        private void BuildInfix(List<object> stack, string op)
        {
            int num = stack.Count - 1;
            stack[num - 1] = ((string) ((string) stack[num - 1])) + op + ((string) ((string) stack[num]));
            stack.RemoveAt(num);
        }

        private void BuildInfixFn(List<object> stack, string op, int argCount)
        {
            int num = stack.Count;
            int num2 = num - 1;
            StringBuilder builder = new StringBuilder();
            if (op.Equals("()"))
            {
                builder.Append("(");
                builder.Append((string) ((string) stack[num2]));
                builder.Append(")");
                stack[num2] = builder.ToString();
            }
            else if (op.Equals("%"))
            {
                builder.Append((string) ((string) stack[num2]));
                builder.Append("%");
                stack[num2] = builder.ToString();
            }
            else if (op.Equals("+") || op.Equals("-"))
            {
                builder.Append(op);
                builder.Append((string) ((string) stack[num2]));
                stack[num2] = builder.ToString();
            }
            else
            {
                builder.Append(op);
                if (argCount > 0)
                {
                    builder.Append("(");
                    string str = (string) ((string) stack[num2 - (argCount - 1)]);
                    if ((str != ",") && (str != "MissArg"))
                    {
                        builder.Append((string) ((string) stack[num2 - (argCount - 1)]));
                    }
                    for (int i = argCount - 1; i > 0; i--)
                    {
                        if (((string) stack[(num2 - i) + 1]).Equals("MissArg"))
                        {
                            builder.Append(",");
                        }
                        else
                        {
                            builder.Append(",");
                            builder.Append(stack[(num2 - i) + 1]);
                        }
                    }
                    builder.Append(")");
                    stack[num2 - (argCount - 1)] = builder.ToString();
                    stack.RemoveRange(num - (argCount - 1), argCount - 1);
                }
                else
                {
                    builder.Append("()");
                    stack.Add(builder.ToString());
                }
            }
        }

        private void BuildUnsupportedFunction(List<object> stack, Xlf fn, int argCount)
        {
            string @this = _xlfToString[fn];
            if (fn == Xlf.Text)
            {
                argCount = 2;
            }
            this.BuildInfixFn(stack, @this.ToUpper(CultureInfo.CurrentCulture), argCount);
        }

        internal void CreateFunctionParamRefClassDefs()
        {
            this.fnParamRefClassDefs = new Dictionary<string, TokenClass[]>();
            this.fnParamRefClassDefs.Add("SUMIF", new TokenClass[] { TokenClass.Reference, TokenClass.Value, TokenClass.Reference });
            this.fnParamRefClassDefs.Add("MIRR", new TokenClass[] { TokenClass.Reference, TokenClass.Value, TokenClass.Value });
            this.fnParamRefClassDefs.Add("NPV", new TokenClass[] { TokenClass.Value, TokenClass.Reference });
            this.fnParamRefClassDefs.Add("XNPV", new TokenClass[] { TokenClass.Value, TokenClass.Reference, TokenClass.Reference });
            this.fnParamRefClassDefs.Add("INDEX", new TokenClass[] { TokenClass.Reference, TokenClass.Value, TokenClass.Value });
        }

        internal static int GetArgCount(Xlf value)
        {
            int num = 0;
            if (_functionArgumentCount.TryGetValue(value, out num))
            {
                return num;
            }
            return 0;
        }

        private string GetCoord(int row, int col, bool isRowRel, bool isColRel, bool isA1, bool isWholeRow, bool isWholeCol, int? rowBase = new int?(), int? columnBase = new int?())
        {
            StringBuilder builder = null;
            string str2 = null;
            if (isA1)
            {
                builder = new StringBuilder();
                if ((col >= 0) && (col < 0x1a))
                {
                    builder.Append(_alphabetTable[col]);
                }
                else
                {
                    int num = col;
                    int num2 = 0;
                    int num3 = 0;
                    int num4 = 0;
                    int num5 = 0;
                    while (num2 <= num)
                    {
                        num2 = (int) Math.Pow(26.0, (double) (++num3));
                    }
                    num3--;
                    while (num3 >= 0)
                    {
                        num4 = (int) Math.Pow(26.0, (double) num3--);
                        num5 = (num / num4) - ((num3 == -1) ? 0 : 1);
                        builder.Append(_alphabetTable[num5]);
                        num -= (num5 + 1) * num4;
                    }
                }
                if (isRowRel)
                {
                    int num6 = row + 1;
                    str2 = ((int) num6).ToString();
                }
                else
                {
                    int num7 = row + 1;
                    str2 = "$" + ((int) num7).ToString();
                }
                if (!isColRel)
                {
                    builder.Insert(0, "$");
                }
                if (isWholeCol)
                {
                    return builder.ToString();
                }
                if (isWholeRow)
                {
                    return str2.ToString();
                }
                return (builder.ToString() + str2);
            }
            if (!isWholeCol)
            {
                if (isRowRel)
                {
                    int num8 = row;
                    int? nullable = rowBase;
                    int? nullable3 = nullable.HasValue ? new int?(num8 - nullable.GetValueOrDefault()) : null;
                    if ((nullable3.GetValueOrDefault() == 0) && nullable3.HasValue)
                    {
                        str2 = "R";
                    }
                    else
                    {
                        int num9 = row;
                        int? nullable4 = rowBase;
                        str2 = "R[" + (nullable4.HasValue ? new int?(num9 - nullable4.GetValueOrDefault()) : null) + "]";
                    }
                }
                else
                {
                    int num10 = row + 1;
                    str2 = "R" + ((int) num10).ToString();
                }
            }
            if (!isWholeRow)
            {
                if (isColRel)
                {
                    int num11 = col;
                    int? nullable6 = columnBase;
                    int? nullable8 = nullable6.HasValue ? new int?(num11 - nullable6.GetValueOrDefault()) : null;
                    if ((nullable8.GetValueOrDefault() == 0) && nullable8.HasValue)
                    {
                        builder = new StringBuilder("C");
                    }
                    else
                    {
                        builder = new StringBuilder("C[");
                        int num12 = col;
                        int? nullable9 = columnBase;
                        builder.Append(nullable9.HasValue ? new int?(num12 - nullable9.GetValueOrDefault()) : null);
                        builder.Append("]");
                    }
                }
                else
                {
                    builder = new StringBuilder("C");
                    builder.Append((int) (col + 1));
                }
            }
            if (isWholeRow)
            {
                return str2;
            }
            if (isWholeCol)
            {
                return builder.ToString();
            }
            return (str2 + builder.ToString());
        }

        private Ptg GetPtg(string token)
        {
            if (token == "+")
            {
                return Ptg.Add;
            }
            if (token == "-")
            {
                return Ptg.Sub;
            }
            if (token == "*")
            {
                return Ptg.Mul;
            }
            if (token == "/")
            {
                return Ptg.Div;
            }
            if (token == "%")
            {
                return Ptg.Percent;
            }
            if (token == "<")
            {
                return Ptg.LT;
            }
            if (token == "<=")
            {
                return Ptg.LE;
            }
            if (token == ">")
            {
                return Ptg.GT;
            }
            if (token == ">=")
            {
                return Ptg.GE;
            }
            if (token == "=")
            {
                return Ptg.EQ;
            }
            if (token == "&")
            {
                return Ptg.Concat;
            }
            if (token == "<>")
            {
                return Ptg.NE;
            }
            if (token == "^")
            {
                return Ptg.Power;
            }
            if (token == ",")
            {
                return Ptg.Union;
            }
            if (token != ":")
            {
                throw new NotSupportedException("not recognized Ptg: " + token);
            }
            return Ptg.Range;
        }

        private Ptg GetUnaryOperationPtg(string token)
        {
            if (token == "+")
            {
                return Ptg.Uplus;
            }
            if (token == "-")
            {
                return Ptg.Uminus;
            }
            if (token == "%")
            {
                return Ptg.Percent;
            }
            return Ptg.None;
        }

        private Xlf GetXlf(string name)
        {
            string str = name.ToUpperInvariant();
            if (_nameToXlfDict.ContainsKey(str))
            {
                return _nameToXlfDict[str];
            }
            if (str == "ERROR.TYPE")
            {
                foreach (KeyValuePair<string, Xlf> pair in _nameToXlfDict)
                {
                    if (pair.Key == "ERRORTYPE")
                    {
                        return pair.Value;
                    }
                }
            }
            return Xlf.Error;
        }

        private string HandleName(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                if (((!char.IsDigit(name[0]) && !name.Contains(" ")) && (!name.Contains("+") && !name.Contains("-"))) && ((!name.Contains(",") && !name.Contains("&")) && !name.Contains(".")))
                {
                    return name;
                }
                name = "'" + name + "'";
            }
            return name;
        }

        private static void InitFunctionArgumnentCount()
        {
            _functionArgumentCount.Add(Xlf.Transpose, 1);
            _functionArgumentCount.Add(Xlf.Isna, 1);
            _functionArgumentCount.Add(Xlf.Iserror, 1);
            _functionArgumentCount.Add(Xlf.Sin, 1);
            _functionArgumentCount.Add(Xlf.Cos, 1);
            _functionArgumentCount.Add(Xlf.Tan, 1);
            _functionArgumentCount.Add(Xlf.Atan, 1);
            _functionArgumentCount.Add(Xlf.Sqrt, 1);
            _functionArgumentCount.Add(Xlf.Exp, 1);
            _functionArgumentCount.Add(Xlf.Ln, 1);
            _functionArgumentCount.Add(Xlf.Log10, 1);
            _functionArgumentCount.Add(Xlf.Abs, 1);
            _functionArgumentCount.Add(Xlf.Int, 1);
            _functionArgumentCount.Add(Xlf.Sign, 1);
            _functionArgumentCount.Add(Xlf.Round, 2);
            _functionArgumentCount.Add(Xlf.Rept, 2);
            _functionArgumentCount.Add(Xlf.Mid, 3);
            _functionArgumentCount.Add(Xlf.Len, 1);
            _functionArgumentCount.Add(Xlf.Value, 1);
            _functionArgumentCount.Add(Xlf.Not, 1);
            _functionArgumentCount.Add(Xlf.Mod, 2);
            _functionArgumentCount.Add(Xlf.Mirr, 3);
            _functionArgumentCount.Add(Xlf.Date, 3);
            _functionArgumentCount.Add(Xlf.Time, 3);
            _functionArgumentCount.Add(Xlf.Day, 1);
            _functionArgumentCount.Add(Xlf.Month, 1);
            _functionArgumentCount.Add(Xlf.Year, 1);
            _functionArgumentCount.Add(Xlf.Hour, 1);
            _functionArgumentCount.Add(Xlf.Minute, 1);
            _functionArgumentCount.Add(Xlf.Second, 1);
            _functionArgumentCount.Add(Xlf.Rows, 1);
            _functionArgumentCount.Add(Xlf.Columns, 1);
            _functionArgumentCount.Add(Xlf.Type, 1);
            _functionArgumentCount.Add(Xlf.Atan2, 2);
            _functionArgumentCount.Add(Xlf.Asin, 1);
            _functionArgumentCount.Add(Xlf.Acos, 1);
            _functionArgumentCount.Add(Xlf.Isref, 1);
            _functionArgumentCount.Add(Xlf.Char, 1);
            _functionArgumentCount.Add(Xlf.Lower, 1);
            _functionArgumentCount.Add(Xlf.Upper, 1);
            _functionArgumentCount.Add(Xlf.Proper, 1);
            _functionArgumentCount.Add(Xlf.Exact, 2);
            _functionArgumentCount.Add(Xlf.Trim, 1);
            _functionArgumentCount.Add(Xlf.Replace, 4);
            _functionArgumentCount.Add(Xlf.Code, 1);
            _functionArgumentCount.Add(Xlf.Find, 3);
            _functionArgumentCount.Add(Xlf.Iserr, 1);
            _functionArgumentCount.Add(Xlf.Istext, 1);
            _functionArgumentCount.Add(Xlf.Isnumber, 1);
            _functionArgumentCount.Add(Xlf.Isblank, 1);
            _functionArgumentCount.Add(Xlf.T, 1);
            _functionArgumentCount.Add(Xlf.N, 1);
            _functionArgumentCount.Add(Xlf.Datevalue, 1);
            _functionArgumentCount.Add(Xlf.Timevalue, 1);
            _functionArgumentCount.Add(Xlf.Sln, 3);
            _functionArgumentCount.Add(Xlf.Syd, 4);
            _functionArgumentCount.Add(Xlf.Clean, 1);
            _functionArgumentCount.Add(Xlf.Fact, 1);
            _functionArgumentCount.Add(Xlf.Isnontext, 1);
            _functionArgumentCount.Add(Xlf.Islogical, 1);
            _functionArgumentCount.Add(Xlf.Roundup, 2);
            _functionArgumentCount.Add(Xlf.Rounddown, 2);
            _functionArgumentCount.Add(Xlf.Sinh, 1);
            _functionArgumentCount.Add(Xlf.Cosh, 1);
            _functionArgumentCount.Add(Xlf.Tanh, 1);
            _functionArgumentCount.Add(Xlf.Asinh, 1);
            _functionArgumentCount.Add(Xlf.Acosh, 1);
            _functionArgumentCount.Add(Xlf.Atanh, 1);
            _functionArgumentCount.Add(Xlf.ErrorType, 1);
            _functionArgumentCount.Add(Xlf.Gammaln, 1);
            _functionArgumentCount.Add(Xlf.Binomdist, 4);
            _functionArgumentCount.Add(Xlf.Chidist, 2);
            _functionArgumentCount.Add(Xlf.Chiinv, 2);
            _functionArgumentCount.Add(Xlf.Combin, 2);
            _functionArgumentCount.Add(Xlf.Confidence, 3);
            _functionArgumentCount.Add(Xlf.Critbinom, 3);
            _functionArgumentCount.Add(Xlf.Even, 1);
            _functionArgumentCount.Add(Xlf.Expondist, 3);
            _functionArgumentCount.Add(Xlf.Fdist, 3);
            _functionArgumentCount.Add(Xlf.Finv, 3);
            _functionArgumentCount.Add(Xlf.Fisher, 1);
            _functionArgumentCount.Add(Xlf.Fisherinv, 1);
            _functionArgumentCount.Add(Xlf.Floor, 2);
            _functionArgumentCount.Add(Xlf.Gammadist, 4);
            _functionArgumentCount.Add(Xlf.Gammainv, 3);
            _functionArgumentCount.Add(Xlf.Ceiling, 2);
            _functionArgumentCount.Add(Xlf.Hypgeomdist, 4);
            _functionArgumentCount.Add(Xlf.Lognormdist, 3);
            _functionArgumentCount.Add(Xlf.Loginv, 3);
            _functionArgumentCount.Add(Xlf.Negbinomdist, 3);
            _functionArgumentCount.Add(Xlf.Normdist, 4);
            _functionArgumentCount.Add(Xlf.Normsdist, 1);
            _functionArgumentCount.Add(Xlf.Norminv, 3);
            _functionArgumentCount.Add(Xlf.Normsinv, 1);
            _functionArgumentCount.Add(Xlf.Standardize, 3);
            _functionArgumentCount.Add(Xlf.Odd, 1);
            _functionArgumentCount.Add(Xlf.Permut, 2);
            _functionArgumentCount.Add(Xlf.Poisson, 3);
            _functionArgumentCount.Add(Xlf.Tdist, 3);
            _functionArgumentCount.Add(Xlf.Weibull, 4);
            _functionArgumentCount.Add(Xlf.Sumxmy2, 2);
            _functionArgumentCount.Add(Xlf.Sumx2my2, 2);
            _functionArgumentCount.Add(Xlf.Sumx2py2, 2);
            _functionArgumentCount.Add(Xlf.Chitest, 2);
            _functionArgumentCount.Add(Xlf.Correl, 2);
            _functionArgumentCount.Add(Xlf.Covar, 2);
            _functionArgumentCount.Add(Xlf.Forecast, 3);
            _functionArgumentCount.Add(Xlf.Ftest, 2);
            _functionArgumentCount.Add(Xlf.Intercept, 2);
            _functionArgumentCount.Add(Xlf.Pearson, 2);
            _functionArgumentCount.Add(Xlf.Rsq, 2);
            _functionArgumentCount.Add(Xlf.Steyx, 2);
            _functionArgumentCount.Add(Xlf.Slope, 2);
            _functionArgumentCount.Add(Xlf.Ttest, 4);
            _functionArgumentCount.Add(Xlf.Large, 2);
            _functionArgumentCount.Add(Xlf.Small, 2);
            _functionArgumentCount.Add(Xlf.Quartile, 2);
            _functionArgumentCount.Add(Xlf.Percentile, 2);
            _functionArgumentCount.Add(Xlf.Trimmean, 2);
            _functionArgumentCount.Add(Xlf.Tinv, 2);
            _functionArgumentCount.Add(Xlf.Power, 2);
            _functionArgumentCount.Add(Xlf.Radians, 1);
            _functionArgumentCount.Add(Xlf.Degrees, 1);
            _functionArgumentCount.Add(Xlf.Countif, 2);
            _functionArgumentCount.Add(Xlf.Countblank, 1);
            _functionArgumentCount.Add(Xlf.Datedif, 3);
            _functionArgumentCount.Add(Xlf.Mmult, 2);
            _functionArgumentCount.Add(Xlf.Frequency, 2);
        }

        private bool IsXlfFunction(Xlf xlf, string name)
        {
            return (string.Compare(xlf.ToString(), name, (StringComparison) StringComparison.CurrentCultureIgnoreCase) == 0);
        }

        private void ParseIFParams(List<object> stack, int trueArgLen, LinkTable linkTable, ref ExcelCalcError error, bool isDefinedName = false)
        {
            string str = null;
            BinaryReader r = null;
            FormulaProcess process = null;
            string str2 = null;
            string str3 = null;
            BinaryReader reader2 = null;
            FormulaProcess process2 = null;
            byte[] buffer = this.formulaReader.ReadBytes(trueArgLen - 4);
            if (this.formulaReader.ReadByte() == 0x19)
            {
                this.formulaReader.ReadByte();
                int num2 = this.formulaReader.ReadInt16();
                if (((num2 != 0) && (num2 != 3)) && (num2 >= 7))
                {
                    reader2 = new BinaryReader((Stream) new MemoryStream(this.formulaReader.ReadBytes((num2 - 4) - 3)));
                    if ((this.name == null) || (this.name.Length == 0))
                    {
                        process2 = new FormulaProcess(reader2, this.formulaExtraReader, this.sheet, this.row, this.column, this.isA1RefStyle);
                    }
                    else
                    {
                        process2 = new FormulaProcess(reader2, this.formulaExtraReader, this.name, this.isA1RefStyle);
                    }
                    process2.HasExternNames = this.hasExternNames;
                    process2.SheetsSelectionList = this.SheetsSelectionList;
                    if (this.formulaReader.ReadByte() == 0x19)
                    {
                        this.formulaReader.ReadByte();
                        num2 = this.formulaReader.ReadInt16();
                    }
                }
                switch (this.formulaReader.ReadByte())
                {
                    case 0x22:
                    case 0x42:
                        this.formulaReader.ReadBytes(3);
                        goto Label_015F;
                }
                this.formulaReader.ReadBytes(3);
            }
        Label_015F:
            r = new BinaryReader((Stream) new MemoryStream(buffer));
            if ((this.name == null) || (this.name.Length == 0))
            {
                process = new FormulaProcess(r, this.formulaExtraReader, this.sheet, this.row, this.column, this.isA1RefStyle);
            }
            else
            {
                process = new FormulaProcess(r, this.formulaExtraReader, this.name, this.isA1RefStyle);
            }
            process.hasExternNames = this.hasExternNames;
            process.SheetsSelectionList = this.SheetsSelectionList;
            if (process2 == null)
            {
                str2 = process.ToString(linkTable, ref error, new int?(this.row), new int?(this.column), isDefinedName);
                str = string.Concat((string[]) new string[] { "IF(", ((string) stack[stack.Count - 1]), ",", str2, ")" });
                if (error != null)
                {
                    stack.Clear();
                    return;
                }
            }
            else
            {
                str2 = process.ToString(linkTable, ref error, new int?(this.row), new int?(this.column), isDefinedName);
                if (error != null)
                {
                    stack.Clear();
                    return;
                }
                str3 = process2.ToString(linkTable, ref error, new int?(this.row), new int?(this.column), isDefinedName);
                if (error != null)
                {
                    stack.Clear();
                    return;
                }
                if ((str2 == null) || (str3 == null))
                {
                    error = ExcelCalcError.WrongFunctionOrRangeName;
                }
                else
                {
                    str = string.Concat((string[]) new string[] { "IF(", ((string) stack[stack.Count - 1]), ",", str2, ",", str3, ")" });
                }
            }
            stack[stack.Count - 1] = str;
            r.Close();
            if (reader2 != null)
            {
                reader2.Close();
            }
        }

        private bool PreProcessWorksheetName(string sheetName, out string newSheetName)
        {
            if (sheetName == null)
            {
                newSheetName = string.Empty;
                return false;
            }
            bool flag = false;
            StringBuilder builder = new StringBuilder();
            foreach (char ch in sheetName)
            {
                switch (ch)
                {
                    case '\'':
                        builder.Append("''");
                        flag = true;
                        break;

                    case ' ':
                    case '"':
                        builder.Append(ch);
                        flag = true;
                        break;

                    default:
                        builder.Append(ch);
                        break;
                }
            }
            newSheetName = builder.ToString();
            return flag;
        }

        private void ProcessFunction(BinaryWriter writer, ParsedToken token, TokenClass tokenClass, LinkTable linkTable, ref int extraDataLength, Ptg ptgType = 0)
        {
            string name = token.Token.ToUpperInvariant();
            if ((((this.GetXlf(name) == Xlf.Error) && !externalFunctionList.Contains(name)) && (!refFunctionList.Contains(name) && !arrayFunctionList.Contains(name))) && !_valueFunctionList.Contains(name))
            {
                int definedNameIndex = linkTable.GetDefinedNameIndex(name, -1);
                writer.Write((byte) 0x23);
                writer.Write((ushort) (definedNameIndex + 1));
                writer.Write((ushort) 0);
                ParsedToken[] tokenArray = token.Value as ParsedToken[];
                for (int j = 0; j < tokenArray.Length; j++)
                {
                    this.WriteArgument(writer, tokenArray[j], tokenClass, linkTable, ref extraDataLength, ptgType);
                }
                int num3 = tokenArray.Length + 1;
                writer.Write((byte) 0x42);
                writer.Write((byte) ((byte) num3));
                writer.Write((short) 0xff);
                return;
            }
            if (((name == "NORMDIST") || (name == "EXPONDIST")) || (name == "BINOMDIST"))
            {
                ParsedToken[] tokenArray2 = token.Value as ParsedToken[];
                int index = tokenArray2.Length - 1;
                for (int k = 0; k < tokenArray2.Length; k++)
                {
                    if ((k == index) && (tokenArray2[index].TokenType != TokenType.Boolean))
                    {
                        bool flag = false;
                        ParsedToken token1 = tokenArray2[index];
                        if (tokenArray2[index].TokenType == TokenType.Double)
                        {
                            if (Math.Abs((double) (((double) tokenArray2[index].Value) - 0.0)) < 1E-05)
                            {
                                flag = false;
                            }
                            else if (Math.Abs((double) (((double) tokenArray2[index].Value) - 1.0)) < 1E-05)
                            {
                                flag = true;
                            }
                            this.WriteBoolean(writer, flag);
                            continue;
                        }
                    }
                    this.WriteArgument(writer, tokenArray2[k], tokenClass, linkTable, ref extraDataLength, ptgType);
                }
                this.WriteFunctionImp(writer, token, this.GetXlf(token.Token), tokenArray2.Length, tokenClass, ref extraDataLength, ptgType);
                return;
            }
            if ((!_SpecialFunctions.Contains(name) && (token.TokenType != TokenType.UndefinedFunction)) && !externalFunctionList.Contains(name))
            {
                TokenClass class3 = tokenClass;
                if (refFunctionList.Contains(name))
                {
                    tokenClass = TokenClass.Reference;
                }
                else if (arrayFunctionList.Contains(name))
                {
                    tokenClass = TokenClass.Array;
                }
                else if (_valueFunctionList.Contains(name))
                {
                    tokenClass = TokenClass.Value;
                }
                ParsedToken[] tokenArray4 = token.Value as ParsedToken[];
                for (int m = 0; m < tokenArray4.Length; m++)
                {
                    if (this.fnParamRefClassDefs.ContainsKey(name))
                    {
                        TokenClass[] classArray = this.fnParamRefClassDefs[name];
                        if (m >= classArray.Length)
                        {
                            tokenClass = classArray[classArray.Length - 1];
                        }
                        else
                        {
                            tokenClass = classArray[m];
                        }
                    }
                    switch (name)
                    {
                        case "TRANSCOPE":
                        case "COLUMNS":
                            tokenClass = TokenClass.Reference;
                            break;

                        case "GROWTH":
                            if (m < 3)
                            {
                                tokenClass = TokenClass.Reference;
                            }
                            else
                            {
                                tokenClass = TokenClass.Value;
                            }
                            break;

                        case "RANK":
                            if (m == 0)
                            {
                                tokenClass = TokenClass.Value;
                            }
                            else
                            {
                                tokenClass = TokenClass.Reference;
                            }
                            break;

                        case "SUM":
                            if ((tokenArray4.Length == 1) && (tokenArray4[0].TokenType == TokenType.Function))
                            {
                                this.WriteSumFunctionWithOneParameter(writer, token, tokenClass, linkTable, ref extraDataLength);
                                return;
                            }
                            if (tokenArray4[m].TokenType == TokenType.BinaryOperation)
                            {
                                tokenClass = TokenClass.Value;
                            }
                            else if (((tokenArray4[m].TokenType == TokenType.Reference) || (tokenArray4[m].TokenType == TokenType.ExReference)) || ((tokenArray4[m].TokenType == TokenType.Name) || (tokenArray4[m].TokenType == TokenType.SubExpression)))
                            {
                                tokenClass = TokenClass.Reference;
                            }
                            else
                            {
                                tokenClass = TokenClass.Value;
                            }
                            break;

                        case "MAX":
                        case "MIN":
                            if (tokenArray4[m].TokenType == TokenType.BinaryOperation)
                            {
                                if (this._isArrayFormula)
                                {
                                    tokenClass = TokenClass.Array;
                                }
                                else
                                {
                                    tokenClass = TokenClass.Value;
                                }
                            }
                            else if (((tokenArray4[m].TokenType == TokenType.Reference) || (tokenArray4[m].TokenType == TokenType.ExReference)) || (tokenArray4[m].TokenType == TokenType.Name))
                            {
                                tokenClass = TokenClass.Reference;
                            }
                            else
                            {
                                tokenClass = TokenClass.Value;
                            }
                            break;

                        case "SUMPRODUCT":
                            tokenClass = TokenClass.Array;
                            break;

                        case "IRR":
                            if (m == 0)
                            {
                                tokenClass = TokenClass.Reference;
                            }
                            else
                            {
                                tokenClass = TokenClass.Value;
                            }
                            break;

                        case "TTEST":
                            if ((tokenArray4[m].Value is RangeExpression) || (tokenArray4[m].Value is ExternalRangeExpression))
                            {
                                tokenClass = TokenClass.Array;
                            }
                            else
                            {
                                tokenClass = TokenClass.Value;
                            }
                            break;

                        case "INDEX":
                            if (m == 0)
                            {
                                if (this._isArrayFormula)
                                {
                                    tokenClass = TokenClass.Array;
                                }
                                else
                                {
                                    tokenClass = TokenClass.Reference;
                                }
                            }
                            else
                            {
                                tokenClass = TokenClass.Value;
                            }
                            break;

                        case "OR":
                        case "AND":
                            if (tokenArray4[m].TokenType == TokenType.Reference)
                            {
                                tokenClass = TokenClass.Reference;
                            }
                            else if (tokenArray4[m].TokenType == TokenType.Array)
                            {
                                tokenClass = TokenClass.Array;
                            }
                            break;

                        case "FORECAST":
                            if (m == 0)
                            {
                                tokenClass = TokenClass.Value;
                            }
                            if (m > 0)
                            {
                                tokenClass = TokenClass.Array;
                            }
                            break;

                        case "CHITEST":
                        case "COVAR":
                        case "CORREL":
                        case "STEYX":
                        case "RSQ":
                        case "PEARSON":
                        case "SKEW":
                        case "INTERCEPT":
                        case "DEVSQ":
                        case "FTEST":
                        case "MDETERM":
                        case "MODE":
                            tokenClass = TokenClass.Array;
                            break;

                        case "PROB":
                            if (m < 2)
                            {
                                tokenClass = TokenClass.Array;
                            }
                            break;

                        case "PERCENTRANK":
                        case "SMALL":
                        case "ZTEST":
                        case "LARGE":
                        case "TRIMMEAN":
                        case "PERCENTILE":
                            if (m == 0)
                            {
                                tokenClass = TokenClass.Array;
                            }
                            break;

                        case "AVEDEV":
                        case "GEOMEAN":
                        case "QUARTILE":
                        case "HARMEAN":
                            if ((tokenArray4[m].Value is RangeExpression) || (tokenArray4[m].Value is ExternalRangeExpression))
                            {
                                tokenClass = TokenClass.Array;
                            }
                            break;

                        case "ISREF":
                            tokenClass = TokenClass.Reference;
                            break;

                        case "VAR":
                            if ((tokenArray4[m].Value is RangeExpression) || (tokenArray4[m].Value is ExternalRangeExpression))
                            {
                                tokenClass = TokenClass.Reference;
                            }
                            break;

                        case "COUNTIF":
                            if (m == 1)
                            {
                                tokenClass = TokenClass.Value;
                            }
                            break;

                        case "N":
                        case "T":
                            if ((tokenArray4[m].Value is RangeExpression) || (tokenArray4[m].Value is ExternalRangeExpression))
                            {
                                tokenClass = TokenClass.Reference;
                            }
                            break;

                        case "LOOKUP":
                        case "HLOOKUP":
                        case "VLOOKUP":
                        case "MATCH":
                        {
                            if (m == 0)
                            {
                                if (this._isArrayFormula)
                                {
                                    this.WriteArgument(writer, tokenArray4[m], TokenClass.Array, linkTable, ref extraDataLength, ptgType);
                                }
                                else
                                {
                                    this.WriteArgument(writer, tokenArray4[m], TokenClass.Value, linkTable, ref extraDataLength, ptgType);
                                }
                            }
                            else
                            {
                                if ((m <= 0) || (tokenClass != TokenClass.Value))
                                {
                                    break;
                                }
                                if (tokenArray4[m].TokenType == TokenType.ExReference)
                                {
                                    this.WriteArgument(writer, tokenArray4[m], TokenClass.Array, linkTable, ref extraDataLength, ptgType);
                                }
                                else
                                {
                                    this.WriteArgument(writer, tokenArray4[m], TokenClass.Reference, linkTable, ref extraDataLength, ptgType);
                                }
                            }
                            continue;
                        }
                        case "ROW":
                        case "COLUMN":
                        {
                            this.WriteArgument(writer, tokenArray4[m], TokenClass.Reference, linkTable, ref extraDataLength, ptgType);
                            continue;
                        }
                    }
                    if (tokenArray4[m].TokenType == TokenType.Function)
                    {
                        Ptg funcVar = Ptg.FuncVar;
                        if (tokenClass == TokenClass.Value)
                        {
                            funcVar = Ptg.FuncVarV;
                        }
                        else if (tokenClass == TokenClass.Array)
                        {
                            funcVar = Ptg.FuncVarA;
                        }
                        this.WriteArgument(writer, tokenArray4[m], tokenClass, linkTable, ref extraDataLength, funcVar);
                    }
                    else if (tokenArray4[m].TokenType == TokenType.Array)
                    {
                        this.WriteArgument(writer, tokenArray4[m], tokenClass, linkTable, ref extraDataLength, Ptg.ArrayA);
                    }
                    else
                    {
                        bool flag2 = false;
                        if ((this._isArrayFormula && _noArrayReferencenFunctions.Contains(name)) && (m == 0))
                        {
                            this._isArrayFormula = false;
                            flag2 = true;
                        }
                        this.WriteArgument(writer, tokenArray4[m], tokenClass, linkTable, ref extraDataLength, ptgType);
                        if (flag2)
                        {
                            this._isArrayFormula = true;
                        }
                    }
                }
                this.WriteFunctionImp(writer, token, this.GetXlf(token.Token), tokenArray4.Length, class3, ref extraDataLength, ptgType);
                tokenClass = class3;
                return;
            }
            new List<object>();
            TokenClass class2 = tokenClass;
            ParsedToken[] args = token.Value as ParsedToken[];
            if (refFunctionList.Contains(name))
            {
                tokenClass = TokenClass.Reference;
            }
            else if (name == "IF")
            {
                tokenClass = TokenClass.Value;
            }
            int nameIndex = -1;
            int length = args.Length;
            if (length == 0)
            {
                int num8 = linkTable.GetDefinedNameIndex(this.name);
                writer.Write((byte) 0x23);
                writer.Write((short) ((short) num8));
                writer.Write((short) 0);
                writer.Write((byte) 0x42);
                writer.Write((byte) 1);
                writer.Write((short) 0xff);
            }
            if (CultureInfo.InvariantCulture.CompareInfo.Compare(name, "IF", (CompareOptions) CompareOptions.IgnoreCase) == 0)
            {
                this.WriteIFFunction(writer, tokenClass, linkTable, ref extraDataLength, args, ptgType);
                return;
            }
            if (CultureInfo.InvariantCulture.CompareInfo.Compare(name, "CELL", (CompareOptions) CompareOptions.IgnoreCase) == 0)
            {
                this.WriteCELLFunction(writer, token, tokenClass, linkTable, ref extraDataLength, ptgType);
                return;
            }
            if (CultureInfo.InvariantCulture.CompareInfo.Compare(name, "INFO", (CompareOptions) CompareOptions.IgnoreCase) == 0)
            {
                this.WriteSpecialAttributeFunction(writer, token, tokenClass, linkTable, ref extraDataLength, ptgType);
                return;
            }
            if (CultureInfo.InvariantCulture.CompareInfo.Compare(name, "OFFSET", (CompareOptions) CompareOptions.IgnoreCase) == 0)
            {
                this.WriteSpecialAttributeFunction(writer, token, tokenClass, linkTable, ref extraDataLength, ptgType);
                return;
            }
            if (_excel2007Functions.Contains(name))
            {
                this.WriteExcel2007PrefixFunction(writer, "_xlfn." + name, tokenClass, linkTable, ref extraDataLength, args, ref nameIndex);
                goto Label_0530;
            }
            int num9 = 0;
            if (linkTable.CustomOrFuctionNames.Contains(name))
            {
                goto Label_03EE;
            }
            linkTable.CustomOrFuctionNames.Add(name);
            ExcelExternSheet sheet = new ExcelExternSheet {
                beginSheetIndex = -2,
                endSheetIndex = -2
            };
            int num10 = 0;
            using (List<ExcelSupBook>.Enumerator enumerator = linkTable.SupBooks.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.IsAddInReferencedSupBook)
                    {
                        sheet.supBookIndex = num10;
                        goto Label_03E3;
                    }
                    num10++;
                }
            }
        Label_03E3:
            num9 = linkTable.GetExcelExternSheetIndex(sheet);
        Label_03EE:
            if (!externalFunctionList.Contains(name))
            {
                if (token.TokenType == TokenType.UndefinedFunction)
                {
                    nameIndex = linkTable.GetCustomOrFunctionNameIndex(name);
                }
                else
                {
                    for (int n = 0; n < args.Length; n++)
                    {
                        this.WriteParsedToken(writer, args[n], tokenClass, linkTable, ref extraDataLength, false, Ptg.None);
                    }
                    this.WriteFunctionImp(writer, token, this.GetXlf(token.Token), args.Length, tokenClass, ref extraDataLength, ptgType);
                    return;
                }
                goto Label_04D3;
            }
            nameIndex = linkTable.GetCustomOrFunctionNameIndex(name);
            ExcelExternSheet sheet2 = new ExcelExternSheet {
                beginSheetIndex = -2,
                endSheetIndex = -2
            };
            int num11 = 0;
            using (List<ExcelSupBook>.Enumerator enumerator2 = linkTable.SupBooks.GetEnumerator())
            {
                while (enumerator2.MoveNext())
                {
                    if (enumerator2.Current.IsAddInReferencedSupBook)
                    {
                        sheet2.supBookIndex = num11;
                        goto Label_046D;
                    }
                    num11++;
                }
            }
        Label_046D:
            num9 = linkTable.GetExcelExternSheetIndex(sheet2);
        Label_04D3:
            writer.Write((byte) 0x39);
            writer.Write((short) ((short) num9));
            writer.Write((short) (nameIndex + 1));
            writer.Write((short) 0);
            for (int i = 0; i < args.Length; i++)
            {
                Ptg none = Ptg.None;
                if (args[i].TokenType == TokenType.Array)
                {
                    none = Ptg.ArrayA;
                }
                this.WriteArgument(writer, args[i], tokenClass, linkTable, ref extraDataLength, none);
            }
        Label_0530:
            this.WriteFunctionImp(writer, token, Xlf.ExternCustom, length + 1, tokenClass, ref extraDataLength, ptgType);
            tokenClass = class2;
        }

        internal string ReadFormulaString(BinaryReader rdr, int charCountSize)
        {
            short charCount = 0;
            byte num2 = 0;
            bool highByte = false;
            bool flag2 = false;
            short num3 = 0;
            int num4 = 0;
            int count = 0;
            string str = null;
            if (charCountSize == 1)
            {
                charCount = rdr.ReadByte();
            }
            else
            {
                charCount = rdr.ReadInt16();
            }
            num2 = rdr.ReadByte();
            highByte = (num2 & 1) == 1;
            flag2 = (num2 & 4) == 4;
            if ((num2 & 8) == 8)
            {
                num3 = rdr.ReadInt16();
            }
            if (flag2)
            {
                num4 = rdr.ReadInt32();
            }
            str = SimpleBinaryReader.BytesToString(rdr.ReadBytes(highByte ? (charCount * 2) : charCount), charCount, highByte);
            count = (num3 * 4) + num4;
            if (count > 0)
            {
                rdr.ReadBytes(count);
            }
            if (str == null)
            {
                return "";
            }
            return ("\"" + str.Replace("\"", "\"\"") + "\"");
        }

        internal bool ReadFuncV(List<object> stack)
        {
            short num = this.formulaReader.ReadInt16();
            int argCount = GetArgCount((Xlf) num);
            if (argCount > 0)
            {
                this.BuildInfixFn(stack, _xlfToString[(Xlf) num], argCount);
            }
            else
            {
                switch (((Xlf) num))
                {
                    case Xlf.True:
                        stack.Add("TRUE()");
                        goto Label_0100;

                    case Xlf.False:
                        stack.Add("FALSE()");
                        goto Label_0100;

                    case Xlf.Rand:
                        stack.Add("RAND()");
                        goto Label_0100;

                    case Xlf.Na:
                        stack.Add("NA()");
                        goto Label_0100;

                    case Xlf.Pi:
                        stack.Add("PI()");
                        goto Label_0100;

                    case Xlf.Now:
                        stack.Add("NOW()");
                        goto Label_0100;

                    case Xlf.Echo:
                        goto Label_0100;

                    case Xlf.Today:
                        stack.Add("TODAY()");
                        goto Label_0100;

                    case Xlf.Numberstring:
                        this.BuildUnsupportedFunction(stack, Xlf.Text, 2);
                        goto Label_0100;
                }
                this.BuildUnsupportedFunction(stack, (Xlf) num, stack.Count);
            }
        Label_0100:
            return true;
        }

        internal bool ReadFuncVar(List<object> stack, LinkTable linkTable)
        {
            byte num = this.formulaReader.ReadByte();
            int num2 = this.formulaReader.ReadInt16();
            int argCount = num & 0x7f;
            int num4 = num2 & 0x7fff;
            if (num4 == 0xff)
            {
                int num5 = num - 1;
                if (stack.Count <= num5)
                {
                    return false;
                }
                StringBuilder builder = new StringBuilder();
                builder.Append(stack[(stack.Count - num5) - 1]);
                builder.Append("(");
                for (int i = stack.Count - num5; i < stack.Count; i++)
                {
                    builder.Append(stack[i]);
                    if (i != (stack.Count - 1))
                    {
                        builder.Append(",");
                    }
                }
                builder.Append(")");
                for (int j = 0; j < num; j++)
                {
                    stack.RemoveAt(stack.Count - 1);
                }
                stack.Add(builder.ToString());
                return true;
            }
            switch (((Xlf) num4))
            {
                case Xlf.If:
                case Xlf.Sum:
                case Xlf.Average:
                case Xlf.Min:
                case Xlf.Max:
                case Xlf.Npv:
                case Xlf.Stdev:
                case Xlf.Dollar:
                case Xlf.Fixed:
                case Xlf.Index:
                case Xlf.And:
                case Xlf.Or:
                case Xlf.Var:
                case Xlf.Pv:
                case Xlf.Fv:
                case Xlf.Nper:
                case Xlf.Pmt:
                case Xlf.Rate:
                case Xlf.Irr:
                case Xlf.Weekday:
                case Xlf.Choose:
                case Xlf.Left:
                case Xlf.Right:
                case Xlf.Substitute:
                case Xlf.Log:
                case Xlf.Find:
                case Xlf.Ddb:
                case Xlf.Ipmt:
                case Xlf.Ppmt:
                case Xlf.Product:
                case Xlf.Stdevp:
                case Xlf.Varp:
                case Xlf.Trunc:
                case Xlf.Days360:
                case Xlf.Vdb:
                case Xlf.Rank:
                case Xlf.Median:
                case Xlf.Sumproduct:
                case Xlf.Avedev:
                case Xlf.Betadist:
                case Xlf.Betainv:
                case Xlf.Db:
                case Xlf.Prob:
                case Xlf.Devsq:
                case Xlf.Geomean:
                case Xlf.Harmean:
                case Xlf.Sumsq:
                case Xlf.Kurt:
                case Xlf.Skew:
                case Xlf.Ztest:
                case Xlf.Percentrank:
                case Xlf.Mode:
                case Xlf.Concatenate:
                    this.BuildInfixFn(stack, _xlfToString[(Xlf) num4], argCount);
                    break;

                default:
                    this.BuildUnsupportedFunction(stack, (Xlf) num4, argCount);
                    break;
            }
            return true;
        }

        internal byte[] ToExcelParsedFormula(short sheetIndex, string formula, LinkTable linkTable, ref int extraDataLength, bool isDefinedNameFormula = false)
        {
            this.isDefinedNameFormula = isDefinedNameFormula;
            this.sheet = sheetIndex;
            if (isDefinedNameFormula)
            {
                Parser.selections = this.SheetsSelectionList;
                Parser.sheetNames = this.sheetNames;
            }
            ParsedToken token = Parser.Parse(formula, this.row, this.column, !this.isA1RefStyle, linkTable);
            try
            {
                if (isDefinedNameFormula)
                {
                    Parser.selections = null;
                    Parser.sheetNames = null;
                }
                if (token != null)
                {
                    this.extraBuff = new MemoryStream();
                    this.extraWriter = new BinaryWriter((Stream) this.extraBuff);
                    MemoryStream stream = new MemoryStream();
                    BinaryWriter bw = new BinaryWriter((Stream) stream);
                    if (isDefinedNameFormula || this._isDataValidationFormula)
                    {
                        this.WriteParsedToken(bw, token, TokenClass.Reference, linkTable, ref extraDataLength, false, Ptg.None);
                    }
                    else
                    {
                        this.WriteParsedToken(bw, token, TokenClass.Value, linkTable, ref extraDataLength, false, Ptg.None);
                    }
                    extraDataLength = (int) this.extraBuff.Length;
                    bw.Write(this.extraBuff.GetBuffer(), 0, (int) this.extraBuff.Length);
                    bw.Dispose();
                    this.extraWriter.Dispose();
                    return stream.ToArray();
                }
            }
            catch
            {
                throw new ExcelException(formula, ExcelExceptionCode.ParseException);
            }
            return new byte[0];
        }

        public string ToString(LinkTable linkTable, ref ExcelCalcError error, int? rowBase = new int?(), int? columnBase = new int?(), bool isDefinedName = false)
        {
            byte num = 0;
            string str = null;
            List<object> stack = new List<object>();
            bool flag2 = false;
            long length = this.formulaReader.BaseStream.Length;
            this.formulaReader.BaseStream.Seek(0L, (SeekOrigin) SeekOrigin.Begin);
            try
            {
                while (!flag2)
                {
                    double num2;
                    uint num3;
                    int num7;
                    int num9;
                    bool flag3;
                    bool flag4;
                    ExcelExternSheet sheet;
                    SimpleBinaryReader reader2;
                    bool flag5;
                    bool flag6;
                    int num12;
                    int num13;
                    ushort num14;
                    int num16;
                    int activeSheet;
                    StringBuilder builder2;
                    ExcelExternSheet sheet2;
                    ExcelExternSheet sheet3;
                    StringBuilder builder3;
                    bool flag7;
                    bool flag8;
                    bool flag9;
                    int num26;
                    int num30;
                    int num31;
                    bool flag11;
                    bool flag12;
                    int num34;
                    bool flag13;
                    bool flag14;
                    int num35;
                    bool flag15;
                    bool flag16;
                    int num36;
                    ushort num37;
                    ushort num38;
                    bool flag17;
                    bool flag18;
                    bool flag19;
                    bool flag20;
                    ushort num42;
                    int num43;
                    int num44;
                    bool flag21;
                    short num48;
                    short num49;
                    BinaryReader reader4;
                    ExcelCalcError error2;
                    ExcelExternSheet sheet5;
                    string str17;
                    IName name;
                    string fileName;
                    byte num56;
                    int num57;
                    int num58;
                    int num59;
                    string str19;
                    byte num64;
                    if (this.formulaReader.BaseStream.Position == length)
                    {
                        if (stack.Count > 0)
                        {
                            str = (string) ((string) stack[0]);
                        }
                        goto Label_216B;
                    }
                    num = this.formulaReader.ReadByte();
                    switch (((Ptg) num))
                    {
                        case Ptg.Exp:
                        {
                            num48 = this.formulaReader.ReadInt16();
                            num49 = this.formulaReader.ReadInt16();
                            byte[] formula = null;
                            byte[] extra = null;
                            if ((linkTable.sharedFormulaList == null) || !linkTable.sharedFormulaList.GetSharedFormula(this.sheet, num48, num49, ref formula, ref extra))
                            {
                                goto Label_18AD;
                            }
                            MemoryStream stream3 = new MemoryStream(formula);
                            reader4 = new BinaryReader((Stream) stream3);
                            FormulaProcess process3 = new FormulaProcess(reader4, null, this.sheet, this.row, this.column, this.isA1RefStyle);
                            error2 = null;
                            process3.HasExternNames = this.hasExternNames;
                            int? nullable = null;
                            nullable = null;
                            string str16 = process3.ToString(linkTable, ref error2, nullable, nullable, false);
                            if (str16 == null)
                            {
                                goto Label_189A;
                            }
                            stack.Add(str16);
                            goto Label_18A1;
                        }
                        case Ptg.Add:
                        {
                            this.BuildInfix(stack, "+");
                            continue;
                        }
                        case Ptg.Sub:
                        {
                            this.BuildInfix(stack, "-");
                            continue;
                        }
                        case Ptg.Mul:
                        {
                            this.BuildInfix(stack, "*");
                            continue;
                        }
                        case Ptg.Div:
                        {
                            this.BuildInfix(stack, "/");
                            continue;
                        }
                        case Ptg.Power:
                        {
                            this.BuildInfix(stack, "^");
                            continue;
                        }
                        case Ptg.Concat:
                        {
                            this.BuildInfix(stack, "&");
                            continue;
                        }
                        case Ptg.LT:
                        {
                            this.BuildInfix(stack, "<");
                            continue;
                        }
                        case Ptg.LE:
                        {
                            this.BuildInfix(stack, "<=");
                            continue;
                        }
                        case Ptg.EQ:
                        {
                            this.BuildInfix(stack, "=");
                            continue;
                        }
                        case Ptg.GE:
                        {
                            this.BuildInfix(stack, ">=");
                            continue;
                        }
                        case Ptg.GT:
                        {
                            this.BuildInfix(stack, ">");
                            continue;
                        }
                        case Ptg.NE:
                        {
                            this.BuildInfix(stack, "<>");
                            continue;
                        }
                        case Ptg.Isect:
                        {
                            flag2 = true;
                            continue;
                        }
                        case Ptg.Union:
                        {
                            this.BuildInfix(stack, ",");
                            continue;
                        }
                        case Ptg.Range:
                        {
                            this.BuildInfix(stack, ":");
                            continue;
                        }
                        case Ptg.Uplus:
                        {
                            this.BuildInfixFn(stack, "+", 1);
                            continue;
                        }
                        case Ptg.Uminus:
                        {
                            this.BuildInfixFn(stack, "-", 1);
                            continue;
                        }
                        case Ptg.Percent:
                        {
                            this.BuildInfixFn(stack, "%", 1);
                            continue;
                        }
                        case Ptg.Paren:
                        {
                            this.BuildInfixFn(stack, "()", 1);
                            continue;
                        }
                        case Ptg.MissArg:
                        {
                            stack.Add("MissArg");
                            continue;
                        }
                        case Ptg.Str:
                        {
                            string str2 = this.ReadFormulaString(this.formulaReader, 1);
                            stack.Add(str2);
                            continue;
                        }
                        case Ptg.Attr:
                        {
                            byte num25 = this.formulaReader.ReadByte();
                            flag7 = (num25 & 2) == 2;
                            flag8 = (num25 & 4) == 4;
                            flag9 = (num25 & 0x10) == 0x10;
                            if ((num25 & 0x40) != 0x40)
                            {
                                goto Label_0F1C;
                            }
                            this.formulaReader.ReadByte();
                            this.formulaReader.ReadByte();
                            continue;
                        }
                        case Ptg.Err:
                        {
                            num64 = this.formulaReader.ReadByte();
                            if (num64 > 15)
                            {
                                break;
                            }
                            switch (num64)
                            {
                                case 0:
                                    goto Label_0376;

                                case 7:
                                    goto Label_0386;

                                case 15:
                                    goto Label_0396;
                            }
                            continue;
                        }
                        case Ptg.Bool:
                        {
                            bool flag = this.formulaReader.ReadBoolean();
                            stack.Add(Convert.ToString(flag, (IFormatProvider) CultureInfo.InvariantCulture));
                            continue;
                        }
                        case Ptg.Int:
                        {
                            num3 = this.formulaReader.ReadUInt16();
                            stack.Add(((uint) num3).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            continue;
                        }
                        case Ptg.Num:
                        {
                            NumberFormatInfo info1 = (NumberFormatInfo) NumberFormatInfo.CurrentInfo.Clone();
                            num2 = this.formulaReader.ReadDouble();
                            stack.Add(((double) num2).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            continue;
                        }
                        case Ptg.FuncVar:
                        case Ptg.FuncVarV:
                        case Ptg.FuncVarA:
                        {
                            if (!this.ReadFuncVar(stack, linkTable))
                            {
                                flag2 = true;
                            }
                            continue;
                        }
                        case Ptg.Name:
                        case Ptg.NameV:
                        case Ptg.NameA:
                        {
                            this.definedNameIndex = this.formulaReader.ReadUInt16() - 1;
                            this.formulaReader.ReadBytes(2);
                            if (linkTable.DefinedNames.Count <= this.definedNameIndex)
                            {
                                return ExcelCalcError.WrongFunctionOrRangeName.ToString();
                            }
                            stack.Add(linkTable.DefinedNames[this.definedNameIndex].Item1);
                            continue;
                        }
                        case Ptg.Ref:
                        case Ptg.RefN:
                        case Ptg.RefV:
                        case Ptg.RefNV:
                        case Ptg.RefA:
                        case Ptg.RefNA:
                        {
                            num7 = this.formulaReader.ReadUInt16();
                            int num8 = this.formulaReader.ReadUInt16();
                            num9 = num8 & 0x3fff;
                            flag3 = (num8 & 0x8000) == 0x8000;
                            flag4 = (num8 & 0x4000) == 0x4000;
                            if (((num != 0x2c) && (num != 0x4c)) && (num != 0x6c))
                            {
                                goto Label_06B2;
                            }
                            byte num10 = (byte) num9;
                            if (flag4)
                            {
                                num10 += (byte) this.column;
                                num9 = num10;
                                if (num9 <= 0xff)
                                {
                                    goto Label_0670;
                                }
                                num9 -= 0x100;
                            }
                            goto Label_067F;
                        }
                        case Ptg.Area:
                        case Ptg.AreaN:
                        case Ptg.AreaV:
                        case Ptg.AreaNV:
                        case Ptg.AreaA:
                        case Ptg.AreaNA:
                        {
                            num30 = this.formulaReader.ReadUInt16();
                            num31 = this.formulaReader.ReadUInt16();
                            int num32 = this.formulaReader.ReadInt16();
                            int num33 = this.formulaReader.ReadInt16();
                            flag11 = (num32 & 0x8000) == 0x8000;
                            flag12 = (num32 & 0x4000) == 0x4000;
                            num34 = num32 & 0x3fff;
                            flag13 = (num33 & 0x8000) == 0x8000;
                            flag14 = (num33 & 0x4000) == 0x4000;
                            num35 = num33 & 0x3fff;
                            flag15 = false;
                            flag16 = false;
                            if (((num != 0x2d) && (num != 0x4d)) && (num != 0x6d))
                            {
                                goto Label_1218;
                            }
                            if (flag11)
                            {
                                num30 += this.row;
                                if (num30 <= 0xffff)
                                {
                                    goto Label_1164;
                                }
                                num30 -= 0x10000;
                            }
                            goto Label_1173;
                        }
                        case Ptg.MemArea:
                        case Ptg.MemAreaV:
                        case Ptg.MemAreaA:
                            throw new NotSupportedException("Excel IE didn't support sub expression at the version");

                        case Ptg.MemErr:
                        case Ptg.MemErrV:
                        case Ptg.MemErrA:
                        {
                            num3 = this.formulaReader.ReadUInt32();
                            num3 = this.formulaReader.ReadUInt16();
                            stack.Add("#REF!");
                            continue;
                        }
                        case Ptg.MemFunc:
                        case Ptg.MemFuncV:
                        {
                            ushort count = this.formulaReader.ReadUInt16();
                            MemoryStream stream = new MemoryStream(this.formulaReader.ReadBytes(count));
                            BinaryReader r = new BinaryReader((Stream) stream);
                            string str3 = new FormulaProcess(r, null, this.sheet, this.row, this.column, this.isA1RefStyle) { isDefinedNameFormula = this.isDefinedNameFormula, HasExternNames = this.hasExternNames, SheetsSelectionList = this.SheetsSelectionList }.ToString(linkTable, ref error, rowBase, columnBase, isDefinedName);
                            if (str3 != null)
                            {
                                stack.Add(str3);
                            }
                            r.Close();
                            continue;
                        }
                        case Ptg.RefErr:
                        case Ptg.RefErrV:
                        case Ptg.RefErrA:
                        {
                            this.formulaReader.ReadBytes(4);
                            stack.Add("#REF!");
                            continue;
                        }
                        case Ptg.AreaErr:
                        case Ptg.AreaErrV:
                        case Ptg.AreaErrA:
                        {
                            num2 = this.formulaReader.ReadDouble();
                            stack.Add("#REF!");
                            continue;
                        }
                        case Ptg.NameX:
                        case Ptg.NameXV:
                        case Ptg.NameXA:
                        {
                            this.externSheetIndex = this.formulaReader.ReadUInt16();
                            this.customNameIndex = this.formulaReader.ReadUInt16() - 1;
                            this.formulaReader.ReadBytes(2);
                            sheet5 = linkTable.ExternalSheets[this.externSheetIndex];
                            str17 = "";
                            if ((sheet5 == null) || string.IsNullOrWhiteSpace(sheet5.fileName))
                            {
                                goto Label_1C08;
                            }
                            if (!linkTable.ExternalNamedCellRanges.ContainsKey(sheet5.fileName) || (linkTable.ExternalNamedCellRanges[sheet5.fileName].Count <= this.customNameIndex))
                            {
                                goto Label_1EF5;
                            }
                            name = linkTable.ExternalNamedCellRanges[sheet5.fileName][this.customNameIndex];
                            fileName = sheet5.fileName;
                            if (sheet5.beginSheetName == null)
                            {
                                goto Label_1AE0;
                            }
                            int num50 = fileName.LastIndexOf('\\');
                            if (num50 == -1)
                            {
                                goto Label_1A79;
                            }
                            fileName = string.Concat((object[]) new object[] { fileName.Substring(0, num50), ((char) '\\'), "[", fileName.Substring(num50 + 1), "]" });
                            goto Label_1A95;
                        }
                        case Ptg.Ref3d:
                        case Ptg.Ref3dV:
                        case Ptg.Ref3dA:
                        {
                            int num11 = this.formulaReader.ReadInt16();
                            sheet = linkTable.ExternalSheets[num11];
                            reader2 = new SimpleBinaryReader(this.formulaReader.ReadBytes(4));
                            num12 = 0;
                            num13 = 0;
                            if (!isDefinedName)
                            {
                                goto Label_087B;
                            }
                            num14 = reader2.ReadUInt16();
                            short num15 = reader2.ReadInt16();
                            num16 = num15 & 0x3fff;
                            flag5 = (num15 & 0x8000) == 0x8000;
                            flag6 = (num15 & 0x4000) == 0x4000;
                            if (flag5)
                            {
                                goto Label_0791;
                            }
                            num12 = num14;
                            goto Label_07FA;
                        }
                        case Ptg.Area3d:
                        case Ptg.Area3dV:
                        case Ptg.Area3dA:
                        {
                            num36 = this.formulaReader.ReadInt16();
                            num37 = this.formulaReader.ReadUInt16();
                            num38 = this.formulaReader.ReadUInt16();
                            ushort num39 = this.formulaReader.ReadUInt16();
                            ushort num40 = this.formulaReader.ReadUInt16();
                            flag17 = (num39 & 0x8000) == 0x8000;
                            flag18 = (num40 & 0x8000) == 0x8000;
                            flag19 = (num39 & 0x4000) == 0x4000;
                            flag20 = (num40 & 0x4000) == 0x4000;
                            ushort num41 = (ushort)(num39 & 0x3ff);
                            num42 = (ushort)(num40 & 0x3ff);
                            num43 = num41;
                            num44 = num42;
                            if (!this.isDefinedNameFormula)
                            {
                                goto Label_142B;
                            }
                            if (flag19)
                            {
                                num43 = (short) num41;
                                num43 += this.column;
                                if (num43 <= 0xff)
                                {
                                    goto Label_13E4;
                                }
                                num43 -= 0x100;
                            }
                            goto Label_13F3;
                        }
                        case Ptg.RefErr3d:
                        case Ptg.RefErr3dV:
                        {
                            int num24 = this.formulaReader.ReadInt16();
                            this.formulaReader.ReadInt16();
                            this.formulaReader.ReadInt16();
                            sheet3 = linkTable.ExternalSheets[num24];
                            builder3 = new StringBuilder();
                            if ((sheet3 != null) && (sheet3.beginSheetName != null))
                            {
                                if (!(sheet3.beginSheetName == sheet3.endSheetName))
                                {
                                    goto Label_0E0B;
                                }
                                builder3.Append(this.HandleName(sheet3.beginSheetName));
                            }
                            goto Label_0E42;
                        }
                        case Ptg.AreaErr3d:
                        case Ptg.AreaErr3dV:
                        case Ptg.AreaErr3dA:
                        {
                            int num21 = this.formulaReader.ReadInt16();
                            this.formulaReader.ReadInt16();
                            this.formulaReader.ReadInt16();
                            int num22 = this.formulaReader.ReadInt16();
                            int num23 = this.formulaReader.ReadInt16();
                            builder2 = new StringBuilder();
                            sheet2 = linkTable.ExternalSheets[num21];
                            if ((sheet2 != null) && (sheet2.beginSheetName != null))
                            {
                                if (!(sheet2.beginSheetName == sheet2.endSheetName))
                                {
                                    goto Label_0D24;
                                }
                                builder2.Append(this.HandleName(sheet2.beginSheetName));
                            }
                            goto Label_0D5B;
                        }
                        case Ptg.ArrayV:
                        case Ptg.ArrayA:
                            this.formulaReader.ReadBytes(7);
                            if (this.formulaExtraReader == null)
                            {
                                continue;
                            }
                            num56 = this.formulaExtraReader.ReadByte();
                            num57 = this.formulaExtraReader.ReadUInt16();
                            str19 = (string) new string('{', 1);
                            num56++;
                            num57++;
                            if (num56 != 0)
                            {
                                goto Label_1F58;
                            }
                            num58 = 0x100;
                            goto Label_1F5C;

                        case Ptg.FuncV:
                        case Ptg.FuncA:
                        {
                            if (!this.ReadFuncV(stack))
                            {
                                flag2 = true;
                            }
                            continue;
                        }
                        default:
                            goto Label_2115;
                    }
                    switch (num64)
                    {
                        case 0x17:
                            goto Label_03A6;

                        case 0x1d:
                            goto Label_03B6;

                        case 0x24:
                            goto Label_03C6;

                        case 0x2a:
                            goto Label_03D6;

                        default:
                        {
                            continue;
                        }
                    }
                Label_0376:
                    stack.Add("#NULL!");
                    continue;
                Label_0386:
                    stack.Add("#DIV/0!");
                    continue;
                Label_0396:
                    stack.Add("#VALUE!");
                    continue;
                Label_03A6:
                    stack.Add("#REF!");
                    continue;
                Label_03B6:
                    stack.Add("#NAME?");
                    continue;
                Label_03C6:
                    stack.Add("#NUM!");
                    continue;
                Label_03D6:
                    stack.Add("#N/A");
                    continue;
                Label_0670:
                    if (num9 < 0)
                    {
                        num9 += 0x100;
                    }
                Label_067F:
                    if (flag3)
                    {
                        num7 += this.row;
                        if (num7 > 0xffff)
                        {
                            num7 -= 0x10000;
                        }
                        else if (num7 < 0)
                        {
                            num7 += 0x10000;
                        }
                    }
                Label_06B2:
                    if (!this.isA1RefStyle)
                    {
                        if (flag3)
                        {
                            num7 -= this.row;
                        }
                        if (flag4)
                        {
                            num9 -= this.column;
                        }
                    }
                    string str4 = this.GetCoord(num7, num9, flag3, flag4, this.isA1RefStyle, false, false, 0, 0);
                    stack.Add(str4);
                    continue;
                Label_0791:
                    activeSheet = sheet.beginSheetIndex;
                    if ((activeSheet < 0) && (this.activeSheet != -1))
                    {
                        activeSheet = this.activeSheet;
                    }
                    if (this.SheetsSelectionList.Count > activeSheet)
                    {
                        num12 = num14 + this.SheetsSelectionList[activeSheet].Item1;
                    }
                    if (num12 > 0xffff)
                    {
                        num12 -= 0x10000;
                    }
                    else if (num12 < 0)
                    {
                        num12 += 0x10000;
                    }
                Label_07FA:
                    if (!flag6)
                    {
                        num13 = num16;
                    }
                    else
                    {
                        int beginSheetIndex = sheet.beginSheetIndex;
                        if ((beginSheetIndex < 0) && (this.activeSheet != -1))
                        {
                            beginSheetIndex = this.activeSheet;
                        }
                        if (this.SheetsSelectionList.Count > beginSheetIndex)
                        {
                            num13 = num16 + this.SheetsSelectionList[beginSheetIndex].Item2;
                        }
                        if (num13 > 0xff)
                        {
                            num13 -= 0x100;
                        }
                        else if (num13 < 0)
                        {
                            num13 += 0x100;
                        }
                    }
                    goto Label_0A13;
                Label_087B:
                    reader2.ReadInt16();
                    int num19 = reader2.ReadInt16();
                    flag5 = (num19 & 0x8000) == 0x8000;
                    flag6 = (num19 & 0x4000) == 0x4000;
                    reader2.Seek(-4, (SeekOrigin) SeekOrigin.Current);
                    if (!flag5)
                    {
                        num12 = reader2.ReadUInt16();
                    }
                    else
                    {
                        num12 = reader2.ReadInt16();
                        if ((!rowBase.HasValue && (sheet.beginSheetIndex >= 0)) && ((this.SheetsSelectionList != null) && (this.SheetsSelectionList.Count > sheet.beginSheetIndex)))
                        {
                            if (sheet.beginSheetIndex != -2)
                            {
                                num12 += this.SheetsSelectionList[sheet.beginSheetIndex].Item1;
                            }
                            else if ((this.activeSheet != -1) && (this.SheetsSelectionList.Count > this.activeSheet))
                            {
                                num12 += this.SheetsSelectionList[this.activeSheet].Item1;
                            }
                        }
                    }
                    if (!flag6)
                    {
                        num13 = reader2.ReadByte();
                    }
                    else
                    {
                        num13 = (sbyte) reader2.ReadByte();
                        if ((!columnBase.HasValue && (sheet.beginSheetIndex >= 0)) && ((this.SheetsSelectionList != null) && (this.SheetsSelectionList.Count > sheet.beginSheetIndex)))
                        {
                            if (sheet.beginSheetIndex != -2)
                            {
                                num13 += this.SheetsSelectionList[sheet.beginSheetIndex].Item2;
                            }
                            else if ((this.activeSheet != -1) && (this.SheetsSelectionList.Count > this.activeSheet))
                            {
                                num13 += this.SheetsSelectionList[this.activeSheet].Item2;
                            }
                        }
                    }
                Label_0A13:
                    if (((num12 < 0) || (num12 > 0x10000)) || ((num13 < 0) || (num13 > 0x100)))
                    {
                        stack.Add("#REF!");
                    }
                    else
                    {
                        if (!this.isA1RefStyle)
                        {
                            if (flag5)
                            {
                                num12 -= (short) this.row;
                            }
                            if (flag6)
                            {
                                num13 -= (short) this.column;
                            }
                        }
                        string str5 = this.GetCoord(num12, num13, flag5, flag6, this.isA1RefStyle, false, false, 0, 0);
                        StringBuilder builder = new StringBuilder();
                        if ((sheet.fileName == null) || (sheet.fileName.Length == 0))
                        {
                            builder.Append("'");
                            string beginSheetName = sheet.beginSheetName;
                            this.PreProcessWorksheetName(sheet.beginSheetName, out beginSheetName);
                            builder.Append(beginSheetName);
                            if (sheet.endSheetName != sheet.beginSheetName)
                            {
                                builder.Append(":");
                                builder.Append(sheet.endSheetName);
                            }
                            builder.Append("'!");
                            builder.Append(str5);
                            string str7 = builder.ToString();
                            if (str7.StartsWith("''!"))
                            {
                                str7 = str7.Substring(2);
                            }
                            stack.Add(str7);
                        }
                        else
                        {
                            builder.Append('\'');
                            int num20 = sheet.fileName.LastIndexOf('\\');
                            if (num20 != -1)
                            {
                                builder.Append(sheet.fileName.Substring(0, num20 + 1));
                                builder.Append("[");
                                builder.Append(sheet.fileName.Substring(num20 + 1));
                                builder.Append("]");
                            }
                            else
                            {
                                builder.Append("[");
                                builder.Append(sheet.fileName);
                                builder.Append("]");
                            }
                            builder.Append(sheet.beginSheetName);
                            if (sheet.endSheetName != sheet.beginSheetName)
                            {
                                builder.Append(":");
                                builder.Append(sheet.endSheetName);
                            }
                            builder.Append("'!");
                            builder.Append(str5);
                            stack.Add(builder.ToString());
                        }
                    }
                    continue;
                Label_0D24:
                    builder2.Append(this.HandleName(sheet2.beginSheetName));
                    builder2.Append(":");
                    builder2.Append(this.HandleName(sheet2.endSheetName));
                Label_0D5B:
                    builder2.Append("!#REF!");
                    stack.Add(builder2.ToString());
                    continue;
                Label_0E0B:
                    builder3.Append(this.HandleName(sheet3.beginSheetName));
                    builder3.Append(":");
                    builder3.Append(this.HandleName(sheet3.endSheetName));
                Label_0E42:
                    if (stack.Count > 0)
                    {
                        if (stack[stack.Count - 1].ToString() == "#REF!")
                        {
                            stack.RemoveAt(stack.Count - 1);
                        }
                        string str8 = builder3.ToString();
                        stack.Add(string.Format("{0}!#REF!", (object[]) new object[] { str8 }));
                    }
                    else
                    {
                        stack.Add(builder3.ToString() + "!#REF!");
                    }
                    continue;
                Label_0F1C:
                    num26 = this.formulaReader.ReadInt16();
                    if (flag7)
                    {
                        this.ParseIFParams(stack, num26, linkTable, ref error, isDefinedName);
                    }
                    else if (flag9)
                    {
                        this.BuildInfixFn(stack, "SUM", 1);
                    }
                    else if (flag8)
                    {
                        int index = num26;
                        int[] numArray = new int[index + 1];
                        long position = this.formulaReader.BaseStream.Position;
                        int num29 = 0;
                        while (num29 < (index + 1))
                        {
                            numArray[num29] = this.formulaReader.ReadInt16();
                            num29++;
                        }
                        for (num29 = 0; num29 < index; num29++)
                        {
                            MemoryStream stream2 = new MemoryStream(this.formulaReader.ReadBytes(numArray[num29 + 1] - numArray[num29]));
                            BinaryReader reader3 = new BinaryReader((Stream) stream2);
                            string str10 = new FormulaProcess(reader3, this.formulaExtraReader, this.sheet, this.row, this.column, this.isA1RefStyle) { SheetsSelectionList = this.SheetsSelectionList, HasExternNames = this.hasExternNames }.ToString(linkTable, ref error, rowBase, columnBase, isDefinedName);
                            stack.Add(str10);
                            reader3.Close();
                        }
                        this.formulaReader.BaseStream.Seek(position + numArray[index], (SeekOrigin) SeekOrigin.Begin);
                        num = this.formulaReader.ReadByte();
                        switch (num)
                        {
                            case 0x42:
                            case 0x22:
                            {
                                this.ReadFuncVar(stack, linkTable);
                                continue;
                            }
                        }
                        if (num == 0x41)
                        {
                            this.ReadFuncV(stack);
                        }
                        else
                        {
                            flag2 = true;
                        }
                    }
                    continue;
                Label_1164:
                    if (num30 < 0)
                    {
                        num30 += 0x10000;
                    }
                Label_1173:
                    if (flag13)
                    {
                        num31 += this.row;
                        if (num31 > 0xffff)
                        {
                            num31 -= 0x10000;
                        }
                        else if (num31 < 0)
                        {
                            num31 += 0x10000;
                        }
                    }
                    if (flag12)
                    {
                        num34 = (num34 + this.column) & 0xff;
                        if (num34 > 0xff)
                        {
                            num34 -= 0x100;
                        }
                        else if (num34 < 0)
                        {
                            num34 += 0x100;
                        }
                    }
                    if (flag14)
                    {
                        num35 = (num35 + this.column) & 0xff;
                        if (num35 > 0xff)
                        {
                            num35 -= 0x100;
                        }
                        else if (num35 < 0)
                        {
                            num35 += 0x100;
                        }
                    }
                Label_1218:
                    if ((num30 == 0) && ((num31 == -1) || (num31 == 0xffff)))
                    {
                        flag16 = true;
                    }
                    if ((num34 == 0) && ((num35 == -1) || (num35 == 0xff)))
                    {
                        flag15 = true;
                    }
                    if (!this.isA1RefStyle)
                    {
                        if (!flag16)
                        {
                            if (flag11)
                            {
                                num30 -= (short) this.row;
                            }
                            if (flag13)
                            {
                                num31 -= (short) this.row;
                            }
                        }
                        if (!flag15)
                        {
                            if (flag12)
                            {
                                num34 -= (short) this.column;
                            }
                            if (flag14)
                            {
                                num35 -= (short) this.column;
                            }
                        }
                    }
                    string str11 = this.GetCoord(num30, num34, flag11, flag12, this.isA1RefStyle, flag15, flag16, 0, 0);
                    string str12 = this.GetCoord(num31, num35, flag13, flag14, this.isA1RefStyle, flag15, flag16, 0, 0);
                    StringBuilder builder4 = new StringBuilder(str11);
                    builder4.Append(":");
                    builder4.Append(str12);
                    stack.Add(builder4.ToString());
                    continue;
                Label_13E4:
                    if (num43 < 0)
                    {
                        num43 += 0x100;
                    }
                Label_13F3:
                    if (flag20)
                    {
                        num44 = (short) num42;
                        num44 += this.column;
                        if (num44 > 0xff)
                        {
                            num44 -= 0x100;
                        }
                        else if (num44 < 0)
                        {
                            num44 += 0x100;
                        }
                    }
                Label_142B:
                    flag21 = false;
                    bool isWholeCol = false;
                    int row = num37;
                    int num46 = num38;
                    if (this.isDefinedNameFormula)
                    {
                        if (flag17)
                        {
                            row += this.row;
                            if (row > 0xffff)
                            {
                                row -= 0x10000;
                            }
                            else if (row < 0)
                            {
                                row += 0x10000;
                            }
                        }
                        if (flag18)
                        {
                            num46 += this.row;
                            if (num46 > 0xffff)
                            {
                                num46 -= 0x10000;
                            }
                            else if (num46 < 0)
                            {
                                num46 += 0x10000;
                            }
                        }
                    }
                    if ((row == 0) && ((num46 == -1) || (num46 == 0xffff)))
                    {
                        isWholeCol = true;
                    }
                    if ((num43 == 0) && ((num44 == -1) || (num44 == 0xff)))
                    {
                        flag21 = true;
                    }
                    if (isWholeCol && flag21)
                    {
                        isWholeCol = flag21 = false;
                    }
                    string str13 = this.GetCoord(row, num43, flag17, flag19, this.isA1RefStyle, flag21, isWholeCol, rowBase, columnBase);
                    string str14 = this.GetCoord(num46, num44, flag18, flag20, this.isA1RefStyle, flag21, isWholeCol, rowBase, columnBase);
                    StringBuilder builder5 = new StringBuilder();
                    ExcelExternSheet sheet4 = linkTable.ExternalSheets[num36];
                    if (string.IsNullOrEmpty(sheet4.fileName))
                    {
                        if (sheet4.beginSheetName == sheet4.endSheetName)
                        {
                            builder5.Append(this.HandleName(sheet4.beginSheetName));
                            builder5.Append("!");
                            builder5.Append(str13);
                            builder5.Append(":");
                            builder5.Append(str14);
                            stack.Add(builder5.ToString());
                        }
                        else
                        {
                            if ((sheet4.beginSheetName != null) && sheet4.beginSheetName.Contains(" "))
                            {
                                builder5.Append('\'');
                                builder5.Append(sheet4.beginSheetName);
                                builder5.Append('\'');
                            }
                            else
                            {
                                builder5.Append(sheet4.beginSheetName);
                            }
                            builder5.Append(":");
                            if ((sheet4.endSheetName != null) && sheet4.endSheetName.Contains(" "))
                            {
                                builder5.Append('\'');
                                builder5.Append(sheet4.endSheetName);
                                builder5.Append('\'');
                            }
                            else
                            {
                                builder5.Append(sheet4.endSheetName);
                            }
                            string str15 = builder5.ToString();
                            builder5.Clear();
                            if (!str15.StartsWith("'") && !str15.EndsWith("'"))
                            {
                                str15 = ((char) '\'') + str15 + ((char) '\'');
                            }
                            builder5.Append(str15);
                            builder5.Append("!");
                            builder5.Append(str13);
                            builder5.Append(":");
                            builder5.Append(str14);
                            stack.Add(builder5.ToString());
                        }
                    }
                    else
                    {
                        builder5.Append('\'');
                        int num47 = sheet4.fileName.LastIndexOf('\\');
                        if (num47 != -1)
                        {
                            builder5.Append(sheet4.fileName.Substring(0, num47 + 1));
                            builder5.Append("[");
                            builder5.Append(sheet4.fileName.Substring(num47 + 1));
                            builder5.Append("]");
                        }
                        else
                        {
                            builder5.Append("[");
                            builder5.Append(sheet4.fileName);
                            builder5.Append("]");
                        }
                        builder5.Append(sheet4.beginSheetName);
                        builder5.Append('\'');
                        builder5.Append("!");
                        builder5.Append(str13);
                        builder5.Append(":");
                        builder5.Append(str14);
                        stack.Add(builder5.ToString());
                    }
                    continue;
                Label_189A:
                    if (error2 != null)
                    {
                        flag2 = true;
                    }
                Label_18A1:
                    reader4.Close();
                    continue;
                Label_18AD:
                    if (linkTable.sharedFormulaList == null)
                    {
                        linkTable.sharedFormulaList = new SharedFormulaList(this.isA1RefStyle ? ExcelReferenceStyle.A1 : ExcelReferenceStyle.R1C1);
                    }
                    linkTable.sharedFormulaList.AddUnfoundFormulaReference(this.sheet, num48, num49);
                    continue;
                Label_1A79:
                    if (!string.IsNullOrWhiteSpace(fileName))
                    {
                        fileName = "[" + fileName + "]";
                    }
                Label_1A95:
                    if (sheet5.beginSheetName != sheet5.endSheetName)
                    {
                        fileName = fileName + sheet5.beginSheetName + ":" + sheet5.endSheetName;
                    }
                    else
                    {
                        fileName = fileName + sheet5.beginSheetName;
                    }
                    goto Label_1BC9;
                Label_1AE0:
                    if (name.Index != -1)
                    {
                        for (int j = 0; j < linkTable.SupBooks.Count; j++)
                        {
                            ExcelSupBook book = linkTable.SupBooks[j];
                            if (((book.FileName == sheet5.fileName) && (book.SheetNames != null)) && (name.Index < book.SheetNames.Count))
                            {
                                int num52 = fileName.LastIndexOf('\\');
                                if (num52 != -1)
                                {
                                    fileName = fileName.Substring(0, num52 + 1) + "[" + fileName.Substring(num52 + 1) + "]";
                                }
                                else if (!string.IsNullOrWhiteSpace(fileName))
                                {
                                    fileName = "[" + fileName + "]";
                                }
                                fileName = fileName + book.SheetNames[name.Index];
                                break;
                            }
                        }
                    }
                Label_1BC9:;
                    str17 = string.Concat((string[]) new string[] { str17, "'", fileName, "'!", name.Name });
                    goto Label_1EF5;
                Label_1C08:
                    if (sheet5 != null)
                    {
                        str17 = str17 + sheet5.fileName;
                        if (sheet5.beginSheetName != null)
                        {
                            int num53 = str17.LastIndexOf('\\');
                            if (num53 != -1)
                            {
                                str17 = string.Concat((object[]) new object[] { str17.Substring(0, num53), ((char) '\\'), "[", str17.Substring(num53 + 1), "]" });
                            }
                            str17 = str17 + sheet5.beginSheetName;
                            if ((sheet5.endSheetName != null) && (sheet5.endSheetName != sheet5.beginSheetName))
                            {
                                str17 = str17 + ":" + sheet5.endSheetName;
                            }
                        }
                    }
                    if (isDefinedName)
                    {
                        if ((linkTable.CustomNames != null) && (this.customNameIndex < linkTable.CustomNames.Count))
                        {
                            object[] objArray = (object[]) linkTable.CustomNames[this.customNameIndex];
                            List<string> internalSheetNames = linkTable.InternalSheetNames;
                            int num54 = int.Parse(objArray[2].ToString());
                            if (((num54 >= 0) && (internalSheetNames != null)) && (internalSheetNames.Count > num54))
                            {
                                str17 = string.Concat((string[]) new string[] { str17, "'", internalSheetNames[num54], "'!", objArray[0].ToString() });
                            }
                            else if (!string.IsNullOrWhiteSpace(str17))
                            {
                                str17 = str17 + "!" + objArray[0].ToString();
                            }
                            else
                            {
                                str17 = objArray[0].ToString();
                            }
                        }
                        else if ((linkTable.CustomOrFuctionNames != null) && (this.customNameIndex < linkTable.CustomOrFuctionNames.Count))
                        {
                            str17 = linkTable.CustomOrFuctionNames[this.customNameIndex];
                        }
                    }
                    else if ((linkTable.CustomOrFuctionNames != null) && (this.customNameIndex < linkTable.CustomOrFuctionNames.Count))
                    {
                        str17 = linkTable.CustomOrFuctionNames[this.customNameIndex];
                    }
                    else if ((linkTable.CustomNames != null) && (this.customNameIndex < linkTable.CustomNames.Count))
                    {
                        object[] objArray2 = (object[]) linkTable.CustomNames[this.customNameIndex];
                        List<string> list3 = linkTable.InternalSheetNames;
                        int num55 = int.Parse(objArray2[2].ToString());
                        if (((num55 >= 0) && (list3 != null)) && (list3.Count > num55))
                        {
                            str17 = string.Concat((string[]) new string[] { str17, "'", list3[num55], "'!", objArray2[0].ToString() });
                        }
                        else if (!string.IsNullOrWhiteSpace(str17))
                        {
                            str17 = str17 + "!" + objArray2[0].ToString();
                        }
                        else
                        {
                            str17 = objArray2[0].ToString();
                        }
                    }
                Label_1EF5:
                    stack.Add(str17);
                    continue;
                Label_1F58:
                    num58 = num56;
                Label_1F5C:
                    num59 = num58 * num57;
                    for (int i = 0; i < num59; i++)
                    {
                        byte num61 = this.formulaExtraReader.ReadByte();
                        switch (num61)
                        {
                            case 1:
                            {
                                double num62 = this.formulaExtraReader.ReadDouble();
                                str19 = str19 + ((double) num62).ToString((IFormatProvider) CultureInfo.InvariantCulture);
                                goto Label_20BF;
                            }
                            case 2:
                            {
                                string str20 = this.ReadFormulaString(this.formulaExtraReader, 2);
                                str19 = str19 + str20;
                                goto Label_20BF;
                            }
                            default:
                                if (num61 != 4)
                                {
                                    goto Label_2013;
                                }
                                switch (this.formulaExtraReader.ReadByte())
                                {
                                    case 0:
                                        str19 = str19 + "false";
                                        goto Label_2001;

                                    case 1:
                                        str19 = str19 + "true";
                                        goto Label_2001;
                                }
                                break;
                        }
                    Label_2001:
                        this.formulaExtraReader.ReadBytes(7);
                        goto Label_20BF;
                    Label_2013:
                        if (num61 == 0x10)
                        {
                            string str21 = "";
                            switch (this.formulaExtraReader.ReadByte())
                            {
                                case 0:
                                    str21 = "#NULL!";
                                    break;

                                case 7:
                                    str21 = "#DIV/0!";
                                    break;

                                case 15:
                                    str21 = "#VALUE!";
                                    break;

                                case 0x24:
                                    str21 = "#NUM!";
                                    break;

                                case 0x2a:
                                    str21 = "#N/A";
                                    break;

                                case 0x17:
                                    str21 = "#REF!";
                                    break;

                                case 0x1d:
                                    str21 = "#NAME?";
                                    break;
                            }
                            str19 = str19 + str21;
                            this.formulaExtraReader.ReadBytes(7);
                        }
                    Label_20BF:
                        if ((i + 1) < num59)
                        {
                            if (((i + 1) % num58) == 0)
                            {
                                str19 = str19 + ";";
                            }
                            else
                            {
                                str19 = str19 + ",";
                            }
                        }
                    }
                    str19 = str19 + "}";
                    stack.Add(str19);
                    continue;
                Label_2115:
                    stack.Add(ExcelCalcError.WrongFunctionOrRangeName);
                    flag2 = true;
                }
            }
            catch (EndOfStreamException)
            {
                if (stack.Count > 0)
                {
                    str = (string) ((string) stack[0]);
                }
            }
            catch (NotSupportedException exception)
            {
                throw new ExcelException(exception.Message, ExcelExceptionCode.FormulaError);
            }
            catch
            {
                throw new ExcelException("ExcelFormula.ToString", ExcelExceptionCode.FormulaError);
            }
            finally
            {
                stack.Clear();
            }
        Label_216B:
            if (str != null)
            {
                str = str.Replace("MissArg", "");
            }
            return str;
        }

        private void WriteArgument(BinaryWriter writer, ParsedToken arg, TokenClass tokenClass, LinkTable linkTable, ref int extraDataLength, Ptg ptgType = 0)
        {
            if (arg.TokenType == TokenType.SubExpression)
            {
                MemoryStream stream = new MemoryStream();
                BinaryWriter bw = new BinaryWriter((Stream) stream);
                int num = 0;
                this.WriteParsedToken(bw, arg.Value as ParsedToken, tokenClass, linkTable, ref num, true, ptgType);
                byte[] buffer = stream.ToArray();
                if (tokenClass == TokenClass.Value)
                {
                    writer.Write((byte) 0x49);
                }
                else
                {
                    writer.Write((byte) 0x29);
                }
                writer.Write((ushort) buffer.Length);
                writer.Write(buffer);
            }
            else
            {
                this.WriteParsedToken(writer, arg, tokenClass, linkTable, ref extraDataLength, true, ptgType);
            }
        }

        private void WriteArray(BinaryWriter writer, object value, ref int extraDataLength, bool writeToken = true, Ptg ptg = 0)
        {
            object[,] objArray = value as object[,];
            int upperBound = objArray.GetUpperBound(0);
            int num2 = objArray.GetUpperBound(1);
            if (num2 == 0x100)
            {
                num2 = 0;
            }
            if (ptg == Ptg.ArrayA)
            {
                writer.Write((byte) 0x60);
            }
            else
            {
                writer.Write((byte) 0x40);
            }
            writer.Write((byte) ((byte) num2));
            writer.Write((short) ((short) upperBound));
            writer.Write(0);
            this.extraWriter.Write((byte) ((byte) num2));
            this.extraWriter.Write((short) ((short) upperBound));
            for (int i = 0; i < (upperBound + 1); i++)
            {
                for (int j = 0; j < (num2 + 1); j++)
                {
                    ParsedToken token = (ParsedToken) objArray[i, j];
                    if (token.Value is double)
                    {
                        this.extraWriter.Write((byte) 1);
                        this.extraWriter.Write((double) ((double) token.Value));
                    }
                    else if (token.TokenType == TokenType.String)
                    {
                        this.extraWriter.Write((byte) 2);
                        this.WriteArrayString(this.extraWriter, token.Token.ToString());
                    }
                    else if (token.TokenType == TokenType.Boolean)
                    {
                        this.extraWriter.Write((byte) 4);
                        if ((bool) token.Value)
                        {
                            this.extraWriter.Write((byte) 1);
                        }
                        else
                        {
                            this.extraWriter.Write((byte) 0);
                        }
                        this.extraWriter.Write(0);
                        this.extraWriter.Write((short) 0);
                        this.extraWriter.Write((byte) 0);
                    }
                    else if (token.TokenType == TokenType.Error)
                    {
                        this.extraWriter.Write((byte) 0x10);
                        ExcelCalcError error = token.Value as ExcelCalcError;
                        if (error == ExcelCalcError.IllegalOrDeletedCellReference)
                        {
                            this.extraWriter.Write((byte) 0);
                        }
                        else if (error == ExcelCalcError.ArgumentOrFunctionNotAvailable)
                        {
                            this.extraWriter.Write((byte) 0x2a);
                        }
                        this.extraWriter.Write(0);
                        this.extraWriter.Write((short) 0);
                        this.extraWriter.Write((byte) 0);
                    }
                }
            }
        }

        private int WriteArrayString(BinaryWriter writer, string token)
        {
            string s = token;
            if (s.StartsWith("\""))
            {
                s = s.Substring(1);
            }
            if (s.EndsWith("\""))
            {
                s = s.Substring(0, s.Length - 1);
            }
            while (s.Contains("\"\""))
            {
                s = s.Replace("\"\"", "\"");
            }
            int length = s.Length;
            writer.Write((short) ((short) length));
            writer.Write((byte) 1);
            writer.Write(Encoding.Unicode.GetBytes(s));
            return ((2 * length) + 3);
        }

        private void WriteBinaryOperation(BinaryWriter writer, string token)
        {
            Ptg ptg = this.GetPtg(token);
            writer.Write((byte) ((byte) ptg));
        }

        private void WriteBoolean(BinaryWriter writer, bool value)
        {
            writer.Write((byte) 0x1d);
            writer.Write(value ? ((byte) 1) : ((byte) 0));
        }

        private void WriteCELLFunction(BinaryWriter writer, ParsedToken token, TokenClass tokenClass, LinkTable linkTable, ref int extraDataLength, Ptg ptgType = 0)
        {
            writer.Write((byte) 0x19);
            writer.Write((byte) 1);
            writer.Write((short) 0);
            ParsedToken[] tokenArray = token.Value as ParsedToken[];
            for (int i = 0; i < tokenArray.Length; i++)
            {
                if (i == 0)
                {
                    tokenClass = TokenClass.Reference;
                }
                this.WriteArgument(writer, tokenArray[i], tokenClass, linkTable, ref extraDataLength, ptgType);
            }
            this.WriteFunctionImp(writer, token, this.GetXlf(token.Token), tokenArray.Length, tokenClass, ref extraDataLength, ptgType);
        }

        private void WriteDouble(BinaryWriter writer, double value)
        {
            if (((value >= 0.0) && (value < 32767.0)) && (Math.Abs((double) (value - Convert.ToInt32(value))) < 1E-12))
            {
                writer.Write((byte) 30);
                writer.Write(Convert.ToUInt16(value));
            }
            else
            {
                writer.Write((byte) 0x1f);
                writer.Write(value);
            }
        }

        private void WriteError(BinaryWriter writer, ExcelCalcError value)
        {
            writer.Write((byte) 0x1c);
            if (value == ExcelCalcError.DivideByZero)
            {
                writer.Write((byte) 7);
            }
            else if (value == ExcelCalcError.WrongFunctionOrRangeName)
            {
                writer.Write((byte) 0x1d);
            }
            else if (value == ExcelCalcError.ArgumentOrFunctionNotAvailable)
            {
                writer.Write((byte) 0x2a);
            }
            else if (value == ExcelCalcError.InterSectionOfTwoCellRangesIsEmpty)
            {
                writer.Write((byte) 0);
            }
            else if (value == ExcelCalcError.ValueRangeOverflow)
            {
                writer.Write((byte) 0x24);
            }
            else if (value == ExcelCalcError.IllegalOrDeletedCellReference)
            {
                writer.Write((byte) 0x17);
            }
            else if (value == ExcelCalcError.WrongTypeOfOperand)
            {
                writer.Write((byte) 15);
            }
        }

        private void WriteExcel2007PrefixFunction(BinaryWriter writer, string name, TokenClass tokenClass, LinkTable linkTable, ref int extraDataLength, ParsedToken[] args, ref int nameIndex)
        {
            string str = name;
            nameIndex = linkTable.GetDefinedNameIndex(str);
            writer.Write((byte) 0x23);
            writer.Write((short) ((short) (nameIndex + 1)));
            writer.Write((short) 0);
            for (int i = 0; i < args.Length; i++)
            {
                if ((name == "_xlfn.AVERAGEIF") || (name == "_xlfn.COUNTIFS"))
                {
                    if ((i % 2) == 0)
                    {
                        this.WriteParsedToken(writer, args[i], TokenClass.Reference, linkTable, ref extraDataLength, false, Ptg.None);
                    }
                    else
                    {
                        this.WriteParsedToken(writer, args[i], TokenClass.Value, linkTable, ref extraDataLength, false, Ptg.None);
                    }
                }
                else
                {
                    this.WriteParsedToken(writer, args[i], tokenClass, linkTable, ref extraDataLength, false, Ptg.None);
                }
            }
        }

        private void WriteExReference(BinaryWriter writer, object value, TokenClass tokenClass, LinkTable linkTable)
        {
            ExternalRangeExpression2 expression2;
            ExcelExternSheet sheet2;
            if (value is ExternalRangeExpression)
            {
                ExternalRangeExpression expression = value as ExternalRangeExpression;
                ushort num = 0;
                ushort row = 0;
                ushort num3 = 0;
                int excelExternSheetIndex = -1;
                ExcelExternSheet sheet = new ExcelExternSheet();
                if (expression.Source == null)
                {
                    writer.Write((byte) 0x1c);
                    writer.Write((byte) 0x17);
                    return;
                }
                new List<string>();
                foreach (ExcelSupBook book in linkTable.SupBooks)
                {
                    if (book.FileName == null)
                    {
                        List<string> sheetNames = book.SheetNames;
                    }
                }
                if (expression.Source is List<object>)
                {
                    List<object> source = expression.Source as List<object>;
                    string str = (string) (source[1] as string);
                    string str2 = (string) (source[0] as string);
                    if (str2.IndexOf(']') != -1)
                    {
                        string[] strArray = str2.Split(new char[] { ']' });
                        str2 = strArray[1];
                        string str3 = strArray[0];
                        int length = str3.LastIndexOf('[');
                        if (length != -1)
                        {
                            str3 = str3.Substring(0, length) + str3.Substring(length + 1);
                        }
                        for (int i = 0; i < linkTable.SupBooks.Count; i++)
                        {
                            ExcelSupBook book2 = linkTable.SupBooks[i];
                            if (((book2.FileName == str3) && (book2.SheetNames != null)) && ((book2.SheetNames.IndexOf(str2) != -1) && (book2.SheetNames.IndexOf(str) != -1)))
                            {
                                sheet.beginSheetIndex = book2.SheetNames.IndexOf(str2);
                                sheet.endSheetIndex = book2.SheetNames.IndexOf(str);
                                sheet.supBookIndex = i;
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int j = 0; j < linkTable.SupBooks.Count; j++)
                        {
                            ExcelSupBook book3 = linkTable.SupBooks[j];
                            if ((book3.IsSelfReferenced && (book3.SheetNames != null)) && ((book3.SheetNames.IndexOf(str2) != -1) && (book3.SheetNames.IndexOf(str) != -1)))
                            {
                                sheet.beginSheetIndex = book3.SheetNames.IndexOf(str2);
                                sheet.endSheetIndex = book3.SheetNames.IndexOf(str);
                                sheet.supBookIndex = j;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    string str4 = (string) (expression.Source as string);
                    if (str4.IndexOf(']') != -1)
                    {
                        string[] strArray2 = str4.Split(new char[] { ']' });
                        string str5 = strArray2[1];
                        string str6 = strArray2[0];
                        int num8 = str6.LastIndexOf('[');
                        if (num8 != -1)
                        {
                            str6 = str6.Substring(0, num8) + str6.Substring(num8 + 1);
                        }
                        for (int k = 0; k < linkTable.SupBooks.Count; k++)
                        {
                            ExcelSupBook book4 = linkTable.SupBooks[k];
                            if (((book4.FileName == str6) && (book4.SheetNames != null)) && (book4.SheetNames.IndexOf(str5) != -1))
                            {
                                sheet.beginSheetIndex = book4.SheetNames.IndexOf(str5);
                                sheet.endSheetIndex = sheet.beginSheetIndex;
                                sheet.supBookIndex = k;
                                break;
                            }
                        }
                    }
                    else if (str4 != null)
                    {
                        if (linkTable.InternalSheetNames.Contains(str4))
                        {
                            for (int m = 0; m < linkTable.SupBooks.Count; m++)
                            {
                                ExcelSupBook book5 = linkTable.SupBooks[m];
                                if ((book5.IsSelfReferenced && (book5.SheetNames != null)) && (book5.SheetNames.IndexOf(str4) != -1))
                                {
                                    sheet.beginSheetIndex = book5.SheetNames.IndexOf(str4);
                                    sheet.endSheetIndex = sheet.beginSheetIndex;
                                    sheet.supBookIndex = m;
                                    break;
                                }
                            }
                        }
                        else if (str4.Contains("\""))
                        {
                            str4 = str4.Replace("\"", "'");
                            if (linkTable.InternalSheetNames.Contains(str4))
                            {
                                for (int n = 0; n < linkTable.SupBooks.Count; n++)
                                {
                                    ExcelSupBook book6 = linkTable.SupBooks[n];
                                    if ((book6.IsSelfReferenced && (book6.SheetNames != null)) && (book6.SheetNames.IndexOf(str4) != -1))
                                    {
                                        sheet.beginSheetIndex = book6.SheetNames.IndexOf(str4);
                                        sheet.endSheetIndex = sheet.beginSheetIndex;
                                        sheet.supBookIndex = n;
                                        break;
                                    }
                                }
                            }
                        }
                        else if (str4.StartsWith("#"))
                        {
                            for (int num12 = 0; num12 < linkTable.SupBooks.Count; num12++)
                            {
                                if (linkTable.SupBooks[num12].IsSelfReferenced)
                                {
                                    sheet.beginSheetIndex = -1;
                                    sheet.endSheetIndex = -1;
                                    sheet.supBookIndex = num12;
                                    break;
                                }
                            }
                        }
                        else if (str4 == "")
                        {
                            for (int num13 = 0; num13 < linkTable.SupBooks.Count; num13++)
                            {
                                if (linkTable.SupBooks[num13].IsCurrentSheetSupBook)
                                {
                                    sheet.beginSheetIndex = -2;
                                    sheet.endSheetIndex = -2;
                                    sheet.supBookIndex = num13;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int num14 = 0; num14 < linkTable.SupBooks.Count; num14++)
                        {
                            if (linkTable.SupBooks[num14].IsCurrentSheetSupBook)
                            {
                                sheet.beginSheetIndex = -2;
                                sheet.endSheetIndex = -2;
                                sheet.supBookIndex = num14;
                                break;
                            }
                        }
                    }
                }
                excelExternSheetIndex = linkTable.GetExcelExternSheetIndex(sheet);
                if (excelExternSheetIndex == -1)
                {
                    writer.Write((byte) 0x1c);
                    writer.Write((byte) 0x17);
                    return;
                }
                if (expression.RowRelative)
                {
                    row = (ushort)(expression.Row + ((this.row >= 0) ? this.row : 0));
                }
                else if (expression.Row >= 0)
                {
                    row = (ushort) expression.Row;
                }
                if (expression.ColumnRelative)
                {
                    num3 = (ushort)(expression.Column + ((this.column >= 0) ? this.column : 0));
                }
                else
                {
                    int column = expression.Column;
                    if ((column >= 0) && (column <= 0xffff))
                    {
                        num3 = (ushort) column;
                    }
                }
                if ((expression.ColumnCount > 1) || (expression.RowCount > 1))
                {
                    if (this.namedExpression)
                    {
                        if (this.isNamedReference)
                        {
                            writer.Write((byte) 0x3b);
                        }
                        else
                        {
                            writer.Write((byte) 0x7b);
                        }
                        writer.Write((short) (excelExternSheetIndex + (this.hasExternNames ? 1 : 0)));
                    }
                    else
                    {
                        if ((tokenClass == TokenClass.Array) && this.isDefinedNameFormula)
                        {
                            writer.Write((byte) 0x7b);
                        }
                        else if ((tokenClass == TokenClass.Array) && this._isArrayFormula)
                        {
                            writer.Write((byte) 0x7b);
                        }
                        else if (tokenClass == TokenClass.Value)
                        {
                            writer.Write((byte) 0x5b);
                        }
                        else
                        {
                            writer.Write((byte) 0x3b);
                        }
                        writer.Write((short) (excelExternSheetIndex + (this.hasExternNames ? 1 : 0)));
                    }
                    if (this.isDefinedNameFormula)
                    {
                        short num16 = (short) row;
                        if (expression.RowRelative)
                        {
                            num16 = (short)(row - this.row);
                            if (num16 < 0)
                            {
                                num16 = (short)(num16 + 0x10000);
                            }
                        }
                        writer.Write(num16);
                        num16 = (short)((row + expression.RowCount) - 1);
                        if (expression.RowRelative)
                        {
                            num16 -= (short)(this.row);
                            if (num16 < 0)
                            {
                                num16 = (short)(num16 + 0x10000);
                            }
                        }
                        writer.Write(num16);
                        short num17 = (short) num3;
                        if (expression.ColumnRelative)
                        {
                            num17 = (short)(num3 - this.column);
                            if (num17 < 0)
                            {
                                num17 += 0x100;
                            }
                        }
                        num = (ushort)(((expression.RowRelative ? 0x8000 : 0) + (expression.ColumnRelative ? 0x4000 : 0)) + (num17 & 0x3fff));
                        writer.Write(num);
                        num17 = (short)((num3 + expression.ColumnCount) - 1);
                        if (expression.ColumnRelative)
                        {
                            num17 -= (short)this.column;
                            if (num17 < 0)
                            {
                                num17 += 0x100;
                            }
                        }
                        num = (ushort)(((expression.RowRelative ? 0x8000 : 0) + (expression.ColumnRelative ? 0x4000 : 0)) + (num17 & 0x3fff));
                        writer.Write(num);
                        return;
                    }
                    writer.Write(row);
                    row = (ushort)((expression.Row + expression.RowCount) - 1);
                    if (expression.RowRelative)
                    {
                        row += (ushort)((this.row >= 0) ? this.row : 0);
                    }
                    writer.Write(row);
                    num = (ushort)(((expression.RowRelative ? 0x8000 : 0) + (expression.ColumnRelative ? 0x4000 : 0)) + (num3 & 0x3fff));
                    writer.Write(num);
                    num3 = (ushort)((expression.Column + expression.ColumnCount) - 1);
                    if (expression.ColumnRelative)
                    {
                        num3 += (ushort)((this.column >= 0) ? this.column : 0);
                    }
                    num = (ushort)(((expression.RowRelative ? 0x8000 : 0) + (expression.ColumnRelative ? 0x4000 : 0)) + (num3 & 0x3fff));
                    writer.Write(num);
                    return;
                }
                bool flag = false;
                bool flag2 = false;
                if (!expression.RowRelative && (expression.Row == -1))
                {
                    flag2 = true;
                }
                if (!expression.ColumnRelative && (expression.Column == -1))
                {
                    flag = true;
                }
                if (flag)
                {
                    if (tokenClass == TokenClass.Value)
                    {
                        writer.Write((byte) 0x5b);
                    }
                    else
                    {
                        writer.Write((byte) 0x3b);
                    }
                    writer.Write((short) (excelExternSheetIndex + (this.hasExternNames ? 1 : 0)));
                    int num18 = expression.Row;
                    int num19 = (expression.Row + expression.RowCount) - 1;
                    writer.Write((short) ((short) num18));
                    writer.Write((short) ((short) num19));
                    int num20 = 0;
                    int num21 = 0;
                    if (expression.RowRelative)
                    {
                        num20 |= 0x80;
                        num21 |= 0x80;
                    }
                    if (expression.ColumnRelative)
                    {
                        num20 |= 0x40;
                        num21 |= 0x40;
                    }
                    writer.Write((byte) 0);
                    writer.Write((byte) ((byte) num20));
                    writer.Write((byte) 0xff);
                    writer.Write((byte) ((byte) num21));
                    return;
                }
                if (flag2)
                {
                    if (tokenClass == TokenClass.Value)
                    {
                        writer.Write((byte) 0x5b);
                    }
                    else
                    {
                        writer.Write((byte) 0x3b);
                    }
                    writer.Write((short) (excelExternSheetIndex + (this.hasExternNames ? 1 : 0)));
                    writer.Write((short) 0);
                    writer.Write((short) (-1));
                    int num22 = 0;
                    int num23 = 0;
                    if (expression.RowRelative)
                    {
                        num22 |= 0x80;
                        num23 |= 0x80;
                    }
                    if (expression.ColumnRelative)
                    {
                        num22 |= 0x40;
                        num23 |= 0x40;
                    }
                    writer.Write((byte) ((byte) expression.Column));
                    writer.Write((byte) ((byte) num22));
                    writer.Write((byte) ((expression.Column + expression.ColumnCount) - 1));
                    writer.Write((byte) ((byte) num23));
                    return;
                }
                if (this.namedExpression)
                {
                    writer.Write((byte) 0x7a);
                    writer.Write((short) (excelExternSheetIndex + (this.hasExternNames ? 1 : 0)));
                }
                else
                {
                    if (tokenClass == TokenClass.Reference)
                    {
                        writer.Write((byte) 0x3a);
                    }
                    else if (tokenClass == TokenClass.Value)
                    {
                        writer.Write((byte) 90);
                    }
                    else
                    {
                        writer.Write((byte) 0x7a);
                    }
                    writer.Write((short) (excelExternSheetIndex + (this.hasExternNames ? 1 : 0)));
                }
                if (this.namedExpression)
                {
                    if (!expression.RowRelative)
                    {
                        writer.Write((short) ((short) row));
                    }
                    else
                    {
                        short num24 = (short) row;
                        writer.Write(num24);
                    }
                    if (!expression.ColumnRelative)
                    {
                        num = (ushort)(((expression.RowRelative ? 0x8000 : 0) + (expression.ColumnRelative ? 0x4000 : 0)) + (num3 & 0x3fff));
                        writer.Write((short) ((short) num));
                        return;
                    }
                    int num25 = (short) num3;
                    if (num25 < 0)
                    {
                        num25 = 0x100 + num25;
                    }
                    else
                    {
                        num25 = num3;
                    }
                    num = (ushort)(((expression.RowRelative ? 0x8000 : 0) + (expression.ColumnRelative ? 0x4000 : 0)) + (num25 & 0x3fff));
                    writer.Write((short) ((short) num));
                    return;
                }
                if (this.isDefinedNameFormula)
                {
                    if (expression.RowRelative)
                    {
                        int num26 = row - this.row;
                        if (num26 < 0)
                        {
                            num26 += 0x10000;
                        }
                        writer.Write((short) ((short) num26));
                    }
                    else
                    {
                        writer.Write((short) ((short) row));
                    }
                    ushort num27 = num3;
                    if (expression.ColumnRelative)
                    {
                        num27 = (ushort)(num3 - this.column);
                        if (num27 < 0)
                        {
                            num27 += 0x100;
                        }
                    }
                    num = (ushort)(((expression.RowRelative ? 0x8000 : 0) + (expression.ColumnRelative ? 0x4000 : 0)) + (num27 & 0x3fff));
                    writer.Write(num);
                    return;
                }
                writer.Write((short) ((short) row));
                num = (ushort)(((expression.RowRelative ? 0x8000 : 0) + (expression.ColumnRelative ? 0x4000 : 0)) + (num3 & 0x3fff));
                writer.Write(num);
                return;
            }
            else
            {
                if (!(value is ExternalRangeExpression2))
                {
                    return;
                }
                expression2 = value as ExternalRangeExpression2;
                sheet2 = new ExcelExternSheet();
                if (expression2.Source == null)
                {
                    writer.Write((byte) 0x1c);
                    writer.Write((byte) 0x17);
                    return;
                }
                new List<string>();
                foreach (ExcelSupBook book10 in linkTable.SupBooks)
                {
                    if (book10.FileName == null)
                    {
                        List<string> list3 = book10.SheetNames;
                    }
                }
                if (expression2.Source is List<object>)
                {
                    List<object> list2 = expression2.Source as List<object>;
                    string str7 = (string) (list2[1] as string);
                    string str8 = (string) (list2[0] as string);
                    if (str8.IndexOf(']') != -1)
                    {
                        string[] strArray3 = str8.Split(new char[] { ']' });
                        str8 = strArray3[1];
                        string str9 = strArray3[0];
                        int num28 = str9.LastIndexOf('[');
                        if (num28 != -1)
                        {
                            str9 = str9.Substring(0, num28) + str9.Substring(num28 + 1);
                        }
                        for (int num29 = 0; num29 < linkTable.SupBooks.Count; num29++)
                        {
                            ExcelSupBook book11 = linkTable.SupBooks[num29];
                            if (((book11.FileName == str9) && (book11.SheetNames != null)) && ((book11.SheetNames.IndexOf(str8) != -1) && (book11.SheetNames.IndexOf(str7) != -1)))
                            {
                                sheet2.beginSheetIndex = book11.SheetNames.IndexOf(str8);
                                sheet2.endSheetIndex = book11.SheetNames.IndexOf(str7);
                                sheet2.supBookIndex = num29;
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int num30 = 0; num30 < linkTable.SupBooks.Count; num30++)
                        {
                            ExcelSupBook book12 = linkTable.SupBooks[num30];
                            if ((book12.IsSelfReferenced && (book12.SheetNames != null)) && ((book12.SheetNames.IndexOf(str8) != -1) && (book12.SheetNames.IndexOf(str7) != -1)))
                            {
                                sheet2.beginSheetIndex = book12.SheetNames.IndexOf(str8);
                                sheet2.endSheetIndex = book12.SheetNames.IndexOf(str7);
                                sheet2.supBookIndex = num30;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    string str10 = (string) (expression2.Source as string);
                    if (str10.IndexOf(']') != -1)
                    {
                        string[] strArray4 = str10.Split(new char[] { ']' });
                        string str11 = strArray4[1];
                        string str12 = strArray4[0];
                        int num31 = str12.LastIndexOf('[');
                        if (num31 != -1)
                        {
                            str12 = str12.Substring(0, num31) + str12.Substring(num31 + 1);
                        }
                        for (int num32 = 0; num32 < linkTable.SupBooks.Count; num32++)
                        {
                            ExcelSupBook book13 = linkTable.SupBooks[num32];
                            if (((book13.FileName == str12) && (book13.SheetNames != null)) && (book13.SheetNames.IndexOf(str11) != -1))
                            {
                                sheet2.beginSheetIndex = book13.SheetNames.IndexOf(str11);
                                sheet2.endSheetIndex = sheet2.beginSheetIndex;
                                sheet2.supBookIndex = num32;
                                break;
                            }
                        }
                    }
                    else if (str10 != null)
                    {
                        if (linkTable.InternalSheetNames.Contains(str10))
                        {
                            for (int num33 = 0; num33 < linkTable.SupBooks.Count; num33++)
                            {
                                ExcelSupBook book14 = linkTable.SupBooks[num33];
                                if ((book14.IsSelfReferenced && (book14.SheetNames != null)) && (book14.SheetNames.IndexOf(str10) != -1))
                                {
                                    sheet2.beginSheetIndex = book14.SheetNames.IndexOf(str10);
                                    sheet2.endSheetIndex = sheet2.beginSheetIndex;
                                    sheet2.supBookIndex = num33;
                                    break;
                                }
                            }
                        }
                        else if (str10.Contains("\""))
                        {
                            str10 = str10.Replace("\"", "'");
                            if (linkTable.InternalSheetNames.Contains(str10))
                            {
                                for (int num34 = 0; num34 < linkTable.SupBooks.Count; num34++)
                                {
                                    ExcelSupBook book15 = linkTable.SupBooks[num34];
                                    if ((book15.IsSelfReferenced && (book15.SheetNames != null)) && (book15.SheetNames.IndexOf(str10) != -1))
                                    {
                                        sheet2.beginSheetIndex = book15.SheetNames.IndexOf(str10);
                                        sheet2.endSheetIndex = sheet2.beginSheetIndex;
                                        sheet2.supBookIndex = num34;
                                        break;
                                    }
                                }
                            }
                        }
                        else if ((str10 == "") || str10.StartsWith("#"))
                        {
                            for (int num35 = 0; num35 < linkTable.SupBooks.Count; num35++)
                            {
                                if (linkTable.SupBooks[num35].IsSelfReferenced)
                                {
                                    sheet2.beginSheetIndex = -1;
                                    sheet2.endSheetIndex = -1;
                                    sheet2.supBookIndex = num35;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        ExcelSupBook book17 = new ExcelSupBook {
                            IsCurrentSheetSupBook = true,
                            SheetCount = 0
                        };
                        linkTable.SupBooks.Add(book17);
                        sheet2.beginSheetIndex = -2;
                        sheet2.endSheetIndex = -2;
                        sheet2.supBookIndex = linkTable.SupBooks.Count - 1;
                    }
                }
            }
            this.externSheetIndex = linkTable.GetExcelExternSheetIndex(sheet2);
            if (this.externSheetIndex == -1)
            {
                writer.Write((byte) 0x1c);
                writer.Write((byte) 0x17);
            }
            else if ((expression2.rowFirst != expression2.rowLast) || (expression2.colFirst != expression2.colLast))
            {
                if (this.namedExpression)
                {
                    if (this.isNamedReference)
                    {
                        writer.Write((byte) 0x3b);
                    }
                    else
                    {
                        writer.Write((byte) 0x7b);
                    }
                    writer.Write((short) (this.externSheetIndex + (this.hasExternNames ? 1 : 0)));
                }
                else
                {
                    if (tokenClass == TokenClass.Array)
                    {
                        writer.Write((byte) 0x7b);
                    }
                    else
                    {
                        writer.Write((byte) 0x3b);
                    }
                    writer.Write((short) (this.externSheetIndex + (this.hasExternNames ? 1 : 0)));
                }
            }
            else if (expression2.isWholeRow)
            {
                if (tokenClass == TokenClass.Value)
                {
                    writer.Write((byte) 0x5b);
                }
                else
                {
                    writer.Write((byte) 0x3b);
                }
            }
            else if (expression2.isWholeColumn)
            {
                if (tokenClass == TokenClass.Value)
                {
                    writer.Write((byte) 0x5b);
                }
                else
                {
                    writer.Write((byte) 0x3b);
                }
            }
            else if (this.namedExpression)
            {
                if (this.isNamedReference)
                {
                    writer.Write((byte) 0x3a);
                }
                else
                {
                    writer.Write((byte) 0x7a);
                }
                writer.Write((short) (this.externSheetIndex + (this.hasExternNames ? 1 : 0)));
            }
            else
            {
                if (tokenClass == TokenClass.Reference)
                {
                    writer.Write((byte) 0x3a);
                }
                else if (tokenClass == TokenClass.Value)
                {
                    writer.Write((byte) 90);
                }
                else if (this.isNamedReference)
                {
                    writer.Write((byte) 0x3a);
                }
                else
                {
                    writer.Write((byte) 0x7a);
                }
                writer.Write((short) (this.externSheetIndex + (this.hasExternNames ? 1 : 0)));
            }
            if (expression2.isWholeRow)
            {
                writer.Write((short) ((short) expression2.rowFirst));
                writer.Write((short) ((short) expression2.rowLast));
                int num36 = 0;
                if (expression2.isRowFirstRel)
                {
                    num36 |= 0x80;
                }
                int num37 = 0;
                if (expression2.isRowLastRel)
                {
                    num37 |= 0x80;
                }
                writer.Write((byte) 0);
                writer.Write((byte) ((byte) num36));
                writer.Write((byte) 0xff);
                writer.Write((byte) ((byte) num37));
            }
            else if (expression2.isWholeColumn)
            {
                writer.Write((short) 0);
                writer.Write((ushort) 0xffff);
                int num38 = 0;
                if (expression2.isColumnFirstRel)
                {
                    num38 |= 0x40;
                }
                int num39 = 0;
                if (expression2.isColumnLastRel)
                {
                    num39 |= 0x40;
                }
                writer.Write((byte) ((byte) expression2.colFirst));
                writer.Write((byte) ((byte) num38));
                writer.Write((byte) ((byte) expression2.colLast));
                writer.Write((byte) ((byte) num39));
            }
            else
            {
                if (!this.isDefinedNameFormula)
                {
                    writer.Write((short) ((short) expression2.rowFirst));
                    writer.Write((short) ((short) expression2.rowLast));
                }
                else
                {
                    int rowFirst = expression2.rowFirst;
                    int rowLast = expression2.rowLast;
                    int activeSheet = this.activeSheet;
                    if (this.sheet != 1)
                    {
                        activeSheet = this.sheet;
                    }
                    Tuple<int, int> tuple = this.SheetsSelectionList[activeSheet];
                    if (expression2.isRowFirstRel)
                    {
                        rowFirst = expression2.rowFirst - tuple.Item1;
                    }
                    if (expression2.isRowLastRel)
                    {
                        rowLast = expression2.rowLast - tuple.Item1;
                    }
                    writer.Write((short) ((short) rowFirst));
                    writer.Write((short) ((short) rowLast));
                }
                int num43 = 0;
                if (expression2.isRowFirstRel)
                {
                    num43 |= 0x8000;
                }
                if (expression2.isColumnFirstRel)
                {
                    num43 |= 0x4000;
                }
                num43 |= expression2.colFirst & 0x3fff;
                writer.Write((short) ((short) num43));
                int num44 = 0;
                if (expression2.isRowLastRel)
                {
                    num44 |= 0x8000;
                }
                if (expression2.isColumnLastRel)
                {
                    num44 |= 0x4000;
                }
                num44 |= expression2.colLast & 0x3fff;
                writer.Write((short) ((short) num44));
            }
        }

        private void WriteFunction(BinaryWriter writer, ParsedToken token, Xlf fn, int args, TokenClass tokenClass, Ptg PtgType = 0)
        {
            if (PtgType == Ptg.None)
            {
                if ((((((((fn == Xlf.Abs) || (fn == Xlf.Acos)) || ((fn == Xlf.Acosh) || (fn == Xlf.Asin))) || (((fn == Xlf.Asinh) || (fn == Xlf.Atan)) || ((fn == Xlf.Atanh) || (fn == Xlf.Atan2)))) || ((((fn == Xlf.Binomdist) || (fn == Xlf.Ceiling)) || ((fn == Xlf.Char) || (fn == Xlf.Chidist))) || (((fn == Xlf.Chiinv) || (fn == Xlf.Chitest)) || ((fn == Xlf.Clean) || (fn == Xlf.Code))))) || (((((fn == Xlf.Columns) || (fn == Xlf.Combin)) || ((fn == Xlf.Confidence) || (fn == Xlf.Correl))) || (((fn == Xlf.Cos) || (fn == Xlf.Cosh)) || ((fn == Xlf.Countif) || (fn == Xlf.Covar)))) || ((((fn == Xlf.Countblank) || (fn == Xlf.Critbinom)) || ((fn == Xlf.Dcount) || (fn == Xlf.Dcounta))) || (((fn == Xlf.Dget) || (fn == Xlf.Dmax)) || ((fn == Xlf.Dmin) || (fn == Xlf.Daverage)))))) || ((((((fn == Xlf.Dproduct) || (fn == Xlf.Dstdev)) || ((fn == Xlf.Dstdevp) || (fn == Xlf.Dsum))) || (((fn == Xlf.Dvar) || (fn == Xlf.Dvarp)) || ((fn == Xlf.Date) || (fn == Xlf.Datedif)))) || ((((fn == Xlf.Datevalue) || (fn == Xlf.Day)) || ((fn == Xlf.Degrees) || (fn == Xlf.Echo))) || (((fn == Xlf.ErrorType) || (fn == Xlf.Even)) || ((fn == Xlf.Exact) || (fn == Xlf.Exp))))) || (((((fn == Xlf.Expondist) || (fn == Xlf.Fact)) || ((fn == Xlf.False) || (fn == Xlf.Fdist))) || (((fn == Xlf.Finv) || (fn == Xlf.Fisher)) || ((fn == Xlf.Fisherinv) || (fn == Xlf.Floor)))) || ((((fn == Xlf.Forecast) || (fn == Xlf.Frequency)) || ((fn == Xlf.Ftest) || (fn == Xlf.Gammadist))) || (((fn == Xlf.Gammainv) || (fn == Xlf.Gammaln)) || ((fn == Xlf.Hour) || (fn == Xlf.Hypgeomdist))))))) || (((((((fn == Xlf.Int) || (fn == Xlf.Intercept)) || ((fn == Xlf.Isblank) || (fn == Xlf.Iserr))) || (((fn == Xlf.Iserror) || (fn == Xlf.Islogical)) || ((fn == Xlf.Isna) || (fn == Xlf.Isnontext)))) || ((((fn == Xlf.Isnumber) || (fn == Xlf.Ispmt)) || ((fn == Xlf.Isref) || (fn == Xlf.Istext))) || (((fn == Xlf.Large) || (fn == Xlf.Len)) || ((fn == Xlf.Ln) || (fn == Xlf.Log10))))) || (((((fn == Xlf.Loginv) || (fn == Xlf.Lognormdist)) || ((fn == Xlf.Lower) || (fn == Xlf.Mdeterm))) || (((fn == Xlf.Mid) || (fn == Xlf.Minute)) || ((fn == Xlf.Minverse) || (fn == Xlf.Mirr)))) || ((((fn == Xlf.Mmult) || (fn == Xlf.Mod)) || ((fn == Xlf.Month) || (fn == Xlf.N))) || (((fn == Xlf.Na) || (fn == Xlf.Negbinomdist)) || ((fn == Xlf.Normdist) || (fn == Xlf.Norminv)))))) || (((((((fn == Xlf.Normsdist) || (fn == Xlf.Normsinv)) || ((fn == Xlf.Not) || (fn == Xlf.Now))) || (((fn == Xlf.Odd) || (fn == Xlf.Pearson)) || ((fn == Xlf.Percentile) || (fn == Xlf.Permut)))) || ((((fn == Xlf.Pi) || (fn == Xlf.Poisson)) || ((fn == Xlf.Power) || (fn == Xlf.Proper))) || (((fn == Xlf.Quartile) || (fn == Xlf.Radians)) || ((fn == Xlf.Rand) || (fn == Xlf.Replace))))) || (((((fn == Xlf.Rept) || (fn == Xlf.Round)) || ((fn == Xlf.Round) || (fn == Xlf.Rounddown))) || (((fn == Xlf.Roundup) || (fn == Xlf.Rows)) || ((fn == Xlf.Rsq) || (fn == Xlf.Second)))) || ((((fn == Xlf.Sign) || (fn == Xlf.Sin)) || ((fn == Xlf.Sinh) || (fn == Xlf.Slope))) || (((fn == Xlf.Sln) || (fn == Xlf.Small)) || ((fn == Xlf.Sqrt) || (fn == Xlf.Standardize)))))) || ((((((fn == Xlf.Steyx) || (fn == Xlf.Sumx2my2)) || ((fn == Xlf.Sumx2py2) || (fn == Xlf.Sumxmy2))) || (((fn == Xlf.Syd) || (fn == Xlf.T)) || ((fn == Xlf.Tan) || (fn == Xlf.Tanh)))) || ((((fn == Xlf.Tdist) || (fn == Xlf.Time)) || ((fn == Xlf.Timevalue) || (fn == Xlf.Tinv))) || (((fn == Xlf.Today) || (fn == Xlf.Transpose)) || ((fn == Xlf.Trim) || (fn == Xlf.Trimmean))))) || ((((fn == Xlf.True) || (fn == Xlf.Ttest)) || ((fn == Xlf.Type) || (fn == Xlf.Upper))) || (((fn == Xlf.Value) || (fn == Xlf.Weibull)) || ((fn == Xlf.Year) || (fn == Xlf.Text))))))))
                {
                    if ((tokenClass == TokenClass.Value) || (tokenClass == TokenClass.Reference))
                    {
                        PtgType = Ptg.FuncV;
                    }
                    else if (tokenClass == TokenClass.Array)
                    {
                        PtgType = Ptg.FuncA;
                    }
                }
                else if (((((((fn == Xlf.And) || (fn == Xlf.Address)) || ((fn == Xlf.Average) || (fn == Xlf.AverageA))) || (((fn == Xlf.Avedev) || (fn == Xlf.Betadist)) || ((fn == Xlf.Betainv) || (fn == Xlf.Choose)))) || ((((fn == Xlf.Column) || (fn == Xlf.Concatenate)) || ((fn == Xlf.Count) || (fn == Xlf.Counta))) || (((fn == Xlf.Days360) || (fn == Xlf.Db)) || ((fn == Xlf.Ddb) || (fn == Xlf.Devsq))))) || (((((fn == Xlf.Dollar) || (fn == Xlf.Find)) || ((fn == Xlf.Fixed) || (fn == Xlf.Fv))) || (((fn == Xlf.Growth) || (fn == Xlf.Geomean)) || ((fn == Xlf.Harmean) || (fn == Xlf.Index)))) || ((((fn == Xlf.Ipmt) || (fn == Xlf.Irr)) || ((fn == Xlf.Kurt) || (fn == Xlf.Left))) || (((fn == Xlf.Linest) || (fn == Xlf.Log)) || ((fn == Xlf.Logest) || (fn == Xlf.Max)))))) || ((((((fn == Xlf.MaxA) || (fn == Xlf.Median)) || ((fn == Xlf.Min) || (fn == Xlf.MinA))) || (((fn == Xlf.Mode) || (fn == Xlf.Nper)) || ((fn == Xlf.Npv) || (fn == Xlf.Offset)))) || ((((fn == Xlf.Or) || (fn == Xlf.Percentrank)) || ((fn == Xlf.Prob) || (fn == Xlf.Product))) || (((fn == Xlf.Pmt) || (fn == Xlf.Ppmt)) || ((fn == Xlf.Pv) || (fn == Xlf.Rank))))) || ((((((fn == Xlf.Rate) || (fn == Xlf.Right)) || ((fn == Xlf.Roman) || (fn == Xlf.Row))) || (((fn == Xlf.Skew) || (fn == Xlf.Search)) || ((fn == Xlf.Stdev) || (fn == Xlf.StDevA)))) || ((((fn == Xlf.Stdevp) || (fn == Xlf.StDevPA)) || ((fn == Xlf.Substitute) || (fn == Xlf.Subtotal))) || (((fn == Xlf.Sum) || (fn == Xlf.Sumif)) || ((fn == Xlf.Sumproduct) || (fn == Xlf.Sumsq))))) || ((((fn == Xlf.Trend) || (fn == Xlf.Trunc)) || ((fn == Xlf.Var) || (fn == Xlf.VarA))) || ((((fn == Xlf.Varp) || (fn == Xlf.VarPA)) || ((fn == Xlf.Vdb) || (fn == Xlf.Weekday))) || ((fn == Xlf.Ztest) || (args != 1)))))))
                {
                    if (tokenClass == TokenClass.Array)
                    {
                        PtgType = Ptg.FuncVarA;
                    }
                    else if (this.isDefinedNameFormula)
                    {
                        PtgType = Ptg.FuncVar;
                    }
                    else
                    {
                        PtgType = Ptg.FuncVarV;
                    }
                }
                else if (fn == Xlf.If)
                {
                    PtgType = Ptg.FuncVar;
                }
                else if (fn == Xlf.Indirect)
                {
                    PtgType = Ptg.FuncVarV;
                }
                else if (fn == Xlf.Info)
                {
                    PtgType = Ptg.FuncV;
                }
                else if (fn == Xlf.Cell)
                {
                    PtgType = Ptg.FuncVarV;
                }
                else if (args == 1)
                {
                    return;
                }
                writer.Write((byte) ((byte) PtgType));
                if (((PtgType == Ptg.FuncVarV) || (PtgType == Ptg.FuncVarA)) || (PtgType == Ptg.FuncVar))
                {
                    writer.Write((byte) ((byte) args));
                }
                writer.Write((short) ((short) fn));
            }
            else
            {
                if (tokenClass == TokenClass.Array)
                {
                    writer.Write((byte) 0x62);
                }
                else
                {
                    writer.Write((byte) ((byte) PtgType));
                }
                if (((PtgType == Ptg.FuncVarV) || (PtgType == Ptg.FuncVarA)) || (PtgType == Ptg.FuncVar))
                {
                    writer.Write((byte) ((byte) args));
                }
                writer.Write((short) ((short) fn));
            }
        }

        private void WriteFunctionImp(BinaryWriter writer, ParsedToken token, Xlf xlf, int argCount, TokenClass tokenClass, ref int extraDataLength, Ptg ptgType = 0)
        {
            this.WriteFunction(writer, token, xlf, argCount, tokenClass, ptgType);
        }

        private void WriteIFFunction(BinaryWriter writer, TokenClass tokenClass, LinkTable linkTable, ref int extraDataLength, ParsedToken[] args, Ptg ptgType = 0)
        {
            if (this.isDefinedNameFormula)
            {
                this.namedExpression = true;
            }
            if (this._isArrayFormula && (ptgType == Ptg.FuncVarV))
            {
                this.WriteParsedToken(writer, args[0], TokenClass.Array, linkTable, ref extraDataLength, false, Ptg.None);
            }
            else
            {
                this.WriteParsedToken(writer, args[0], TokenClass.Value, linkTable, ref extraDataLength, false, Ptg.None);
            }
            writer.Write((byte) 0x19);
            writer.Write((byte) 2);
            if (this.isDefinedNameFormula)
            {
                this.namedExpression = false;
            }
            ParsedToken token = args[1];
            if (this.isDefinedNameFormula)
            {
                this.namedExpression = true;
                if (token.TokenType == TokenType.Name)
                {
                    this.isNamedReference = true;
                }
            }
            MemoryStream stream = new MemoryStream();
            BinaryWriter bw = new BinaryWriter((Stream) stream);
            int num = 0;
            if ((token.TokenType == TokenType.Name) && this._isArrayFormula)
            {
                this.WriteParsedToken(bw, token, TokenClass.Reference, linkTable, ref num, false, Ptg.None);
            }
            else if ((token.TokenType == TokenType.Function) && this._isArrayFormula)
            {
                this.WriteParsedToken(bw, token, TokenClass.Array, linkTable, ref num, false, Ptg.None);
            }
            else
            {
                this.WriteParsedToken(bw, token, tokenClass, linkTable, ref num, false, Ptg.None);
            }
            byte[] buffer = stream.ToArray();
            writer.Write((short) (buffer.Length + 4));
            writer.Write(buffer);
            if (this.isDefinedNameFormula)
            {
                this.namedExpression = false;
                if (token.TokenType == TokenType.Name)
                {
                    this.isNamedReference = false;
                }
            }
            if (args.Length == 3)
            {
                ParsedToken token2 = args[2];
                if (this.isDefinedNameFormula)
                {
                    this.namedExpression = true;
                    if (token2.TokenType == TokenType.Name)
                    {
                        this.isNamedReference = true;
                    }
                }
                MemoryStream stream2 = new MemoryStream();
                BinaryWriter writer3 = new BinaryWriter((Stream) stream2);
                int num2 = 0;
                if (this._isArrayFormula && (token2.TokenType == TokenType.Name))
                {
                    this.WriteParsedToken(writer3, token2, TokenClass.Reference, linkTable, ref num2, false, Ptg.None);
                }
                else if (this._isArrayFormula && (token2.TokenType == TokenType.Function))
                {
                    this.WriteParsedToken(writer3, token2, TokenClass.Array, linkTable, ref num2, false, Ptg.None);
                }
                else
                {
                    this.WriteParsedToken(writer3, token2, tokenClass, linkTable, ref num2, false, Ptg.None);
                }
                if (this.isDefinedNameFormula)
                {
                    this.namedExpression = false;
                    if (token2.TokenType == TokenType.Name)
                    {
                        this.isNamedReference = false;
                    }
                }
                writer.Write((byte) 0x19);
                writer.Write((byte) 8);
                byte[] buffer2 = stream2.ToArray();
                writer.Write((short) (buffer2.Length + 7));
                writer.Write(buffer2);
                writer.Write((byte) 0x19);
                writer.Write((byte) 8);
                writer.Write((short) 3);
                writer.Write((byte) 0x22);
                writer.Write((byte) 3);
                writer.Write((short) 1);
            }
            else
            {
                writer.Write((byte) 0x19);
                writer.Write((byte) 8);
                writer.Write((short) 3);
                writer.Write((byte) 0x22);
                writer.Write((byte) 2);
                writer.Write((short) 1);
            }
        }

        private void WriteName(BinaryWriter writer, string name, TokenClass tokenClass, LinkTable linkTable)
        {
            int definedNameIndex = linkTable.GetDefinedNameIndex(name);
            if (definedNameIndex != -1)
            {
                if ((tokenClass == TokenClass.Reference) || this._isDataValidationFormula)
                {
                    writer.Write((byte) 0x23);
                }
                else if (tokenClass == TokenClass.Array)
                {
                    writer.Write((byte) 0x63);
                }
                else
                {
                    writer.Write((byte) 0x43);
                }
                writer.Write((short) (definedNameIndex + 1));
                writer.Write((short) 0);
            }
            else
            {
                tokenClass = TokenClass.Reference;
                if (!name.Contains("!"))
                {
                    int num16 = linkTable.GetDefinedNameIndex(name, -1);
                    if (tokenClass == TokenClass.Reference)
                    {
                        if (this._isDataValidationFormula)
                        {
                            writer.Write((byte) 0x23);
                        }
                        else
                        {
                            writer.Write((byte) 0x43);
                        }
                    }
                    else if (tokenClass == TokenClass.Array)
                    {
                        writer.Write((byte) 0x63);
                    }
                    else
                    {
                        writer.Write((byte) 0x39);
                    }
                    writer.Write((short) (num16 + 1));
                    writer.Write((short) 0);
                }
                else
                {
                    int num6;
                    int num7;
                    int index = name.IndexOf("!");
                    string str = name.Substring(0, index);
                    string str2 = name.Substring(index + 1);
                    if (!str2.StartsWith("#REF!"))
                    {
                        if (((str.Length > 1) && (str[0] == '\'')) && (str[str.Length - 1] == '\''))
                        {
                            str = str.Substring(1, str.Length - 2);
                        }
                        num6 = -1;
                        if (!linkTable.InternalSheetNames.Contains(str))
                        {
                            if (str != null)
                            {
                                if ((str == "[0]") || (str.IndexOf(']') == -1))
                                {
                                    if ((linkTable.ExternalNamedCellRanges == null) || !linkTable.ExternalNamedCellRanges.ContainsKey(str))
                                    {
                                        this.WriteError(writer, ExcelCalcError.WrongFunctionOrRangeName);
                                    }
                                    else
                                    {
                                        List<IName> list2 = null;
                                        linkTable.ExternalNamedCellRanges.TryGetValue(str, out list2);
                                        if (list2 != null)
                                        {
                                            for (int k = 0; k < list2.Count; k++)
                                            {
                                                if (list2[k].Name == str2)
                                                {
                                                    definedNameIndex = k;
                                                    break;
                                                }
                                            }
                                        }
                                        for (int j = 0; j < linkTable.SupBooks.Count; j++)
                                        {
                                            if (linkTable.SupBooks[j].FileName == str)
                                            {
                                                ExcelExternSheet sheet7 = new ExcelExternSheet {
                                                    supBookIndex = j,
                                                    beginSheetIndex = -2,
                                                    endSheetIndex = -2
                                                };
                                                linkTable.ExternalSheets.Add(sheet7);
                                                num6 = linkTable.ExternalSheets.Count - 1;
                                                break;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    string[] strArray = str.Split(new char[] { ']' });
                                    string str3 = strArray[1];
                                    string str4 = strArray[0];
                                    int length = str4.LastIndexOf('[');
                                    if (length != -1)
                                    {
                                        str4 = str4.Substring(0, length) + str4.Substring(length + 1);
                                    }
                                    List<IName> list = null;
                                    linkTable.ExternalNamedCellRanges.TryGetValue(str4, out list);
                                    int num11 = -1;
                                    for (int m = 0; m < linkTable.SupBooks.Count; m++)
                                    {
                                        if (linkTable.SupBooks[m].FileName == str4)
                                        {
                                            if (((linkTable.SupBooks[m].SheetNames != null) && !string.IsNullOrWhiteSpace(str3)) && linkTable.SupBooks[m].SheetNames.Contains(str3))
                                            {
                                                ExcelExternSheet sheet5 = new ExcelExternSheet {
                                                    supBookIndex = m
                                                };
                                                num11 = linkTable.SupBooks[m].SheetNames.IndexOf(str3);
                                                sheet5.beginSheetIndex = num11;
                                                sheet5.endSheetIndex = num11;
                                                linkTable.ExternalSheets.Add(sheet5);
                                                num6 = linkTable.ExternalSheets.Count - 1;
                                                break;
                                            }
                                            if (string.IsNullOrWhiteSpace(str3))
                                            {
                                                ExcelExternSheet sheet6 = new ExcelExternSheet {
                                                    supBookIndex = m,
                                                    beginSheetIndex = -2,
                                                    endSheetIndex = -2
                                                };
                                                linkTable.ExternalSheets.Add(sheet6);
                                                num6 = linkTable.ExternalSheets.Count - 1;
                                                break;
                                            }
                                        }
                                    }
                                    if (list != null)
                                    {
                                        for (int n = 0; n < list.Count; n++)
                                        {
                                            if ((list[n].Name == str2) && (list[n].Index == num11))
                                            {
                                                definedNameIndex = n;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            if (tokenClass == TokenClass.Reference)
                            {
                                writer.Write((byte) 0x59);
                            }
                            else if (tokenClass == TokenClass.Array)
                            {
                                writer.Write((byte) 0x79);
                            }
                            else
                            {
                                writer.Write((byte) 0x39);
                            }
                            writer.Write((short) ((short) num6));
                            writer.Write((short) (definedNameIndex + 1));
                            writer.Write((short) 0);
                            return;
                        }
                        num7 = linkTable.GetDefinedNameIndex(str2, linkTable.InternalSheetNames.IndexOf(str));
                        for (int i = 0; i < linkTable.SupBooks.Count; i++)
                        {
                            ExcelSupBook book3 = linkTable.SupBooks[i];
                            if ((book3.IsSelfReferenced && (book3.SheetNames != null)) && (book3.SheetNames.IndexOf(str) != -1))
                            {
                                for (int num9 = linkTable.ExternalSheets.Count - 1; num9 >= 0; num9--)
                                {
                                    ExcelExternSheet sheet3 = linkTable.ExternalSheets[num9];
                                    if (((sheet3.supBookIndex == i) && (sheet3.beginSheetIndex == -2)) && (sheet3.endSheetIndex == -2))
                                    {
                                        num6 = num9;
                                        break;
                                    }
                                }
                                if (num6 == -1)
                                {
                                    ExcelExternSheet sheet4 = new ExcelExternSheet {
                                        supBookIndex = i,
                                        beginSheetIndex = -2,
                                        endSheetIndex = -2
                                    };
                                    linkTable.ExternalSheets.Add(sheet4);
                                    num6 = linkTable.ExternalSheets.Count - 1;
                                }
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (!linkTable.InternalSheetNames.Contains(str))
                        {
                            for (int num5 = 0; num5 < linkTable.SupBooks.Count; num5++)
                            {
                                ExcelSupBook book2 = linkTable.SupBooks[num5];
                                if (book2.IsSelfReferenced && (book2.SheetNames != null))
                                {
                                    ExcelExternSheet sheet2 = new ExcelExternSheet {
                                        supBookIndex = num5,
                                        beginSheetIndex = -2,
                                        endSheetIndex = -2
                                    };
                                    linkTable.ExternalSheets.Add(sheet2);
                                    this.externSheetIndex = linkTable.ExternalSheets.Count - 1;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            linkTable.GetDefinedNameIndex(str2, linkTable.InternalSheetNames.IndexOf(str));
                            for (int num3 = 0; num3 < linkTable.SupBooks.Count; num3++)
                            {
                                ExcelSupBook book = linkTable.SupBooks[num3];
                                if (book.IsSelfReferenced && (book.SheetNames != null))
                                {
                                    int num4 = book.SheetNames.IndexOf(str);
                                    ExcelExternSheet sheet = new ExcelExternSheet {
                                        supBookIndex = num3,
                                        beginSheetIndex = (num4 == -1) ? -2 : num4,
                                        endSheetIndex = (num4 == -1) ? -2 : num4
                                    };
                                    linkTable.ExternalSheets.Add(sheet);
                                    this.externSheetIndex = linkTable.ExternalSheets.Count - 1;
                                    break;
                                }
                            }
                            writer.Write((byte) 60);
                            writer.Write((short) ((short) this.externSheetIndex));
                            writer.Write((short) 0);
                            writer.Write((short) 0);
                            return;
                        }
                        writer.Write((byte) 60);
                        writer.Write((short) ((short) this.externSheetIndex));
                        writer.Write((short) 0);
                        writer.Write((short) 0);
                        return;
                    }
                    if (tokenClass == TokenClass.Reference)
                    {
                        writer.Write((byte) 0x59);
                    }
                    else if (tokenClass == TokenClass.Array)
                    {
                        writer.Write((byte) 0x79);
                    }
                    else
                    {
                        writer.Write((byte) 0x39);
                    }
                    writer.Write((short) ((short) num6));
                    writer.Write((short) (num7 + 1));
                    writer.Write((short) 0);
                }
            }
        }

        private void WriteParentheses(BinaryWriter writer)
        {
            writer.Write((byte) 0x15);
        }

        private void WriteParsedToken(BinaryWriter bw, ParsedToken token, TokenClass tokenClass, LinkTable linkTable, ref int extraDataLength, bool isParemeter = false, Ptg ptgType = 0)
        {
            ParsedToken token2;
            switch (token.TokenType)
            {
                case TokenType.Double:
                    if (token.Value == null)
                    {
                        this.WriteDouble(bw, 0.0);
                        return;
                    }
                    this.WriteDouble(bw, (double) ((double) token.Value));
                    return;

                case TokenType.String:
                    this.WriteString(bw, (string) ((string) token.Value));
                    return;

                case TokenType.Error:
                    this.WriteError(bw, (ExcelCalcError) token.Value);
                    return;

                case TokenType.Boolean:
                    this.WriteBoolean(bw, (bool) ((bool) token.Value));
                    return;

                case TokenType.Array:
                    this.WriteArray(bw, token.Value, ref extraDataLength, !isParemeter, ptgType);
                    return;

                case TokenType.Function:
                case TokenType.UndefinedFunction:
                    this.ProcessFunction(bw, token, tokenClass, linkTable, ref extraDataLength, ptgType);
                    return;

                case TokenType.Name:
                    this.WriteName(bw, (string) ((string) token.Value), tokenClass, linkTable);
                    return;

                case TokenType.Reference:
                    this.WriteReference(bw, token.Value, linkTable, tokenClass);
                    return;

                case TokenType.ExReference:
                    this.WriteExReference(bw, token.Value, tokenClass, linkTable);
                    return;

                case TokenType.Parentheses:
                {
                    token2 = (ParsedToken) token.Value;
                    MemoryStream stream2 = new MemoryStream();
                    BinaryWriter writer2 = new BinaryWriter((Stream) stream2);
                    int num = 0;
                    this.WriteParsedToken(writer2, token2, tokenClass, linkTable, ref num, false, Ptg.None);
                    extraDataLength += num;
                    byte[] buffer2 = stream2.ToArray();
                    bw.Write(buffer2, 0, buffer2.Length - num);
                    this.WriteParentheses(bw);
                    bw.Write(buffer2, buffer2.Length - num, num);
                    return;
                }
                case TokenType.UnaryOperation:
                {
                    token2 = (ParsedToken) token.Value;
                    MemoryStream stream3 = new MemoryStream();
                    BinaryWriter writer3 = new BinaryWriter((Stream) stream3);
                    int num2 = 0;
                    this.WriteParsedToken(writer3, token2, TokenClass.Value, linkTable, ref num2, false, Ptg.None);
                    byte[] buffer3 = stream3.ToArray();
                    bw.Write(buffer3, 0, buffer3.Length);
                    this.WriteUnaryOperation(bw, token.Token);
                    return;
                }
                case TokenType.BinaryOperation:
                {
                    MemoryStream stream4 = new MemoryStream();
                    BinaryWriter writer4 = new BinaryWriter((Stream) stream4);
                    int num3 = 0;
                    TokenClass reference = TokenClass.Value;
                    if (((token.Token == ",") || (token.Token == " ")) || (token.Token == ":"))
                    {
                        reference = TokenClass.Reference;
                    }
                    ParsedToken[] tokenArray = (ParsedToken[]) token.Value;
                    if (this._isArrayFormula && ((tokenArray[0].TokenType == TokenType.Name) || (tokenArray[0].TokenType == TokenType.Function)))
                    {
                        reference = TokenClass.Array;
                    }
                    if (tokenArray[0].TokenType == TokenType.Array)
                    {
                        reference = TokenClass.Array;
                    }
                    if (tokenClass == TokenClass.Array)
                    {
                        reference = TokenClass.Array;
                    }
                    Ptg arrayA = ptgType;
                    if ((reference == TokenClass.Array) && this.isDefinedNameFormula)
                    {
                        arrayA = Ptg.ArrayA;
                    }
                    this.WriteParsedToken(writer4, tokenArray[0], reference, linkTable, ref num3, isParemeter, arrayA);
                    TokenClass array = TokenClass.Value;
                    if (((token.Token == ",") || (token.Token == " ")) || (token.Token == ":"))
                    {
                        array = TokenClass.Reference;
                    }
                    if (this._isArrayFormula && ((tokenArray[1].TokenType == TokenType.Name) || (tokenArray[1].TokenType == TokenType.Function)))
                    {
                        array = TokenClass.Array;
                    }
                    if (tokenArray[1].TokenType == TokenType.Array)
                    {
                        array = TokenClass.Array;
                    }
                    if (tokenClass == TokenClass.Array)
                    {
                        array = TokenClass.Array;
                    }
                    arrayA = ptgType;
                    if ((array == TokenClass.Array) && this.isDefinedNameFormula)
                    {
                        arrayA = Ptg.ArrayA;
                    }
                    MemoryStream stream5 = new MemoryStream();
                    BinaryWriter writer5 = new BinaryWriter((Stream) stream5);
                    int num4 = 0;
                    this.WriteParsedToken(writer5, ((ParsedToken[]) token.Value)[1], array, linkTable, ref num4, isParemeter, arrayA);
                    byte[] buffer4 = stream4.ToArray();
                    byte[] buffer5 = stream5.ToArray();
                    bw.Write(buffer4, 0, buffer4.Length);
                    bw.Write(buffer5, 0, buffer5.Length);
                    this.WriteBinaryOperation(bw, token.Token);
                    return;
                }
                case TokenType.BinaryReferenceOperation:
                {
                    MemoryStream stream = new MemoryStream();
                    BinaryWriter writer = new BinaryWriter((Stream) stream);
                    SubExternalRangeExpression expression = token.Value as SubExternalRangeExpression;
                    this.WriteExReference(writer, expression.ExternalReference1, TokenClass.Reference, linkTable);
                    this.WriteExReference(writer, expression.ExternalReference2, TokenClass.Reference, linkTable);
                    this.WriteBinaryOperation(writer, expression.RangeOperator);
                    writer.Dispose();
                    byte[] buffer = stream.ToArray();
                    bw.Write((byte) 0x29);
                    bw.Write((ushort) buffer.Length);
                    bw.Write(buffer);
                    return;
                }
                case TokenType.MissArg:
                    bw.Write((byte) 0x16);
                    break;

                case TokenType.SubExpression:
                case TokenType.UnKnown:
                    break;

                default:
                    return;
            }
        }

        private void WriteReference(BinaryWriter writer, object value, LinkTable linkTable, TokenClass tokenClass)
        {
            bool flag4;
            int num12;
            if (!(value is RangeExpression))
            {
                if (value is RangeExprssion2)
                {
                    RangeExprssion2 exprssion = value as RangeExprssion2;
                    if (exprssion.isWholeRow)
                    {
                        if (tokenClass == TokenClass.Value)
                        {
                            if (this._isConditionalFormatFormula)
                            {
                                writer.Write((byte) 0x4d);
                            }
                            else
                            {
                                writer.Write((byte) 0x45);
                            }
                        }
                        else if (this._isConditionalFormatFormula)
                        {
                            writer.Write((byte) 0x2d);
                        }
                        else
                        {
                            writer.Write((byte) 0x25);
                        }
                        writer.Write((short) ((short) exprssion.rowFirst));
                        writer.Write((short) ((short) exprssion.rowLast));
                        int num14 = 0;
                        if (exprssion.isRowFirstRel)
                        {
                            num14 |= 0x80;
                        }
                        int num15 = 0;
                        if (exprssion.isRowLastRel)
                        {
                            num15 |= 0x80;
                        }
                        writer.Write((byte) 0);
                        writer.Write((byte) ((byte) num14));
                        writer.Write((byte) 0xff);
                        writer.Write((byte) ((byte) num15));
                        return;
                    }
                    if (exprssion.isWholeColumn)
                    {
                        if (tokenClass == TokenClass.Value)
                        {
                            if (this._isConditionalFormatFormula)
                            {
                                writer.Write((byte) 0x4d);
                            }
                            else
                            {
                                writer.Write((byte) 0x45);
                            }
                        }
                        else if (this._isConditionalFormatFormula)
                        {
                            writer.Write((byte) 0x2d);
                        }
                        else
                        {
                            writer.Write((byte) 0x25);
                        }
                        writer.Write((short) 0);
                        writer.Write((ushort) 0xffff);
                        int num16 = 0;
                        if (exprssion.isColumnFirstRel)
                        {
                            num16 |= 0x40;
                        }
                        int num17 = 0;
                        if (exprssion.isColumnLastRel)
                        {
                            num17 |= 0x40;
                        }
                        writer.Write((byte) ((byte) exprssion.colFirst));
                        writer.Write((byte) ((byte) num16));
                        writer.Write((byte) ((byte) exprssion.colLast));
                        writer.Write((byte) ((byte) num17));
                        return;
                    }
                    if (this._isConditionalFormatFormula)
                    {
                        writer.Write((byte) 0x2d);
                        int rowFirst = exprssion.rowFirst;
                        int rowLast = exprssion.rowLast;
                        int colFirst = exprssion.colFirst;
                        int colLast = exprssion.colLast;
                        if (exprssion.isRowFirstRel)
                        {
                            rowFirst -= this.row;
                        }
                        if (rowFirst < 0)
                        {
                            rowFirst += 0xffff;
                        }
                        if (exprssion.isRowLastRel)
                        {
                            rowLast -= this.row;
                        }
                        if (rowLast < 0)
                        {
                            rowLast += 0xffff;
                        }
                        if (exprssion.isColumnFirstRel)
                        {
                            colFirst -= this.column;
                        }
                        if (colFirst < 0)
                        {
                            colFirst += 0x100;
                        }
                        if (exprssion.isColumnLastRel)
                        {
                            colLast -= this.column;
                        }
                        if (colLast < 0)
                        {
                            colLast += 0x100;
                        }
                        writer.Write((short) ((short) rowFirst));
                        writer.Write((short) ((short) rowLast));
                        int num22 = 0;
                        if (exprssion.isRowFirstRel)
                        {
                            num22 |= 0x8000;
                        }
                        if (exprssion.isColumnFirstRel)
                        {
                            num22 |= 0x4000;
                        }
                        num22 |= colFirst & 0x3fff;
                        writer.Write((short) ((short) num22));
                        int num23 = 0;
                        if (exprssion.isRowLastRel)
                        {
                            num23 |= 0x8000;
                        }
                        if (exprssion.isColumnLastRel)
                        {
                            num23 |= 0x4000;
                        }
                        num23 |= colLast & 0x3fff;
                        writer.Write((short) ((short) num23));
                        return;
                    }
                    writer.Write((byte) 0x25);
                    writer.Write((short) ((short) exprssion.rowFirst));
                    writer.Write((short) ((short) exprssion.rowLast));
                    int num24 = 0;
                    if (exprssion.isRowFirstRel)
                    {
                        num24 |= 0x8000;
                    }
                    if (exprssion.isColumnFirstRel)
                    {
                        num24 |= 0x4000;
                    }
                    num24 |= exprssion.colFirst & 0x3fff;
                    writer.Write((short) ((short) num24));
                    int num25 = 0;
                    if (exprssion.isRowLastRel)
                    {
                        num25 |= 0x8000;
                    }
                    if (exprssion.isColumnLastRel)
                    {
                        num25 |= 0x4000;
                    }
                    num25 |= exprssion.colLast & 0x3fff;
                    writer.Write((short) ((short) num25));
                }
                return;
            }
            RangeExpression expression = value as RangeExpression;
            ushort num = 0;
            ushort row = 0;
            ushort column = 0;
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            if ((expression.Column + expression.ColumnCount) > 0x100)
            {
                flag3 = true;
            }
            if ((expression.Row + expression.RowCount) > 0x10000)
            {
                flag3 = true;
            }
            if (!expression.RowRelative && (expression.Row == -1))
            {
                flag2 = true;
            }
            if (!expression.ColumnRelative && (expression.Column == -1))
            {
                flag = true;
            }
            if (!flag2 && !flag3)
            {
                int num4 = expression.RowRelative ? (expression.Row + this.row) : expression.Row;
                if (!this._isConditionalFormatFormula)
                {
                    if (!expression.RowRelative)
                    {
                        row = (ushort) num4;
                    }
                    else if ((0 <= num4) && (num4 <= 0xffff))
                    {
                        row = (ushort) num4;
                    }
                    else if (expression.RowRelative && (num4 < 0))
                    {
                        row = (ushort) num4;
                    }
                    else
                    {
                        flag3 = true;
                    }
                }
                else
                {
                    row = (ushort) expression.Row;
                }
            }
            if (!flag && !flag3)
            {
                int num5 = expression.ColumnRelative ? (expression.Column + this.column) : expression.Column;
                if (!this._isConditionalFormatFormula)
                {
                    if (!expression.ColumnRelative)
                    {
                        column = (ushort) num5;
                    }
                    else if ((0 <= num5) && (num5 < 0x100))
                    {
                        column = (ushort) num5;
                    }
                    else if (expression.ColumnRelative && (num5 < 0))
                    {
                        column = (ushort) num5;
                    }
                    else
                    {
                        flag3 = true;
                    }
                }
                else
                {
                    column = (ushort) expression.Column;
                }
            }
            if (flag3)
            {
                byte[] buffer = new byte[] { 0x1c, 0x17 };
                writer.Write(buffer);
                return;
            }
            if ((expression.isEntireColumn || (expression.RowCount < 1)) && (expression.isEntrieRow || (expression.ColumnCount < 1)))
            {
                if (expression.isEntrieRow)
                {
                    if (tokenClass == TokenClass.Value)
                    {
                        if (this._isConditionalFormatFormula)
                        {
                            writer.Write((byte) 0x4d);
                        }
                        else
                        {
                            writer.Write((byte) 0x45);
                        }
                    }
                    else if (this._isConditionalFormatFormula)
                    {
                        writer.Write((byte) 0x2d);
                    }
                    else
                    {
                        writer.Write((byte) 0x25);
                    }
                    if (expression.RowRelative)
                    {
                        writer.Write((short) (expression.Row + this.row));
                        writer.Write((short) (((expression.Row + this.row) + expression.RowCount) - 1));
                    }
                    else
                    {
                        writer.Write((short) ((short) expression.Row));
                        writer.Write((short) ((expression.Row + expression.RowCount) - 1));
                    }
                    int num10 = 0;
                    if (expression.RowRelative)
                    {
                        num10 |= 0x80;
                    }
                    else
                    {
                        num10 |= 0x40;
                    }
                    writer.Write((byte) 0);
                    writer.Write((byte) ((byte) num10));
                    writer.Write((byte) 0xff);
                    writer.Write((byte) ((byte) num10));
                    return;
                }
                if (expression.isEntireColumn)
                {
                    if (tokenClass == TokenClass.Value)
                    {
                        if (this._isConditionalFormatFormula)
                        {
                            writer.Write((byte) 0x4d);
                        }
                        else
                        {
                            writer.Write((byte) 0x45);
                        }
                    }
                    else if (tokenClass == TokenClass.Reference)
                    {
                        if (this._isConditionalFormatFormula)
                        {
                            writer.Write((byte) 0x2d);
                        }
                        else
                        {
                            writer.Write((byte) 0x25);
                        }
                    }
                    else if (tokenClass == TokenClass.Array)
                    {
                        writer.Write((byte) 0x65);
                    }
                    writer.Write((short) 0);
                    writer.Write((ushort) 0xffff);
                    int num11 = 0;
                    if (expression.ColumnRelative)
                    {
                        num11 |= 0x40;
                    }
                    if (expression.ColumnRelative)
                    {
                        writer.Write((byte) (expression.Column + this.column));
                    }
                    else
                    {
                        writer.Write((byte) ((byte) expression.Column));
                    }
                    writer.Write((byte) ((byte) num11));
                    if (expression.ColumnRelative)
                    {
                        writer.Write((byte) (((expression.Column + this.column) + expression.ColumnCount) - 1));
                    }
                    else
                    {
                        writer.Write((byte) ((byte) expression.Column));
                    }
                    writer.Write((byte) ((byte) num11));
                    return;
                }
                if ((expression.RowCount == 1) && (expression.ColumnCount == 1))
                {
                    if (!flag2)
                    {
                        if (!this._isConditionalFormatFormula)
                        {
                            row = (ushort)((expression.Row + expression.RowCount) - 1);
                            row = expression.RowRelative ? ((ushort) (row + this.row)) : ((ushort) row);
                        }
                    }
                    else
                    {
                        row = 0xffff;
                    }
                    if (!flag)
                    {
                        if (!this._isConditionalFormatFormula)
                        {
                            column = (ushort)((expression.Column + expression.ColumnCount) - 1);
                            column = expression.ColumnRelative ? ((ushort) (column + this.column)) : ((ushort) column);
                        }
                    }
                    else
                    {
                        column = 0xff;
                    }
                }
                flag4 = false;
                if (!this.namedExpression)
                {
                    if ((tokenClass & TokenClass.Offset) == TokenClass.Offset)
                    {
                        flag4 = true;
                        tokenClass ^= TokenClass.Offset;
                    }
                    if (tokenClass == TokenClass.Reference)
                    {
                        if (flag4 || this._isConditionalFormatFormula)
                        {
                            writer.Write((byte) 0x2c);
                        }
                        else if (this._isArrayFormula)
                        {
                            writer.Write((byte) 100);
                        }
                        else
                        {
                            writer.Write((byte) 0x24);
                        }
                    }
                    else if (tokenClass == TokenClass.Value)
                    {
                        if (flag4 || this._isConditionalFormatFormula)
                        {
                            writer.Write((byte) 0x4c);
                        }
                        else if (this._isArrayFormula)
                        {
                            writer.Write((byte) 100);
                        }
                        else
                        {
                            writer.Write((byte) 0x44);
                        }
                    }
                    else if (tokenClass == TokenClass.Array)
                    {
                        if (flag4 || this._isConditionalFormatFormula)
                        {
                            writer.Write((byte) 0x6c);
                        }
                        else
                        {
                            writer.Write((byte) 100);
                        }
                    }
                    goto Label_087B;
                }
                if (this.isNamedReference)
                {
                    writer.Write((byte) 0x3a);
                }
                else
                {
                    writer.Write((byte) 0x7a);
                }
                if (this.sheet == -1)
                {
                    ExcelExternSheet sheet4 = new ExcelExternSheet {
                        beginSheetIndex = 0,
                        endSheetIndex = 0
                    };
                    linkTable.ExternalSheets.Add(sheet4);
                    writer.Write((int) (((short) linkTable.ExternalSheets.Count) - 1));
                }
                num12 = -1;
                for (int i = 0; i < linkTable.ExternalSheets.Count; i++)
                {
                    ExcelExternSheet sheet5 = linkTable.ExternalSheets[i];
                    if ((sheet5.beginSheetIndex == this.sheet) && (sheet5.endSheetIndex == this.sheet))
                    {
                        this.externNameIndex = i;
                        break;
                    }
                }
            }
            else
            {
                int num6;
                int num7;
                if (!flag2)
                {
                    num6 = (expression.Row + expression.RowCount) - 1;
                    num6 = expression.RowRelative ? (num6 + this.row) : num6;
                }
                else
                {
                    num6 = 0xffff;
                }
                if (!flag)
                {
                    num7 = (expression.Column + expression.ColumnCount) - 1;
                    num7 = expression.ColumnRelative ? (num7 + this.column) : num7;
                }
                else
                {
                    num7 = 0xff;
                }
                if (((0 > num6) || (num6 > 0xffff)) || ((0 > num7) || (num7 >= 0x100)))
                {
                    flag3 = true;
                    return;
                }
                if (!this.namedExpression)
                {
                    if (tokenClass == TokenClass.Reference)
                    {
                        if (this._isConditionalFormatFormula)
                        {
                            writer.Write((byte) 0x2d);
                        }
                        else
                        {
                            writer.Write((byte) 0x25);
                        }
                    }
                    else if (tokenClass == TokenClass.Value)
                    {
                        if (this._isConditionalFormatFormula)
                        {
                            writer.Write((byte) 0x4d);
                        }
                        else
                        {
                            writer.Write((byte) 0x45);
                        }
                    }
                    else if (tokenClass == TokenClass.Array)
                    {
                        writer.Write((byte) 0x65);
                    }
                }
                else
                {
                    if (this.isNamedReference)
                    {
                        writer.Write((byte) 0x3b);
                    }
                    else
                    {
                        writer.Write((byte) 0x7b);
                    }
                    int excelExternSheetIndex = -1;
                    for (int j = 0; j < linkTable.ExternalSheets.Count; j++)
                    {
                        ExcelExternSheet sheet = linkTable.ExternalSheets[j];
                        if ((sheet.beginSheetIndex == this.sheet) && (sheet.endSheetIndex == this.sheet))
                        {
                            this.externNameIndex = j;
                            break;
                        }
                    }
                    if (this.externNameIndex == -1)
                    {
                        ExcelSupBook book = new ExcelSupBook {
                            IsCurrentSheetSupBook = true,
                            SheetCount = 0
                        };
                        linkTable.SupBooks.Add(book);
                        ExcelExternSheet sheet2 = new ExcelExternSheet {
                            supBookIndex = linkTable.SupBooks.Count - 1,
                            beginSheetIndex = -2,
                            endSheetIndex = -2
                        };
                        excelExternSheetIndex = linkTable.GetExcelExternSheetIndex(sheet2);
                    }
                    writer.Write((short) (excelExternSheetIndex + (this.hasExternNames ? 1 : 0)));
                }
                writer.Write(row);
                if (this._isConditionalFormatFormula)
                {
                    row = expression.RowRelative ? ((ushort) (num6 - this.row)) : ((ushort) num6);
                }
                else
                {
                    row = (ushort) num6;
                }
                writer.Write(row);
                num = (ushort)(((expression.RowRelative ? 0x8000 : 0) + (expression.ColumnRelative ? 0x4000 : 0)) + (column & 0x3fff));
                writer.Write(num);
                if (this._isConditionalFormatFormula)
                {
                    column = expression.ColumnRelative ? ((ushort) (num7 - this.column)) : ((ushort) num7);
                }
                else
                {
                    column = (ushort) num7;
                }
                num = (ushort)(((expression.RowRelative ? 0x8000 : 0) + (expression.ColumnRelative ? 0x4000 : 0)) + (column & 0x3fff));
                writer.Write(num);
                return;
            }
            if (this.externNameIndex == -1)
            {
                ExcelSupBook book2 = new ExcelSupBook {
                    IsCurrentSheetSupBook = true,
                    IsSelfReferenced = false,
                    SheetCount = 0
                };
                linkTable.SupBooks.Add(book2);
                ExcelExternSheet sheet6 = new ExcelExternSheet {
                    supBookIndex = linkTable.SupBooks.Count - 1,
                    beginSheetIndex = -2,
                    endSheetIndex = -2
                };
                num12 = linkTable.GetExcelExternSheetIndex(sheet6);
            }
            writer.Write((short) (num12 + (this.hasExternNames ? 1 : 0)));
        Label_087B:
            if (flag4)
            {
                writer.Write(expression.RowRelative ? ((ushort) ((ushort) expression.Row)) : ((ushort) 0));
                num = (ushort)(((expression.RowRelative ? 0x8000 : 0) + (expression.ColumnRelative ? 0x4000 : 0)) + ((expression.ColumnRelative ? expression.Column : 0) & 0xff));
            }
            else
            {
                writer.Write(row);
                num = (ushort)(((expression.RowRelative ? 0x8000 : 0) + (expression.ColumnRelative ? 0x4000 : 0)) + (column & 0xff));
            }
            writer.Write(num);
        }

        private void WriteSpecialAttributeFunction(BinaryWriter writer, ParsedToken token, TokenClass tokenClass, LinkTable linkTable, ref int extraDataLength, Ptg ptgType = 0)
        {
            writer.Write((byte) 0x19);
            writer.Write((byte) 1);
            writer.Write((short) 0);
            ParsedToken[] tokenArray = token.Value as ParsedToken[];
            for (int i = 0; i < tokenArray.Length; i++)
            {
                if (i == 0)
                {
                    this.WriteArgument(writer, tokenArray[i], TokenClass.Reference, linkTable, ref extraDataLength, ptgType);
                }
                else
                {
                    this.WriteArgument(writer, tokenArray[i], TokenClass.Value, linkTable, ref extraDataLength, ptgType);
                }
            }
            this.WriteFunctionImp(writer, token, this.GetXlf(token.Token), tokenArray.Length, tokenClass, ref extraDataLength, ptgType);
        }

        private int WriteString(BinaryWriter writer, string token)
        {
            string s = token;
            if (s.StartsWith("\""))
            {
                s = s.Substring(1);
            }
            if (s.EndsWith("\""))
            {
                s = s.Substring(0, s.Length - 1);
            }
            while (s.Contains("\"\""))
            {
                s = s.Replace("\"\"", "\"");
            }
            int length = s.Length;
            int num2 = length;
            bool flag = false;
            for (int i = 0; i < length; i++)
            {
                char ch = s[i];
                if ((ch < '\0') || (ch > '\x007f'))
                {
                    flag = true;
                    num2 *= 2;
                    break;
                }
            }
            writer.Write((byte) 0x17);
            writer.Write((byte) ((byte) length));
            writer.Write(flag ? ((byte) 1) : ((byte) 0));
            if (flag)
            {
                writer.Write(Encoding.Unicode.GetBytes(s));
            }
            else
            {
                writer.Write(s.ToCharArray());
            }
            return (length + 3);
        }

        private void WriteSubExReference(BinaryWriter writer)
        {
        }

        private void WriteSumFunctionWithOneParameter(BinaryWriter writer, ParsedToken token, TokenClass tokenClass, LinkTable linkTable, ref int extraDataLength)
        {
            writer.Write((byte) 0x19);
            writer.Write((byte) 1);
            writer.Write((short) 0);
            ParsedToken[] tokenArray = token.Value as ParsedToken[];
            for (int i = 0; i < 1; i++)
            {
                tokenClass = TokenClass.Value;
                this.WriteArgument(writer, tokenArray[i], tokenClass, linkTable, ref extraDataLength, Ptg.FuncVar);
            }
            writer.Write((byte) 0x19);
            writer.Write((byte) 0x10);
            writer.Write((short) 0);
        }

        private void WriteUnaryOperation(BinaryWriter writer, string token)
        {
            Ptg unaryOperationPtg = this.GetUnaryOperationPtg(token);
            writer.Write((byte) ((byte) unaryOperationPtg));
        }

        public bool HasExternNames
        {
            set { this.hasExternNames = value; }
        }
    }
}

