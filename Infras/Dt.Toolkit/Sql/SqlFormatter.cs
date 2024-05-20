using System;
using System.Collections.Generic;

namespace Dt.Toolkit.Sql
{
    /// <summary>
    /// https://github.com/hogimn/sql-formatter
    /// </summary>
    public class SqlFormatter
    {
        public static string Format(string query)
        {
            return Standard().Format(query);
        }

        public static string Format(string query, FormatConfig cfg)
        {
            return Standard().Format(query, cfg);
        }

        public static string Format<T>(string query, string indent, List<T> parameters)
        {
            return Standard().Format(query, indent, parameters);
        }

        public static string Format<T>(string query, List<T> parameters)
        {
            return Standard().Format(query, parameters);
        }

        public static string Format<T>(string query, string indent, Dictionary<string, T> parameters)
        {
            return Standard().Format(query, indent, parameters);
        }

        public static string Format<T>(string query, Dictionary<string, T> parameters)
        {
            return Standard().Format(query, parameters);
        }

        public static string Format(string query, string indent)
        {
            return Standard().Format(query, indent);
        }

        public static Formatter Extend(Func<DialectConfig, DialectConfig> sqlOperator)
        {
            return Standard().Extend(sqlOperator);
        }

        static Formatter _standardFormatter;
        
        public static Formatter Standard()
        {
            if (_standardFormatter == null)
                _standardFormatter = Of(Dialect.StandardSql);
            return _standardFormatter;
        }

        public static Formatter Of(string name)
        {
            var dialect = Dialect.NameOf(name);
            return dialect == null ? throw new Exception("Unsupported SQL dialect: " + name) :
                new Formatter(dialect);
        }

        public static Formatter Of(Dialect dialect)
        {
            return new Formatter(dialect);
        }

        public class Formatter
        {

            private readonly Func<FormatConfig, AbstractFormatter> underlying;

            Formatter(Func<FormatConfig, AbstractFormatter> underlying)
            {
                this.underlying = underlying;
            }

            public Formatter(Dialect dialect) : this(dialect.func) { }

            public string Format(string query, FormatConfig cfg)
            {
                return underlying.Invoke(cfg).Format(query);
            }

            public string Format<T>(string query, string indent, List<T> parameters)
            {
                return Format(query, FormatConfig.Builder().Indent(indent).Params(parameters).Build());
            }

            public string Format<T>(string query, List<T> parameters)
            {
                return Format(query, FormatConfig.Builder().Params(parameters).Build());
            }

            public string Format<T>(string query, string indent, Dictionary<string, T> parameters)
            {
                return Format(query, FormatConfig.Builder().Indent(indent).Params(parameters).Build());
            }

            public string Format<T>(string query, Dictionary<string, T> parameters)
            {
                return Format(query, FormatConfig.Builder().Params(parameters).Build());
            }

            public string Format(string query, string indent)
            {
                return Format(query, FormatConfig.Builder().Indent(indent).Build());
            }

            FormatConfig _defaultConfig;
            public string Format(string query)
            {
                if (_defaultConfig == null)
                    _defaultConfig = FormatConfig.Builder().Build();
                return Format(query, _defaultConfig);
            }

            public Formatter Extend(Func<DialectConfig, DialectConfig> sqlOperator)
            {
                AbstractFormatter Func(FormatConfig config)
                {
                    var abstractFormatter = new AbstractFormatter(config)
                    {
                        doDialectConfigFunc = () => sqlOperator.Invoke(underlying.Invoke(config).DoDialectConfig())
                    };
                    return abstractFormatter;
                }

                return new Formatter(Func);
            }
        }
    }
}

