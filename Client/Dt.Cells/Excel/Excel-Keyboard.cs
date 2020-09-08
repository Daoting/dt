#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-08-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using Dt.Cells.UI;
using System.Collections.Generic;
using Windows.System;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Base
{
    public partial class Excel
    {
        void InitKeyboard()
        {
            AddHandler(KeyDownEvent, new KeyEventHandler(OnKeyDown), true);
            AddHandler(KeyUpEvent, new KeyEventHandler(OnKeyUp), true);
        }

        void OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if ((_tabStrip != null) && _tabStrip.IsEditing)
            {
                if ((e.Key == (VirtualKey)13) || (e.Key == (VirtualKey)9))
                {
                    _tabStrip.StopTabEditing(false);
                    e.Handled = true;
                }
                else if (e.Key == (VirtualKey)0x1b)
                {
                    _tabStrip.StopTabEditing(true);
                }
                return;
            }

            bool flag2;
            bool flag3;
            bool flag4;
            bool flag = AllowEnterEditing(e);
            if (InputDeviceType == InputDeviceType.Touch)
            {
                RefreshSelection();
            }
            InputDeviceType = InputDeviceType.Keyboard;
            UpdateCursorType();
            VirtualKey keyCode = e.Key;
            if (keyCode == VirtualKey.Enter)
            {
                Worksheet activeSheet = ActiveSheet;
                if (EditorInfo.Sheet != activeSheet)
                {
                    int index = activeSheet.Workbook.Sheets.IndexOf(EditorInfo.Sheet);
                    activeSheet.Workbook.ActiveSheetIndex = index;
                    StopCellEditing(false);
                    e.Handled = true;
                    return;
                }
            }
            KeyboardHelper.GetMetaKeyState(out flag3, out flag2, out flag4);
            VirtualKeyModifiers none = VirtualKeyModifiers.None;
            if (flag2)
            {
                none |= VirtualKeyModifiers.Control;
            }
            if (flag3)
            {
                none |= VirtualKeyModifiers.None | VirtualKeyModifiers.Shift;
            }
            if (flag4)
            {
                none |= VirtualKeyModifiers.Menu;
            }
            KeyStroke ks = new KeyStroke(keyCode, none, false);
            if (ProcessKeyDownOnFloatingObjectSelected(ks))
            {
                e.Handled = true;
            }
            else
            {
                if (KeyMap.ContainsKey(ks))
                {
                    SpreadAction action = KeyMap[ks];
                    if (action != null)
                    {
                        CloseDragFillPopup();
                        ActionEventArgs args = new ActionEventArgs();
                        action(this, args);
                        if (args.Handled)
                        {
                            e.Handled = true;
                        }
                    }
                }
                if (IsDragDropping)
                {
                    SwitchDragDropIndicator();
                }
                else
                {
                    if (!IsEditing && flag)
                    {
                        // Enter状态表示因键盘输入触发，Edit状态表示双击触发
                        StartCellEditing(true, null, EditorStatus.Enter);
                    }
                    if (!IsEditing)
                    {
                        FocusInternal();
                    }
                }
            }
        }

        void OnKeyUp(object sender, KeyRoutedEventArgs e)
        {
            UpdateCursorType();
            if (!IsEditing)
            {
                bool flag;
                bool flag2;
                VirtualKey keyCode = e.Key;
                KeyboardHelper.GetMetaKeyState(out flag, out flag2);
                VirtualKeyModifiers none = VirtualKeyModifiers.None;
                if (flag2)
                {
                    none |= VirtualKeyModifiers.Control;
                }
                if (flag)
                {
                    none |= VirtualKeyModifiers.None | VirtualKeyModifiers.Shift;
                }
                KeyStroke stroke = new KeyStroke(keyCode, none, true);
                if (KeyMap.ContainsKey(stroke))
                {
                    SpreadAction action = KeyMap[stroke];
                    if (action != null)
                    {
                        ActionEventArgs args = new ActionEventArgs();
                        action(this, args);
                        if (args.Handled)
                        {
                            e.Handled = true;
                        }
                    }
                }
            }
            if (IsDragDropping)
            {
                SwitchDragDropIndicator();
            }
        }

        bool ProcessKeyDownOnFloatingObjectSelected(KeyStroke ks)
        {
            if ((((ks.KeyCode != VirtualKey.Z) || (ks.Modifiers != VirtualKeyModifiers.Control)) && ((ks.KeyCode != VirtualKey.Y) || (ks.Modifiers != VirtualKeyModifiers.Control))) && (HasSelectedFloatingObject() || FloatingObjectKeyMap.ContainsKey(ks)))
            {
                SpreadAction action = null;
                if (FloatingObjectKeyMap.TryGetValue(ks, out action) && (action != null))
                {
                    ActionEventArgs e = new ActionEventArgs();
                    action(this, e);
                    return e.Handled;
                }
            }
            return false;
        }

        bool AllowEnterEditing(KeyRoutedEventArgs e)
        {
            bool flag;
            bool flag2;
            bool flag3;
            KeyboardHelper.GetMetaKeyState(out flag, out flag2, out flag3);
            if (flag2 || flag3)
            {
                return false;
            }
            if (((((e.Key != VirtualKey.Space) && (((VirtualKey.Search | VirtualKey.Shift) > e.Key) || (e.Key > ((VirtualKey)0xc0)))) && (((VirtualKey.Scroll | VirtualKey.J) > e.Key) || (e.Key > (VirtualKey.NumberKeyLock | VirtualKey.N)))) && (((VirtualKey.Number0 > e.Key) || (e.Key > VirtualKey.Number9)) && ((VirtualKey.A > e.Key) || (e.Key > VirtualKey.Z)))) && ((VirtualKey.NumberPad0 > e.Key) || (e.Key > VirtualKey.NumberPad9)))
            {
                return ((VirtualKey.Multiply <= e.Key) && (e.Key <= VirtualKey.Divide));
            }
            return true;
        }

        Dictionary<KeyStroke, SpreadAction> _keyMap;
        Dictionary<KeyStroke, SpreadAction> _floatingObjectsKeyMap = new Dictionary<KeyStroke, SpreadAction>();

        Dictionary<KeyStroke, SpreadAction> KeyMap
        {
            get
            {
                if (_keyMap == null)
                    InitDefaultKeyMap();
                return _keyMap;
            }
        }

        Dictionary<KeyStroke, SpreadAction> FloatingObjectKeyMap
        {
            get
            {
                if (_floatingObjectsKeyMap == null)
                    InitFloatingObjectKeyMap();
                return _floatingObjectsKeyMap;
            }
        }

        void InitDefaultKeyMap()
        {
            _keyMap = new Dictionary<KeyStroke, SpreadAction>();
            _keyMap.Add(new KeyStroke(VirtualKey.Z, VirtualKeyModifiers.Control), new SpreadAction(SpreadActions.Undo));
            _keyMap.Add(new KeyStroke(VirtualKey.Y, VirtualKeyModifiers.Control), new SpreadAction(SpreadActions.Redo));
            _keyMap.Add(new KeyStroke(VirtualKey.Down, VirtualKeyModifiers.Control), new SpreadAction(SpreadActions.NavigationBottom));
            _keyMap.Add(new KeyStroke(VirtualKey.Down, VirtualKeyModifiers.None), new SpreadAction(SpreadActions.NavigationDown));
            _keyMap.Add(new KeyStroke(VirtualKey.End, VirtualKeyModifiers.None), new SpreadAction(SpreadActions.NavigationEnd));
            _keyMap.Add(new KeyStroke(VirtualKey.Right, VirtualKeyModifiers.Control), new SpreadAction(SpreadActions.NavigationEnd));
            _keyMap.Add(new KeyStroke(VirtualKey.Home, VirtualKeyModifiers.Control), new SpreadAction(SpreadActions.NavigationFirst));
            _keyMap.Add(new KeyStroke(VirtualKey.Home, VirtualKeyModifiers.None), new SpreadAction(SpreadActions.NavigationHome));
            _keyMap.Add(new KeyStroke(VirtualKey.Left, VirtualKeyModifiers.Control), new SpreadAction(SpreadActions.NavigationHome));
            _keyMap.Add(new KeyStroke(VirtualKey.End, VirtualKeyModifiers.Control), new SpreadAction(SpreadActions.NavigationLast));
            _keyMap.Add(new KeyStroke(VirtualKey.Left, VirtualKeyModifiers.None), new SpreadAction(SpreadActions.NavigationLeft));
            _keyMap.Add(new KeyStroke(VirtualKey.Tab, VirtualKeyModifiers.None), new SpreadAction(SpreadActions.CommitInputNavigationTabNext));
            _keyMap.Add(new KeyStroke(VirtualKey.PageDown, VirtualKeyModifiers.None), new SpreadAction(SpreadActions.NavigationPageDown));
            _keyMap.Add(new KeyStroke(VirtualKey.PageUp, VirtualKeyModifiers.Control), new SpreadAction(SpreadActions.NavigationPreviousSheet));
            _keyMap.Add(new KeyStroke(VirtualKey.PageDown, VirtualKeyModifiers.Control), new SpreadAction(SpreadActions.NavigationNextSheet));
            _keyMap.Add(new KeyStroke(VirtualKey.PageUp, VirtualKeyModifiers.None), new SpreadAction(SpreadActions.NavigationPageUp));
            _keyMap.Add(new KeyStroke(VirtualKey.Tab, VirtualKeyModifiers.None | VirtualKeyModifiers.Shift), new SpreadAction(SpreadActions.CommitInputNavigationTabPrevious));
            _keyMap.Add(new KeyStroke(VirtualKey.Right, VirtualKeyModifiers.None), new SpreadAction(SpreadActions.NavigationRight));
            _keyMap.Add(new KeyStroke(VirtualKey.Up, VirtualKeyModifiers.Control), new SpreadAction(SpreadActions.NavigationTop));
            _keyMap.Add(new KeyStroke(VirtualKey.Up, VirtualKeyModifiers.None), new SpreadAction(SpreadActions.NavigationUp));
            _keyMap.Add(new KeyStroke(VirtualKey.Delete, VirtualKeyModifiers.None), new SpreadAction(SpreadActions.Clear));
            _keyMap.Add(new KeyStroke(VirtualKey.Back, VirtualKeyModifiers.None), new SpreadAction(SpreadActions.ClearAndEditing));
            _keyMap.Add(new KeyStroke(VirtualKey.Enter, VirtualKeyModifiers.None), new SpreadAction(SpreadActions.CommitInputNavigationDown));
            _keyMap.Add(new KeyStroke(VirtualKey.Enter, VirtualKeyModifiers.None | VirtualKeyModifiers.Shift), new SpreadAction(SpreadActions.CommitInputNavigationUp));
            _keyMap.Add(new KeyStroke(VirtualKey.Escape, VirtualKeyModifiers.None), new SpreadAction(SpreadActions.CancelInput));
            _keyMap.Add(new KeyStroke(VirtualKey.Left, VirtualKeyModifiers.None | VirtualKeyModifiers.Shift), new SpreadAction(SpreadActions.SelectionLeft));
            _keyMap.Add(new KeyStroke(VirtualKey.Right, VirtualKeyModifiers.None | VirtualKeyModifiers.Shift), new SpreadAction(SpreadActions.SelectionRight));
            _keyMap.Add(new KeyStroke(VirtualKey.Up, VirtualKeyModifiers.None | VirtualKeyModifiers.Shift), new SpreadAction(SpreadActions.SelectionUp));
            _keyMap.Add(new KeyStroke(VirtualKey.Down, VirtualKeyModifiers.None | VirtualKeyModifiers.Shift), new SpreadAction(SpreadActions.SelectionDown));
            _keyMap.Add(new KeyStroke(VirtualKey.Home, VirtualKeyModifiers.None | VirtualKeyModifiers.Shift), new SpreadAction(SpreadActions.SelectionHome));
            _keyMap.Add(new KeyStroke(VirtualKey.Left, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift), new SpreadAction(SpreadActions.SelectionHome));
            _keyMap.Add(new KeyStroke(VirtualKey.End, VirtualKeyModifiers.None | VirtualKeyModifiers.Shift), new SpreadAction(SpreadActions.SelectionEnd));
            _keyMap.Add(new KeyStroke(VirtualKey.Right, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift), new SpreadAction(SpreadActions.SelectionEnd));
            _keyMap.Add(new KeyStroke(VirtualKey.PageUp, VirtualKeyModifiers.None | VirtualKeyModifiers.Shift), new SpreadAction(SpreadActions.SelectionPageUp));
            _keyMap.Add(new KeyStroke(VirtualKey.PageDown, VirtualKeyModifiers.None | VirtualKeyModifiers.Shift), new SpreadAction(SpreadActions.SelectionPageDown));
            _keyMap.Add(new KeyStroke(VirtualKey.Up, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift), new SpreadAction(SpreadActions.SelectionTop));
            _keyMap.Add(new KeyStroke(VirtualKey.Down, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift), new SpreadAction(SpreadActions.SelectionBottom));
            _keyMap.Add(new KeyStroke(VirtualKey.Home, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift), new SpreadAction(SpreadActions.SelectionFirst));
            _keyMap.Add(new KeyStroke(VirtualKey.End, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift), new SpreadAction(SpreadActions.SelectionLast));
            _keyMap.Add(new KeyStroke(VirtualKey.C, VirtualKeyModifiers.Control), new SpreadAction(SpreadActions.Copy));
            _keyMap.Add(new KeyStroke(VirtualKey.X, VirtualKeyModifiers.Control), new SpreadAction(SpreadActions.Cut));
            _keyMap.Add(new KeyStroke(VirtualKey.V, VirtualKeyModifiers.Control), new SpreadAction(SpreadActions.Paste));
            _keyMap.Add(new KeyStroke(VirtualKey.Enter, VirtualKeyModifiers.Menu), new SpreadAction(SpreadActions.InputNewLine));
            _keyMap.Add(new KeyStroke(VirtualKey.F2, VirtualKeyModifiers.None), new SpreadAction(SpreadActions.StartEditing));
            _keyMap.Add(new KeyStroke(VirtualKey.Enter, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift), new SpreadAction(SpreadActions.InputArrayFormula));
            _keyMap.Add(new KeyStroke(VirtualKey.A, VirtualKeyModifiers.Control), new SpreadAction(SpreadActions.SelectionAll));
        }

        void InitFloatingObjectKeyMap()
        {
            _floatingObjectsKeyMap = new Dictionary<KeyStroke, SpreadAction>();
            _floatingObjectsKeyMap.Add(new KeyStroke(VirtualKey.Escape, VirtualKeyModifiers.None), new SpreadAction(SpreadActions.UnSelectAllFloatingObjects));
            _floatingObjectsKeyMap.Add(new KeyStroke(VirtualKey.Delete, VirtualKeyModifiers.None), new SpreadAction(SpreadActions.DeleteFloatingObject));
            _floatingObjectsKeyMap.Add(new KeyStroke(VirtualKey.Tab, VirtualKeyModifiers.None), new SpreadAction(SpreadActions.NavigationNextFloatingObject));
            _floatingObjectsKeyMap.Add(new KeyStroke(VirtualKey.Tab, VirtualKeyModifiers.None | VirtualKeyModifiers.Shift), new SpreadAction(SpreadActions.NavigationPreviousFloatingObject));
            _floatingObjectsKeyMap.Add(new KeyStroke(VirtualKey.X, VirtualKeyModifiers.Control), new SpreadAction(SpreadActions.ClipboardCutFloatingObjects));
            _floatingObjectsKeyMap.Add(new KeyStroke(VirtualKey.C, VirtualKeyModifiers.Control), new SpreadAction(SpreadActions.ClipboardCopyFloatingObjects));
            _floatingObjectsKeyMap.Add(new KeyStroke(VirtualKey.V, VirtualKeyModifiers.Control), new SpreadAction(SpreadActions.ClipboardPasteFloatingObjects));
            _floatingObjectsKeyMap.Add(new KeyStroke(VirtualKey.A, VirtualKeyModifiers.Control), new SpreadAction(SpreadActions.SelectionAll));
            _floatingObjectsKeyMap.Add(new KeyStroke(VirtualKey.Left, VirtualKeyModifiers.None), new SpreadAction(SpreadActions.MoveFloatingObjectLeft));
            _floatingObjectsKeyMap.Add(new KeyStroke(VirtualKey.Up, VirtualKeyModifiers.None), new SpreadAction(SpreadActions.MoveFloatingObjectTop));
            _floatingObjectsKeyMap.Add(new KeyStroke(VirtualKey.Right, VirtualKeyModifiers.None), new SpreadAction(SpreadActions.MoveFloatingObjectRight));
            _floatingObjectsKeyMap.Add(new KeyStroke(VirtualKey.Down, VirtualKeyModifiers.None), new SpreadAction(SpreadActions.MoveFloatingObjectDown));
        }
    }
}

