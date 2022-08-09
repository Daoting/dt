using System;
using System.Collections.Generic;

namespace Dt.SingleTbl
{
    class FileParams
    {
        public string NameSpace { get; set; }

        public string Agent { get; set; }

        public string Entity { get; set; }

        public string Title { get; set; }

        public string Time { get; } = DateTime.Now.ToString("yyyy-MM-dd");

        public string UserName => Environment.UserName;

        public Dictionary<string, string> Params => new Dictionary<string, string>
            {
                {"$rootnamespace$", NameSpace },
                {"$agent$", Agent },
                {"$entityname$", Entity },
                {"$entitytitle$", Title },
                {"$time$", Time },
                {"$username$", UserName },
            };
    }
}
