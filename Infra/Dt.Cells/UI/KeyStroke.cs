#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.System;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents the user's gesture of pressing a key on the computer.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct KeyStroke : IEquatable<KeyStroke>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:GrapeCity.Windows.SpreadSheet.UI.KeyStroke" /> structure 
        /// with the specified character.
        /// </summary>
        /// <param name="keyChar">
        /// Character defined by this <see cref="T:GrapeCity.Windows.SpreadSheet.UI.KeyStroke" /> structure.
        /// </param>
        public KeyStroke(char keyChar)
        {
            this = new KeyStroke();
            KeyChar = keyChar;
            KeyCode = VirtualKey.None;
            Modifiers = VirtualKeyModifiers.None;
            OnKeyRelease = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:GrapeCity.Windows.SpreadSheet.UI.KeyStroke" /> structure 
        /// with the specified <see cref="T:System.Windows.Input.Key" /> and 
        /// <see cref="T:System.Windows.Input.ModifierKeys" />.
        /// </summary>
        /// <param name="keyCode">
        /// The key code defined by this <see cref="T:GrapeCity.Windows.SpreadSheet.UI.KeyStroke" /> structure.
        /// </param>
        /// <param name="modifiers">
        /// The modifier keys defined by this <see cref="T:GrapeCity.Windows.SpreadSheet.UI.KeyStroke" /> structure.
        /// </param>
        public KeyStroke(VirtualKey keyCode, VirtualKeyModifiers modifiers)
        {
            this = new KeyStroke();
            KeyChar = '\0';
            KeyCode = keyCode;
            Modifiers = modifiers;
            OnKeyRelease = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:GrapeCity.Windows.SpreadSheet.UI.KeyStroke" /> structure 
        /// with the specified <see cref="T:System.Windows.Input.Key" />, <see cref="T:System.Windows.Input.ModifierKeys" />, 
        /// and a value that indicates whether this <see cref="T:GrapeCity.Windows.SpreadSheet.UI.KeyStroke" /> is active on the key
        /// release.
        /// </summary>
        /// <param name="keyCode">
        /// The key code defined by this <see cref="T:GrapeCity.Windows.SpreadSheet.UI.KeyStroke" /> structure.
        /// </param>
        /// <param name="modifiers">
        /// The modifier keys defined by this <see cref="T:GrapeCity.Windows.SpreadSheet.UI.KeyStroke" /> structure.
        /// </param>
        /// <param name="onKeyRelease">
        /// A value that indicates whether the <see cref="T:GrapeCity.Windows.SpreadSheet.UI.KeyStroke" /> is active on the key release.
        /// </param>
        public KeyStroke(VirtualKey keyCode, VirtualKeyModifiers modifiers, bool onKeyRelease)
        {
            this = new KeyStroke();
            KeyChar = '\0';
            KeyCode = keyCode;
            Modifiers = modifiers;
            OnKeyRelease = onKeyRelease;
        }

        /// <summary>
        /// Gets the character defined by the <see cref="T:GrapeCity.Windows.SpreadSheet.UI.KeyStroke" /> structure.
        /// </summary>
        public char KeyChar { get; private set; }
        /// <summary>
        /// Gets the key code defined by the <see cref="T:GrapeCity.Windows.SpreadSheet.UI.KeyStroke" /> structure.
        /// </summary>
        public VirtualKey KeyCode { get; private set; }
        /// <summary>
        /// Gets the modifier keys defined by the <see cref="T:GrapeCity.Windows.SpreadSheet.UI.KeyStroke" /> structure.
        /// </summary>
        public VirtualKeyModifiers Modifiers { get; private set; }
        /// <summary>
        /// Gets a value that indicates whether the <see cref="T:GrapeCity.Windows.SpreadSheet.UI.KeyStroke" /> is active on the key release.
        /// </summary>
        public bool OnKeyRelease { get; private set; }
        /// <summary>
        /// Determines whether two <see cref="T:GrapeCity.Windows.SpreadSheet.UI.KeyStroke" /> objects are equal.
        /// </summary>
        /// <param name="lhs">
        /// The first <see cref="T:GrapeCity.Windows.SpreadSheet.UI.KeyStroke" /> (left side of equality operator). 
        /// </param>
        /// <param name="rhs">
        /// The second <see cref="T:GrapeCity.Windows.SpreadSheet.UI.KeyStroke" /> (right side of equality operator).
        /// </param>
        /// <returns>
        /// <c>true</c> if these two <see cref="T:GrapeCity.Windows.SpreadSheet.UI.KeyStroke" /> objects are equal; otherwise, returns <c>false</c>.
        /// </returns>
        public static bool operator ==(KeyStroke lhs, KeyStroke rhs)
        {
            return ((((lhs.KeyChar == rhs.KeyChar) && (lhs.KeyCode == rhs.KeyCode)) && (lhs.Modifiers == rhs.Modifiers)) && (lhs.OnKeyRelease == rhs.OnKeyRelease));
        }

        /// <summary>
        /// Determines whether two <see cref="T:GrapeCity.Windows.SpreadSheet.UI.KeyStroke" /> objects are not equal.
        /// </summary>
        /// <param name="lhs">
        /// The first <see cref="T:GrapeCity.Windows.SpreadSheet.UI.KeyStroke" /> (left side of inequality operator). 
        /// </param>
        /// <param name="rhs">
        /// The second <see cref="T:GrapeCity.Windows.SpreadSheet.UI.KeyStroke" /> (right side of inequality operator).
        /// </param>
        /// <returns>
        /// <c>true</c> if these two <see cref="T:GrapeCity.Windows.SpreadSheet.UI.KeyStroke" /> objects are not equal; otherwise, returns <c>false</c>.
        /// </returns>
        public static bool operator !=(KeyStroke lhs, KeyStroke rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:arPoint.Silverlight.Prototype.KeyStroke" /> is equal to the current object.
        /// </summary>
        /// <param name="other">
        /// The <see cref="T:arPoint.Silverlight.Prototype.KeyStroke" /> object to compare with the current object. 
        /// </param>
        /// <returns>
        /// Returns <c>true</c> if these two <see cref="T:arPoint.Silverlight.Prototype.KeyStroke" /> objects are equal; otherwise, returns <c>false</c>.
        /// </returns>
        public bool Equals(KeyStroke other)
        {
            return (other == this);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">
        /// The object to compare with the current object. 
        /// </param>
        /// <returns>
        /// Returns <c>true</c> if these two objects are equal; otherwise, returns <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return ((obj is KeyStroke) && (((KeyStroke) obj) == this));
        }

        /// <summary>
        /// Generates a hash code for the current object.
        /// </summary>
        /// <returns>
        /// The hash code.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

