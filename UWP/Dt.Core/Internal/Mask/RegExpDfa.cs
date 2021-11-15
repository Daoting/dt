#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2010-09-24 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Globalization;
#endregion

namespace Dt.Core.Mask
{
    /// <summary>
    /// 
    /// </summary>
    public class RegExpDfa
    {
        #region 静态内容
        static RegExpDfa HardAnd(RegExpDfa head, RegExpDfa tail)
        {
            RegExpDfa dfa = new RegExpDfa(head);
            Dictionary<RegExpState, RegExpState> dictionary = new Dictionary<RegExpState, RegExpState>();
            foreach (RegExpState state in tail.GetAllStates())
            {
                RegExpState state2 = object.ReferenceEquals(tail._initialState, state) ? dfa._finalState : new RegExpState();
                dictionary.Add(state, state2);
            }
            dfa._finalState = dictionary[tail._finalState];
            foreach (RegExpState state3 in dictionary.Keys)
            {
                foreach (Transition transition in state3.Transitions)
                {
                    RegExpState state4 = dictionary[state3];
                    RegExpState target = dictionary[transition.Target];
                    state4.AddTransition(transition.Copy(target));
                }
            }
            return dfa;
        }

        static RegExpDfa HardOr(RegExpDfa one, RegExpDfa merged)
        {
            RegExpDfa dfa = new RegExpDfa(one);
            Dictionary<RegExpState, RegExpState> dictionary = new Dictionary<RegExpState, RegExpState>();
            foreach (RegExpState state in merged.GetAllStates())
            {
                RegExpState initialState;
                if (object.ReferenceEquals(merged._initialState, state))
                {
                    initialState = dfa._initialState;
                }
                else if (object.ReferenceEquals(merged._finalState, state))
                {
                    initialState = dfa._finalState;
                }
                else
                {
                    initialState = new RegExpState();
                }
                dictionary.Add(state, initialState);
            }
            foreach (RegExpState state3 in dictionary.Keys)
            {
                foreach (Transition transition in state3.Transitions)
                {
                    RegExpState state4 = dictionary[state3];
                    RegExpState target = dictionary[transition.Target];
                    state4.AddTransition(transition.Copy(target));
                }
            }
            return dfa;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pattern"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public static bool IsMatch(string input, string pattern, CultureInfo cultureInfo)
        {
            return Parse(pattern, cultureInfo).IsMatch(input);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static RegExpDfa operator &(RegExpDfa left, RegExpDfa right)
        {
            if (right == null)
            {
                return left;
            }
            if (left == null)
            {
                return right;
            }
            if (left.CanReturnFromFinalState() && right.CanReturnToInitialState())
            {
                left = HardAnd(left, EmptyTransitionDfa);
            }
            return HardAnd(left, right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static RegExpDfa operator |(RegExpDfa left, RegExpDfa right)
        {
            if (left.CanReturnToInitialState())
            {
                left = HardAnd(EmptyTransitionDfa, left);
            }
            if (right.CanReturnToInitialState())
            {
                right = HardAnd(EmptyTransitionDfa, right);
            }
            if (left.CanReturnFromFinalState())
            {
                left = HardAnd(left, EmptyTransitionDfa);
            }
            if (right.CanReturnFromFinalState())
            {
                right = HardAnd(right, EmptyTransitionDfa);
            }
            return HardOr(left, right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public static RegExpDfa Parse(string pattern, CultureInfo cultureInfo)
        {
            return Parse(pattern, false, cultureInfo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="reverseAutomate"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public static RegExpDfa Parse(string pattern, bool reverseAutomate, CultureInfo cultureInfo)
        {
            return RegExpParser.Parse(pattern, reverseAutomate, cultureInfo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operand"></param>
        /// <param name="minMatches"></param>
        /// <param name="maxMatches"></param>
        /// <returns></returns>
        public static RegExpDfa Power(RegExpDfa operand, int minMatches, int maxMatches)
        {
            RegExpDfa empty;
            if (object.ReferenceEquals(operand._initialState, operand._finalState))
            {
                return operand;
            }
            if (maxMatches == -1)
            {
                if (minMatches == 0)
                {
                    empty = Power0Unlimited(operand);
                }
                else
                {
                    empty = PowerExact(operand, minMatches - 1) & Power1Unlimited(operand);
                }
            }
            else
            {
                empty = PowerExact(operand, minMatches) & PowerOptional(operand, maxMatches - minMatches);
            }
            if (empty == null)
            {
                empty = Empty;
            }
            return empty;
        }

        static RegExpDfa Power0Unlimited(RegExpDfa operand)
        {
            if (operand.CanReturnFromFinalState())
            {
                operand = HardAnd(operand, EmptyTransitionDfa);
            }
            if (operand.CanReturnToInitialState())
            {
                operand = HardAnd(EmptyTransitionDfa, operand);
            }
            return HardOr(Empty, operand);
        }

        static RegExpDfa Power1Unlimited(RegExpDfa operand)
        {
            RegExpDfa dfa = new RegExpDfa(operand);
            dfa._finalState.AddTransition(new EmptyTransition(dfa._initialState));
            return dfa;
        }

        static RegExpDfa PowerExact(RegExpDfa operand, int power)
        {
            if (power == 0)
            {
                return null;
            }
            if (power < 0)
            {
                throw new ArgumentException("Incorrect power", "power");
            }
            if (power == 1)
            {
                return operand;
            }
            int num = power / 2;
            RegExpDfa dfa = PowerExact(operand, num);
            dfa &= dfa;
            if ((num + num) != power)
            {
                dfa &= operand;
            }
            return dfa;
        }

        static RegExpDfa PowerOptional(RegExpDfa operand, int count)
        {
            if (count == 0)
            {
                return null;
            }
            RegExpDfa head = new RegExpDfa(operand);
            if (head.CanReturnFromFinalState())
            {
                head = HardAnd(head, EmptyTransitionDfa);
            }
            if (head.CanReturnToInitialState())
            {
                head = HardAnd(EmptyTransitionDfa, head);
            }
            RegExpDfa dfa2 = new RegExpDfa(operand) | EmptyTransitionDfa;
            for (int i = 1; i < count; i++)
            {
                Dictionary<RegExpState, RegExpState> dictionary = new Dictionary<RegExpState, RegExpState>();
                foreach (RegExpState state in head.GetAllStates())
                {
                    RegExpState initialState;
                    if (object.ReferenceEquals(state, head._finalState))
                    {
                        initialState = dfa2._initialState;
                    }
                    else
                    {
                        initialState = new RegExpState();
                    }
                    dictionary.Add(state, initialState);
                }
                dfa2._initialState = dictionary[head._initialState];
                foreach (RegExpState state3 in dictionary.Keys)
                {
                    foreach (Transition transition in state3.Transitions)
                    {
                        RegExpState state4 = dictionary[state3];
                        RegExpState target = dictionary[transition.Target];
                        state4.AddTransition(transition.Copy(target));
                    }
                }
                dfa2._initialState.AddTransition(new EmptyTransition(dfa2._finalState));
            }
            return dfa2;
        }
        #endregion
        
        #region 成员变量
        RegExpState _finalState;
        RegExpState _initialState;
        RegExpStringKey _lastState;
        string _lastText;
        readonly Dictionary<RegExpDfaWave, RegExpDfaWave> _statesCache;
        readonly RegExpStringKeyTable _stringsToStatesCache;
        #endregion

        #region 构造方法
        RegExpDfa()
        {
            this._statesCache = new Dictionary<RegExpDfaWave, RegExpDfaWave>();
            this._stringsToStatesCache = new RegExpStringKeyTable();
            this._initialState = new RegExpState();
            this._finalState = this._initialState;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public static RegExpDfa Empty
        {
            get { return new RegExpDfa(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static RegExpDfa EmptyTransitionDfa
        {
            get { return new RegExpDfa(new EmptyTransition()); }
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="initialTransition"></param>
        public RegExpDfa(Transition initialTransition)
        {
            this._statesCache = new Dictionary<RegExpDfaWave, RegExpDfaWave>();
            this._stringsToStatesCache = new RegExpStringKeyTable();
            this._initialState = new RegExpState();
            this._initialState.AddTransition(initialTransition);
            this._finalState = initialTransition.Target;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ICollection<RegExpState> GetAllStates()
        {
            return this._initialState.GetReachableStates();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public RegExpAutoCompleteInfo GetAutoCompleteInfo(string text)
        {
            return this.GetWave(text).GetAutoCompleteInfo();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="displayText"></param>
        /// <returns></returns>
        public string GetOptimisticHint(string displayText)
        {
            return this.GetWave(displayText).GetOptimisticHint();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="displayText"></param>
        /// <param name="anySymbolHolder"></param>
        /// <returns></returns>
        public string GetPlaceHolders(string displayText, char anySymbolHolder)
        {
            string str = string.Empty;
            foreach (object obj2 in this.GetPlaceHoldersInfo(displayText))
            {
                str = str + ((obj2 == null) ? anySymbolHolder : ((char)obj2));
            }
            return str;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="displayText"></param>
        /// <returns></returns>
        public object[] GetPlaceHoldersInfo(string displayText)
        {
            return this.GetWave(displayText).GetPlaceHoldersInfo();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool IsFinal(string input)
        {
            return (this.GetWave(input).GetAutoCompleteInfo().DfaAutoCompleteType == DfaAutoCompleteType.Final);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool IsMatch(string input)
        {
            return this.GetWave(input).Contains(this._finalState);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public bool IsValidStart(string start)
        {
            return (this.GetWave(start).Count > 0);
        }
        #endregion

        #region 内部方法
        RegExpDfa(RegExpDfa source)
        {
            this._statesCache = new Dictionary<RegExpDfaWave, RegExpDfaWave>();
            this._stringsToStatesCache = new RegExpStringKeyTable();
            Dictionary<RegExpState, RegExpState> dictionary = new Dictionary<RegExpState, RegExpState>();
            foreach (RegExpState state in source.GetAllStates())
            {
                dictionary.Add(state, new RegExpState());
            }
            this._initialState = dictionary[source._initialState];
            this._finalState = dictionary[source._finalState];
            foreach (RegExpState state2 in dictionary.Keys)
            {
                foreach (Transition transition in state2.Transitions)
                {
                    Transition transition2 = transition.Copy(dictionary[transition.Target]);
                    dictionary[state2].AddTransition(transition2);
                }
            }
        }

        RegExpStringKey CacheWave(string str, RegExpStringKey next, char symbol, RegExpDfaWave candidateWave)
        {
            RegExpDfaWave wave;
            if (candidateWave.Count == 0)
            {
                return new RegExpStringKey(str, candidateWave, next, symbol);
            }
            if (!this._statesCache.TryGetValue(candidateWave, out wave))
            {
                wave = candidateWave;
                this._statesCache.Add(wave, wave);
            }
            RegExpStringKey key = new RegExpStringKey(str, wave, next, symbol);
            this._stringsToStatesCache.Add(key);
            return key;
        }

        bool CanReturnFromFinalState()
        {
            if (!object.ReferenceEquals(this._initialState, this._finalState))
            {
                return (this._finalState.Transitions.Count > 0);
            }
            return true;
        }

        bool CanReturnToInitialState()
        {
            if (object.ReferenceEquals(this._initialState, this._finalState))
            {
                return true;
            }
            foreach (RegExpState state in this.GetAllStates())
            {
                foreach (Transition transition in state.Transitions)
                {
                    if (object.ReferenceEquals(this._initialState, transition.Target))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        RegExpDfaWave GetInitialStates()
        {
            RegExpDfaWave wave = new RegExpDfaWave(this._finalState);
            wave.AddStateWithEmptyTransitionsTargets(this._initialState);
            return wave;
        }

        RegExpDfaWave GetWave(string text)
        {
            RegExpStringKey next = null;
            int length = text.Length;
            do
            {
                if (text != this._lastText)
                {
                    next = this._stringsToStatesCache[(text.Length == length) ? text : text.Substring(0, length)];
                }
                else
                {
                    next = this._lastState;
                }
                length--;
            }
            while ((length >= 0) && (next == null));
            if (next == null)
            {
                next = this.CacheWave(string.Empty, null, '\0', this.GetInitialStates());
            }
            length++;
            while (length < text.Length)
            {
                next = this.CacheWave((text.Length == (length + 1)) ? text : text.Substring(0, length + 1), next, text[length], next._Wave.GetNextWave(text[length]));
                length++;
            }
            this._lastState = next;
            this._lastText = text;
            return next._Wave;
        }

        string ToString(RegExpDfaWave wave)
        {
            ICollection<RegExpState> allStates = this.GetAllStates();
            Dictionary<RegExpState, string> dictionary = new Dictionary<RegExpState, string>();
            int num = 0;
            foreach (RegExpState state in allStates)
            {
                string str = string.Empty;
                if (object.ReferenceEquals(this._initialState, state))
                {
                    str = str + 'i';
                }
                if (object.ReferenceEquals(this._finalState, state))
                {
                    str = str + 'f';
                }
                if (str.Length == 0)
                {
                    num++;
                    str = num.ToString();
                }
                dictionary.Add(state, str);
            }
            string str2 = string.Empty;
            if (wave != null)
            {
                str2 = str2 + "Wave:";
                foreach (RegExpState state2 in wave)
                {
                    str2 = str2 + ' ';
                    str2 = str2 + dictionary[state2];
                }
                str2 = str2 + Environment.NewLine;
            }
            str2 = str2 + "Dfa:";
            foreach (RegExpState state3 in allStates)
            {
                foreach (Transition transition in state3.Transitions)
                {
                    string str3 = str2;
                    str2 = str3 + Environment.NewLine + dictionary[state3] + " " + transition.ToString() + " -> " + dictionary[transition.Target];
                }
            }
            return str2;
        }
        #endregion 

        #region 重写方法
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.ToString(null);
        }
        #endregion
    }
}

