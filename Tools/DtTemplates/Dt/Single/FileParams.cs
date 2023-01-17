using System;
using System.Collections.Generic;
using System.Text;

namespace Dt
{
    class FileParams
    {
        public string NameSpace { get; set; }

        public string ClsRoot { get; set; }

        public List<string> Tbls { get; } = new List<string>();

        public List<string> Entities { get; } = new List<string>();

        public string Time { get; } = DateTime.Now.ToString("yyyy-MM-dd");

        public string UserName => Environment.UserName;

        public Dictionary<string, string> Params
        {
            get
            {
                if (_params == null)
                {
                    _params = new Dictionary<string, string>
                    {
                        {"$rootnamespace$", NameSpace },
                        {"$clsroot$", ClsRoot },
                        {"$time$", Time },
                        {"$username$", UserName },
                    };

                    if (Entities.Count > 1)
                    {
                        StringBuilder sb = new StringBuilder("VirObj<");
                        for (int i = 0; i < Entities.Count; i++)
                        {
                            if (i > 0)
                                sb.Append(", ");
                            sb.Append(Entities[i]);
                        }
                        sb.Append(">");
                        _params["$entity$"] = sb.ToString();
                    }
                    else
                    {
                        _params["$entity$"] = Entities[0];
                    }
                }
                return _params;
            }
        }
        Dictionary<string, string> _params;
    }
}
