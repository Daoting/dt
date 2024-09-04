using System;
using System.Collections.Generic;
using System.Linq;

namespace Dt.Toolkit.Sql
{
    public class Dialect
    {
        public static readonly Dialect Db2 = new Dialect(cfg => new Db2Formatter(cfg), "Db2");
        public static readonly Dialect MariaDb = new Dialect(cfg => new MariaDbFormatter(cfg), "MariaDb");
        public static readonly Dialect MySql = new Dialect(cfg => new MySqlFormatter(cfg), "MySql");
        public static readonly Dialect N1ql = new Dialect(cfg => new N1qlFormatter(cfg), "N1ql");
        public static readonly Dialect PlSql = new Dialect(cfg => new PlSqlFormatter(cfg), "PlSql", "pl/sql");
        public static readonly Dialect PostgreSql = new Dialect(cfg => new PostgreSqlFormatter(cfg), "PostgreSql");
        public static readonly Dialect Redshift = new Dialect(cfg => new RedshiftFormatter(cfg), "Redshift");
        public static readonly Dialect SparkSql = new Dialect(cfg => new SparkSqlFormatter(cfg), "SparkSql", "spark");
        public static readonly Dialect StandardSql = new Dialect(cfg => new StandardSqlFormatter(cfg), "StandardSql", "sql");
        public static readonly Dialect TSql = new Dialect(cfg => new TSqlFormatter(cfg), "TSql");

        public static IEnumerable<Dialect> Values
        {
            get
            {
                yield return Db2;
                yield return MariaDb;
                yield return MySql;
                yield return N1ql;
                yield return PlSql;
                yield return PostgreSql;
                yield return Redshift;
                yield return SparkSql;
                yield return StandardSql;
                yield return TSql;
            }
        }

        public readonly string name;
        public readonly Func<FormatConfig, AbstractFormatter> func;
        public readonly List<string> aliases;

        private Dialect(Func<FormatConfig, AbstractFormatter> func, string name, params string[] aliases)
        {
            this.func = func;
            this.name = name;
            this.aliases = new List<string>(aliases);
        }

        private bool Matches(string name)
        {
            return this.name.ToLower().Equals(name.ToLower())
                || aliases.Select(s => s.ToLower()).Intersect(new string[] { name.ToLower() }).Any();
        }

        public static Dialect NameOf(string name)
        {
            var dialects = Values.Where(d => d.Matches(name));

            if (dialects.Count() == 0)
            {
                return null;
            }

            return dialects.First();
        }
    }
}
