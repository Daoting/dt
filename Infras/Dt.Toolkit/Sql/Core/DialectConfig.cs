using System.Collections.Generic;
using System.Linq;


namespace Dt.Toolkit.Sql
{
    public class DialectConfig
    {
        public readonly List<string> lineCommentTypes;
        public readonly List<string> reservedTopLevelWords;
        public readonly List<string> reservedTopLevelWordsNoIndent;
        public readonly List<string> reservedNewlineWords;
        public readonly List<string> reservedWords;
        public readonly List<string> specialWordChars;
        public readonly List<string> stringTypes;
        public readonly List<string> openParens;
        public readonly List<string> closeParens;
        public readonly List<string> indexedPlaceholderTypes;
        public readonly List<string> namedPlaceholderTypes;
        public readonly List<string> operators;

        private DialectConfig(
            List<string> lineCommentTypes,
            List<string> reservedTopLevelWords,
            List<string> reservedTopLevelWordsNoIndent,
            List<string> reservedNewlineWords,
            List<string> reservedWords,
            List<string> specialWordChars,
            List<string> stringTypes,
            List<string> openParens,
            List<string> closeParens,
            List<string> indexedPlaceholderTypes,
            List<string> namedPlaceholderTypes,
            List<string> operators)
        {
            this.lineCommentTypes = Utils.NullToEmpty(lineCommentTypes);
            this.reservedTopLevelWords = Utils.NullToEmpty(reservedTopLevelWords);
            this.reservedTopLevelWordsNoIndent = Utils.NullToEmpty(reservedTopLevelWordsNoIndent);
            this.reservedNewlineWords = Utils.NullToEmpty(reservedNewlineWords);
            this.reservedWords = Utils.NullToEmpty(reservedWords);
            this.specialWordChars = Utils.NullToEmpty(specialWordChars);
            this.stringTypes = Utils.NullToEmpty(stringTypes);
            this.openParens = Utils.NullToEmpty(openParens);
            this.closeParens = Utils.NullToEmpty(closeParens);
            this.indexedPlaceholderTypes = Utils.NullToEmpty(indexedPlaceholderTypes);
            this.namedPlaceholderTypes = Utils.NullToEmpty(namedPlaceholderTypes);
            this.operators = Utils.NullToEmpty(operators);
        }
        public DialectConfig WithLineCommentTypes(List<string> lineCommentTypes)
        {
            return ToBuilder()
                .LineCommentTypes(lineCommentTypes)
                .Build();
        }

        public DialectConfig PlusLineCommentTypes(params string[] lineCommentTypes)
        {
            return PlusLineCommentTypes(lineCommentTypes.ToList());
        }

        public DialectConfig PlusLineCommentTypes(List<string> lineCommentTypes)
        {
            return ToBuilder()
                .LineCommentTypes(Utils.Concat(this.lineCommentTypes, lineCommentTypes))
                .Build();
        }

        public DialectConfig WithReservedTopLevelWords(List<string> reservedTopLevelWords)
        {
            return ToBuilder()
                .ReservedTopLevelWords(reservedTopLevelWords)
                .Build();
        }

        public DialectConfig PlusReservedTopLevelWords(params string[] reservedTopLevelWords)
        {
            return PlusReservedTopLevelWords(reservedTopLevelWords.ToList());
        }

        public DialectConfig PlusReservedTopLevelWords(List<string> reservedTopLevelWords)
        {
            return ToBuilder()
                .ReservedTopLevelWords(Utils.Concat(this.reservedTopLevelWords, reservedTopLevelWords))
                .Build();
        }

        public DialectConfig WithReservedNewlineWords(List<string> reservedNewLineWords)
        {
            return ToBuilder()
                .ReservedNewlineWords(reservedNewLineWords)
                .Build();
        }

        public DialectConfig PlusReservedNewlineWords(params string[] reservedNewLineWords)
        {
            return PlusReservedNewlineWords(reservedNewLineWords.ToList());
        }

        public DialectConfig PlusReservedNewlineWords(List<string> reservedNewlineWords)
        {
            return ToBuilder()
                .ReservedNewlineWords(Utils.Concat(this.reservedNewlineWords, reservedNewlineWords))
                .Build();
        }

        public DialectConfig WithReservedTopLevelWordsNoIndent(List<string> reservedTopLevelWordsNoIndent)
        {
            return ToBuilder()
                .ReservedTopLevelWordsNoIndent(reservedTopLevelWordsNoIndent)
                .Build();
        }

