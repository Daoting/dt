#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a sheet collection.
    /// </summary>
    public class WorksheetCollection : NotifyCollection<Worksheet>
    {
        /// <summary>
        /// The workbook.
        /// </summary>
        Dt.Cells.Data.Workbook workbook;

        /// <summary>
        /// Initializes a new instance of the <see cref="P:Dt.Cells.Data.WorksheetCollection.Workbook" /> class.
        /// </summary>
        internal WorksheetCollection(Dt.Cells.Data.Workbook workbook)
        {
            this.workbook = workbook;
        }

        /// <summary>
        /// Adds the specified sheet to the collection.
        /// </summary>
        /// <param name="worksheet">The <see cref="T:Dt.Cells.Data.Worksheet" /> object to add.</param>
        public override void Add(Worksheet worksheet)
        {
            if (worksheet == null)
            {
                throw new ArgumentNullException("worksheet");
            }
            if (!base.Contains(worksheet))
            {
                this.AddWorksheetInWorkbookInternal(this.workbook, worksheet);
                base.Add(worksheet);
            }
            if (this.workbook.ActiveSheetIndex < 0)
            {
                this.workbook.ActiveSheetIndex = 0;
            }
            if (this.workbook.StartSheetIndex < 0)
            {
                this.workbook.StartSheetIndex = 0;
            }
        }

        /// <summary>
        /// Adds the worksheet in workbook.
        /// </summary>
        /// <param name="workbook">The workbook.</param>
        /// <param name="worksheet">The worksheet.</param>
        internal static void AddWorksheetInWorkbook(Dt.Cells.Data.Workbook workbook, Worksheet worksheet)
        {
            if (workbook == null)
            {
                throw new ArgumentNullException("workbook");
            }
            if (worksheet == null)
            {
                throw new ArgumentNullException("worksheet");
            }
            using (IEnumerator<Worksheet> enumerator = workbook.Sheets.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.Name == worksheet.Name)
                    {
                        throw new NotSupportedException(ResourceStrings.AnotherWorksheetWithTheSameNameError);
                    }
                }
            }
            if ((worksheet.Name == null) || (worksheet.Name == string.Empty))
            {
                worksheet.Name = GeneralNewSheetName(workbook);
            }
            new Dictionary<ulong, ulong>();
            if (!workbook.Sheets.Contains(worksheet))
            {
                if ((worksheet.NamedStyles == null) && (workbook != null))
                {
                    worksheet.NamedStyles = workbook.NamedStyles;
                }
                if ((workbook != null) && (worksheet.NamedStyles == workbook.NamedStyles))
                {
                    worksheet.NamedStyles.Owner = workbook;
                }
                worksheet.Workbook = workbook;
            }
            int index = workbook.Sheets.IndexOf(worksheet);
            if ((index > 0) && (index < (workbook.Sheets.Count - 1)))
            {
                worksheet.CalcAxial.InsertSheet(workbook.Sheets);
            }
        }

        /// <summary>
        /// Adds the worksheet in workbook.
        /// </summary>
        /// <param name="workbook">The workbook.</param>
        /// <param name="worksheet">The worksheet.</param>
        protected internal virtual void AddWorksheetInWorkbookInternal(Dt.Cells.Data.Workbook workbook, Worksheet worksheet)
        {
            AddWorksheetInWorkbook(workbook, worksheet);
        }

        /// <summary>
        /// Clears the workbook.
        /// </summary>
        protected override void ClearInternal()
        {
            if (base.items != null)
            {
                foreach (Worksheet worksheet in base.items.ToArray())
                {
                    RemoveWorksheetFromWorkbook(this.workbook, worksheet);
                }
                base.items.Clear();
            }
        }

        /// <summary>
        /// Generals the new name of the sheet.
        /// </summary>
        /// <returns></returns>
        static string GeneralNewSheetName(Dt.Cells.Data.Workbook workbook)
        {
            int num = 0;
            if (workbook != null)
            {
                foreach (Worksheet worksheet in workbook.Sheets)
                {
                    if ((worksheet.Name != null) && Regex.Match(worksheet.Name, "^Sheet[1-9][0-9]*$", (RegexOptions) RegexOptions.IgnoreCase).Success)
                    {
                        string s = worksheet.Name.Substring("Sheet".Length, worksheet.Name.Length - "Sheet".Length);
                        int result = -1;
                        if (int.TryParse(s, out result))
                        {
                            num = Math.Max(num, result);
                        }
                    }
                }
            }
            return string.Format("Sheet{0}", (object[]) new object[] { ((int) (num + 1)) });
        }

        /// <summary>
        /// Inserts the specified sheet at the specified index in the collection.
        /// </summary>
        /// <param name="index">The index at which to add the sheet.</param>
        /// <param name="worksheet">The <see cref="T:Dt.Cells.Data.Worksheet" /> object to add.</param>
        public override void Insert(int index, Worksheet worksheet)
        {
            if (worksheet == null)
            {
                throw new ArgumentNullException("worksheet");
            }
            if (!base.Contains(worksheet))
            {
                this.AddWorksheetInWorkbookInternal(this.workbook, worksheet);
                base.Insert(index, worksheet);
            }
            if (this.workbook.ActiveSheetIndex < 0)
            {
                this.workbook.ActiveSheetIndex = 0;
            }
        }

        /// <summary>
        /// Removes the specified sheet from the collection.
        /// </summary>
        /// <param name="worksheet">The <see cref="T:Dt.Cells.Data.Worksheet" /> object to remove.</param> 
        /// /// <returns>
        /// Returns <c>true</c> if the sheet is removed; otherwise, <c>false</c>.
        /// </returns>
        public override bool Remove(Worksheet worksheet)
        {
            int index = base.IndexOf(worksheet);
            RemoveWorksheetFromWorkbook(this.workbook, worksheet);
            if (index == (this.Count - 1))
            {
                this.workbook.ActiveSheetIndex = index - 1;
            }
            return base.Remove(worksheet);
        }

        /// <summary>
        /// Removes the sheet at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the sheet to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is not a valid index in the collection.</exception>
        /// <exception cref="T:System.NotSupportedException">The collection is read-only.</exception>
        public override void RemoveAt(int index)
        {
            if ((index < 0) || (index >= this.Count))
            {
                throw new ArgumentOutOfRangeException("index");
            }
            RemoveWorksheetFromWorkbook(this.workbook, this[index]);
            if (this.workbook.ActiveSheetIndex == (this.workbook.SheetCount - 1))
            {
                this.workbook.ActiveSheetIndex--;
            }
            base.RemoveAt(index);
        }

        /// <summary>
        /// Removes the worksheet from workbook.
        /// </summary>
        /// <param name="workbook">The workbook.</param>
        /// <param name="worksheet">The worksheet.</param>
        internal static void RemoveWorksheetFromWorkbook(Dt.Cells.Data.Workbook workbook, Worksheet worksheet)
        {
            int index = workbook.Sheets.IndexOf(worksheet);
            int num2 = (index == (workbook.Sheets.Count - 1)) ? (workbook.Sheets.Count - 2) : (index + 1);
            ICalcSource replacedSource = (num2 == -1) ? null : ((ICalcSource) workbook.Sheets[num2]);
            workbook.Sheets[index].CalcAxial.RemoveSheet(workbook.Sheets, replacedSource);
            worksheet.Workbook = null;
        }

        internal void WriteXmlInternal(XmlWriter writer, bool dataOnly, bool saveDataSource, params Worksheet[] ignoreSheets)
        {
        }

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        /// <value>The number of elements contained in the collection.</value>
        [DefaultValue(0)]
        public new int Count
        {
            get { return  base.Count; }
            set
            {
                if (-1 < value)
                {
                    int num = base.Count - value;
                    if (num < 0)
                    {
                        for (int i = 0; i < Math.Abs(num); i++)
                        {
                            this.Add(new Worksheet((this.workbook != null) ? this.workbook.NamedStyles : null));
                        }
                    }
                    else if (num > 0)
                    {
                        int num3 = this.Count - 1;
                        for (int j = 0; j < num; j++)
                        {
                            this.RemoveAt(num3--);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="T:Dt.Cells.Data.Worksheet" /> with the specified name.
        /// </summary>
        /// <param name="name">The sheet name for which to search.</param>
        /// <value>The <see cref="T:Dt.Cells.Data.Worksheet" /> object with the name.</value>
        public Worksheet this[string name]
        {
            get
            {
                foreach (Worksheet worksheet in this)
                {
                    if ((worksheet != null) && (worksheet.Name == name))
                    {
                        return worksheet;
                    }
                }
                return null;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                int num = -1;
                for (int i = 0; i < base.Items.Count; i++)
                {
                    Worksheet worksheet = base.Items[i];
                    if ((worksheet != null) && (worksheet.Name == name))
                    {
                        num = i;
                        break;
                    }
                }
                if (num <= -1)
                {
                    throw new ArgumentException(string.Format(ResourceStrings.FailedFoundWorksheetError, (object[]) new object[] { name }));
                }
                this[num] = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="T:Dt.Cells.Data.Worksheet" /> at the specified index.
        /// </summary>
        /// <param name="index">The sheet index for which to search.</param>
        /// <value>The <see cref="T:Dt.Cells.Data.Worksheet" /> object with the name.</value>
        public override Worksheet this[int index]
        {
            get { return  base[index]; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                Worksheet worksheet = base[index];
                if (worksheet != null)
                {
                    RemoveWorksheetFromWorkbook(this.workbook, worksheet);
                }
                this.AddWorksheetInWorkbookInternal(this.workbook, value);
                base[index] = value;
            }
        }

        /// <summary>
        /// Gets the workbook.
        /// </summary>
        /// <value>The <see cref="P:Dt.Cells.Data.WorksheetCollection.Workbook" /> object for this sheet.</value>
        internal Dt.Cells.Data.Workbook Workbook
        {
            get { return  this.workbook; }
        }
    }
}

