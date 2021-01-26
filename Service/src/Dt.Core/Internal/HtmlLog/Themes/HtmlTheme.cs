#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-01-22 创建
******************************************************************************/
#endregion

#region 引用命名
using System.IO;
#endregion

namespace Dt.Core.HtmlLog
{
    class HtmlTheme
    {
        public HtmlTheme()
        {
        }

        public virtual StyleReset Apply(TextWriter output, HtmlThemeStyle style)
        {
            switch (style)
            {
                case HtmlThemeStyle.LevelVerbose:
                case HtmlThemeStyle.LevelDebug:
                case HtmlThemeStyle.SecondaryText:
                case HtmlThemeStyle.Name:
                    output.Write("<span style=\"color: gray\">");
                    break;

                case HtmlThemeStyle.TertiaryText:
                    output.Write("<span style=\"color: darkgray\">");
                    break;

                case HtmlThemeStyle.LevelWarning:
                case HtmlThemeStyle.Invalid:
                    output.Write("<span style=\"color: yellow\">");
                    break;

                case HtmlThemeStyle.Null:
                case HtmlThemeStyle.Boolean:
                    output.Write("<span style=\"color: blue\">");
                    break;

                case HtmlThemeStyle.String:
                    output.Write("<span style=\"color: cyan\">");
                    break;

                case HtmlThemeStyle.Number:
                    output.Write("<span style=\"color: magenta\">");
                    break;

                case HtmlThemeStyle.Scalar:
                    output.Write("<span style=\"color: green\">");
                    break;


                case HtmlThemeStyle.LevelError:
                case HtmlThemeStyle.LevelFatal:
                    output.Write("<span style=\"color: white; background: red;\">");
                    break;
            }
            return new StyleReset(this, output, style);
        }

        public virtual void Reset(TextWriter output, HtmlThemeStyle style)
        {
            // 默认为黑底白字
            if (style != HtmlThemeStyle.Text && style != HtmlThemeStyle.LevelInformation)
                output.Write("</span>");
        }
    }

    class EmptyHtmlTheme : HtmlTheme
    {
        public override StyleReset Apply(TextWriter output, HtmlThemeStyle style)
        {
            return new StyleReset(this, output, style);
        }

        public override void Reset(TextWriter output, HtmlThemeStyle style)
        {

        }
    }
}
