using ScottPlot;
using SkiaSharp;
using Windows.Storage;

namespace Dt.Base;

class SkiaFontResolver : IFontResolver
{
    const string DefaultFontName = "HarmonySans";
    const string IconFontName = "IconFont";
    static string _fontPath;
    static string _boldFontPath;
    static string _iconFontPath;
    
    public static async Task Register()
    {
        Fonts.FontResolvers.Add(new SkiaFontResolver());
        Fonts.Default = DefaultFontName;

        var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Fonts/HarmonySans.ttf"));
        _fontPath = file.Path;

        // 仍需要获取文件路径，通过上述路径计算的路径在android中报文件不存在，可能android中StorageFile内部有获取访问权限的异步操作
        file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Fonts/HarmonySans_Bold.ttf"));
        _boldFontPath = file.Path;

        file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Fonts/icon.ttf"));
        _iconFontPath = file.Path;
    }
    
    public SKTypeface CreateTypeface(string p_fontName, FontWeight p_weight, FontSlant p_slant, FontSpacing p_spacing)
    {
        if (p_fontName == DefaultFontName)
        {
            return p_weight == FontWeight.Bold ? 
                SKTypeface.FromFile(_boldFontPath)
                : SKTypeface.FromFile(_fontPath);
        }

        if (p_fontName == IconFontName)
        {
            return SKTypeface.FromFile(_iconFontPath);
        }
        return null;
    }

    public SKTypeface CreateTypeface(string fontName, bool bold, bool italic)
    {
        return CreateTypeface(fontName,
            bold ? FontWeight.Bold : FontWeight.Normal,
            italic ? FontSlant.Italic : FontSlant.Upright,
            FontSpacing.Normal);
    }
}