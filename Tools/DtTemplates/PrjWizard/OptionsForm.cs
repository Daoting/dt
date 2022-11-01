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

        public SvcType UseSvcType => rbAll.Checked ? SvcType.DtSvc : (rbCustom.Checked ? SvcType.CustomSvc : SvcType.None);
    }

    public enum SvcType
    {
        DtSvc,
        CustomSvc,
        None
    }
}
