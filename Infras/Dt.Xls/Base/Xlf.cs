#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Excel Function (XLF) defines.  
    /// These values are unique identifiers used when parsing Excel Formula buffers to determine the current function being referenced.
    /// </summary>
    internal enum Xlf
    {
        /// <summary>
        /// [24] ABS function
        /// </summary>
        [ArgCount(1)]
        Abs = 0x18,
        /// <summary>
        /// [79] ABSREF function
        /// </summary>
        Absref = 0x4f,
        /// <summary>
        /// [99] ACOS function
        /// </summary>
        [ArgCount(1)]
        Acos = 0x63,
        /// <summary>
        /// [233] ACOSH function
        /// </summary>
        [ArgCount(1)]
        Acosh = 0xe9,
        /// <summary>
        /// [94] ACTIVECELL function
        /// </summary>
        ActiveCell = 0x5e,
        /// <summary>
        /// [151] ADDBAR function
        /// </summary>
        AddBar = 0x97,
        /// <summary>
        /// [153] ADDCOMMAND function
        /// </summary>
        AddCommand = 0x99,
        /// <summary>
        /// [152] ADDMENU function
        /// </summary>
        AddMenu = 0x98,
        /// <summary>
        /// [219] ADDRESS function
        /// </summary>
        Address = 0xdb,
        /// <summary>
        /// [253] ADDTOOLBAR function
        /// </summary>
        AddToolbar = 0xfd,
        /// <summary>
        /// [36] AND function
        /// </summary>
        And = 0x24,
        /// <summary>
        /// [262] APPTITLE function
        /// </summary>
        AppTitle = 0x106,
        /// <summary>
        /// [75] AREAS function
        /// </summary>
        Areas = 0x4b,
        /// <summary>
        /// [81] ARGUMENT function
        /// </summary>
        Argument = 0x51,
        /// <summary>
        /// [214] ASC function
        /// </summary>
        Asc = 0xd6,
        /// <summary>
        /// [98] ASIN function
        /// </summary>
        [ArgCount(1)]
        Asin = 0x62,
        /// <summary>
        /// [232] ASINH function
        /// </summary>
        [ArgCount(1)]
        Asinh = 0xe8,
        /// <summary>
        /// [18] ATAN function
        /// </summary>
        [ArgCount(1)]
        Atan = 0x12,
        /// <summary>
        /// [97] ATAN2 function
        /// </summary>
        [ArgCount(2)]
        Atan2 = 0x61,
        /// <summary>
        /// [234] ATANH function
        /// </summary>
        [ArgCount(1)]
        Atanh = 0xea,
        /// <summary>
        /// [269] AVEDEV function
        /// </summary>
        Avedev = 0x10d,
        /// <summary>
        /// [5] AVERAGE function
        /// </summary>
        Average = 5,
        /// <summary>
        /// [361] AVERAGEA function
        /// </summary>
        AverageA = 0x169,
        /// <summary>
        /// [270] BETADIST function
        /// </summary>
        Betadist = 270,
        /// <summary>
        /// [272] BETAINV function
        /// </summary>
        Betainv = 0x110,
        /// <summary>
        /// [273] BINOMDIST function
        /// </summary>
        [ArgCount(4)]
        Binomdist = 0x111,
        /// <summary>
        /// [150] CALL function
        /// </summary>
        Call = 150,
        /// <summary>
        /// [89] CALLER function
        /// </summary>
        Caller = 0x59,
        /// <summary>
        /// [170] CANCELKEY function
        /// </summary>
        CancelKey = 170,
        /// <summary>
        /// [288] CEILING function
        /// </summary>
        [ArgCount(2)]
        Ceiling = 0x120,
        /// <summary>
        /// [125] CELL function
        /// </summary>
        Cell = 0x7d,
        /// <summary>
        /// [111] CHAR function
        /// </summary>
        [ArgCount(1)]
        Char = 0x6f,
        /// <summary>
        /// [155] CHECKCOMMAND function
        /// </summary>
        CheckCommand = 0x9b,
        /// <summary>
        /// [274] CHIDIST function
        /// </summary>
        [ArgCount(2)]
        Chidist = 0x112,
        /// <summary>
        /// [275] CHIINV function
        /// </summary>
        [ArgCount(2)]
        Chiinv = 0x113,
        /// <summary>
        /// [306] CHITEST function
        /// </summary>
        [ArgCount(2)]
        Chitest = 0x132,
        /// <summary>
        /// [100] CHOOSE function
        /// </summary>
        Choose = 100,
        /// <summary>
        /// [162] CLEAN function
        /// </summary>
        [ArgCount(1)]
        Clean = 0xa2,
        /// <summary>
        /// [121] CODE function
        /// </summary>
        [ArgCount(1)]
        Code = 0x79,
        /// <summary>
        /// [9] COLUMN function
        /// </summary>
        Column = 9,
        /// <summary>
        /// [77] COLUMNS function
        /// </summary>
        [ArgCount(1)]
        Columns = 0x4d,
        /// <summary>
        /// [276] COMBIN function
        /// </summary>
        [ArgCount(2)]
        Combin = 0x114,
        /// <summary>
        /// [336] CONCATENATE function
        /// </summary>
        Concatenate = 0x150,
        /// <summary>
        /// [277] CONFIDENCE function
        /// </summary>
        [ArgCount(3)]
        Confidence = 0x115,
        /// <summary>
        /// [307] CORREL function
        /// </summary>
        [ArgCount(2)]
        Correl = 0x133,
        /// <summary>
        /// [16] COS function
        /// </summary>
        [ArgCount(1)]
        Cos = 0x10,
        /// <summary>
        /// [230] COSH function
        /// </summary>
        [ArgCount(1)]
        Cosh = 230,
        /// <summary>
        /// [0] COUNT function
        /// </summary>
        Count = 0,
        /// <summary>
        /// [169] COUNTA function
        /// </summary>
        Counta = 0xa9,
        /// <summary>
        /// [347] COUNTBLANK function
        /// </summary>
        [ArgCount(1)]
        Countblank = 0x15b,
        /// <summary>
        /// [346] COUNTIF function
        /// </summary>
        [ArgCount(2)]
        Countif = 0x15a,
        /// <summary>
        /// [308] COVAR function
        /// </summary>
        [ArgCount(2)]
        Covar = 0x134,
        /// <summary>
        /// [236] CREATEOBJECT function
        /// </summary>
        CreateObject = 0xec,
        /// <summary>
        /// [278] CRITBINOM function
        /// </summary>
        [ArgCount(3)]
        Critbinom = 0x116,
        /// <summary>
        /// [240] CUSTOMREPEAT function
        /// </summary>
        CustomRepeat = 240,
        /// <summary>
        /// [239] CUSTOMUNDO function
        /// </summary>
        CustomUndo = 0xef,
        /// <summary>
        /// [65] DATE function
        /// </summary>
        [ArgCount(3)]
        Date = 0x41,
        /// <summary>
        /// [351] DATEDIF function
        /// </summary>
        [ArgCount(3)]
        Datedif = 0x15f,
        /// <summary>
        /// [352] DATESTRING function
        /// </summary>
        Datestring = 0x160,
        /// <summary>
        /// [140] DATEVALUE function
        /// </summary>
        [ArgCount(1)]
        Datevalue = 140,
        /// <summary>
        /// [42] DAVERAGE function
        /// </summary>
        Daverage = 0x2a,
        /// <summary>
        /// [67] DAY function
        /// </summary>
        [ArgCount(1)]
        Day = 0x43,
        /// <summary>
        /// [220] DAYS360 function
        /// </summary>
        Days360 = 220,
        /// <summary>
        /// [247] DB function
        /// </summary>
        Db = 0xf7,
        /// <summary>
        /// [215] DBCS function
        /// </summary>
        Dbcs = 0xd7,
        /// <summary>
        /// [40] DCOUNT function
        /// </summary>
        Dcount = 40,
        /// <summary>
        /// [199] DCOUNTA function
        /// </summary>
        Dcounta = 0xc7,
        /// <summary>
        /// [14] DBD function
        /// </summary>
        Ddb = 0x90,
        /// <summary>
        /// [343] DEGREES function
        /// </summary>
        [ArgCount(1)]
        Degrees = 0x157,
        /// <summary>
        /// [200] DELETEBAR function
        /// </summary>
        DeleteBar = 200,
        /// <summary>
        /// [159] DELETECOMMAND function
        /// </summary>
        DeleteCommand = 0x9f,
        /// <summary>
        /// [158] DELETEMENU function
        /// </summary>
        DeleteMenu = 0x9e,
        /// <summary>
        /// [254] DELETETOOLBAR function
        /// </summary>
        DeleteToolbar = 0xfe,
        /// <summary>
        /// [90] DEREF function
        /// </summary>
        Deref = 90,
        /// <summary>
        /// [318] DEVSQ function
        /// </summary>
        Devsq = 0x13e,
        /// <summary>
        /// [235] DGET function
        /// </summary>
        Dget = 0xeb,
        /// <summary>
        /// [161] DIALOGBOX function
        /// </summary>
        DialogBox = 0xa1,
        /// <summary>
        /// [123] DIRECTORY function
        /// </summary>
        Directory = 0x7b,
        /// <summary>
        /// [44] DMAX function
        /// </summary>
        Dmax = 0x2c,
        /// <summary>
        /// [43] DMIN function
        /// </summary>
        Dmin = 0x2b,
        /// <summary>
        /// [93] DOCUMENTS function
        /// </summary>
        Documents = 0x5d,
        /// <summary>
        /// [13] DOLLAR function
        /// </summary>
        Dollar = 13,
        /// <summary>
        /// [189] DPRODUCT function
        /// </summary>
        Dproduct = 0xbd,
        /// <summary>
        /// [45] DSTDEV function
        /// </summary>
        Dstdev = 0x2d,
        /// <summary>
        /// [195] DSTDEVP function
        /// </summary>
        Dstdevp = 0xc3,
        /// <summary>
        /// [41] DSUM function
        /// </summary>
        Dsum = 0x29,
        /// <summary>
        /// [47] DVAR function
        /// </summary>
        Dvar = 0x2f,
        /// <summary>
        /// [196] DVARP function
        /// </summary>
        Dvarp = 0xc4,
        /// <summary>
        /// [87] ECHO function
        /// </summary>
        Echo = 0x57,
        /// <summary>
        /// [223] ELSE function
        /// </summary>
        Else = 0xdf,
        /// <summary>
        /// [224] ELSE.IF function
        /// </summary>
        ElseIf = 0xe0,
        /// <summary>
        /// [154] ENABLECOMMAND function
        /// </summary>
        EnableCommand = 0x9a,
        /// <summary>
        /// [265] ENABLETOOL function
        /// </summary>
        EnableTool = 0x109,
        /// <summary>
        /// [225] END.IF function
        /// </summary>
        EndIf = 0xe1,
        /// <summary>
        /// [84] ERROR function
        /// </summary>
        Error = 0x54,
        /// <summary>
        /// [261] ERRORTYPE function
        /// </summary>
        [ArgCount(1)]
        ErrorType = 0x105,
        /// <summary>
        /// [257] EVALUATE function
        /// </summary>
        Evaluate = 0x101,
        /// <summary>
        /// [279] EVEN function
        /// </summary>
        [ArgCount(1)]
        Even = 0x117,
        /// <summary>
        /// [117] EXACT function
        /// </summary>
        [ArgCount(2)]
        Exact = 0x75,
        /// <summary>
        /// [110] EXEC function
        /// </summary>
        Exec = 110,
        /// <summary>
        /// [178] EXECUTE function
        /// </summary>
        Execute = 0xb2,
        /// <summary>
        /// [21] EXP function
        /// </summary>
        [ArgCount(1)]
        Exp = 0x15,
        /// <summary>
        /// [280] EXPONDIST function
        /// </summary>
        [ArgCount(3)]
        Expondist = 280,
        /// <summary>
        /// [0xFF] External or custom function
        /// </summary>
        ExternCustom = 0xff,
        /// <summary>
        /// [184] FACT function
        /// </summary>
        [ArgCount(1)]
        Fact = 0xb8,
        /// <summary>
        /// [35] FALSE function
        /// </summary>
        False = 0x23,
        /// <summary>
        /// [133] FCLOSE function
        /// </summary>
        Fclose = 0x85,
        /// <summary>
        /// [281] FDIST function
        /// </summary>
        [ArgCount(3)]
        Fdist = 0x119,
        /// <summary>
        /// [166] FILES function
        /// </summary>
        Files = 0xa6,
        /// <summary>
        /// [124] FIND function
        /// </summary>
        [ArgCount(3)]
        Find = 0x7c,
        /// <summary>
        /// [205] FINDB function
        /// </summary>
        Findb = 0xcd,
        /// <summary>
        /// [282] FINV function
        /// </summary>
        [ArgCount(3)]
        Finv = 0x11a,
        /// <summary>
        /// [283] FISHER function
        /// </summary>
        [ArgCount(1)]
        Fisher = 0x11b,
        /// <summary>
        /// [284] FISHERINV function
        /// </summary>
        [ArgCount(1)]
        Fisherinv = 0x11c,
        /// <summary>
        /// [14] FIXED function
        /// </summary>
        Fixed = 14,
        /// <summary>
        /// [285] FLOOR function
        /// </summary>
        [ArgCount(2)]
        Floor = 0x11d,
        /// <summary>
        /// [132] FOPEN function
        /// </summary>
        Fopen = 0x84,
        /// <summary>
        /// [226] FOR.CELL function
        /// </summary>
        ForCell = 0xe2,
        /// <summary>
        /// [309] FORECAST function
        /// </summary>
        [ArgCount(3)]
        Forecast = 0x135,
        /// <summary>
        /// [241] FORMULACONVERT function
        /// </summary>
        FormulaConvert = 0xf1,
        /// <summary>
        /// [139] FPOS function
        /// </summary>
        Fpos = 0x8b,
        /// <summary>
        /// [136] FREAD function
        /// </summary>
        Fread = 0x88,
        /// <summary>
        /// [135] FREADIN function
        /// </summary>
        Freadln = 0x87,
        /// <summary>
        /// [252] FREQUENCY function
        /// </summary>
        Frequency = 0xfc,
        /// <summary>
        /// [134] FSIZE function
        /// </summary>
        Fsize = 0x86,
        /// <summary>
        /// [310] FTEST function
        /// </summary>
        [ArgCount(2)]
        Ftest = 310,
        /// <summary>
        /// [57] FV function
        /// </summary>
        Fv = 0x39,
        /// <summary>
        /// [138] FWRITE function
        /// </summary>
        Fwrite = 0x8a,
        /// <summary>
        /// [137] FWRITEIN function
        /// </summary>
        Fwriteln = 0x89,
        /// <summary>
        /// [286] GAMMADIST function
        /// </summary>
        [ArgCount(4)]
        Gammadist = 0x11e,
        /// <summary>
        /// [287] GAMMAINV function
        /// </summary>
        [ArgCount(3)]
        Gammainv = 0x11f,
        /// <summary>
        /// [271] GAMMALN function
        /// </summary>
        [ArgCount(1)]
        Gammaln = 0x10f,
        /// <summary>
        /// [319] GEOMEAN function
        /// </summary>
        Geomean = 0x13f,
        /// <summary>
        /// [182] GETBAR function
        /// </summary>
        GetBar = 0xb6,
        /// <summary>
        /// [185] GETCELL function
        /// </summary>
        GetCell = 0xb9,
        /// <summary>
        /// [160] GETCHARTITEM function
        /// </summary>
        GetChartItem = 160,
        /// <summary>
        /// [145] GETDEF function
        /// </summary>
        GetDef = 0x91,
        /// <summary>
        /// [188] GETDOCUMENT function
        /// </summary>
        GetDocument = 0xbc,
        /// <summary>
        /// [106] GETFORMULA function
        /// </summary>
        GetFormula = 0x6a,
        /// <summary>
        /// [242] GETLINKINFO function
        /// </summary>
        GetLinkInfo = 0xf2,
        /// <summary>
        /// [335] GETMOVIE function
        /// </summary>
        GetMovie = 0x14f,
        /// <summary>
        /// [107] GETNAME function
        /// </summary>
        GetName = 0x6b,
        /// <summary>
        /// [191] GETNOTE function
        /// </summary>
        GetNote = 0xbf,
        /// <summary>
        /// [246] GETOBJECT function
        /// </summary>
        GetObject = 0xf6,
        /// <summary>
        /// [358] GETPIVOTDATA function
        /// </summary>
        GetPivotData = 0x166,
        /// <summary>
        /// [340] GETPIVOTFIELD function
        /// </summary>
        GetPivotField = 340,
        /// <summary>
        /// [341] GETPIVOTITEM function
        /// </summary>
        GetPivotItem = 0x155,
        /// <summary>
        /// [339] GETPIVOTTABLE function
        /// </summary>
        GetPivotTable = 0x153,
        /// <summary>
        /// [259] GETTOOL function
        /// </summary>
        GetTool = 0x103,
        /// <summary>
        /// [258] GETTOOLBAR function
        /// </summary>
        GetToolbar = 0x102,
        /// <summary>
        /// [187] GETWINDOW function
        /// </summary>
        GetWindow = 0xbb,
        /// <summary>
        /// [268] GETWORKBOOK function
        /// </summary>
        GetWorkbook = 0x10c,
        /// <summary>
        /// [186] GETWORKSPACE function
        /// </summary>
        GetWorkspace = 0xba,
        /// <summary>
        /// [53] GOTO function
        /// </summary>
        Goto = 0x35,
        /// <summary>
        /// [245] GROUP function
        /// </summary>
        Group = 0xf5,
        /// <summary>
        /// [52] GROWTH function
        /// </summary>
        Growth = 0x34,
        /// <summary>
        /// [54] HALT function
        /// </summary>
        Halt = 0x36,
        /// <summary>
        /// [320] HARMEAN function
        /// </summary>
        Harmean = 320,
        /// <summary>
        /// [181] HELP function
        /// </summary>
        Help = 0xb5,
        /// <summary>
        /// [101] HLOOKUP function
        /// </summary>
        Hlookup = 0x65,
        /// <summary>
        /// [71] HOUR function
        /// </summary>
        [ArgCount(1)]
        Hour = 0x47,
        /// <summary>
        /// [359] HYPERLINK function
        /// </summary>
        Hyperlink = 0x167,
        /// <summary>
        /// [289] HYPGEOMDIST function
        /// </summary>
        [ArgCount(4)]
        Hypgeomdist = 0x121,
        /// <summary>
        /// [1] IF function
        /// </summary>
        If = 1,
        /// <summary>
        /// [29] INDEX function
        /// </summary>
        Index = 0x1d,
        /// <summary>
        /// [148] INDIRECT function
        /// </summary>
        Indirect = 0x94,
        /// <summary>
        /// [244] INFO function
        /// </summary>
        Info = 0xf4,
        /// <summary>
        /// [175] INITIATE function
        /// </summary>
        Initiate = 0xaf,
        /// <summary>
        /// [104] INPUT function
        /// </summary>
        Input = 0x68,
        /// <summary>
        /// [25] INT function
        /// </summary>
        [ArgCount(1)]
        Int = 0x19,
        /// <summary>
        /// [311] INTERCEPT function
        /// </summary>
        [ArgCount(2)]
        Intercept = 0x137,
        /// <summary>
        /// [167] IPMT function
        /// </summary>
        Ipmt = 0xa7,
        /// <summary>
        /// [62] IRR function
        /// </summary>
        Irr = 0x3e,
        /// <summary>
        /// [129] ISBLANK function
        /// </summary>
        [ArgCount(1)]
        Isblank = 0x81,
        /// <summary>
        /// [126] ISERR function
        /// </summary>
        [ArgCount(1)]
        Iserr = 0x7e,
        /// <summary>
        /// [3] ISERROR function
        /// </summary>
        [ArgCount(1)]
        Iserror = 3,
        /// <summary>
        /// [198] ISLOGICAL function
        /// </summary>
        [ArgCount(1)]
        Islogical = 0xc6,
        /// <summary>
        /// [2] ISNA function
        /// </summary>
        [ArgCount(1)]
        Isna = 2,
        /// <summary>
        /// [190] ISNONTEXT function
        /// </summary>
        [ArgCount(1)]
        Isnontext = 190,
        /// <summary>
        /// [128] ISNUMBER function
        /// </summary>
        [ArgCount(1)]
        Isnumber = 0x80,
        /// <summary>
        /// [350] ISPMT function
        /// </summary>
        Ispmt = 350,
        /// <summary>
        /// [105] ISREF function
        /// </summary>
        [ArgCount(1)]
        Isref = 0x69,
        /// <summary>
        /// [127] ISTEXT function
        /// </summary>
        [ArgCount(1)]
        Istext = 0x7f,
        /// <summary>
        /// [322] KURT function
        /// </summary>
        Kurt = 0x142,
        /// <summary>
        /// [325] LARGE function
        /// </summary>
        [ArgCount(2)]
        Large = 0x145,
        /// <summary>
        /// [238] LASTERROR function
        /// </summary>
        LastError = 0xee,
        /// <summary>
        /// [115] LEFT function
        /// </summary>
        Left = 0x73,
        /// <summary>
        /// [208] LEFTB function
        /// </summary>
        Leftb = 0xd0,
        /// <summary>
        /// [32] LEN function
        /// </summary>
        [ArgCount(1)]
        Len = 0x20,
        /// <summary>
        /// [211] LENB function
        /// </summary>
        Lenb = 0xd3,
        /// <summary>
        /// [49] LINEST function
        /// </summary>
        Linest = 0x31,
        /// <summary>
        /// [103] LINKS function
        /// </summary>
        Links = 0x67,
        /// <summary>
        /// [22] LN function
        /// </summary>
        [ArgCount(1)]
        Ln = 0x16,
        /// <summary>
        /// [109] LOG function
        /// </summary>
        Log = 0x6d,
        /// <summary>
        /// [23] LOG10 function
        /// </summary>
        [ArgCount(1)]
        Log10 = 0x17,
        /// <summary>
        /// [51] LOGEST function
        /// </summary>
        Logest = 0x33,
        /// <summary>
        /// [291] LOGINV function
        /// </summary>
        [ArgCount(3)]
        Loginv = 0x123,
        /// <summary>
        /// [290] LOGNORMDIST function
        /// </summary>
        [ArgCount(3)]
        Lognormdist = 290,
        /// <summary>
        /// [28] LOOKUP function
        /// </summary>
        Lookup = 0x1c,
        /// <summary>
        /// [112] LOWER function
        /// </summary>
        [ArgCount(1)]
        Lower = 0x70,
        /// <summary>
        /// [64] MATCH function
        /// </summary>
        Match = 0x40,
        /// <summary>
        /// [7] MAX function
        /// </summary>
        Max = 7,
        /// <summary>
        /// [362] MAXA function
        /// </summary>
        MaxA = 0x16a,
        /// <summary>
        /// [163] MDETERM function
        /// </summary>
        Mdeterm = 0xa3,
        /// <summary>
        /// [227] MEDIAN function
        /// </summary>
        Median = 0xe3,
        /// <summary>
        /// [31] MID function
        /// </summary>
        [ArgCount(3)]
        Mid = 0x1f,
        /// <summary>
        /// [210] MIDB function
        /// </summary>
        Midb = 210,
        /// <summary>
        /// [6] MIN function
        /// </summary>
        Min = 6,
        /// <summary>
        /// [363] MINA function
        /// </summary>
        MinA = 0x16b,
        /// <summary>
        /// [72] MINUTE function
        /// </summary>
        [ArgCount(1)]
        Minute = 0x48,
        /// <summary>
        /// [164] MINVERSE function
        /// </summary>
        Minverse = 0xa4,
        /// <summary>
        /// [61] MIRR function
        /// </summary>
        [ArgCount(3)]
        Mirr = 0x3d,
        /// <summary>
        /// [165] MMULT function
        /// </summary>
        Mmult = 0xa5,
        /// <summary>
        /// [39] MOD function
        /// </summary>
        [ArgCount(2)]
        Mod = 0x27,
        /// <summary>
        /// [330] MODE function
        /// </summary>
        Mode = 330,
        /// <summary>
        /// [68] MONTH function
        /// </summary>
        [ArgCount(1)]
        Month = 0x44,
        /// <summary>
        /// [334] MOVIECOMMAND function
        /// </summary>
        MovieCommand = 0x14e,
        /// <summary>
        /// [131] N function
        /// </summary>
        [ArgCount(1)]
        N = 0x83,
        /// <summary>
        /// [10] NA function
        /// </summary>
        Na = 10,
        /// <summary>
        /// [122] NAMES function
        /// </summary>
        Names = 0x7a,
        /// <summary>
        /// [292] NEGBINOMDIST function
        /// </summary>
        [ArgCount(3)]
        Negbinomdist = 0x124,
        /// <summary>
        /// [293] NORMDIST function
        /// </summary>
        [ArgCount(4)]
        Normdist = 0x125,
        /// <summary>
        /// [295] NORMINV function
        /// </summary>
        [ArgCount(3)]
        Norminv = 0x127,
        /// <summary>
        /// [294] NORMSDIST function
        /// </summary>
        [ArgCount(1)]
        Normsdist = 0x126,
        /// <summary>
        /// [296] NORMSINV function
        /// </summary>
        [ArgCount(1)]
        Normsinv = 0x128,
        /// <summary>
        /// [38] NOT function
        /// </summary>
        [ArgCount(1)]
        Not = 0x26,
        /// <summary>
        /// [192] NOTE function
        /// </summary>
        Note = 0xc0,
        /// <summary>
        /// [74] NOW function
        /// </summary>
        Now = 0x4a,
        /// <summary>
        /// [58] NPER function
        /// </summary>
        Nper = 0x3a,
        /// <summary>
        /// [11] NPV function
        /// </summary>
        Npv = 11,
        /// <summary>
        /// [353] NUMBERSTRING function
        /// </summary>
        Numberstring = 0x161,
        /// <summary>
        /// [298] ODD function
        /// </summary>
        [ArgCount(1)]
        Odd = 0x12a,
        /// <summary>
        /// [78] OFFSET function
        /// </summary>
        Offset = 0x4e,
        /// <summary>
        /// [355] OPENDIALOG function
        /// </summary>
        OpenDialog = 0x163,
        /// <summary>
        /// [349] OPTIONSLISTSGET function
        /// </summary>
        OptionsListsGet = 0x15d,
        /// <summary>
        /// [37] OR function
        /// </summary>
        Or = 0x25,
        /// <summary>
        /// [248] PAUSE function
        /// </summary>
        Pause = 0xf8,
        /// <summary>
        /// [312] PEARSON function
        /// </summary>
        [ArgCount(2)]
        Pearson = 0x138,
        /// <summary>
        /// [328] PERCENTILE function
        /// </summary>
        [ArgCount(2)]
        Percentile = 0x148,
        /// <summary>
        /// [329] PERCENTRANK function
        /// </summary>
        Percentrank = 0x149,
        /// <summary>
        /// [299] PERMUT function
        /// </summary>
        [ArgCount(2)]
        Permut = 0x12b,
        /// <summary>
        /// [360] PHONETIC function
        /// </summary>
        Phonetic = 360,
        /// <summary>
        /// [19] PI function
        /// </summary>
        Pi = 0x13,
        /// <summary>
        /// [338] PIVOTADDDATA function
        /// </summary>
        PivotAddData = 0x152,
        /// <summary>
        /// [59] PMT function
        /// </summary>
        Pmt = 0x3b,
        /// <summary>
        /// [300] POISSON function
        /// </summary>
        [ArgCount(3)]
        Poisson = 300,
        /// <summary>
        /// [177] POKE function
        /// </summary>
        Poke = 0xb1,
        /// <summary>
        /// [337] POWER function
        /// </summary>
        [ArgCount(2)]
        Power = 0x151,
        /// <summary>
        /// [168] PPMT function
        /// </summary>
        Ppmt = 0xa8,
        /// <summary>
        /// [266] PRESSTOOL function
        /// </summary>
        PressTool = 0x10a,
        /// <summary>
        /// [317] PROB function
        /// </summary>
        Prob = 0x13d,
        /// <summary>
        /// [183] PRODUCT function
        /// </summary>
        Product = 0xb7,
        /// <summary>
        /// [114] PROPER function
        /// </summary>
        [ArgCount(1)]
        Proper = 0x72,
        /// <summary>
        /// [56] PV function
        /// </summary>
        Pv = 0x38,
        /// <summary>
        /// [327] QUARTILE function
        /// </summary>
        [ArgCount(2)]
        Quartile = 0x147,
        /// <summary>
        /// [342] RADIANS function
        /// </summary>
        [ArgCount(1)]
        Radians = 0x156,
        /// <summary>
        /// [63] RAND function
        /// </summary>
        Rand = 0x3f,
        /// <summary>
        /// [216] RANK function
        /// </summary>
        Rank = 0xd8,
        /// <summary>
        /// [60] RATE function
        /// </summary>
        Rate = 60,
        /// <summary>
        /// [146] REFTEXT function
        /// </summary>
        Reftext = 0x92,
        /// <summary>
        /// [149] REGISTER function
        /// </summary>
        Register = 0x95,
        /// <summary>
        /// [267] REGISTERID function
        /// </summary>
        RegisterId = 0x10b,
        /// <summary>
        /// [80] RELREF function
        /// </summary>
        Relref = 80,
        /// <summary>
        /// [156] RENAMECOMMAND function
        /// </summary>
        RenameCommand = 0x9c,
        /// <summary>
        /// [119] REPLACE function
        /// </summary>
        [ArgCount(4)]
        Replace = 0x77,
        /// <summary>
        /// [207] REPLACEB function
        /// </summary>
        Replaceb = 0xcf,
        /// <summary>
        /// [30] REPT function
        /// </summary>
        [ArgCount(2)]
        Rept = 30,
        /// <summary>
        /// [176] REQUEST function
        /// </summary>
        Request = 0xb0,
        /// <summary>
        /// [256] RESETTOOLBAR function
        /// </summary>
        ResetToolbar = 0x100,
        /// <summary>
        /// [180] RESTART function
        /// </summary>
        Restart = 180,
        /// <summary>
        /// [96] RESULT function
        /// </summary>
        Result = 0x60,
        /// <summary>
        /// [251] RESUME function
        /// </summary>
        Resume = 0xfb,
        /// <summary>
        /// [116] RIGHT function
        /// </summary>
        Right = 0x74,
        /// <summary>
        /// [209] RIGHTB function
        /// </summary>
        Rightb = 0xd1,
        /// <summary>
        /// [354] ROMAN function
        /// </summary>
        Roman = 0x162,
        /// <summary>
        /// [27] ROUND function
        /// </summary>
        [ArgCount(2)]
        Round = 0x1b,
        /// <summary>
        /// [213] ROUNDDOWN function
        /// </summary>
        [ArgCount(2)]
        Rounddown = 0xd5,
        /// <summary>
        /// [212] ROUNDUP function
        /// </summary>
        [ArgCount(2)]
        Roundup = 0xd4,
        /// <summary>
        /// [8] ROW function
        /// </summary>
        Row = 8,
        /// <summary>
        /// [76] ROWS function
        /// </summary>
        [ArgCount(1)]
        Rows = 0x4c,
        /// <summary>
        /// [313] RSQ function
        /// </summary>
        [ArgCount(2)]
        Rsq = 0x139,
        /// <summary>
        /// [356] SAVEDIALOG function
        /// </summary>
        SaveDialog = 0x164,
        /// <summary>
        /// [264] SAVETOOLBAR function
        /// </summary>
        SaveToolbar = 0x108,
        /// <summary>
        /// [348] SCENARIOGET function
        /// </summary>
        ScenarioGet = 0x15c,
        /// <summary>
        /// [82] SEARCH function
        /// </summary>
        Search = 0x52,
        /// <summary>
        /// [206] SEARCHB function
        /// </summary>
        Searchb = 0xce,
        /// <summary>
        /// [73] SECOND function
        /// </summary>
        [ArgCount(1)]
        Second = 0x49,
        /// <summary>
        /// [95] SELECTION function
        /// </summary>
        Selection = 0x5f,
        /// <summary>
        /// [92] SERIES function
        /// </summary>
        Series = 0x5c,
        /// <summary>
        /// [88] SETNAME function
        /// </summary>
        SetName = 0x58,
        /// <summary>
        /// [108] SETVALUE function
        /// </summary>
        SetValue = 0x6c,
        /// <summary>
        /// [157] SHOWBAR function
        /// </summary>
        ShowBar = 0x9d,
        /// <summary>
        /// [26] SIGN function
        /// </summary>
        [ArgCount(1)]
        Sign = 0x1a,
        /// <summary>
        /// [15] SIN function
        /// </summary>
        [ArgCount(1)]
        Sin = 15,
        /// <summary>
        /// [229] SINH function
        /// </summary>
        [ArgCount(1)]
        Sinh = 0xe5,
        /// <summary>
        /// [323] SKEW function
        /// </summary>
        Skew = 0x143,
        /// <summary>
        /// [142] SLN function
        /// </summary>
        [ArgCount(3)]
        Sln = 0x8e,
        /// <summary>
        /// [315] SLOPE function
        /// </summary>
        [ArgCount(2)]
        Slope = 0x13b,
        /// <summary>
        /// [326] SMALL function
        /// </summary>
        [ArgCount(2)]
        Small = 0x146,
        /// <summary>
        /// [260] SPELLINGCHECK function
        /// </summary>
        SpellingCheck = 260,
        /// <summary>
        /// [20] SQRT function
        /// </summary>
        [ArgCount(1)]
        Sqrt = 20,
        /// <summary>
        /// [297] STANDARDIZE function
        /// </summary>
        [ArgCount(3)]
        Standardize = 0x129,
        /// <summary>
        /// [12] STDEV function
        /// </summary>
        Stdev = 12,
        /// <summary>
        /// [366] STDEVA function
        /// </summary>
        StDevA = 0x16e,
        /// <summary>
        /// [193] STDEVP function
        /// </summary>
        Stdevp = 0xc1,
        /// <summary>
        /// [364] STDEVPA function
        /// </summary>
        StDevPA = 0x16c,
        /// <summary>
        /// [85] STEP function
        /// </summary>
        Step = 0x55,
        /// <summary>
        /// [314] STEYX function
        /// </summary>
        [ArgCount(2)]
        Steyx = 0x13a,
        /// <summary>
        /// [120] SUBSTITUTE function
        /// </summary>
        Substitute = 120,
        /// <summary>
        /// [344] SUBTOTAL function
        /// </summary>
        Subtotal = 0x158,
        /// <summary>
        /// [4] SUM function
        /// </summary>
        Sum = 4,
        /// <summary>
        /// [345] SUMIF function
        /// </summary>
        Sumif = 0x159,
        /// <summary>
        /// [228] SUMPRODUCT function
        /// </summary>
        Sumproduct = 0xe4,
        /// <summary>
        /// [321] SUMSQ function
        /// </summary>
        Sumsq = 0x141,
        /// <summary>
        /// [304] SUMX2MY2 function
        /// </summary>
        [ArgCount(2)]
        Sumx2my2 = 0x130,
        /// <summary>
        /// [305] SUMX2PY2 function
        /// </summary>
        [ArgCount(2)]
        Sumx2py2 = 0x131,
        /// <summary>
        /// [303] SUMXMY2 function
        /// </summary>
        [ArgCount(2)]
        Sumxmy2 = 0x12f,
        /// <summary>
        /// [143] SYD function
        /// </summary>
        [ArgCount(4)]
        Syd = 0x8f,
        /// <summary>
        /// [130] T function
        /// </summary>
        [ArgCount(1)]
        T = 130,
        /// <summary>
        /// [17] TAN function
        /// </summary>
        [ArgCount(1)]
        Tan = 0x11,
        /// <summary>
        /// [231] TANH function
        /// </summary>
        [ArgCount(1)]
        Tanh = 0xe7,
        /// <summary>
        /// [301] TDIST function
        /// </summary>
        [ArgCount(3)]
        Tdist = 0x12d,
        /// <summary>
        /// [179] TERMINATE function
        /// </summary>
        Terminate = 0xb3,
        /// <summary>
        /// [48] TEST function
        /// </summary>
        Text = 0x30,
        /// <summary>
        /// [243] TEXTBOX function
        /// </summary>
        TextBox = 0xf3,
        /// <summary>
        /// [147] TEXTREF function
        /// </summary>
        Textref = 0x93,
        /// <summary>
        /// [66] TIME function
        /// </summary>
        [ArgCount(3)]
        Time = 0x42,
        /// <summary>
        /// [141] TIMEVALUE function
        /// </summary>
        [ArgCount(1)]
        Timevalue = 0x8d,
        /// <summary>
        /// [332] TINV function
        /// </summary>
        [ArgCount(2)]
        Tinv = 0x14c,
        /// <summary>
        /// [221] TODAY function
        /// </summary>
        Today = 0xdd,
        /// <summary>
        /// [83] TRANSPOSE function
        /// </summary>
        Transpose = 0x53,
        /// <summary>
        /// [50] TREND function
        /// </summary>
        Trend = 50,
        /// <summary>
        /// [118] TRIM function
        /// </summary>
        [ArgCount(1)]
        Trim = 0x76,
        /// <summary>
        /// [331] TRIMMEAN function
        /// </summary>
        [ArgCount(2)]
        Trimmean = 0x14b,
        /// <summary>
        /// [34] TRUE function
        /// </summary>
        True = 0x22,
        /// <summary>
        /// [197] TRUNC function
        /// </summary>
        Trunc = 0xc5,
        /// <summary>
        /// [316] TTEST function
        /// </summary>
        [ArgCount(4)]
        Ttest = 0x13c,
        /// <summary>
        /// [86] TYPE function
        /// </summary>
        [ArgCount(1)]
        Type = 0x56,
        /// <summary>
        /// [201] UNREGISTER function
        /// </summary>
        Unregister = 0xc9,
        /// <summary>
        /// [113] UPPER function
        /// </summary>
        [ArgCount(1)]
        Upper = 0x71,
        /// <summary>
        /// [204] USDOLLAR function
        /// </summary>
        Usdollar = 0xcc,
        /// <summary>
        /// [33] VALUE function
        /// </summary>
        [ArgCount(1)]
        Value = 0x21,
        /// <summary>
        /// [46] VAR function
        /// </summary>
        Var = 0x2e,
        /// <summary>
        /// [367] VARA function
        /// </summary>
        VarA = 0x16f,
        /// <summary>
        /// [194] VARP function
        /// </summary>
        Varp = 0xc2,
        /// <summary>
        /// [365] VARPA function
        /// </summary>
        VarPA = 0x16d,
        /// <summary>
        /// [222] VDB function
        /// </summary>
        Vdb = 0xde,
        /// <summary>
        /// [357] VIEWGET function
        /// </summary>
        ViewGet = 0x165,
        /// <summary>
        /// [102] VLOOKUP function
        /// </summary>
        Vlookup = 0x66,
        /// <summary>
        /// [111] VOLATILE function
        /// </summary>
        Volatile = 0xed,
        /// <summary>
        /// [70] WEEKDAY function
        /// </summary>
        Weekday = 70,
        /// <summary>
        /// [302] WEIBULL function
        /// </summary>
        [ArgCount(4)]
        Weibull = 0x12e,
        /// <summary>
        /// [91] WINDOWS function
        /// </summary>
        Windows = 0x5b,
        /// <summary>
        /// [263] WINDOWTITLE function
        /// </summary>
        WindowTitle = 0x107,
        /// <summary>
        /// [69] YEAR function
        /// </summary>
        [ArgCount(1)]
        Year = 0x45,
        /// <summary>
        /// [324] ZTEST function
        /// </summary>
        Ztest = 0x144
    }
}

