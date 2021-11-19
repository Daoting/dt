#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a suspend state.
    /// </summary>
    internal sealed class StateManager
    {
        /// <summary>
        /// The states
        /// </summary>
        IDictionary<string, WorkingState> states;

        /// <summary>
        /// Suspends the specified key.
        /// </summary>
        /// <param name="key">Key to suspend.</param>
        public void AddRef(string key)
        {
            if (this.states == null)
            {
                this.states = (IDictionary<string, WorkingState>) new Dictionary<string, WorkingState>();
            }
            if (this.states.ContainsKey(key))
            {
                this.states[key].AddRef();
            }
            else
            {
                WorkingState state2 = new WorkingState();
                state2.AddRef();
                this.states.Add(key, state2);
            }
        }

        /// <summary>
        /// Determines whether the specified key is suspended.
        /// </summary>
        /// <param name="key">Key to check.</param>
        /// <returns>
        /// <c>true</c> if the specified key is suspended; otherwise, <c>false</c>.
        /// </returns>
        public bool IsOnState(string key)
        {
            return (((this.states != null) && this.states.ContainsKey(key)) && this.states[key].IsWorking);
        }

        /// <summary>
        /// Resumes the specified key.
        /// </summary>
        /// <param name="key">Key to resume.</param>
        public void Release(string key)
        {
            if ((this.states != null) && this.states.ContainsKey(key))
            {
                this.states[key].Release();
            }
        }

        /// <summary>
        /// Resets the specified key.
        /// </summary>
        /// <param name="key">Key to reset.</param>
        public void Reset(string key)
        {
            if ((this.states != null) && this.states.ContainsKey(key))
            {
                this.states[key].Reset();
            }
        }
    }
}

