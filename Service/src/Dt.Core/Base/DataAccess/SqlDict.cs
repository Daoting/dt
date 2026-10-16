#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-04-17 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Text;
using System.Text.RegularExpressions;
#endregion

namespace Dt.Core;

/// <summary>
/// 缓存Sql字典
/// </summary>
internal static class SqlDict
{
    const string _subDir = "etc\\sql";
    static FrozenDictionary<string, string> _sqlDict;
    static FrozenDictionary<string, string>.AlternateLookup<ReadOnlySpan<char>> _sqlSpanDict;
    // 工作字典：用来增量修改（新增/删除key）
    static readonly ConcurrentDictionary<string, string> _workingDict = new();
    static readonly Regex KeyCommentRegex = new Regex(@"^\s*--\s*@key:\s*(\w+)\s*$",
        RegexOptions.Multiline | RegexOptions.Compiled);

    // 热加载相关
    static FileSystemWatcher _watcher;
    static Timer _debounceTimer;
    static readonly object _debounceLock = new object();
    // 文件路径 -> 该文件内所有key列表（用于文件删除时清理旧key）
    static ConcurrentDictionary<string, HashSet<string>> _fileToKeys;
    static ConcurrentDictionary<string, DateTime> _fileProcessTs;

    /// <summary>
    /// 初始化：加载SQL + 启动文件监听
    /// </summary>
    public static void Start()
    {
        LoadAllSql();
        if (Kit.GetCfg<bool>("SqlHotReload", false))
            StartFileWatcher();
    }

    public static string GetSql(string p_key)
    {
        if (_sqlDict != null && _sqlDict.TryGetValue(p_key, out var sql))
            return sql;
        return null;
    }

    public static string GetSql(in ReadOnlySpan<char> p_key)
    {
        if (_sqlDict != null && _sqlSpanDict.TryGetValue(p_key, out var sql))
            return sql;
        return null;
    }

    static void LoadAllSql()
    {
        var path = Path.Combine(Kit.PathBase, _subDir);
        if (!Directory.Exists(path))
        {
            Log.Information("SQL文件目录(etc\\sql)不存在，SQL字典空！");
            return;
        }

        if (Kit.GetCfg<bool>("SqlHotReload", false))
            _fileToKeys = new();

        var sqlFiles = Directory.GetFiles(path, "*.sql", SearchOption.AllDirectories);
        foreach (var file in sqlFiles)
        {
            ParseSqlContent(file);
        }
        Log.Information($"SQL字典共 {_fileToKeys.Count} 个文件，{_workingDict.Count} 条语句");

        _sqlDict = _workingDict.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
        _sqlSpanDict = _sqlDict.GetAlternateLookup<ReadOnlySpan<char>>();
    }

    static void ParseSqlContent(string p_file)
    {
        var content = ReadFileWithRetry(p_file);
        var matches = KeyCommentRegex.Matches(content);
        if (matches.Count == 0) return;

        HashSet<string> keySet = null;
        if (_fileToKeys != null)
            keySet = new();

        for (int i = 0; i < matches.Count; i++)
        {
            var match = matches[i];
            string key = match.Groups[1].Value.Trim();
            int startIndex = match.Index + match.Length;
            int endIndex = i + 1 < matches.Count ? matches[i + 1].Index : content.Length;

            string rawBlock = content.Substring(startIndex, endIndex - startIndex);

            // 1. 删除key和SQL之间的前置注释
            string sqlBody = TrimLeadingComments(rawBlock);
            // 2. 删除SQL末尾的注释（尾部--、末尾/* */）
            string finalSql = TrimTrailingComments(sqlBody);

            if (_workingDict.ContainsKey(key))
            {
                Log.Warning($"SQL键名重复：{key} -> {p_file}");
            }
            else
            {
                _workingDict.TryAdd(key, finalSql);
                keySet?.Add(key);
            }
        }

        if (_fileToKeys != null)
        {
            _fileToKeys[p_file] = keySet;
        }
    }

    /// <summary>
    /// 删除块开头连续注释/空行，找到第一条SQL正文，正文后面原样保留
    /// </summary>
    static string TrimLeadingComments(string block)
    {
        StringBuilder sb = new StringBuilder();
        int i = 0;
        int len = block.Length;

        bool inMultiComment = false;
        bool foundFirstSqlContent = false;

        while (i < len)
        {
            char c = block[i];

            if (foundFirstSqlContent)
            {
                sb.Append(c);
                i++;
                continue;
            }

            if (inMultiComment)
            {
                if (c == '*' && i + 1 < len && block[i + 1] == '/')
                {
                    inMultiComment = false;
                    i++;
                }
                i++;
                continue;
            }

            if (char.IsWhiteSpace(c))
            {
                i++;
                continue;
            }

            if (c == '/' && i + 1 < len && block[i + 1] == '*')
            {
                inMultiComment = true;
                i++;
                i++;
                continue;
            }

            if (c == '-' && i + 1 < len && block[i + 1] == '-')
            {
                while (i < len && block[i] != '\n')
                    i++;
                continue;
            }

            foundFirstSqlContent = true;
            sb.Append(c);
            i++;
        }

        return sb.ToString();
    }

