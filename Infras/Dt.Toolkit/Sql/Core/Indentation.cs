using System.Collections.Generic;
using System.Linq;

namespace Dt.Toolkit.Sql
{
    class Indentation
    {
        private enum IndentTypes
        {
            INDENT_TYPE_TOP_LEVEL,
            INDENT_TYPE_BLOCK_LEVEL
        }

        private readonly string indent;
        private readonly Stack<IndentTypes> indentTypes;

        public Indentation(string indent)
        {
            this.indent = indent;
            indentTypes = new Stack<IndentTypes>();
        }

        public string GetIndent() =>
            string.Concat(Enumerable.Range(0, indentTypes.Count)
                .Select(_ => indent));

        public void IncreaseTopLevel()
        {
            indentTypes.Push(IndentTypes.INDENT_TYPE_TOP_LEVEL);
        }

        public void IncreaseBlockLevel()
        {
            indentTypes.Push(IndentTypes.INDENT_TYPE_BLOCK_LEVEL);
        }

        public void DecreaseTopLevel()
        {
            if (indentTypes.Count != 0 &&
                indentTypes.Peek() == IndentTypes.INDENT_TYPE_TOP_LEVEL)
            {
                indentTypes.Pop();
            }
        }

        public void DecreaseBlockLevel()
        {
            while (indentTypes.Count > 0)
            {
                var type = indentTypes.Pop();
                if (type != IndentTypes.INDENT_TYPE_TOP_LEVEL)
                {
                    break;
                }
            }
        }

        public void ResetIndentation()
        {
            indentTypes.Clear();
        }
    }
}
