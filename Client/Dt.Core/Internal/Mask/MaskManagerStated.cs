#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2010-09-24 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core.Mask
{
    /// <summary>
    /// 有状态的掩码员抽象基类
    /// 子类有 RegExpMaskManager, LegacyMaskManager, NumericMaskManager
    /// </summary>
    public abstract class MaskManagerStated : MaskManager
    {
        #region 成员变量
        MaskManagerState backupState;
        StateChangeType backupType = StateChangeType.Terminator;
        // 光标位置
        int cachedDCP = -1;
        // 选中区域的起始位置
        int cachedDSA = -1;
        // 显示文本
        string cachedDT;
        // 内部缓存的掩码状态，用来判断以上三项是否需要重新计算值
        MaskManagerState cachedDState;
        // 当前掩码状态
        MaskManagerState currentState;

        #endregion

        #region 构造方法
        /// <summary>
        /// 根据给的掩码状态构造掩码员
        /// </summary>
        /// <param name="initialState"></param>
        protected MaskManagerStated(MaskManagerState initialState)
        {
            SetInitialState(initialState);
        }
        #endregion

        #region 属性
        /// <summary>
        /// 是否可撤消
        /// </summary>
        public override bool CanUndo
        {
            get { return ((backupState != null) && (currentState != null)); }
        }

        /// <summary>
        /// 当前掩码状态
        /// </summary>
        protected MaskManagerState CurrentState
        {
            get { return currentState; }
        }

        /// <summary>
        /// 光标位置
        /// </summary>
        public sealed override int DisplayCursorPosition
        {
            get
            {
                VerifyCache();
                if (cachedDCP < 0)
                {
                    cachedDCP = GetCursorPosition(CurrentState);
                }
                return cachedDCP;
            }
        }

        /// <summary>
        /// 选中区域的起始位置
        /// </summary>
        public sealed override int DisplaySelectionAnchor
        {
            get
            {
                VerifyCache();
                if (cachedDSA < 0)
                {
                    cachedDSA = GetSelectionAnchor(CurrentState);
                }
                return cachedDSA;
            }
        }

        /// <summary>
        /// 显示文本
        /// </summary>
        public sealed override string DisplayText
        {
            get
            {
                VerifyCache();
                if (cachedDT == null)
                {
                    cachedDT = GetDisplayText(CurrentState);
                }
                return cachedDT;
            }
        }
        #endregion

        #region 重写方法
        /// <summary>
        /// 获取掩码员的当前值
        /// </summary>
        /// <returns></returns>
        public sealed override object GetCurrentEditValue()
        {
            return GetEditValue(CurrentState);
        }

        /// <summary>
        /// 获取当前文本
        /// </summary>
        /// <returns></returns>
        public sealed override string GetCurrentEditText()
        {
            return GetEditText(CurrentState);
        }

        /// <summary>
        /// 下调(向下方向键)转为光标右移
        /// </summary>
        /// <returns></returns>
        public override bool SpinDown()
        {
            return base.CursorRight(false);
        }

        /// <summary>
        /// 上调(向上方向键)转为光标左移
        /// </summary>
        /// <returns></returns>
        public override bool SpinUp()
        {
            return base.CursorLeft(false);
        }

        /// <summary>
        /// 撤消操作
        /// </summary>
        /// <returns></returns>
        public override bool Undo()
        {
            if (!CanUndo)
            {
                return false;
            }
            if (!RaiseEditTextChanging(GetEditText(backupState)))
            {
                return false;
            }
            backupType = StateChangeType.Terminator;
            MaskManagerState currentState = this.currentState;
            currentState = backupState;
            backupState = currentState;
            base.RaiseEditTextChanged();
            return true;
        }

        #endregion

        #region 内部方法
        /// <summary>
        /// 设置初始掩码状态
        /// </summary>
        /// <param name="newState"></param>
        protected void SetInitialState(MaskManagerState newState)
        {
            backupState = null;
            currentState = newState;
            backupType = StateChangeType.Terminator;
        }

        /// <summary>
        /// 应用新状态的修改
        /// </summary>
        /// <param name="newState">新状态</param>
        /// <param name="changeType">状态变换种类</param>
        /// <returns>应用是否成功</returns>
        protected bool Apply(MaskManagerState newState, StateChangeType changeType)
        {
            return Apply(newState, changeType, false);
        }

        /// <summary>
        /// 应用新状态的修改
        /// </summary>
        /// <param name="newState">新状态</param>
        /// <param name="changeType">状态变换种类</param>
        /// <param name="isNeededKeyCheck"></param>
        /// <returns>应用是否成功</returns>
        protected bool Apply(MaskManagerState newState, StateChangeType changeType, bool isNeededKeyCheck)
        {
            if (!IsValid(newState))
            {
                return false;
            }
            if (newState.IsSame(CurrentState))
            {
                return false;
            }
            string editText = GetEditText(newState);
            if (!isNeededKeyCheck)
            {
                if (GetCurrentEditText() != editText)
                {
                    if (!RaiseEditTextChanging(editText))
                    {
                        return false;
                    }
                    ApplyInternal(newState, changeType);
                    base.RaiseEditTextChanged();
                }
                else
                {
                    ApplyInternal(newState, changeType);
                }
            }
            return true;
        }

        /// <summary>
        /// 记录状态及状态变换类别
        /// </summary>
        /// <param name="newState"></param>
        /// <param name="changeType"></param>
        void ApplyInternal(MaskManagerState newState, StateChangeType changeType)
        {
            if (backupType != changeType)
            {
                if (changeType != StateChangeType.Terminator)
                {
                    backupState = currentState;
                }
                backupType = changeType;
            }
            currentState = newState;
        }

        /// <summary>
        /// 同步状态缓存
        /// </summary>
        void VerifyCache()
        {
            if (!object.ReferenceEquals(cachedDState, CurrentState))
            {
                cachedDState = CurrentState;
                // 光标位置
                cachedDCP = -1;
                // 选中区域的起始位置
                cachedDSA = -1;
                // 显示文本
                cachedDT = null;
            }
        }
        #endregion

        #region 抽象方法
        /// <summary>
        /// 根据掩码状态获取当前值
        /// </summary>
        /// <returns></returns>
        protected abstract object GetEditValue(MaskManagerState state);

        /// <summary>
        /// 根据掩码状态获取当前文本
        /// </summary>
        /// <returns></returns>
        protected abstract string GetEditText(MaskManagerState state);

        /// <summary>
        /// 根据掩码状态获取当前光标位置
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected abstract int GetCursorPosition(MaskManagerState state);

        /// <summary>
        /// 获取给定掩码状态的显示文本
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected abstract string GetDisplayText(MaskManagerState state);
        
        /// <summary>
        /// 选择区域的起始位置
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected abstract int GetSelectionAnchor(MaskManagerState state);
        #endregion 

        #region 虚拟方法
        /// <summary>
        /// 是否有效
        /// </summary>
        /// <param name="newState"></param>
        /// <returns></returns>
        protected virtual bool IsValid(MaskManagerState newState)
        {
            return true;
        }
        #endregion

        #region   枚举成员
        /// <summary>
        /// 状态变换种类
        /// </summary>
        protected enum StateChangeType
        {
            // 插入
            /// <summary>
            /// 
            /// </summary>
            Insert,

            // 删除
            /// <summary>
            /// 
            /// </summary>
            Delete,

            // 终止
            /// <summary>
            /// 
            /// </summary>
            Terminator
        }
        #endregion
    }
}

