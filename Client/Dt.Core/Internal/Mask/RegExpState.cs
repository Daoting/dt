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
#endregion

namespace Dt.Core.Mask
{
    /// <summary>
    /// 
    /// </summary>
    public class RegExpState
    {
        #region 静态内容
        static void CollectReachableStates(RegExpState nextState, IDictionary<RegExpState, bool> states)
        {
            if (!states.ContainsKey(nextState))
            {
                states.Add(nextState, true);
                foreach (Transition transition in nextState.Transitions)
                {
                    CollectReachableStates(transition.Target, states);
                }
            }
        }
        #endregion

        #region 构造方法
        IDictionary<RegExpState, bool> ReachableStatesDictionary
        {
            get
            {
                if (this.reachableStatesDictionary == null)
                {
                    this.reachableStatesDictionary = new Dictionary<RegExpState, bool>();
                    CollectReachableStates(this, this.reachableStatesDictionary);
                }
                return this.reachableStatesDictionary;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ICollection Transitions
        {
            get { return this.transitions; }
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="transition"></param>
        public void AddTransition(Transition transition)
        {
            this.transitions.Add(transition);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool CanReach(RegExpState state)
        {
            return this.ReachableStatesDictionary.ContainsKey(state);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ICollection<RegExpState> GetReachableStates()
        {
            return this.ReachableStatesDictionary.Keys;
        }
        #endregion

        #region 内部方法
        IDictionary<RegExpState, bool> reachableStatesDictionary;
        readonly List<Transition> transitions = new List<Transition>();
        #endregion
    }
}

