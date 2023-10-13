using System.Windows.Forms;

namespace Dt.PrjWizard
{
    public partial class OptionsForm : Form
    {
        public OptionsForm()
        {
            InitializeComponent();
        }

        public bool UseWebAssembly => cbWebAssembly.Checked;

        public bool UseiOS => cbIOS.Checked;

        public bool UseAndroid => cbAndroid.Checked;

        public bool UseSvc => rbAll.Checked;
    }
}
