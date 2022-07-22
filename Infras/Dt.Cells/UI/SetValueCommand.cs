#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Cells.UndoRedo;
using System;
using System.Windows.Input;
#endregion

namespace Dt.Cells.UI
{
    internal class SetValueCommand : ICommand
    {
        DataValidationListButtonInfo _info;
        Excel _excel;
        object _value;

        public event EventHandler CanExecuteChanged;

        public SetValueCommand(Excel p_excel, DataValidationListButtonInfo info)
        {
            _info = info;
            _excel = p_excel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (((_excel != null) && (_excel.ActiveSheet != null)) && ((_info != null) && (_info.Validator != null)))
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
                CellEditUndoAction command = new CellEditUndoAction(_excel.ActiveSheet, extent);
                _excel.DoCommand(command);
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

