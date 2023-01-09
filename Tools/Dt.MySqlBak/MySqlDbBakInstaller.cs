using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace Dt.MySqlBak
{
    [RunInstaller(true)]
    public partial class MySqlDbBakInstaller : System.Configuration.Install.Installer
    {
        public MySqlDbBakInstaller()
        {
            InitializeComponent();
        }
    }
}
