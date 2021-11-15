#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a text condition.
    /// </summary>
    public sealed class TextCondition : ConditionBase
    {
        static readonly string AsteriskWildcard = "*";
        static readonly string AsteriskWildcardRegularExpression = @"((.|\n)*)";
        TextCompareType compareType;
        bool forceValue2Text;
        bool ignoreCase;
        static readonly string QuestionMarkWildcard = "?";
        Regex regex;
        bool useWildCards;

        /// <summary>
        /// Creates a new text condition.
        /// </summary>
        public TextCondition() : this(TextCompareType.EqualsTo, null, null)
        {
        }

        /// <summary>
        /// Creates a new text condition with the specified text comparison type based on the specified cell.
        /// </summary>
        /// <param name="compareType">The type of comparison.</param>
        /// <param name="expected">The expected text.</param>
        /// <param name="formula">The expected formula.</param>
        internal TextCondition(TextCompareType compareType, object expected, string formula) : base(expected, formula)
        {
            this.regex = null;
            this.compareType = compareType;
            this.useWildCards = true;
            this.ignoreCase = false;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            TextCondition condition = base.Clone() as TextCondition;
            condition.compareType = this.compareType;
            condition.useWildCards = this.useWildCards;
            condition.ignoreCase = this.ignoreCase;
            condition.forceValue2Text = this.forceValue2Text;
            return condition;
        }

        /// <summary>
        /// Creates the regular expression that contains???.
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <returns>Returns the regular expression.</returns>
        Regex CreateContainsRegex(string expression)
        {
            if (this.regex != null)
            {
                return this.regex;
            }
            expression = EncodeExpression(expression);
            StringBuilder builder = new StringBuilder(expression);
            builder.Replace(QuestionMarkWildcard, ".");
            builder.Replace(AsteriskWildcard, AsteriskWildcardRegularExpression);
            Regex regex = null;
            try
            {
                RegexOptions options = this.IgnoreCase ? ((RegexOptions) RegexOptions.IgnoreCase) : ((RegexOptions) RegexOptions.None);
                regex = new Regex(builder.ToString(), options);
            }
            catch
            {
                return null;
            }
            return regex;
        }

        /// <summary>
        /// Creates the regular expression to end the condition.
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <returns>Returns the regular expression.</returns>
        Regex CreateEndWithRegex(string expression)
        {
            if (this.regex != null)
            {
                return this.regex;
            }
            expression = EncodeExpression(expression);
            StringBuilder builder = new StringBuilder(expression);
            builder.Replace(QuestionMarkWildcard, ".");
            builder.Replace(AsteriskWildcard, AsteriskWildcardRegularExpression);
            Regex regex = null;
            try
            {
                RegexOptions options = this.IgnoreCase ? ((RegexOptions) RegexOptions.IgnoreCase) : ((RegexOptions) RegexOptions.None);
                regex = new Regex(builder.ToString() + "$", options);
            }
            catch
            {
                return null;
            }
            return regex;
        }

        /// <summary>
        /// Creates the regular expression to compare???.
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <returns>Returns the regular expression.</returns>
        Regex CreateEqualsRegex(string expression)
        {
            if (this.regex != null)
            {
                return this.regex;
            }
            expression = EncodeExpression(expression);
            StringBuilder builder = new StringBuilder(expression);
            builder.Replace(QuestionMarkWildcard, ".");
            builder.Replace(AsteriskWildcard, AsteriskWildcardRegularExpression);
            Regex regex = null;
            try
            {
                RegexOptions options = this.IgnoreCase ? ((RegexOptions) RegexOptions.IgnoreCase) : ((RegexOptions) RegexOptions.None);
                regex = new Regex("^" + builder.ToString() + "$", options);
            }
            catch
            {
                return null;
            }
            return regex;
        }

        /// <summary>
        /// Creates the regular expression to start the condition.
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <returns>Returns the regular expression.</returns>
        Regex CreateStartWithRegex(string expression)
        {
            if (this.regex != null)
            {
                return this.regex;
            }
            expression = EncodeExpression(expression);
            StringBuilder builder = new StringBuilder(expression);
            builder.Replace(QuestionMarkWildcard, ".");
            builder.Replace(AsteriskWildcard, AsteriskWildcardRegularExpression);
            Regex regex = null;
            try
            {
                RegexOptions options = this.IgnoreCase ? ((RegexOptions) RegexOptions.IgnoreCase) : ((RegexOptions) RegexOptions.None);
                regex = new Regex("^" + builder.ToString(), options);
            }
            catch
            {
                return null;
            }
            return regex;
        }

        /// <summary>
        /// Encodes the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>Returns the encoded expression.</returns>
        static string EncodeExpression(string expression)
        {
            if (expression == null)
            {
                return null;
            }
            StringBuilder builder = new StringBuilder(expression);
            builder.Replace("^", @"\^");
            builder.Replace("$", @"\$");
            builder.Replace("(", @"\(");
            builder.Replace(")", @"\)");
            builder.Replace("[", @"\[");
            builder.Replace("]", @"\]");
            builder.Replace("{", @"\{");
            builder.Replace("}", @"\}");
            builder.Replace(".", @"\.");
            builder.Replace("+", @"\+");
            builder.Replace("|", @"\|");
            return builder.ToString();
        }

        /// <summary>
        /// Evaluates using the specified evaluator.
        /// </summary>
        /// <param name="evaluator">The evaluator.</param>
        /// <param name="baseRow">The base row.</param>
        /// <param name="baseColumn">The base column.</param>
        /// <param name="actualObj">The actual value object.</param>
        /// <returns><c>true</c> if the result is successful; otherwise, <c>false</c>.</returns>
        public override bool Evaluate(ICalcEvaluator evaluator, int baseRow, int baseColumn, IActualValue actualObj)
        {
            string text;
            if (this.forceValue2Text)
            {
                text = actualObj.GetText();
            }
            else
            {
                object obj2 = actualObj.GetValue();
                if ((obj2 is DateTime) || (obj2 is TimeSpan))
                {
                    return ((((this.compareType != TextCompareType.BeginsWith) && (this.compareType != TextCompareType.EndsWith)) && (this.compareType != TextCompareType.Contains)) && (((this.compareType == TextCompareType.DoesNotBeginWith) || (this.compareType == TextCompareType.DoesNotEndWith)) || (this.compareType == TextCompareType.DoesNotContain)));
                }
                text = (obj2 == null) ? actualObj.GetText() : obj2.ToString();
            }
            if (this.IgnoreBlank && string.IsNullOrEmpty(text))
            {
                return true;
            }
            string expected = base.GetExpectedString(evaluator, baseRow, baseColumn);
            switch (this.CompareType)
            {
                case TextCompareType.EqualsTo:
                    return this.IsEquals(expected, text);

                case TextCompareType.NotEqualsTo:
                    return !this.IsEquals(expected, text);

                case TextCompareType.BeginsWith:
                    return this.IsStartWith(expected, text);

                case TextCompareType.DoesNotBeginWith:
                    return !this.IsStartWith(expected, text);

                case TextCompareType.EndsWith:
                    return this.IsEndWith(expected, text);

                case TextCompareType.DoesNotEndWith:
                    return !this.IsEndWith(expected, text);

                case TextCompareType.Contains:
                    return this.IsContains(expected, text);

                case TextCompareType.DoesNotContain:
                    return !this.IsContains(expected, text);
            }
            return false;
        }

        /// <summary>
        /// Creates a TextCondition  object from a formula.
        /// </summary>
        /// <param name="compareType">The compare type.</param>
        /// <param name="formula">The formula.</param>
        /// <returns>The TextCondition object.</returns>
        public static TextCondition FromFormula(TextCompareType compareType, string formula)
        {
            return new TextCondition(compareType, null, formula);
        }

        /// <summary>
        /// Creates a TextCondition object from a string.
        /// </summary>
        /// <param name="compareType">The compare type.</param>
        /// <param name="expected">The expected string.</param>
        /// <returns>The TextCondition object.</returns>
        public static TextCondition FromString(TextCompareType compareType, string expected)
        {
            return new TextCondition(compareType, expected, null);
        }

        /// <summary>
        /// Determines whether the specified text has a wildcard.
        /// </summary>
        /// <param name="text">The text expression.</param>
        /// <returns>
        /// <c>true</c> if the specified text has a wildcard; otherwise, <c>false</c>.
        /// </returns>
        static bool HasWildcard(string text)
        {
            if ((text == null) || (text == string.Empty))
            {
                return false;
            }
            if (!text.Contains(AsteriskWildcard))
            {
                return text.Contains(QuestionMarkWildcard);
            }
            return true;
        }

        /// <summary>
        /// Determines whether the specified expected value contains a certain string.
        /// </summary>
        /// <param name="expected">The expected value</param>
        /// <param name="value">The value</param>
        /// <returns>
        /// <c>true</c> if the specified expected value contains the string; otherwise, <c>false</c>.
        /// </returns>
        bool IsContains(string expected, string value)
        {
            if (this.UseWildCards && HasWildcard(expected))
            {
                Regex regex = this.CreateContainsRegex(expected);
                return ((regex != null) && regex.Match((value == null) ? string.Empty : value).Success);
            }
            if ((value != null) && (value != string.Empty))
            {
                if (expected == null)
                {
                    return false;
                }
                if (this.IgnoreCase)
                {
                    return (value.IndexOf(expected, 0, (StringComparison) StringComparison.CurrentCultureIgnoreCase) > -1);
                }
                return (value.IndexOf(expected, 0, (StringComparison) StringComparison.CurrentCulture) > -1);
            }
            if (expected != null)
            {
                return (expected == string.Empty);
            }
            return true;
        }

        /// <summary>
        /// Determines whether the specified text ends with a certain string.
        /// </summary>
        /// <param name="expected">String to check for</param>
        /// <param name="value">Text to check</param>
        /// <returns>
        /// <c>true</c> if the specified text ends with the string; otherwise, <c>false</c>.
        /// </returns>
        bool IsEndWith(string expected, string value)
        {
            if (this.UseWildCards && HasWildcard(expected))
            {
                Regex regex = this.CreateEndWithRegex(expected);
                return ((regex != null) && regex.Match((value == null) ? string.Empty : value).Success);
            }
            if ((value != null) && (value != string.Empty))
            {
                if (expected == null)
                {
                    return false;
                }
                if (this.IgnoreCase)
                {
                    return value.EndsWith(expected, (StringComparison) StringComparison.CurrentCultureIgnoreCase);
                }
                return value.EndsWith(expected, (StringComparison) StringComparison.CurrentCulture);
            }
            if (expected != null)
            {
                return (expected == string.Empty);
            }
            return true;
        }

        /// <summary>
        /// Determines whether the specified expected value is equal to the specified value.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// <c>true</c> if the specified expected value is equal; otherwise, <c>false</c>.
        /// </returns>
        bool IsEquals(string expected, string value)
        {
            if (this.UseWildCards && HasWildcard(expected))
            {
                Regex regex = this.CreateEqualsRegex(expected);
                return ((regex != null) && regex.Match((value == null) ? string.Empty : value).Success);
            }
            if ((value != null) && (value != string.Empty))
            {
                if (this.IgnoreCase)
                {
                    return value.Equals((expected != null) ? expected : string.Empty, (StringComparison) StringComparison.CurrentCultureIgnoreCase);
                }
                return value.Equals((expected != null) ? expected : string.Empty, (StringComparison) StringComparison.CurrentCulture);
            }
            if (expected != null)
            {
                return (expected == string.Empty);
            }
            return true;
        }

        /// <summary>
        /// Determines whether the specified text starts with a certain string.
        /// </summary>
        /// <param name="expected">The string to check for.</param>
        /// <param name="value">The text to check.</param>
        bool IsStartWith(string expected, string value)
        {
            if (this.UseWildCards && HasWildcard(expected))
            {
                Regex regex = this.CreateStartWithRegex(expected);
                return ((regex != null) && regex.Match((value == null) ? string.Empty : value).Success);
            }
            if ((value != null) && (value != string.Empty))
            {
                if (expected == null)
                {
                    return false;
                }
                if (this.IgnoreCase)
                {
                    return value.StartsWith(expected, (StringComparison) StringComparison.CurrentCultureIgnoreCase);
                }
                return value.StartsWith(expected, (StringComparison) StringComparison.CurrentCulture);
            }
            if (expected != null)
            {
                return (expected == string.Empty);
            }
            return true;
        }

        /// <summary>
        /// Called when reading XML.
        /// </summary>
        /// <param name="reader">The reader.</param>
        protected override void OnReadXml(XmlReader reader)
        {
            base.OnReadXml(reader);
            string name = reader.Name;
            if (name != null)
            {
                if (name != "Type")
                {
                    if (name != "UseWildCards")
                    {
                        if (name == "IgnoreCase")
                        {
                            this.ignoreCase = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                            return;
                        }
                        if (name == "ForceText")
                        {
                            this.forceValue2Text = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                        }
                        return;
                    }
                }
                else
                {
                    this.compareType = (TextCompareType) Serializer.DeserializeObj(typeof(TextCompareType), reader);
                    return;
                }
                this.useWildCards = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
            }
        }

        /// <summary>
        /// Called when writing XML.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected override void OnWriteXml(XmlWriter writer)
        {
            base.OnWriteXml(writer);
            if (this.compareType != TextCompareType.EqualsTo)
            {
                Serializer.SerializeObj(this.compareType, "Type", writer);
            }
            if (!this.useWildCards)
            {
                Serializer.SerializeObj((bool) this.useWildCards, "UseWildCards", writer);
            }
            if (this.ignoreCase)
            {
                Serializer.SerializeObj((bool) this.ignoreCase, "IgnoreCase", writer);
            }
            if (this.forceValue2Text)
            {
                Serializer.SerializeObj((bool) this.forceValue2Text, "ForceText", writer);
            }
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        protected override void Reset()
        {
            base.Reset();
            this.compareType = TextCompareType.EqualsTo;
            this.useWildCards = true;
            this.ignoreCase = false;
            this.forceValue2Text = false;
        }

        /// <summary>
        /// Gets the type of comparison to perform.
        /// </summary>
        /// <value>The type of comparison. The default value is <see cref="T:Dt.Cells.Data.TextCompareType">Equals</see>.</value>
        [DefaultValue(0)]
        public TextCompareType CompareType
        {
            get { return  this.compareType; }
            set { this.compareType = value; }
        }

        /// <summary>
        /// Gets the expected type of the value.
        /// </summary>
        public override Type ExpectedValueType
        {
            get { return  typeof(string); }
        }

        internal bool ForceValue2Text
        {
            get { return  this.forceValue2Text; }
            set { this.forceValue2Text = value; }
        }

        /// <summary>
        /// Gets or sets whether to ignore case when performing the comparison.
        /// </summary>
        /// <value>
        /// <c>true</c> if the comparison should ignore case when matching; otherwise, <c>false</c>.
        /// The default value is <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        public bool IgnoreCase
        {
            get { return  this.ignoreCase; }
            set { this.ignoreCase = value; }
        }

        /// <summary>
        /// Gets or sets whether to compare strings using wildcards.
        /// </summary>
        /// <value>
        /// <c>true</c> if the comparison should use a wild card for matching; otherwise, <c>false</c>.
        /// The default value is <c>true</c>.
        /// </value>
        [DefaultValue(true)]
        public bool UseWildCards
        {
            get { return  this.useWildCards; }
            set { this.useWildCards = value; }
        }
    }
}

