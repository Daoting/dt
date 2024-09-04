using System.Collections.Generic;

namespace Dt.Toolkit.Sql
{
    public abstract class Params
    {
        public static readonly Params Empty = new Empty();

        public abstract bool IsEmpty();

        public abstract object Get();

        public abstract object GetByName(string key);

        public static Params Of<T>(Dictionary<string, T> parameters)
        {
            return new NamedParams<T>(parameters);
        }

        public static Params Of<T>(List<T> parameters)
        {
            return new IndexedParams<T>(new Queue<T>(parameters));
        }

        public object Get(Token token)
        {
            if (IsEmpty())
            {
                return token.value;
            }

            if (!(token.key == null || string.IsNullOrEmpty(token.key)))
            {
                return GetByName(token.key);
            }

            return Get();
        }
    }

    internal class NamedParams<T> : Params
    {
        private readonly Dictionary<string, T> parameters;

        public NamedParams(Dictionary<string, T> parameters)
        {
            this.parameters = parameters;
        }

        public override bool IsEmpty()
        {
            return parameters.Count == 0;
        }

        public override object Get()
        {
            return null;
        }

        public override object GetByName(string key)
        {
            return parameters[key];
        }

        public override string ToString()
        {
            return parameters.ToString();
        }
    }

    internal class IndexedParams<T> : Params
    {
        private readonly Queue<T> parameters;

        public IndexedParams(Queue<T> parameters)
        {
            this.parameters = parameters;
        }

        public override bool IsEmpty()
        {
            return parameters.Count == 0;
        }

        public override object Get()
        {
            return parameters.Dequeue();
        }

        public override object GetByName(string key)
        {
            return null;
        }

        public override string ToString()
        {
            return parameters.ToString();
        }
    }

    internal class Empty : Params
    {
        public override bool IsEmpty()
        {
            return true;
        }

        public override object Get()
        {
            return null;
        }

        public override object GetByName(string key)
        {
            return null;
        }

        public override string ToString()
        {
            return "[]";
        }
    }
}
