#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-29 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Microsoft.UI.Text;
#endregion

namespace Dt.UIDemo
{
    [FvCall]
    public class TenTimesMid : IFvCall
    {
        public object Get(Mid m)
        {
            return m.Int / 100;
        }

        public object Set(Mid m)
        {
            return m.Int * 100;
        }
    }

    [FvCall]
    public class StyleMid : IFvCall
    {
        public object Get(Mid m)
        {
            ApplyStyle(m);
            return m.Val;
        }

        public object Set(Mid m)
        {
            ApplyStyle(m);
            return m.Val;
        }

        void ApplyStyle(Mid m)
        {
            int n = m.Int;
            if (n < 10)
            {
                m.Warn("不可小于10");
                m.Foreground = Res.RedBrush;
                m.FontWeight = FontWeights.Bold;
            }
            else if (n < 50)
            {
                m.Msg("偏小");
                m.Foreground = Res.GreenBrush;
                m.FontWeight = FontWeights.Normal;
            }
            else
            {
                m.Msg("偏大");
                m.Foreground = Res.亮蓝;
                m.FontWeight = FontWeights.Bold;
            }
        }
    }

    [FvCall]
    public class PrefixMid : IFvCall
    {
        public object Get(Mid m)
        {
            return $"第{m.Str}号";
        }

        public object Set(Mid m)
        {
            return m.Str.TrimStart('第').TrimEnd('号');
        }
    }

    [FvCall]
    public class MergeMid : IFvCall
    {
        public object Get(Mid m)
        {
            return $"{m.Str} - {m.GetVal<string>("merge1")} - {m.GetVal<string>("merge2")}";
        }

        public object Set(Mid m)
        {
            var strs = m.Str.Split('-');
            if (strs.Length > 2)
            {
                m["merge1"] = strs[1].Trim();
                m["merge2"] = strs[2].Trim();
            }
            else if (strs.Length > 1)
            {
                m["merge1"] = strs[1].Trim();
            }
            return strs[0].Trim();
        }
    }

    [FvCall]
    public class ReplaceMid : IFvCall
    {
        public object Get(Mid m)
        {
            switch (m.Str)
            {
                case "a":
                    return "1";

                case "b":
                    return "2";

                default:
                    return "3";
            }
        }

        public object Set(Mid m)
        {
            switch (m.Str)
            {
                case "1":
                    return "a";

                case "2":
                    return "b";

                default:
                    return "c";
            }
        }
    }
}