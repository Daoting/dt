using Mapsui.Styles;

namespace Dt.Base;

public class TextStyle : LabelStyle
{
    public TextStyle()
    {
#if WIN
        Font.FontFamily = "Microsoft YaHei UI";
#else
        Font.FontFamily = "HarmonyOS Sans SC";
#endif
    }

    public TextStyle(TextStyle labelStyle) : base(labelStyle) { }
}