using Dt.Toolkit.Sql;

namespace TestToolkit
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        void button1_Click(object sender, EventArgs e)
        {
            _lbl.Text = SqlFormatter.Format(_tb.Text);
        }
    }
}
