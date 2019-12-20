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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Windows.UI;
#endregion

namespace Dt.Cells.UI
{
    internal class FormulaSelectionItem : INotifyPropertyChanged
    {
        private bool _canChangeBoundsByUI;
        private Windows.UI.Color _color;
        private bool _isFlickering;
        private bool _isMouseOver;
        private bool _isResizing;
        private CellRange _range;

        public event PropertyChangedEventHandler PropertyChanged;

        public FormulaSelectionItem(CellRange range, bool isFlickering = true)
        {
            this._range = range;
            this._isFlickering = isFlickering;
        }

        public FormulaSelectionItem(int row, int column, int rowCount, int columnCount, bool isFlickering = true) : this(new CellRange(row, column, rowCount, columnCount), isFlickering)
        {
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public bool CanChangeBoundsByUI
        {
            get { return  this._canChangeBoundsByUI; }
            set
            {
                if (this._canChangeBoundsByUI != value)
                {
                    this._canChangeBoundsByUI = value;
                    this.OnPropertyChanged("CanChangeBoundsByUI");
                }
            }
        }

        public Windows.UI.Color Color
        {
            get { return  this._color; }
            set
            {
                if (this._color != value)
                {
                    this._color = value;
                    this.OnPropertyChanged("Color");
                }
            }
        }

        public SheetView.FormulaExpression Expression { get; set; }

        public bool IsFlickering
        {
            get { return  this._isFlickering; }
            set
            {
                if (this._isFlickering != value)
                {
                    this._isFlickering = value;
                    this.OnPropertyChanged("IsFlickering");
                }
            }
        }

        public bool IsMouseOver
        {
            get { return  this._isMouseOver; }
            set
            {
                if (this._isMouseOver != value)
                {
                    this._isMouseOver = value;
                    this.OnPropertyChanged("IsMouseOver");
                }
            }
        }

        public bool IsResizing
        {
            get { return  this._isResizing; }
            set
            {
                if (this._isResizing != value)
                {
                    this._isResizing = value;
                    this.OnPropertyChanged("IsResizing");
                }
            }
        }

        public CellRange Range
        {
            get { return  this._range; }
            set
            {
                if (this._range != value)
                {
                    this._range = value;
                    this.OnPropertyChanged("Range");
                }
            }
        }
    }
}

