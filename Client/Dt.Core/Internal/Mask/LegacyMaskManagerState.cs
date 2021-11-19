#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2010-09-24 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Core.Mask
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class LegacyMaskManagerState : MaskManagerState
    {
        #region 成员变量
        /// <summary>
        /// 
        /// </summary>
        public int CursorPositionElement;

        /// <summary>
        /// 
        /// </summary>
        public int CursorPositionInsideElement;

        /// <summary>
        /// 
        /// </summary>
        public string[] Elements;

        /// <summary>
        /// 
        /// </summary>
        public LegacyMaskInfo Info;

        /// <summary>
        /// 
        /// </summary>
        public int SelectionAnchorElement;

        /// <summary>
        /// 
        /// </summary>
        public int SelectionAnchorInsideElement;
        #endregion
        
        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public int DisplayCursorPosition
        {
            get { return this.Info.GetPosition(this.Elements, this.CursorPositionElement, this.CursorPositionInsideElement); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int DisplaySelectionAnchor
        {
            get { return this.Info.GetPosition(this.Elements, this.SelectionAnchorElement, this.SelectionAnchorInsideElement); }
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        public LegacyMaskManagerState(LegacyMaskInfo info) : this(info, info.GetElementsEmpty(), 0, 0, 0, 0) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="elements"></param>
        /// <param name="cursorPositionElement"></param>
        /// <param name="cursorPositionInsideElement"></param>
        /// <param name="selectionAnchorElement"></param>
        /// <param name="selectionAnchorInsideElement"></param>
        public LegacyMaskManagerState(LegacyMaskInfo info, string[] elements, int cursorPositionElement, int cursorPositionInsideElement, int selectionAnchorElement, int selectionAnchorInsideElement)
        {
            this.Info = info;
            this.Elements = elements;
            this.CursorPositionElement = cursorPositionElement;
            this.CursorPositionInsideElement = cursorPositionInsideElement;
            this.SelectionAnchorElement = selectionAnchorElement;
            this.SelectionAnchorInsideElement = selectionAnchorInsideElement;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Backspace()
        {
            if (this.DisplayCursorPosition == this.DisplaySelectionAnchor)
            {
                this.CursorLeft();
            }
            this.Erase();
            this.CursorLeft();
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public LegacyMaskManagerState Clone()
        {
            return new LegacyMaskManagerState(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="forceSelection"></param>
        /// <returns></returns>
        public bool CursorEnd(bool forceSelection)
        {
            if (forceSelection || !this.Info.GetIsEditable())
            {
                return this.CursorTo(this.Info.GetDisplayText(this.Elements, '@').Length, forceSelection);
            }
            int lastEditableIndex = this.Info.GetLastEditableIndex();
            return this.SetCaretTo(lastEditableIndex, this.Info[lastEditableIndex].GetDisplayText(this.Elements[lastEditableIndex], '@').Length - 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="forceSelection"></param>
        /// <returns></returns>
        public bool CursorHome(bool forceSelection)
        {
            if (!forceSelection && this.Info.GetIsEditable())
            {
                return this.SetCaretTo(this.Info.GetFirstEditableIndex(), 0);
            }
            return this.CursorTo(0, forceSelection);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool CursorLeft()
        {
            if (!this.Info[this.CursorPositionElement].IsLiteral && (this.CursorPositionInsideElement > 0))
            {
                return this.SetCaretTo(this.CursorPositionElement, this.CursorPositionInsideElement - 1);
            }
            int prevEditableElement = this.Info.GetPrevEditableElement(this.CursorPositionElement);
            if (prevEditableElement >= 0)
            {
                return this.SetCaretTo(prevEditableElement, this.Info[prevEditableElement].GetDisplayText(this.Elements[prevEditableElement], '@').Length - 1);
            }
            prevEditableElement = this.Info.GetFirstEditableIndex();
            return ((prevEditableElement >= 0) && this.SetCaretTo(prevEditableElement, 0));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool CursorRight()
        {
            if (!this.Info[this.CursorPositionElement].IsLiteral && (this.CursorPositionInsideElement < (this.Info[this.CursorPositionElement].GetDisplayText(this.Elements[this.CursorPositionElement], '@').Length - 1)))
            {
                return this.SetCaretTo(this.CursorPositionElement, this.CursorPositionInsideElement + 1);
            }
            int nextEditableElement = this.Info.GetNextEditableElement(this.CursorPositionElement);
            if (nextEditableElement >= 0)
            {
                return this.SetCaretTo(nextEditableElement, 0);
            }
            nextEditableElement = this.Info.GetLastEditableIndex();
            return ((nextEditableElement >= 0) && this.SetCaretTo(nextEditableElement, this.Info[nextEditableElement].GetDisplayText(this.Elements[nextEditableElement], '@').Length - 1));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newPosition"></param>
        /// <param name="forceSelection"></param>
        /// <returns></returns>
        public bool CursorTo(int newPosition, bool forceSelection)
        {
            if ((newPosition < 0) || (newPosition > this.Info.GetDisplayText(this.Elements, '@').Length))
            {
                return false;
            }
            int element = 0;
            while (this.Info.GetPosition(this.Elements, element + 1, 0) < newPosition)
            {
                element++;
            }
            this.CursorPositionElement = element;
            this.CursorPositionInsideElement = newPosition - this.Info.GetPosition(this.Elements, element, 0);
            if (!forceSelection)
            {
                this.SelectionAnchorElement = this.CursorPositionElement;
                this.SelectionAnchorInsideElement = this.CursorPositionInsideElement;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Delete()
        {
            if (this.DisplayCursorPosition == this.DisplaySelectionAnchor)
            {
                this.CursorRight();
            }
            bool flag = (this.DisplaySelectionAnchor - this.DisplayCursorPosition) == 1;
            this.Erase();
            if (flag && (this.Elements[this.CursorPositionElement].Length <= this.CursorPositionInsideElement))
            {
                this.CursorRight();
            }
            this.SetCaretTo(this.CursorPositionElement, this.CursorPositionInsideElement);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blank"></param>
        /// <returns></returns>
        public string GetDisplayText(char blank)
        {
            return this.Info.GetDisplayText(this.Elements, blank);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blank"></param>
        /// <param name="saveLiteral"></param>
        /// <returns></returns>
        public string GetEditText(char blank, bool saveLiteral)
        {
            return this.Info.GetEditText(this.Elements, blank, saveLiteral);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="insertion"></param>
        /// <returns></returns>
        public bool Insert(string insertion)
        {
            if (insertion.Length == 0)
            {
                return this.Erase();
            }
            bool flag = false;
            foreach (char ch in insertion)
            {
                if (this.Insert(ch))
                {
                    flag = true;
                }
            }
            if (!flag)
            {
                return false;
            }
            if (this.Info[this.CursorPositionElement].IsLiteral)
            {
                this.CursorRight();
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            for (int i = 0; i < this.Info.Count; i++)
            {
                string str = this.Elements[i];
                if (((str != null) && (str.Length != 0)) && !this.Info[i].IsLiteral)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blank"></param>
        /// <returns></returns>
        public bool IsFinal(char blank)
        {
            if (!this.IsMatch(blank))
            {
                return false;
            }
            return ((this.CursorPositionElement == this.Info.GetLastEditableIndex()) && (this.CursorPositionInsideElement == this.Info[this.CursorPositionElement].MaxMatches));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blank"></param>
        /// <returns></returns>
        public bool IsMatch(char blank)
        {
            for (int i = 0; i < this.Info.Count; i++)
            {
                if ((!this.Info[i].IsLiteral && (this.Elements[i].Length < this.Info[i].MinMatches)) && !this.Info[i].IsAcceptableStrong(blank))
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region 内部方法
        LegacyMaskManagerState(LegacyMaskManagerState source) : this(source.Info, (string[]) source.Elements.Clone(), source.CursorPositionElement, source.CursorPositionInsideElement, source.SelectionAnchorElement, source.SelectionAnchorInsideElement) { }

        bool Erase()
        {
            int cursorPositionElement;
            int selectionAnchorElement;
            int cursorPositionInsideElement;
            int selectionAnchorInsideElement;
            if ((this.CursorPositionElement < this.SelectionAnchorElement) || ((this.CursorPositionElement == this.SelectionAnchorElement) && (this.CursorPositionInsideElement < this.SelectionAnchorInsideElement)))
            {
                cursorPositionElement = this.CursorPositionElement;
                cursorPositionInsideElement = this.CursorPositionInsideElement;
                selectionAnchorElement = this.SelectionAnchorElement;
                selectionAnchorInsideElement = this.SelectionAnchorInsideElement;
            }
            else
            {
                cursorPositionElement = this.SelectionAnchorElement;
                cursorPositionInsideElement = this.SelectionAnchorInsideElement;
                selectionAnchorElement = this.CursorPositionElement;
                selectionAnchorInsideElement = this.CursorPositionInsideElement;
            }
            if (cursorPositionElement == selectionAnchorElement)
            {
                string str = this.Elements[cursorPositionElement];
                if (cursorPositionInsideElement > str.Length)
                {
                    cursorPositionInsideElement = str.Length;
                }
                if (selectionAnchorInsideElement > str.Length)
                {
                    selectionAnchorInsideElement = str.Length;
                }
                this.Elements[cursorPositionElement] = str.Substring(0, cursorPositionInsideElement) + str.Substring(selectionAnchorInsideElement);
            }
            else
            {
                if (cursorPositionInsideElement < this.Elements[cursorPositionElement].Length)
                {
                    this.Elements[cursorPositionElement] = this.Elements[cursorPositionElement].Substring(0, cursorPositionInsideElement);
                }
                if (selectionAnchorInsideElement < this.Elements[selectionAnchorElement].Length)
                {
                    this.Elements[selectionAnchorElement] = this.Elements[selectionAnchorElement].Substring(selectionAnchorInsideElement);
                }
                else
                {
                    this.Elements[selectionAnchorElement] = string.Empty;
                }
                for (int i = cursorPositionElement + 1; i < selectionAnchorElement; i++)
                {
                    this.Elements[i] = string.Empty;
                }
            }
            this.SetPositions(cursorPositionElement, cursorPositionInsideElement);
            return true;
        }

        bool Insert(char inp)
        {
            this.Erase();
            if (this.Info[this.CursorPositionElement].MaxMatches <= this.CursorPositionInsideElement)
            {
                if ((this.CursorPositionElement + 1) >= this.Info.Count)
                {
                    return false;
                }
                this.CursorPositionElement++;
                this.CursorPositionInsideElement = 0;
            }
            if (!this.Info[this.CursorPositionElement].IsAcceptable(inp))
            {
                for (int i = 1; (this.CursorPositionElement + i) < this.Info.Count; i++)
                {
                    if (this.Info[this.CursorPositionElement + i].IsAcceptable(inp))
                    {
                        this.CursorPositionElement += i;
                        this.CursorPositionInsideElement = 0;
                        goto Label_00BB;
                    }
                }
                return false;
            }
        Label_00BB:
            if (this.Info[this.CursorPositionElement].IsLiteral)
            {
                this.CursorPositionInsideElement++;
                return this.SetCaretTo(this.CursorPositionElement, this.CursorPositionInsideElement);
            }
            inp = this.Info[this.CursorPositionElement].GetAcceptableChar(inp);
            if (this.Elements[this.CursorPositionElement].Length <= this.CursorPositionInsideElement)
            {
                string[] strArray;
                IntPtr ptr;
                (strArray = this.Elements)[(int) (ptr = (IntPtr) this.CursorPositionElement)] = strArray[(int) ptr] + inp;
                return this.SetCaretTo(this.CursorPositionElement, this.Elements[this.CursorPositionElement].Length);
            }
            string str = this.Elements[this.CursorPositionElement].Substring(this.CursorPositionInsideElement);
            if (this.Elements[this.CursorPositionElement].Length >= this.Info[this.CursorPositionElement].MaxMatches)
            {
                str = str.Substring(1);
            }
            this.Elements[this.CursorPositionElement] = this.Elements[this.CursorPositionElement].Substring(0, this.CursorPositionInsideElement) + inp + str;
            return this.SetCaretTo(this.CursorPositionElement, this.CursorPositionInsideElement + 1);
        }

        bool SetCaretTo(int element, int insideElement)
        {
            if (insideElement >= this.Info[element].GetDisplayText(this.Elements[element], '@').Length)
            {
                if (element >= (this.Info.Count - 1))
                {
                    this.SetPositions(element, insideElement);
                    return true;
                }
                element++;
                insideElement = 0;
            }
            this.SetPositions(element, insideElement);
            this.SelectionAnchorInsideElement++;
            return true;
        }

        void SetPositions(int element, int insideElement)
        {
            this.CursorPositionElement = element;
            this.CursorPositionInsideElement = insideElement;
            this.SelectionAnchorElement = element;
            this.SelectionAnchorInsideElement = insideElement;
        }
        #endregion

        #region 重写方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="comparedState"></param>
        /// <returns></returns>
        public override bool IsSame(MaskManagerState comparedState)
        {
            if (comparedState == null)
            {
                return false;
            }
            if (base.GetType() != typeof(LegacyMaskManagerState))
            {
                throw new NotImplementedException("Internal error");
            }
            if (comparedState.GetType() != typeof(LegacyMaskManagerState))
            {
                throw new InvalidOperationException("Internal error");
            }
            LegacyMaskManagerState state = (LegacyMaskManagerState) comparedState;
            if (this.CursorPositionElement != state.CursorPositionElement)
            {
                return false;
            }
            if (this.CursorPositionInsideElement != state.CursorPositionInsideElement)
            {
                return false;
            }
            if (this.SelectionAnchorElement != state.SelectionAnchorElement)
            {
                return false;
            }
            if (this.SelectionAnchorInsideElement != state.SelectionAnchorInsideElement)
            {
                return false;
            }
            if (!object.ReferenceEquals(this.Info, state.Info))
            {
                throw new InvalidOperationException("Internal error");
            }
            if (this.Elements.Length != state.Elements.Length)
            {
                throw new InvalidOperationException("Internal error");
            }
            for (int i = 0; i < this.Elements.Length; i++)
            {
                if (this.Elements[i] != state.Elements[i])
                {
                    return false;
                }
            }
            return true;
        }
        #endregion
    }
}