    /// <summary>
    /// 删除SQL文本【尾部】的注释：尾部--单行注释、末尾整块/* */多行注释
    /// SQL中间的注释保持不动
    /// </summary>
    static string TrimTrailingComments(string sql)
    {
        if (string.IsNullOrWhiteSpace(sql))
            return sql;

        int len = sql.Length;
        bool inString = false;
        bool inMultiComment = false;

        // 记录最后一个【有效SQL字符】的位置
        int lastValidPos = len - 1;
        // 标记是否已经进入尾部注释
        bool inTrailingComment = false;

        for (int i = 0; i < len; i++)
        {
            char c = sql[i];

            if (inTrailingComment)
            {
                continue;
            }

            if (inMultiComment)
            {
                if (c == '*' && i + 1 < len && sql[i + 1] == '/')
                {
                    inMultiComment = false;
                    i++;
                    lastValidPos = i;
                }
                continue;
            }

            if (inString)
            {
                if (c == '\'')
                {
                    // 转义单引号 ''
                    if (i + 1 < len && sql[i + 1] == '\'')
                    {
                        i++;
                    }
                    else
                    {
                        inString = false;
                    }
                }
                lastValidPos = i;
                continue;
            }

            // 遇到单引号，进入字符串
            if (c == '\'')
            {
                inString = true;
                lastValidPos = i;
                continue;
            }

            // 多行注释开始 /*
            if (c == '/' && i + 1 < len && sql[i + 1] == '*')
            {
                inMultiComment = true;
                continue;
            }

            // 单行注释开始 --，并且当前不在字符串、不在多行注释：后面全部是尾部注释
            if (c == '-' && i + 1 < len && sql[i + 1] == '-')
            {
                inTrailingComment = true;
                break;
            }

            // 普通字符，更新有效位置
            lastValidPos = i;
        }

        string result = sql.Substring(0, lastValidPos + 1);
        return result.TrimEnd();
    }

    static void StartFileWatcher()
    {
        var path = Path.Combine(Kit.PathBase, _subDir);
        if (!Directory.Exists(path))
            return;

        _watcher = new FileSystemWatcher(path, "*.sql")
        {
            IncludeSubdirectories = true,
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName
        };

        _watcher.Changed += OnSqlFileChanged;
        _watcher.Created += OnSqlFileChanged;
        _watcher.Renamed += OnSqlFileChanged;
        _watcher.Deleted += OnSqlFileChanged;

        _watcher.EnableRaisingEvents = true;
        Log.Information("启用热加载SQL字典");
    }

    static void OnSqlFileChanged(object sender, FileSystemEventArgs e)
    {
        // 跳过目录，只处理sql文件
        if (Directory.Exists(e.FullPath)) return;
        if (!e.Name.EndsWith(".sql", StringComparison.OrdinalIgnoreCase)) return;

        lock (_debounceLock)
        {
            // 重置防抖计时器
            _debounceTimer?.Dispose();
            var filePath = e.FullPath;
            var changeType = e.ChangeType;
            // 编辑器保存文件会多次触发文件变更事件，等待 300ms 一次性重载，避免重复加载
            _debounceTimer = new Timer(_ =>
            {
                try
                {
                    ProcessSingleFileChange(filePath, changeType);
                }
                catch (Exception ex)
                {
                    // 日志：重载SQL失败
                    Log.Warning($"SQL文件变更，重新加载异常：{ex.Message}");
                }
            }, null, 300, Timeout.Infinite);
        }
    }

    /// <summary>
    /// 处理单个文件变更：增量更新，不是全量重载
    /// </summary>
    static void ProcessSingleFileChange(string filePath, WatcherChangeTypes changeType)
    {
        if (_fileProcessTs == null)
            _fileProcessTs = new();

        var now = DateTime.UtcNow;
        // 时间戳防抖动：短时间内重复事件直接跳过
        if (_fileProcessTs.TryGetValue(filePath, out var lastTs))
        {
            if (now - lastTs < TimeSpan.FromMilliseconds(500))
                return;
        }
        _fileProcessTs[filePath] = now;

        // 第一步：删除这个文件旧的所有key
        if (_fileToKeys.TryGetValue(filePath, out var oldKeys))
        {
            foreach (var k in oldKeys)
            {
                _workingDict.TryRemove(k, out _);
            }
            _fileToKeys.TryRemove(filePath, out _);
        }

        // 文件删除：到此结束
        if (changeType == WatcherChangeTypes.Deleted)
        {
            Log.Information($"文件删除 -> {filePath}，清理key");
            return;
        }

        // 文件存在则读取并解析
        if (!File.Exists(filePath))
            return;

        ParseSqlContent(filePath);
        _sqlDict = _workingDict.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
        _sqlSpanDict = _sqlDict.GetAlternateLookup<ReadOnlySpan<char>>();
        Log.Information($"重新加载 -> {filePath}，共 {_fileToKeys[filePath].Count} 个key");
    }

    /// <summary>
    /// 带重试读取sql文件，处理文件被占用锁
    /// </summary>
    static string ReadFileWithRetry(string filePath, int retryCount = 3, int delayMs = 50)
    {
        for (int i = 0; i < retryCount; i++)
        {
            try
            {
                using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var sr = new StreamReader(fs, Encoding.UTF8);
                return sr.ReadToEnd();
            }
            catch (IOException)
            {
                if (i == retryCount - 1)
                {
                    // 最后一次失败，抛出
                    throw;
                }
                Thread.Sleep(delayMs);
            }
        }
        throw new IOException("读取SQL文件重试全部失败！");
    }
}