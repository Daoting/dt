#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名

using System;
using System.Collections.Generic;
using System.Reflection;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents all built-in themes.
    /// </summary>
    public class SpreadThemes : NotifyCollection<SpreadTheme>
    {
        /// <summary>
        /// The Apex theme.
        /// </summary>
        public static SpreadTheme Apex = new SpreadTheme("Apex", ThemeColors.Apex, "Lucida Sans", "Book Antiqua");
        /// <summary>
        /// The Aspect theme.
        /// </summary>
        public static SpreadTheme Aspect = new SpreadTheme("Aspect", ThemeColors.Aspect, "Verdana", "Verdana");
        /// <summary>
        /// The Civic theme.
        /// </summary>
        public static SpreadTheme Civic = new SpreadTheme("Civic", ThemeColors.Civic, "Georgia", "Georgia");
        /// <summary>
        /// The Concourse theme.
        /// </summary>
        public static SpreadTheme Concourse = new SpreadTheme("Concourse", ThemeColors.Concourse, "Lucida Sans Unicode", "Lucida Sans Unicode");
        /// <summary>
        /// The Equity theme.
        /// </summary>
        public static SpreadTheme Equity = new SpreadTheme("Equity", ThemeColors.Equity, "Franklin Gothic Book", "Perpetua");
        /// <summary>
        /// The Flow theme.
        /// </summary>
        public static SpreadTheme Flow = new SpreadTheme("Flow", ThemeColors.Flow, NameConstans.DEFAULT_FONT_FAMILY, "Constantia");
        /// <summary>
        /// The Foundry theme.
        /// </summary>
        public static SpreadTheme Foundry = new SpreadTheme("Foundry", ThemeColors.Foundry, "Rockwell", "Rockwell");
        /// <summary>
        /// The Median theme.
        /// </summary>
        public static SpreadTheme Median = new SpreadTheme("Median", ThemeColors.Median, "Tw Cen MT", "Tw Cen MT");
        /// <summary>
        /// The Metro theme.
        /// </summary>
        public static SpreadTheme Metro = new SpreadTheme("Metro", ThemeColors.Metro, "Consolas", "Corbel");
        /// <summary>
        /// The Module theme.
        /// </summary>
        public static SpreadTheme Module = new SpreadTheme("Module", ThemeColors.Module, "Corbel", "Corbel");
        /// <summary>
        /// The Office theme.
        /// </summary>
        public static SpreadTheme Office = new SpreadTheme("Office", ThemeColors.Office, "Cambria", NameConstans.DEFAULT_FONT_FAMILY);
        /// <summary>
        /// The Opulent theme.
        /// </summary>
        public static SpreadTheme Opulent = new SpreadTheme("Opulent", ThemeColors.Opulent, "Trebuchet MS", "Trebuchet MS");
        /// <summary>
        /// The Oriel theme.
        /// </summary>
        public static SpreadTheme Oriel = new SpreadTheme("Oriel", ThemeColors.Oriel, "Century Schoolbook", "Century Schoolbook");
        /// <summary>
        /// The Origin theme.
        /// </summary>
        public static SpreadTheme Origin = new SpreadTheme("Origin", ThemeColors.Origin, "Bookman Old Style", "Gill Sans MT");
        /// <summary>
        /// The Paper theme.
        /// </summary>
        public static SpreadTheme Paper = new SpreadTheme("Paper", ThemeColors.Paper, "Constantia", "Constantia");
        /// <summary>
        /// The Solstice theme.
        /// </summary>
        public static SpreadTheme Solstice = new SpreadTheme("Solstice", ThemeColors.Solstice, "Gill Sans MT", "Gill Sans MT");
        /// <summary>
        /// The Technic theme.
        /// </summary>
        public static SpreadTheme Technic = new SpreadTheme("Technic", ThemeColors.Technic, "Franklin Gothic Book", "Arial");
        /// <summary>
        /// The Trek theme.
        /// </summary>
        public static SpreadTheme Trek = new SpreadTheme("Trek", ThemeColors.Trek, "Franklin Gothic Medium", "Franklin Gothic Book");
        /// <summary>
        /// The Urban theme.
        /// </summary>
        public static SpreadTheme Urban = new SpreadTheme("Urban", ThemeColors.Urban, "Trebuchet MS", "Georgia");
        /// <summary>
        /// The Verve theme.
        /// </summary>
        public static SpreadTheme Verve = new SpreadTheme("Verve", ThemeColors.Verve, "Century Gothic", "Century Gothic");

        internal static SpreadTheme[] All = new SpreadTheme[] { 
            Office, Apex, Aspect, Civic, Concourse, Equity, Flow, Foundry, Median, Metro, Module, Opulent, Oriel, Origin, Paper, Solstice, 
            Technic, Trek, Urban, Verve
         };

        /// <summary>
        /// Adds the specified theme.
        /// </summary>
        /// <param name="item">The added theme.</param>
        public override void Add(SpreadTheme item)
        {
            if (this.Contains(item.Name))
            {
                throw new ArgumentException(ResourceStrings.CouldnotAddThemeWithSameName);
            }
            base.Add(item);
        }

        /// <summary>
        /// Determines whether the theme contains the specified theme. 
        /// </summary>
        /// <param name="themeName">Name of the theme.</param>
        /// <returns>
        /// <c>true</c> if it contains the specified theme name; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(string themeName)
        {
            using (IEnumerator<SpreadTheme> enumerator = base.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (string.Equals(enumerator.Current.Name, themeName))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the index of the specified theme in the theme collection.
        /// </summary>
        /// <param name="themeName">Name of the theme.</param>
        /// <returns></returns>
        public int IndexOf(string themeName)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (string.Equals(this[i].Name, themeName))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Removes the specified theme from the theme collection.
        /// </summary>
        /// <param name="themeName">Name of the theme.</param>
        public void Remove(string themeName)
        {
            SpreadTheme item = this[themeName];
            if (item != null)
            {
                this.Remove(item);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="T:Dt.Cells.Data.SpreadTheme" /> with the specified theme name.
        /// </summary>
        public SpreadTheme this[string themeName]
        {
            get
            {
                foreach (SpreadTheme theme in this)
                {
                    if (string.Equals(theme.Name, themeName))
                    {
                        return theme;
                    }
                }
                return null;
            }
            set
            {
                int index = this.IndexOf(themeName);
                if (index > -1)
                {
                    this[index] = value;
                }
            }
        }
    }
}

