#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System;
#endregion

namespace Dt.Cells.UndoRedo
{
    /// <summary>
    /// Represents a Clipboard paste action extent that supports Clipboard paste on the sheet.
    /// </summary>
    public class ClipboardPasteExtent
    {
        string _clipboardText;
        bool _isCutting;
        CellRange _sourceRange;
        CellRange[] _targetRanges;

        /// <summary>
        /// Creates a new instance of the <see cref="T:Dt.Cells.UndoRedo.ClipboardPasteExtent" /> class.
        /// </summary>
        /// <param name="sourceRange">The source range of the copy or cut.</param>
        /// <param name="targetRanges">The target ranges to paste.</param>
        /// <param name="isCutting">if set to <c>true</c> the action is a cut; otherwise, copy.</param>
        /// <param name="clipboardText">The Clipboard text.</param>
        public ClipboardPasteExtent(CellRange sourceRange, CellRange[] targetRanges, bool isCutting, string clipboardText)
        {
            if ((targetRanges == null) || (targetRanges.Length == 0))
            {
                throw new ArgumentException(ResourceStrings.undoActionPasteTargetEmptyError);
            }
            _sourceRange = sourceRange;
            _targetRanges = targetRanges;
            _isCutting = isCutting;
            _clipboardText = clipboardText;
        }

        /// <summary>
        /// Gets the text on the Clipboard.
        /// </summary>
        public string ClipboardText
        {
            get { return  _clipboardText; }
        }

        /// <summary>
        /// Gets a value that indicates whether the operation is a Clipboard cut.
        /// </summary>
        /// <value>
        /// <c>true</c> if a cut; otherwise, copy.
        /// </value>
        public bool IsCutting
        {
            get { return  _isCutting; }
        }

        /// <summary>
        /// Gets the Clipboard paste source range.
        /// </summary>
        public CellRange SourceRange
        {
            get { return  _sourceRange; }
        }

        /// <summary>
        /// Gets the Clipboard paste target ranges.
        /// </summary>
        public CellRange[] TargetRanges
        {
            get { return  _targetRanges; }
        }
    }
}

