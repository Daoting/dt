using System;
using System.Collections.Generic;

namespace Dt.OnToMany
{
    class FileParams
    {
        public string NameSpace { get; set; }

        public string Agent { get; set; }

        public string MainTbl { get; set; }

        public bool IsSelectedMainTbl => !string.IsNullOrEmpty(MainTbl);

        public string MainEntity { get; set; }

        public string MainTitle { get; set; }

        public string[] ChildTbls { get; set; }

        public bool IsSelectedChildTbls => ChildTbls != null && ChildTbls.Length > 0;

        public string[] ChildEntities { get; set; }

        public string[] ChildTitles { get; set; }

        public string Time { get; } = DateTime.Now.ToString("yyyy-MM-dd");

        public string UserName => Environment.UserName;
    }
}
