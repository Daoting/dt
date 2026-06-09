using Mapsui.Styles;

namespace Dt.Base;

public class TextStyle : LabelStyle
{
    public TextStyle()
    {
        Font.FontFamily = SkiaFontResolver.DefaultFontName;
    }

    public TextStyle(TextStyle labelStyle) : base(labelStyle) { }
}

public class IconStyle : LabelStyle
{
    public IconStyle()
    {
        Font.FontFamily = SkiaFontResolver.IconFontName;
    }

    public IconStyle(IconStyle labelStyle) : base(labelStyle) { }
}