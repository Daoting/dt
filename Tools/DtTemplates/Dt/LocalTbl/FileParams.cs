using System;
using System.Collections.Generic;

namespace Dt.LocalTbl
{
    class FileParams
    {
        public string NameSpace { get; set; }

        public string Table { get; set; }

        public string Entity { get; set; }

        public string Title { get; set; }

        public string Time { get; } = DateTime.Now.ToString("yyyy-MM-dd");

        public string UserName => Environment.UserName;

    }
}
