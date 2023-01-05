using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dt.Editor
{
    public partial class BatchCells : UserControl, ICellControl
    {
        string _xaml;

        public BatchCells()
        {
            InitializeComponent();

            AtSvc.BindSvcUrl(_svcUrl);
            AddTooltip();
        }

        string ICellControl.GetText()
        {
           if (string.IsNullOrEmpty(_xaml))
            {
                MessageBox.Show("请选择表！");
                return "";
            }
            return _xaml;
         }

        void ICellControl.Reset()
        {
            
        }

        async void _cbTbls_DropDown(object sender, EventArgs e)
        {
            var ls = await AtSvc.GetAllTables();
            if (_cbTbls.DataSource != ls)
                _cbTbls.DataSource = ls;
        }

        async void _cbTbls_SelectedIndexChanged(object sender, EventArgs e)
        {
            string tbl = null;
            if (_cbTbls.SelectedItem != null)
                tbl = _cbTbls.SelectedItem.ToString();
            if (!string.IsNullOrEmpty(tbl))
            {
                // $namespace$ $rootnamespace$只能手动
                _xaml = await AtSvc.GetFvCells(tbl);
            }
        }

        void AddTooltip()
        {
            ToolTip tip = new ToolTip();
            tip.SetToolTip(linkLabel3, Kit.SvcUrlTip);
            tip.SetToolTip(linkLabel4, Kit.AllTblsTip);
        }
    }
}