        public DialectConfig PlusReservedTopLevelWordsNoIndent(params string[] reservedTopLevelWordsNoIndent)
        {
            return PlusReservedTopLevelWordsNoIndent(reservedTopLevelWordsNoIndent.ToList());
        }

        public DialectConfig PlusReservedTopLevelWordsNoIndent(List<string> reservedTopLevelWordsNoIndent)
        {
            return ToBuilder()
                .ReservedTopLevelWordsNoIndent(Utils.Concat(this.reservedTopLevelWordsNoIndent, reservedTopLevelWordsNoIndent))
                .Build();
        }

        public DialectConfig WithReservedWords(List<string> reservedWords)
        {
            return ToBuilder()
                .ReservedWords(reservedWords)
                .Build();
        }

        public DialectConfig PlusReservedWords(params string[] reservedWords)
        {
            return PlusReservedWords(reservedWords.ToList());
        }

        public DialectConfig PlusReservedWords(List<string> reservedWords)
        {
            return ToBuilder()
                .ReservedWords(Utils.Concat(this.reservedWords, reservedWords))
                .Build();
        }

        public DialectConfig WithSpecialWordChars(List<string> specialWordChars)
        {
            return ToBuilder()
                .SpecialWordChars(specialWordChars)
                .Build();
        }

        public DialectConfig PlusSpecialWordChars(params string[] specialWordChars)
        {
            return PlusSpecialWordChars(specialWordChars.ToList());
        }

        public DialectConfig PlusSpecialWordChars(List<string> specialWordChars)
        {
            return ToBuilder()
                .SpecialWordChars(Utils.Concat(this.specialWordChars, specialWordChars))
                .Build();
        }

        public DialectConfig WithStringTypes(List<string> stringTypes)
        {
            return ToBuilder()
                .StringTypes(stringTypes)
                .Build();
        }

        public DialectConfig PlusStringTypes(params string[] stringTypes)
        {
            return PlusStringTypes(stringTypes.ToList());
        }

        public DialectConfig PlusStringTypes(List<string> stringTypes)
        {
            return ToBuilder()
                .StringTypes(Utils.Concat(this.stringTypes, stringTypes))
                .Build();
        }

        public DialectConfig WithOpenParens(List<string> openParens)
        {
            return ToBuilder()
                .OpenParens(openParens)
                .Build();
        }

        public DialectConfig PlusOpenParens(params string[] openParens)
        {
            return PlusOpenParens(openParens.ToList());
        }

        public DialectConfig PlusOpenParens(List<string> openParens)
        {
            return ToBuilder()
                .OpenParens(Utils.Concat(this.openParens, openParens))
                .Build();
        }

        public DialectConfig WithCloseParens(List<string> closeParens)
        {
            return ToBuilder()
                .CloseParens(closeParens)
                .Build();
        }

        public DialectConfig PlusCloseParens(params string[] closeParens)
        {
            return PlusCloseParens(closeParens.ToList());
        }

        public DialectConfig PlusCloseParens(List<string> closeParens)
        {
            return ToBuilder()
                .CloseParens(Utils.Concat(this.closeParens, closeParens))
                .Build();
        }

        public DialectConfig WithIndexedPlaceholderTypes(List<string> indexedPlaceholderTypes)
        {
            return ToBuilder()
                .IndexedPlaceholderTypes(indexedPlaceholderTypes)
                .Build();
        }

        public DialectConfig PlusIndexedPlaceholderTypes(params string[] indexedPlaceholderTypes)
        {
            return PlusIndexedPlaceholderTypes(indexedPlaceholderTypes.ToList());
        }

        public DialectConfig PlusIndexedPlaceholderTypes(List<string> indexedPlaceholderTypes)
        {
            return ToBuilder()
                .IndexedPlaceholderTypes(Utils.Concat(this.indexedPlaceholderTypes, indexedPlaceholderTypes))
                .Build();
        }

        public DialectConfig WithNamedPlaceholderTypes(List<string> namedPlaceholderTypes)
        {
            return ToBuilder()
                .NamedPlaceholderTypes(namedPlaceholderTypes)
                .Build();
        }

        public DialectConfig PlusNamedPlaceholderTypes(params string[] namedPlaceholderTypes)
        {
            return PlusNamedPlaceholderTypes(namedPlaceholderTypes.ToList());
        }

