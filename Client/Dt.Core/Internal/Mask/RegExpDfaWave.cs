#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2010-09-24 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Core.Mask
{
    /// <summary>
    /// 
    /// </summary>
    public class RegExpDfaWave : IEnumerable
    {
        #region 静态内容
        static object[] MergeMasks(object[] firstMask, object[] secondMask)
        {
            object[] objArray = firstMask;
            object[] objArray2 = secondMask;
            int index = 0;
            for (int i = 0; index < objArray.Length; i++)
            {
                if (objArray[index] != null)
                {
                    bool flag = false;
                    for (int j = i; (objArray2.Length - j) >= (objArray.Length - index); j++)
                    {
                        if ((objArray2[j] != null) && (((char)objArray[index]) == ((char)objArray2[j])))
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        objArray[index] = null;
                    }
                }
                index++;
            }
            return objArray;
        }
        #endregion

        #region 成员变量
        RegExpAutoCompleteInfo _autoCompleteInfo;
        readonly RegExpState _finalState;
        int _hashCode;
        bool _hashCodeCalculated;
        object[] _placeHoldersInfo;
        string _sample;
        readonly Dictionary<RegExpState, bool> _states = new Dictionary<RegExpState, bool>();
        #endregion

        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get { return this._states.Count; }
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="finalState"></param>
        public RegExpDfaWave(RegExpState finalState)
        {
            this._finalState = finalState;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        public void AddStateWithEmptyTransitionsTargets(RegExpState state)
        {
            if (!this.Contains(state))
            {
                this.Add(state);
                foreach (Transition transition in state.Transitions)
                {
                    if (transition.IsEmpty)
                    {
                        this.AddStateWithEmptyTransitionsTargets(transition.Target);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool Contains(RegExpState state)
        {
            return this._states.ContainsKey(state);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public RegExpAutoCompleteInfo GetAutoCompleteInfo()
        {
            if (this._autoCompleteInfo == null)
            {
                DfaAutoCompleteType none;
                GetAutoCompleteInfoTransitionsProcessingResult autoCompleteInfoTransitionsProcessing = this.GetAutoCompleteInfoTransitionsProcessing();
                if (autoCompleteInfoTransitionsProcessing._NonExactsFound)
                {
                    none = DfaAutoCompleteType.None;
                }
                else if (autoCompleteInfoTransitionsProcessing._ExactCharFound)
                {
                    if (!this.Contains(this._finalState))
                    {
                        none = DfaAutoCompleteType.ExactChar;
                    }
                    else
                    {
                        GetAutoCompleteInfoTransitionsProcessingResult result2 = autoCompleteInfoTransitionsProcessing;
                        RegExpDfaWave nextWave = this;
                        do
                        {
                            nextWave = nextWave.GetNextWave(result2._ExactChar);
                            result2 = nextWave.GetAutoCompleteInfoTransitionsProcessing();
                        }
                        while ((!result2._NonExactsFound && result2._ExactCharFound) && !nextWave.Contains(this._finalState));
                        if (nextWave.Contains(this._finalState))
                        {
                            if (result2._NonExactsFound)
                            {
                                none = DfaAutoCompleteType.FinalOrExactBeforeFinalOrNone;
                            }
                            else
                            {
                                none = DfaAutoCompleteType.FinalOrExactBeforeFinal;
                            }
                        }
                        else
                        {
                            none = DfaAutoCompleteType.FinalOrExactBeforeNone;
                        }
                    }
                }
                else
                {
                    none = DfaAutoCompleteType.Final;
                }
                this._autoCompleteInfo = new RegExpAutoCompleteInfo(none, autoCompleteInfoTransitionsProcessing._ExactChar);
            }
            return this._autoCompleteInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return this._states.Keys.GetEnumerator();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public RegExpDfaWave GetNextWave(char input)
        {
            RegExpDfaWave wave = new RegExpDfaWave(this._finalState);
            foreach (RegExpState state in this)
            {
                foreach (Transition transition in state.Transitions)
                {
                    if (transition.IsMatch(input))
                    {
                        wave.AddStateWithEmptyTransitionsTargets(transition.Target);
                    }
                }
            }
            return wave;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetOptimisticHint()
        {
            this.CalculatePlaceHoldersInfo();
            return this._sample;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object[] GetPlaceHoldersInfo()
        {
            this.CalculatePlaceHoldersInfo();
            return this._placeHoldersInfo;
        }
        #endregion

        #region 内部方法
        void Add(RegExpState state)
        {
            this._states.Add(state, false);
        }

        void CalculatePlaceHoldersInfo()
        {
            if (this._placeHoldersInfo == null)
            {
                List<PlaceHoldersPredictAssociation> completeHolders = new List<PlaceHoldersPredictAssociation>();
                ICollection<PlaceHoldersPredictAssociation> currentHolders = new List<PlaceHoldersPredictAssociation>();
                foreach (RegExpState state in this)
                {
                    if (object.ReferenceEquals(this._finalState, state))
                    {
                        this._placeHoldersInfo = new object[0];
                        this._sample = string.Empty;
                        return;
                    }
                    currentHolders.Add(new PlaceHoldersPredictAssociation(state));
                }
                while (currentHolders.Count != 0)
                {
                    currentHolders = this.PlaceHoldersPredictGetNextHolders(completeHolders, currentHolders);
                }
                if (completeHolders.Count != 0)
                {
                    completeHolders.Sort(delegate(PlaceHoldersPredictAssociation a, PlaceHoldersPredictAssociation b)
                    {
                        return a._PlaceHolders.Length - b._PlaceHolders.Length;
                    });
                    foreach (PlaceHoldersPredictAssociation association in completeHolders)
                    {
                        if (this._placeHoldersInfo == null)
                        {
                            this._placeHoldersInfo = association._PlaceHolders;
                        }
                        else
                        {
                            this._placeHoldersInfo = MergeMasks(this._placeHoldersInfo, association._PlaceHolders);
                        }
                        if (((this._sample == null) || (this._sample.Length > association._OptimisticHint.Length)) || ((this._sample.Length == association._OptimisticHint.Length) && (string.Compare(this._sample, association._OptimisticHint) > 0)))
                        {
                            this._sample = association._OptimisticHint;
                        }
                    }
                }
                else
                {
                    this._placeHoldersInfo = new object[0];
                    this._sample = string.Empty;
                }
            }
        }

        GetAutoCompleteInfoTransitionsProcessingResult GetAutoCompleteInfoTransitionsProcessing()
        {
            GetAutoCompleteInfoTransitionsProcessingResult result = new GetAutoCompleteInfoTransitionsProcessingResult();
            result._ExactCharFound = false;
            result._ExactChar = '\0';
            result._NonExactsFound = false;
            foreach (RegExpState state in this)
            {
                foreach (Transition transition in state.Transitions)
                {
                    if (transition.IsEmpty)
                    {
                        continue;
                    }
                    if (transition.IsExact)
                    {
                        if (result._ExactCharFound)
                        {
                            if (result._ExactChar != transition.GetSampleChar())
                            {
                                result._NonExactsFound = true;
                            }
                        }
                        else
                        {
                            result._ExactCharFound = true;
                            result._ExactChar = transition.GetSampleChar();
                        }
                    }
                    else
                    {
                        result._NonExactsFound = true;
                    }
                    if (result._NonExactsFound)
                    {
                        break;
                    }
                }
                if (result._NonExactsFound)
                {
                    return result;
                }
            }
            return result;
        }

        ICollection<PlaceHoldersPredictAssociation> PlaceHoldersPredictGetNextHolders(ICollection<PlaceHoldersPredictAssociation> completeHolders, ICollection<PlaceHoldersPredictAssociation> currentHolders)
        {
            List<PlaceHoldersPredictAssociation> list = new List<PlaceHoldersPredictAssociation>();
            foreach (PlaceHoldersPredictAssociation association in currentHolders)
            {
                foreach (Transition transition in association._State.Transitions)
                {
                    if (!association.CanSkip(transition.Target, this._finalState))
                    {
                        PlaceHoldersPredictAssociation item = new PlaceHoldersPredictAssociation(association, transition);
                        if (object.ReferenceEquals(this._finalState, item._State))
                        {
                            completeHolders.Add(item);
                        }
                        else
                        {
                            list.Add(item);
                        }
                    }
                }
            }
            return list;
        }
        #endregion

        #region 重写方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (!object.ReferenceEquals(obj, this))
            {
                if (obj.GetType() != base.GetType())
                {
                    return false;
                }
                if (((RegExpDfaWave) obj).Count != this.Count)
                {
                    return false;
                }
                if (obj.GetHashCode() != this.GetHashCode())
                {
                    return false;
                }
                foreach (RegExpState state in (RegExpDfaWave) obj)
                {
                    if (!this.Contains(state))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (!this._hashCodeCalculated)
            {
                this._hashCodeCalculated = true;
                this._hashCode = -1;
                foreach (RegExpState state in this)
                {
                    this._hashCode ^= state.GetHashCode();
                }
            }
            return this._hashCode;
        }
        #endregion

        [StructLayout(LayoutKind.Sequential)]
        struct GetAutoCompleteInfoTransitionsProcessingResult
        {
            #region 成员变量
            public bool _NonExactsFound;
            public bool _ExactCharFound;
            public char _ExactChar;
            #endregion
        }

        class PlaceHoldersPredictAssociation
        {
            #region 静态内容
            static bool CanReachPast(RegExpState nextState, RegExpState targetState, RegExpState pastState, IDictionary<RegExpState, bool> states)
            {
                if (nextState == targetState)
                {
                    return true;
                }
                if (nextState != pastState)
                {
                    if (states.ContainsKey(nextState))
                    {
                        return false;
                    }
                    states.Add(nextState, true);
                    foreach (Transition transition in nextState.Transitions)
                    {
                        if (CanReachPast(transition.Target, targetState, pastState, states))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            #endregion

            #region 成员变量
            public readonly string _OptimisticHint;
            public readonly RegExpDfaWave.PlaceHoldersPredictAssociation _PassedStatesSource;
            public readonly object[] _PlaceHolders;
            public readonly RegExpState _State;
            #endregion

            #region 外部方法
            public PlaceHoldersPredictAssociation(RegExpState state)
            {
                this._PlaceHolders = new object[0];
                this._State = state;
                this._PassedStatesSource = null;
                this._OptimisticHint = string.Empty;
            }

            public PlaceHoldersPredictAssociation(RegExpDfaWave.PlaceHoldersPredictAssociation prevHolder, Transition transition)
            {
                if (transition.IsEmpty)
                {
                    this._PlaceHolders = prevHolder._PlaceHolders;
                    this._OptimisticHint = prevHolder._OptimisticHint;
                }
                else
                {
                    this._PlaceHolders = new object[prevHolder._PlaceHolders.Length + 1];
                    prevHolder._PlaceHolders.CopyTo(this._PlaceHolders, 0);
                    if (transition.IsExact)
                    {
                        this._PlaceHolders[this._PlaceHolders.Length - 1] = transition.GetSampleChar();
                    }
                    this._OptimisticHint = prevHolder._OptimisticHint + transition.GetSampleChar();
                }
                this._State = transition.Target;
                this._PassedStatesSource = prevHolder;
            }

            public bool CanSkip(RegExpState suspectState, RegExpState finalState)
            {
                if (this.IsPassed(suspectState))
                {
                    return true;
                }
                if (suspectState != finalState)
                {
                    foreach (Transition transition in this._State.Transitions)
                    {
                        if (((transition.Target != suspectState) && transition.IsEmpty) && (suspectState.CanReach(transition.Target) && !CanReachPast(suspectState, finalState, transition.Target, new Dictionary<RegExpState, bool>())))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            #endregion

            #region 内部方法
            bool IsPassed(RegExpState state)
            {
                return ((this._State == state) || ((this._PassedStatesSource != null) && this._PassedStatesSource.IsPassed(state)));
            }
            #endregion
        }
    }
}

