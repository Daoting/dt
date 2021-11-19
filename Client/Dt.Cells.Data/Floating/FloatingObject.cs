#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class FloatingObject : IFloatingObject, IRangeSupport, IXmlSerializable, ICloneable, INotifyPropertyChanged
    {
        bool _canPrint;
        bool _dynamicMove;
        bool _dynamicSize;
        int _endColumn;
        double _endColumnOffset;
        int _endRow;
        double _endRowOffset;
        bool _isSelected;
        Windows.Foundation.Point _location;
        bool _locked;
        string _name;
        IList _owner;
        IFloatingObjectSheet _sheet;
        internal Windows.Foundation.Size _size;
        bool _sizeWithSameRatio;
        int _startColumn;
        double _startColumnOffset;
        int _startRow;
        double _startRowOffset;
        WorkingState _suspendState;
        bool _visible;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.FloatingObject" /> class.
        /// </summary>
        public FloatingObject() : this(string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.FloatingObject" /> class.
        /// </summary>
        /// <param name="name">The name of the floating object.</param>
        public FloatingObject(string name) : this(name, 0.0, 0.0, 300.0, 300.0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.FloatingObject" /> class.
        /// </summary>
        /// <param name="name">The name of the floating object.</param>
        /// <param name="x">The x location of the floating object.</param>
        /// <param name="y">The y location of the floating object.</param>
        public FloatingObject(string name, double x, double y)
        {
            this._suspendState = new WorkingState();
            this.Init(name, x, y, 300.0, 300.0);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.FloatingObject" /> class.
        /// </summary>
        /// <param name="name">The name of the floating object.</param>
        /// <param name="x">The x location of the floating object.</param>
        /// <param name="y">The y location of the floating object.</param>
        /// <param name="width">The width of the floating object.</param>
        /// <param name="height">The height of the floating object.</param>
        public FloatingObject(string name, double x, double y, double width, double height)
        {
            this._suspendState = new WorkingState();
            this.Init(name, x, y, width, height);
        }

        void AdjustLocation()
        {
            Windows.Foundation.Size sheetBounds = this.GetSheetBounds();
            double num = this._location.X + this.Size.Width;
            if (num > sheetBounds.Width)
            {
                this._location.X = sheetBounds.Width - this.Size.Width;
            }
            if (this._location.X < 0.0)
            {
                this._location.X = 0.0;
            }
            double num2 = this._location.Y + this.Size.Height;
            if (num2 > sheetBounds.Height)
            {
                this._location.Y = sheetBounds.Height - this.Size.Height;
            }
            if (this._location.Y < 0.0)
            {
                this._location.Y = 0.0;
            }
        }

        void AdjustSize()
        {
            Windows.Foundation.Size sheetBounds = this.GetSheetBounds();
            if (this._size.Width > sheetBounds.Width)
            {
                this._size.Width = sheetBounds.Width;
            }
            if (this._size.Height > sheetBounds.Height)
            {
                this._size.Height = sheetBounds.Height;
            }
        }

        Windows.Foundation.Size AdjustSizeWithRatio(Windows.Foundation.Size newSize)
        {
            if (!this.SizeWithSameRatio)
            {
                return newSize;
            }
            double num = Math.Abs((double) (newSize.Width - this._size.Width));
            double num2 = Math.Abs((double) (newSize.Height - this._size.Height));
            if (num >= num2)
            {
                double num3 = newSize.Width;
                double num4 = newSize.Width / this._size.Width;
                return new Windows.Foundation.Size(num3, this._size.Height * num4);
            }
            double num6 = newSize.Height / this._size.Height;
            double width = this._size.Width * num6;
            return new Windows.Foundation.Size(width, newSize.Height);
        }

        internal virtual void AfterReadXml()
        {
            this.ResumeEvents();
        }

        internal virtual void BeforeReadXml()
        {
            this.SuspendEvents();
            this.Init(string.Empty, 0.0, 0.0, 200.0, 200.0);
        }

        int CalcAnchorColumn(double x, out double coumnOffset)
        {
            double num = 0.0;
            coumnOffset = 0.0;
            for (int i = 0; i < this._sheet.ColumnCount; i++)
            {
                double actualColumnWidth = this._sheet.GetActualColumnWidth(i, SheetArea.Cells);
                num += actualColumnWidth;
                double num4 = num - x;
                if (num4 > 0.0)
                {
                    coumnOffset = (num4 > 0.0) ? (actualColumnWidth - num4) : -1.0;
                    return i;
                }
                if (num4.IsZero())
                {
                    coumnOffset = 0.0;
                    return (i + 1);
                }
            }
            return -1;
        }

        int CalcAnchorRow(double y, out double rowOffset)
        {
            double num = 0.0;
            rowOffset = 0.0;
            for (int i = 0; i < this._sheet.RowCount; i++)
            {
                double actualRowHeight = this._sheet.GetActualRowHeight(i, SheetArea.Cells);
                num += actualRowHeight;
                double num4 = num - y;
                if (num4 > 0.0)
                {
                    rowOffset = (num4 > 0.0) ? (actualRowHeight - num4) : -1.0;
                    return i;
                }
                if (num4.IsZero())
                {
                    rowOffset = 0.0;
                    return (i + 1);
                }
            }
            return -1;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public virtual object Clone()
        {
            MemoryStream stream = new MemoryStream();
            try
            {
                XmlWriter writer = XmlWriter.Create((Stream) stream);
                if (writer != null)
                {
                    Serializer.SerializeObj(this, "FloatingObject", writer);
                    writer.Close();
                    stream.Seek(0L, (SeekOrigin) SeekOrigin.Begin);
                    XmlReader @this = XmlReader.Create((Stream) stream);
                    if (@this != null)
                    {
                        FloatingObject obj2 = this.CreateObjectOnClone();
                        ((IXmlSerializable) obj2).ReadXml(@this);
                        @this.Close();
                        return obj2;
                    }
                }
            }
            finally
            {
                ((Stream) stream).Close();
            }
            return null;
        }

        internal FloatingObject CreateObjectOnClone()
        {
            try
            {
                return (Activator.CreateInstance(base.GetType()) as FloatingObject);
            }
            catch (Exception)
            {
            }
            return null;
        }

        Windows.Foundation.Size GetSheetBounds()
        {
            if (this.Sheet == null)
            {
                return new Windows.Foundation.Size(double.MaxValue, double.MaxValue);
            }
            double width = 0.0;
            for (int i = 0; i < this.Sheet.ColumnCount; i++)
            {
                width += this.Sheet.GetActualColumnWidth(i, SheetArea.Cells);
            }
            double height = 0.0;
            for (int j = 0; j < this.Sheet.RowCount; j++)
            {
                height += this.Sheet.GetActualRowHeight(j, SheetArea.Cells);
            }
            return new Windows.Foundation.Size(width, height);
        }

        internal virtual IFloatingObjectSheet GetSheetFromOwner()
        {
            if (this.Owner != null)
            {
                return (this.Owner as FloatingObjects).Sheet;
            }
            return null;
        }

        void IRangeSupport.AddColumns(int column, int columnCount)
        {
            if (column <= this.StartColumn)
            {
                if (this.DynamicMove)
                {
                    this.StartColumn += columnCount;
                    this.EndColumn += columnCount;
                }
            }
            else if (((column > this.StartColumn) && (column < this.EndColumn)) && this.DynamicSize)
            {
                this.EndColumn += columnCount;
            }
            this.OnAddColumns(column, columnCount);
        }

        void IRangeSupport.AddRows(int row, int rowCount)
        {
            if (row <= this.StartRow)
            {
                if (this.DynamicMove)
                {
                    this.StartRow += rowCount;
                    this.EndRow += rowCount;
                }
            }
            else if ((row < this.EndRow) && this.DynamicSize)
            {
                this.EndRow += rowCount;
            }
            this.OnAddRows(row, rowCount);
        }

        void IRangeSupport.Clear(int row, int column, int rowCount, int columnCount)
        {
            this.OnClear(row, column, rowCount, columnCount);
        }

        void IRangeSupport.Copy(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            this.OnCopy(fromRow, fromColumn, toRow, toColumn, rowCount, columnCount);
        }

        void IRangeSupport.Move(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            this.OnMove(fromRow, fromColumn, toRow, toColumn, rowCount, columnCount);
        }

        void IRangeSupport.RemoveColumns(int column, int count)
        {
            int num = (column + count) - 1;
            if (column <= this.StartColumn)
            {
                if (num < this.StartColumn)
                {
                    if (this.DynamicMove)
                    {
                        this.StartColumn -= count;
                        this.EndColumn -= count;
                    }
                }
                else if (num < this.EndColumn)
                {
                    int num2 = (num - this.StartColumn) + 1;
                    int num3 = ((this.EndColumn - this.StartColumn) + 1) - num2;
                    if (this.DynamicMove)
                    {
                        this.StartColumn = column;
                        this.StartColumnOffset = 0.0;
                    }
                    if (this.DynamicSize)
                    {
                        this.EndColumn = (column + num3) - 1;
                    }
                }
            }
            else if (column <= this.EndColumn)
            {
                if (num < this.EndColumn)
                {
                    if (this.DynamicSize)
                    {
                        this.EndColumn -= count;
                    }
                }
                else if (this.DynamicSize)
                {
                    this.EndColumn = column;
                    this.EndColumnOffset = 0.0;
                }
            }
            this.OnRemoveColumns(column, count);
        }

        void IRangeSupport.RemoveRows(int row, int count)
        {
            int num = (row + count) - 1;
            if (row <= this.StartRow)
            {
                if (num < this.StartRow)
                {
                    if (this.DynamicMove)
                    {
                        this.StartRow -= count;
                        this.EndRow -= count;
                    }
                }
                else if (num < this.EndRow)
                {
                    int num2 = (num - this.StartRow) + 1;
                    int num3 = ((this.EndRow - this.StartRow) + 1) - num2;
                    if (this.DynamicMove)
                    {
                        this.StartRow = row;
                        this.StartRowOffset = 0.0;
                    }
                    if (this.DynamicSize)
                    {
                        this.EndRow = (row + num3) - 1;
                    }
                }
            }
            else if (row <= this.EndRow)
            {
                if (num < this.EndRow)
                {
                    if (this.DynamicSize)
                    {
                        this.EndRow -= count;
                    }
                }
                else if (this.DynamicSize)
                {
                    this.EndRow = row;
                    this.EndRowOffset = 0.0;
                }
            }
            this.OnRemoveRows(row, count);
        }

        void IRangeSupport.Swap(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            this.OnSwap(fromRow, fromColumn, toRow, toColumn, rowCount, columnCount);
        }

        internal virtual void Init(string name, double x, double y, double width, double height)
        {
            this._name = name;
            this._location = new Windows.Foundation.Point(x, y);
            this._size = new Windows.Foundation.Size(width, height);
            this._dynamicMove = true;
            this._dynamicSize = true;
            this._sizeWithSameRatio = false;
            this._canPrint = true;
            this._visible = true;
            this._suspendState.Reset();
        }

        /// <summary>
        /// Determines whether the PropertyChanged event is suspended.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the PropertyChanged event suspended; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEventSuspened()
        {
            return this._suspendState.IsWorking;
        }

        internal virtual void OnAddColumns(int column, int columnCount)
        {
        }

        internal virtual void OnAddRows(int row, int rowCount)
        {
        }

        internal virtual void OnClear(int row, int column, int rowCount, int columnCount)
        {
            throw new NotImplementedException();
        }

        internal virtual void OnCopy(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            throw new NotImplementedException();
        }

        internal virtual void OnMove(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            throw new NotImplementedException();
        }

        internal virtual void OnOwnerChanged()
        {
        }

        internal virtual void OnPropertyChanged(string propertyName)
        {
            if (this.Worksheet != null)
            {
                this.Worksheet.RaiseFloatingObjectChanged(this, propertyName);
                if (propertyName == "IsSelected")
                {
                    this.Worksheet.RaiseFloatingObjectSelectionChangedEvent(this);
                }
            }
        }

        internal virtual void OnRemoveColumns(int column, int columnCount)
        {
        }

        internal virtual void OnRemoveRows(int row, int rowCount)
        {
        }

        internal virtual void OnResumeAfterDeserialization()
        {
            if (this.Sheet != null)
            {
                this.UpdateFloatingObjectCoordinates();
            }
        }

        internal virtual void OnSwap(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            throw new NotImplementedException();
        }

        internal void RaisePropertyChanged(string propertyName)
        {
            this.OnPropertyChanged(propertyName);
            if (!this.IsEventSuspened() && (this.PropertyChanged != null))
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        internal virtual void ReadXmlInternal(XmlReader reader)
        {
            if (reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element)))
            {
                switch (reader.Name)
                {
                    case "Name":
                        this._name = (string) (Serializer.DeserializeObj(typeof(string), reader) as string);
                        return;

                    case "Location":
                        this._location = (Windows.Foundation.Point) Serializer.DeserializeObj(typeof(Windows.Foundation.Point), reader);
                        return;

                    case "Size":
                        this._size = (Windows.Foundation.Size) Serializer.DeserializeObj(typeof(Windows.Foundation.Size), reader);
                        return;

                    case "IsSelected":
                        this._isSelected = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                        return;

                    case "Locked":
                        this._locked = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                        return;

                    case "IsVisible":
                        this._visible = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                        return;

                    case "AllowPrint":
                        this._canPrint = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                        return;

                    case "DynamicMove":
                        this._dynamicMove = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                        return;

                    case "DynamicSize":
                        this._dynamicSize = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                        return;

                    case "SizeWithSameRatio":
                        this._sizeWithSameRatio = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                        break;

                    default:
                        return;
                }
            }
        }

        internal void ResumeAfterDeserialization()
        {
            this.SuspendEvents();
            this.OnResumeAfterDeserialization();
            this.ResumeEvents();
        }

        /// <summary>
        /// Resumes the PropertyChanged event of the floating object.
        /// </summary>
        public void ResumeEvents()
        {
            this._suspendState.Release();
        }

        internal virtual void SetOwnerInternal(IList owner)
        {
            this._owner = owner;
            IFloatingObjectSheet sheetFromOwner = this.GetSheetFromOwner();
            this._sheet = sheetFromOwner;
        }

        /// <summary>
        /// Suspends the PropertyChanged event of the floating object.
        /// </summary>
        public void SuspendEvents()
        {
            this._suspendState.AddRef();
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.BeforeReadXml();
            while (reader.Read())
            {
                this.ReadXmlInternal(reader);
            }
            this.AfterReadXml();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            Serializer.InitWriter(writer);
            this.WriteXmlInternal(writer);
        }

        void UpdateConverRange()
        {
            if (this._sheet != null)
            {
                this._startRow = this.CalcAnchorRow(this.Location.Y, out this._startRowOffset);
                if (this._startRowOffset == -1.0)
                {
                    this._startRow++;
                    this._startRowOffset = 0.0;
                    if (this._startRow == this._sheet.RowCount)
                    {
                        this._startRow = this._sheet.RowCount - 1;
                    }
                }
                this._startColumn = this.CalcAnchorColumn(this.Location.X, out this._startColumnOffset);
                if (this._startColumnOffset == -1.0)
                {
                    this._startColumn++;
                    this._startColumnOffset = 0.0;
                    if (this._startColumn == this._sheet.ColumnCount)
                    {
                        this._startColumn = this._sheet.ColumnCount - 1;
                    }
                }
                this._endRow = this.CalcAnchorRow(this.Location.Y + this.Size.Height, out this._endRowOffset);
                this._endColumn = this.CalcAnchorColumn(this.Location.X + this.Size.Width, out this._endColumnOffset);
            }
        }

        internal void UpdateFloatingObjectCoordinates()
        {
            this.AdjustSize();
            this.AdjustLocation();
            this.UpdateConverRange();
        }

        internal void UpdateFloatingObjectLayoutOnRowColumnChanged()
        {
            if (this.DynamicMove)
            {
                this.UpdateFloatingObjectLocation();
            }
            if (this.DynamicSize)
            {
                this.UpdateFloatingObjectSize();
            }
        }

        void UpdateFloatingObjectLocation()
        {
            if (this._sheet != null)
            {
                double y = 0.0;
                for (int i = 0; i < this.StartRow; i++)
                {
                    y += this._sheet.GetActualRowHeight(i, SheetArea.Cells);
                }
                y += this.StartRowOffset;
                double x = 0.0;
                for (int j = 0; j < this.StartColumn; j++)
                {
                    x += this._sheet.GetActualColumnWidth(j, SheetArea.Cells);
                }
                x += this.StartColumnOffset;
                this._location = new Windows.Foundation.Point(x, y);
                this.RaisePropertyChanged("Location");
            }
        }

        void UpdateFloatingObjectSize()
        {
            if (this._sheet != null)
            {
                double height = 0.0;
                for (int i = this.StartRow; i < this.EndRow; i++)
                {
                    height += this._sheet.GetActualRowHeight(i, SheetArea.Cells);
                }
                if (height != 0.0)
                {
                    if (this._sheet.GetActualRowHeight(this.StartRow, SheetArea.Cells) > 0.0)
                    {
                        height -= this.StartRowOffset;
                    }
                    if (this._sheet.GetActualRowHeight(this.EndRow, SheetArea.Cells) > 0.0)
                    {
                        height += this.EndRowOffset;
                    }
                }
                else if (this.StartRow == this.EndRow)
                {
                    height = this.EndRowOffset - this.StartRowOffset;
                }
                double width = 0.0;
                for (int j = this.StartColumn; j < this.EndColumn; j++)
                {
                    width += this._sheet.GetActualColumnWidth(j, SheetArea.Cells);
                }
                if (width != 0.0)
                {
                    if (this._sheet.GetActualColumnWidth(this.StartColumn, SheetArea.Cells) > 0.0)
                    {
                        width -= this.StartColumnOffset;
                    }
                    if (this._sheet.GetActualColumnWidth(this.EndColumn, SheetArea.Cells) > 0.0)
                    {
                        width += this.EndColumnOffset;
                    }
                }
                else if (this.StartColumn == this.EndColumn)
                {
                    width = this.EndColumnOffset - this.StartColumnOffset;
                }
                if (width < 0.0)
                {
                    width = 0.0;
                }
                if (height < 0.0)
                {
                    height = 0.0;
                }
                this._size = new Windows.Foundation.Size(width, height);
                this.RaisePropertyChanged("Size");
            }
        }

        internal virtual void WriteXmlInternal(XmlWriter writer)
        {
            Serializer.SerializeObj(this.Name, "Name", writer);
            Serializer.SerializeObj(this.Location, "Location", writer);
            Serializer.SerializeObj(this.Size, "Size", writer);
            Serializer.SerializeObj((bool) this.IsSelected, "IsSelected", writer);
            Serializer.SerializeObj((bool) this.Visible, "IsVisible", writer);
            Serializer.SerializeObj((bool) this.Locked, "Locked", writer);
            Serializer.SerializeObj((bool) this.CanPrint, "AllowPrint", writer);
            Serializer.SerializeObj((bool) this.DynamicMove, "DynamicMove", writer);
            Serializer.SerializeObj((bool) this.DynamicSize, "DynamicSize", writer);
            Serializer.SerializeObj((bool) this.SizeWithSameRatio, "SizeWithSameRatio", writer);
        }

        /// <summary>
        /// Gets or sets a value that indicates whether this floating object can print.
        /// </summary>
        /// <value>
        /// <c>true</c> if this floating object can print; otherwise, <c>false</c>.
        /// </value>
        public bool CanPrint
        {
            get { return  this._canPrint; }
            set
            {
                if (value != this.CanPrint)
                {
                    this._canPrint = value;
                    this.RaisePropertyChanged("CanPrint");
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the object moves when hiding or showing, resizing, or moving rows or columns.
        /// </summary>
        /// <value>
        /// <c>true</c> if the object dynamically moves; otherwise, <c>false</c>.
        /// </value>
        public bool DynamicMove
        {
            get { return  this._dynamicMove; }
            set
            {
                if (value != this.DynamicMove)
                {
                    this._dynamicMove = value;
                    this.RaisePropertyChanged("DynamicMove");
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the size of the object changes when hiding or showing, resizing, or moving rows or columns.
        /// </summary>
        /// <value>
        /// <c>true</c> if the object dynamically changes size; otherwise, <c>false</c>.
        /// </value>
        public bool DynamicSize
        {
            get { return  this._dynamicSize; }
            set
            {
                if (value != this.DynamicSize)
                {
                    this._dynamicSize = value;
                    this.RaisePropertyChanged("DynamicResize");
                }
            }
        }

        /// <summary>
        /// Gets the end column index of the floating object position.
        /// </summary>
        /// <value>
        /// The end column index of the floating object position.
        /// </value>
        public int EndColumn
        {
            get { return  this._endColumn; }
            private set
            {
                if (value != this.EndColumn)
                {
                    this._endColumn = value;
                    this.UpdateFloatingObjectSize();
                }
            }
        }

        /// <summary>
        /// Gets the offset relative to the end column of the floating object.
        /// </summary>
        /// <value>
        /// The offset relative to the end column of the floating object.
        /// </value>
        public double EndColumnOffset
        {
            get
            {
                if (this._endColumnOffset == -1.0)
                {
                    return this.Sheet.GetActualColumnWidth(this.EndColumn, SheetArea.Cells);
                }
                return this._endColumnOffset;
            }
            private set
            {
                if (value != this.EndColumnOffset)
                {
                    this._endColumnOffset = value;
                    this.UpdateFloatingObjectSize();
                }
            }
        }

        /// <summary>
        /// Gets the end row index of the floating object position.
        /// </summary>
        /// <value>
        /// The end row index of the floating object position.
        /// </value>
        public int EndRow
        {
            get { return  this._endRow; }
            private set
            {
                if (value != this.EndRow)
                {
                    this._endRow = value;
                    this.UpdateFloatingObjectSize();
                }
            }
        }

        /// <summary>
        /// Gets the offset relative to the end row of the floating object.
        /// </summary>
        /// <value>
        /// The offset relative to the end row of the floating object.
        /// </value>
        public double EndRowOffset
        {
            get
            {
                if (this._endRowOffset == -1.0)
                {
                    return this.Sheet.GetActualRowHeight(this.EndRow, SheetArea.Cells);
                }
                return this._endRowOffset;
            }
            private set
            {
                if (value != this.EndRowOffset)
                {
                    this._endRowOffset = value;
                    this.UpdateFloatingObjectSize();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether this floating object is selected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this floating object is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelected
        {
            get { return  this._isSelected; }
            set
            {
                if (value != this._isSelected)
                {
                    this._isSelected = value;
                    this.RaisePropertyChanged("IsSelected");
                }
            }
        }

        /// <summary>
        /// Gets or sets the location of a floating object.
        /// </summary>
        /// <value>
        /// The location of a floating object.
        /// </value>
        public Windows.Foundation.Point Location
        {
            get { return  this._location; }
            set
            {
                if (value != this.Location)
                {
                    this._location = value;
                    this.AdjustLocation();
                    this.UpdateConverRange();
                    this.RaisePropertyChanged("Location");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether this floating object is locked.
        /// </summary>
        /// <value>
        /// <c>true</c> if locked; otherwise, <c>false</c>.
        /// </value>
        public bool Locked
        {
            get { return  this._locked; }
            set
            {
                if (value != this.Locked)
                {
                    this._locked = value;
                    this.RaisePropertyChanged("Locked");
                }
            }
        }

        /// <summary>
        /// Gets the name of the floating object.
        /// </summary>
        public string Name
        {
            get { return  this._name; }
            set
            {
                if (value != this._name)
                {
                    this._name = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }

        internal IList Owner
        {
            get { return  this._owner; }
            set
            {
                if (value != this.Owner)
                {
                    this._owner = value;
                    if (this._owner != null)
                    {
                        IFloatingObjectSheet sheetFromOwner = this.GetSheetFromOwner();
                        this.Sheet = sheetFromOwner;
                    }
                    else
                    {
                        this.Sheet = null;
                    }
                    this.UpdateFloatingObjectCoordinates();
                    this.OnOwnerChanged();
                }
            }
        }

        internal CellRange Range
        {
            get { return  new CellRange(this.StartRow, this.StartColumn, (this.EndRow - this.StartRow) + 1, (this.EndColumn - this.StartColumn) + 1); }
        }

        internal IFloatingObjectSheet Sheet
        {
            get { return  this._sheet; }
            private set { this._sheet = value; }
        }

        /// <summary>
        /// Gets or sets the size of a floating object.
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        public Windows.Foundation.Size Size
        {
            get { return  this._size; }
            set
            {
                if (value != this.Size)
                {
                    this._size = this.AdjustSizeWithRatio(value);
                    this.AdjustSize();
                    this.AdjustLocation();
                    this.UpdateConverRange();
                    this.RaisePropertyChanged("Size");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [size with same ratio].
        /// </summary>
        /// <value>
        /// <c>true</c> if [size with same ratio]; otherwise, <c>false</c>.
        /// </value>
        internal bool SizeWithSameRatio
        {
            get { return  this._sizeWithSameRatio; }
            set
            {
                if (value != this.SizeWithSameRatio)
                {
                    this._sizeWithSameRatio = value;
                    this.RaisePropertyChanged("LockAspectRatio");
                }
            }
        }

        /// <summary>
        /// Gets the starting column index of the floating object position.
        /// </summary>
        /// <value>
        /// The starting column index of the floating object position.
        /// </value>
        public int StartColumn
        {
            get { return  this._startColumn; }
            private set
            {
                if (value != this.StartColumn)
                {
                    this._startColumn = value;
                    this.UpdateFloatingObjectLocation();
                }
            }
        }

        /// <summary>
        /// Gets the offset relative to the start column of the floating object.
        /// </summary>
        /// <value>
        /// The offset relative to the start column of the floating object.
        /// </value>
        public double StartColumnOffset
        {
            get { return  this._startColumnOffset; }
            private set
            {
                if (value != this.StartColumnOffset)
                {
                    this._startColumnOffset = value;
                    this.UpdateFloatingObjectLocation();
                }
            }
        }

        /// <summary>
        /// Gets the starting row index of the floating object position.
        /// </summary>
        /// <value>
        /// The starting row index of the floating object position.
        /// </value>
        public int StartRow
        {
            get { return  this._startRow; }
            private set
            {
                if (value != this.StartRow)
                {
                    this._startRow = value;
                    this.UpdateFloatingObjectLocation();
                }
            }
        }

        /// <summary>
        /// Gets the offset relative to the start row of the floating object.
        /// </summary>
        /// <value>
        /// The offset relative to the start row of the floating object.
        /// </value>
        public double StartRowOffset
        {
            get { return  this._startRowOffset; }
            private set
            {
                if (value != this.StartRowOffset)
                {
                    this._startRowOffset = value;
                    this.UpdateFloatingObjectLocation();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether this floating object is visible.
        /// </summary>
        /// <value>
        /// <c>true</c> if visible; otherwise, <c>false</c>.
        /// </value>
        public bool Visible
        {
            get { return  this._visible; }
            set
            {
                if (value != this.Visible)
                {
                    this._visible = value;
                    this.RaisePropertyChanged("Visible");
                }
            }
        }

        internal Dt.Cells.Data.Worksheet Worksheet
        {
            get { return  (this.Sheet as Dt.Cells.Data.Worksheet); }
        }
    }
}

