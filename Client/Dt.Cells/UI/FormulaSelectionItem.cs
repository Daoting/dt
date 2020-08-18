#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Cells.Data;
using System.ComponentModel;
#endregion

namespace Dt.Cells.UI
{
    internal class FormulaSelectionItem : INotifyPropertyChanged
    {
        bool _canChangeBoundsByUI;
        Windows.UI.Color _color;
        bool _isFlickering;
        bool _isMouseOver;
        bool _isResizing;
        CellRange _range;

        public event PropertyChangedEventHandler PropertyChanged;

        public FormulaSelectionItem(CellRange range, bool isFlickering = true)
        {
            _range = range;
            _isFlickering = isFlickering;
        }

        public FormulaSelectionItem(int row, int column, int rowCount, int columnCount, bool isFlickering = true) : this(new CellRange(row, column, rowCount, columnCount), isFlickering)
        {
        }

        void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public bool CanChangeBoundsByUI
        {
            get { return  _canChangeBoundsByUI; }
            set
            {
                if (_canChangeBoundsByUI != value)
                {
                    _canChangeBoundsByUI = value;
                    OnPropertyChanged("CanChangeBoundsByUI");
                }
            }
        }

        public Windows.UI.Color Color
        {
            get { return  _color; }
            set
            {
                if (_color != value)
                {
                    _color = value;
                    OnPropertyChanged("Color");
                }
            }
        }

        public Excel.FormulaExpression Expression { get; set; }

        public bool IsFlickering
        {
            get { return  _isFlickering; }
            set
            {
                if (_isFlickering != value)
                {
                    _isFlickering = value;
                    OnPropertyChanged("IsFlickering");
                }
            }
        }

        public bool IsMouseOver
        {
            get { return  _isMouseOver; }
            set
            {
                if (_isMouseOver != value)
                {
                    _isMouseOver = value;
                    OnPropertyChanged("IsMouseOver");
                }
            }
        }

        public bool IsResizing
        {
            get { return  _isResizing; }
            set
            {
                if (_isResizing != value)
                {
                    _isResizing = value;
                    OnPropertyChanged("IsResizing");
                }
            }
        }

        public CellRange Range
        {
            get { return  _range; }
            set
            {
                if (_range != value)
                {
                    _range = value;
                    OnPropertyChanged("Range");
                }
            }
        }
    }
}

