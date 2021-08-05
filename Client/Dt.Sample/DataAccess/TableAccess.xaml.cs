#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using Dt.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public partial class TableAccess : Win
    {
        int _index;

        public TableAccess()
        {
            InitializeComponent();
        }

        Table CreateTable()
        {
            return new Table
            {
                { "id" },
                { "bh", typeof(int) },
                { "chushengrq", typeof(DateTime) },
                { "hunfou", typeof(bool) },
                { "shengao", typeof(double) },
                { "bumen", typeof(Gender) },
            };
        }

        void OnCreateTable(object sender, RoutedEventArgs e)
        {
            WriteColumns(CreateTable());
        }

        void OnCreateTableByName(object sender, RoutedEventArgs e)
        {
            Table tbl = Table.Create("cm_menu");
            WriteColumns(tbl);
        }

        void OnCreateTableByLocalName(object sender, RoutedEventArgs e)
        {
            Table tbl = Table.Create(new ClientLog());
            WriteColumns(tbl);
        }

        void OnCreateTableByTable(object sender, RoutedEventArgs e)
        {
            Table src = CreateTable();
            Table tbl = Table.Create(src);
            WriteColumns(tbl);
        }

        void OnCreateTableByRow(object sender, RoutedEventArgs e)
        {
            Table src = CreateTable();
            Row row = src.NewRow();
            Table tbl = Table.Create(row);
            WriteColumns(tbl);
        }

        void WriteColumns(Table p_tbl)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var col in p_tbl.Columns)
            {
                sb.AppendLine($"{col.ID}    {col.Type.Name}");
            }
            _tbInfo.Text = sb.ToString();
        }

        void WriteRows(Table p_tbl)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var row in p_tbl)
            {
                foreach (var cell in row.Cells)
                {
                    sb.AppendFormat("{0}：{1}    ", cell.ID, cell.Val);
                }
                sb.AppendLine();
            }
            _tbInfo.Text = sb.ToString();
        }

        void OnAddEmptyRow(object sender, RoutedEventArgs e)
        {
            var tbl = CreateTable();
            var row = tbl.AddRow();
            WriteRows(tbl);
        }

        void OnAddRow(object sender, RoutedEventArgs e)
        {
            var tbl = CreateTable();
            var row = tbl.AddRow(new
            {
                id = "abc",
                bh = 110,
                chushengrq = DateTime.Now,
                hunfou = true,
                shengao = 1.80,
                bumen = Gender.女
            });
            WriteRows(tbl);
        }

        void OnAddSingleRow(object sender, RoutedEventArgs e)
        {
            var tbl = CreateTable();
            var row = tbl.NewRow(new { id = "123" });
            _tbInfo.Text = "创建独立行: tbl.CreateRow()";
        }

        void OnCloneRow(object sender, RoutedEventArgs e)
        {
            var tbl = CreateTable();
            var row = tbl.AddRow(new
            {
                id = "abc",
                bh = 110,
                chushengrq = DateTime.Now,
                hunfou = true,
                shengao = 1.80,
                bumen = Gender.未知
            });
            var clone = row.Clone();
            _tbInfo.Text = "克隆行: row.Clone()";
        }

        void OnRemoveRow(object sender, RoutedEventArgs e)
        {
            //tbl.RemoveAt(tbl.Count - 1);
            _tbInfo.Text = "删除行: Remove,RemoveAt";
        }

        void OnAddCol(object sender, RoutedEventArgs e)
        {
            var tbl = CreateData();
            tbl.Columns.Add(new Column($"add{_index++}"));
            WriteRows(tbl);
        }

        void OnRemoveCol(object sender, RoutedEventArgs e)
        {
            var tbl = CreateTable();
            tbl.Columns.RemoveAt(tbl.Columns.Count - 1);
            WriteColumns(tbl);
        }

        Table CreateData()
        {
            var tbl = CreateTable();
            for (int i = 0; i < 10; i++)
            {
                tbl.AddRow(new
                {
                    id = "abc",
                    bh = _index++,
                    chushengrq = DateTime.Now,
                    hunfou = true,
                    shengao = 1.80,
                    bumen = Gender.未知
                });
            }
            return tbl;
        }
    }
}