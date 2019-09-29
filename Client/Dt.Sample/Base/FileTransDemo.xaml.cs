using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Dt.Base;
using Dt.Core;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace Dt.Sample
{
    public sealed partial class FileTransDemo : PageWin
    {
        public FileTransDemo()
        {
            InitializeComponent();
            this.LoadAsync(() =>
            {
                var xml = AtLocal.GetCookie("FileTransDemo");
                if (!string.IsNullOrEmpty(xml))
                    _trans.Data = xml;
            });
        }

        void OnUploadFinished(FileList sender, bool suc)
        {
            if (suc)
                AtLocal.SaveCookie("FileTransDemo", _trans.Data);
        }

        void OnClear(object sender, RoutedEventArgs e)
        {
            AtLocal.DeleteCookie("FileTransDemo");
            _trans.Data = null;
        }

        void OnDelTemp(object sender, RoutedEventArgs e)
        {
            AtSys.ClearDoc();
        }
    }
}
