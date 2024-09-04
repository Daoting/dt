

using System.ComponentModel;

namespace Dt.Cells.Data
{
    public partial class Worksheet
    {
        #region PrintArea
        string _printArea;

        [DefaultValue(null)]
        public string PrintArea
        {
            get { return _printArea; }
            set
            {
                if (value != _printArea)
                {
                    _printArea = value;
                    RaisePropertyChanged("PrintArea");
                }
            }
        }
        #endregion
    }
}