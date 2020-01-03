#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.CalcEngine;
using Dt.Cells.Data;
using Dt.Cells.UI;
using Dt.Charts;
using Dt.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Cell = Dt.Cells.Data.Cell;
#endregion

namespace Dt.Sample
{
    public partial class FormulaTextBox : Win
    {
        Worksheet _editSheet = null;
        int _editRowIndex;
        int _editColumnIndex;
        string _originalText;

        public FormulaTextBox()
        {
            InitializeComponent();
            OnInit();
        }

        void OnInit()
        {
            _ftBox.SpreadSheet = _excel;
            _excel.ActiveSheetChanged += OnExcelActiveSheetChanged;
            _excel.EnterCell += OnExcelEnterCell;
            _ftBox.KeyUp += OnBoxKeyUp;
            _ftBox.LostFocus += OnBoxLostFocus;

            ApplyTextToFormulaTextBox();
        }

        void OnExcelActiveSheetChanged(object sender, EventArgs e)
        {
            if (_excel.View.CanSelectFormula) return;

            ApplyTexttoCell();
            ApplyTextToFormulaTextBox();
        }

        void OnExcelEnterCell(object sender, EnterCellEventArgs e)
        {
            ApplyTexttoCell();
            ApplyTextToFormulaTextBox();
        }

        void OnBoxKeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                ApplyTexttoCell();
                if (_editSheet != null)
                {
                    _excel.Workbook.ActiveSheet = _editSheet;
                    _editRowIndex++;
                    _editSheet.SetActiveCell(_editRowIndex, _editColumnIndex);
                    _excel.Focus(FocusState.Keyboard);
                }
                ApplyTextToFormulaTextBox();
            }
        }

        void OnBoxLostFocus(object sender, RoutedEventArgs e)
        {
            if (_excel.View.CanSelectFormula) return;

            ApplyTexttoCell();
        }

        void ApplyTexttoCell()
        {
            if (_editSheet == null || _ftBox.Text == _originalText) return;

            string formulaText = _ftBox.Text.Trim();
            if (formulaText.StartsWith("=") && formulaText.Length > 0)
            {
                try
                {
                    _editSheet.Cells[_editRowIndex, _editColumnIndex].Formula = formulaText;
                }
                catch (CalcParseException)
                {
                    AtKit.Error("公式 '" + formulaText + "' 应用有误！");
                }
            }
            else
            {
                _editSheet.Cells[_editRowIndex, _editColumnIndex].Text = _ftBox.Text;
            }

            _originalText = _ftBox.Text;
        }

        void ApplyTextToFormulaTextBox()
        {
            _ftBox.Text = string.Empty;
            _editSheet = _excel.ActiveSheet;

            if (_editSheet != null)
            {
                _editRowIndex = _editSheet.ActiveRowIndex;
                _editColumnIndex = _editSheet.ActiveColumnIndex;

                _ftBox.Text = string.IsNullOrEmpty(_editSheet.ActiveCell.Formula) ?
                              _editSheet.ActiveCell.Text : "=" + _editSheet.ActiveCell.Formula;
            }
            _originalText = _ftBox.Text;
        }
    }
}