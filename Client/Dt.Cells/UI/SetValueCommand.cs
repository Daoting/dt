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
        DataValidationListButtonInfo _info;
        SheetView _sheetView;
        object _value;

        public event EventHandler CanExecuteChanged;

        public SetValueCommand(SheetView sheetView, DataValidationListButtonInfo info)
        {
            _info = info;
            _sheetView = sheetView;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (((_sheetView != null) && (_sheetView.ActiveSheet != null)) && ((_info != null) && (_info.Validator != null)))
            {
                if (parameter != null)
                {
                    _value = parameter.ToString();
                }
                else
                {
                    _value = null;
                }
                CellEditExtent extent = new CellEditExtent(_info.Row, _info.Column, (string) (_value as string));
                CellEditUndoAction command = new CellEditUndoAction(_sheetView.ActiveSheet, extent);
                _sheetView.DoCommand(command);
            }
        }

        protected virtual void OnCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
}

