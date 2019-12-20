#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.UndoRedo;
using System;
using System.Threading;
using System.Windows.Input;
#endregion

namespace Dt.Cells.UI
{
    internal class SetValueCommand : ICommand
    {
        private DataValidationListButtonInfo _info;
        private SheetView _sheetView;
        private object _value;

        public event EventHandler CanExecuteChanged;

        public SetValueCommand(SheetView sheetView, DataValidationListButtonInfo info)
        {
            this._info = info;
            this._sheetView = sheetView;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (((this._sheetView != null) && (this._sheetView.Worksheet != null)) && ((this._info != null) && (this._info.Validator != null)))
            {
                if (parameter != null)
                {
                    this._value = parameter.ToString();
                }
                else
                {
                    this._value = null;
                }
                CellEditExtent extent = new CellEditExtent(this._info.Row, this._info.Column, (string) (this._value as string));
                CellEditUndoAction command = new CellEditUndoAction(this._sheetView.Worksheet, extent);
                this._sheetView.DoCommand(command);
            }
        }

        protected virtual void OnCanExecuteChanged()
        {
            if (this.CanExecuteChanged != null)
            {
                this.CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
}