        public DialectConfig PlusNamedPlaceholderTypes(List<string> namedPlaceholderTypes)
        {
            return ToBuilder()
                .NamedPlaceholderTypes(Utils.Concat(this.namedPlaceholderTypes, namedPlaceholderTypes))
                .Build();
        }

        public DialectConfig WithOperators(List<string> operators)
        {
            return ToBuilder()
                .Operators(operators)
                .Build();
        }

        public DialectConfig PlusOperators(params string[] operators)
        {
            return PlusOperators(operators.ToList());
        }

        public DialectConfig PlusOperators(List<string> operators)
        {
            return ToBuilder()
                .Operators(Utils.Concat(this.operators, operators))
                .Build();
        }

        public DialectConfigBuilder ToBuilder()
        {
            return Builder()
                .LineCommentTypes(lineCommentTypes)
                .ReservedTopLevelWords(reservedTopLevelWords)
                .ReservedTopLevelWordsNoIndent(reservedTopLevelWordsNoIndent)
                .ReservedNewlineWords(reservedNewlineWords)
                .ReservedWords(reservedWords)
                .SpecialWordChars(specialWordChars)
                .StringTypes(stringTypes)
                .OpenParens(openParens)
                .CloseParens(closeParens)
                .IndexedPlaceholderTypes(indexedPlaceholderTypes)
                .NamedPlaceholderTypes(namedPlaceholderTypes)
                .Operators(operators);
        }

        public static DialectConfigBuilder Builder()
        {
            return new DialectConfigBuilder();
        }

        public class DialectConfigBuilder
        {
            private List<string> lineCommentTypes;
            private List<string> reservedTopLevelWords;
            private List<string> reservedTopLevelWordsNoIndent;
            private List<string> reservedNewlineWords;
            private List<string> reservedWords;
            private List<string> specialWordChars;
            private List<string> stringTypes;
            private List<string> openParens;
            private List<string> closeParens;
            private List<string> indexedPlaceholderTypes;
            private List<string> namedPlaceholderTypes;
            private List<string> operators;

            public DialectConfigBuilder LineCommentTypes(List<string> lineCommentTypes)
            {
                this.lineCommentTypes = lineCommentTypes;
                return this;
            }

            public DialectConfigBuilder ReservedTopLevelWords(List<string> reservedTopLevelWords)
            {
                this.reservedTopLevelWords = reservedTopLevelWords;
                return this;
            }

            public DialectConfigBuilder ReservedTopLevelWordsNoIndent(List<string> reservedTopLevelWordsNoIndent)
            {
                this.reservedTopLevelWordsNoIndent = reservedTopLevelWordsNoIndent;
                return this;
            }

            public DialectConfigBuilder ReservedNewlineWords(List<string> reservedNewlineWords)
            {
                this.reservedNewlineWords = reservedNewlineWords;
                return this;
            }

            public DialectConfigBuilder ReservedWords(List<string> reservedWords)
            {
                this.reservedWords = reservedWords;
                return this;
            }

            public DialectConfigBuilder SpecialWordChars(List<string> specialWordChars)
            {
                this.specialWordChars = specialWordChars;
                return this;
            }

            public DialectConfigBuilder StringTypes(List<string> stringTypes)
            {
                this.stringTypes = stringTypes;
                return this;
            }

            public DialectConfigBuilder OpenParens(List<string> openParens)
            {
                this.openParens = openParens;
                return this;
            }

            public DialectConfigBuilder CloseParens(List<string> closeParens)
            {
                this.closeParens = closeParens;
                return this;
            }

            public DialectConfigBuilder IndexedPlaceholderTypes(List<string> indexedPlaceholderTypes)
            {
                this.indexedPlaceholderTypes = indexedPlaceholderTypes;
                return this;
            }

            public DialectConfigBuilder NamedPlaceholderTypes(List<string> namedPlaceholderTypes)
            {
                this.namedPlaceholderTypes = namedPlaceholderTypes;
                return this;
            }

            public DialectConfigBuilder Operators(List<string> operators)
            {
                this.operators = operators;
                return this;
            }

            public DialectConfig Build()
            {
                return new DialectConfig(
                    lineCommentTypes,
                    reservedTopLevelWords,
                    reservedTopLevelWordsNoIndent,
                    reservedNewlineWords,
                    reservedWords,
                    specialWordChars,
                    stringTypes,
                    openParens,
                    closeParens,
                    indexedPlaceholderTypes,
                    namedPlaceholderTypes,
                    operators);
            }
        }
    }
}
