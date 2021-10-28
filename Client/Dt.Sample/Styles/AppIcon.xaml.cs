#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
#endregion

namespace Dt.Sample
{
    /// <summary>
    /// 
    /// </summary>
    public partial class AppIcon : Win
    {
        public AppIcon()
        {
            InitializeComponent();

            var tbl = new Table { { "icon", typeof(Icons) } };
            _fv.Data = tbl.NewRow();
        }

        async void OnIOS(object sender, RoutedEventArgs e)
        {
            Icons icon = (Icons)_fv.Row["icon"];
            if (icon == Icons.None)
            {
                Kit.Msg("请选择图标");
                return;
            }

            _bd.Background = Res.主蓝;

            // 字体 / 外框 = 0.8
            var folder = await OpenFolder($"{icon}_ios");

            var assets = await folder.CreateFolderAsync("Assets", CreationCollisionOption.OpenIfExists);
            await SaveIcon(408, 484, 326, icon, "SplashScreen@2x.png", assets);
            await SaveIcon(612, 726, 490, icon, "SplashScreen@3x.png", assets);
            await SaveIcon(50, 50, 40, icon, "logo.png", assets);

            await SaveIcon(120, 120, 96, icon, "Icon-120.png", folder);
            await SaveIcon(1024, 1024, 819, icon, "Icon-1024.png", folder);
            await SaveIcon(40, 40, 32, icon, "Icon-20@2x.png", folder);
            await SaveIcon(60, 60, 48, icon, "Icon-20@3x.png", folder);
            await SaveIcon(58, 58, 46, icon, "Icon-29@2x.png", folder);
            await SaveIcon(87, 87, 69, icon, "Icon-29@3x.png", folder);
            await SaveIcon(80, 80, 64, icon, "Icon-40@2x.png", folder);
            await SaveIcon(180, 180, 144, icon, "Icon-60@3x.png", folder);
            
            Kit.Msg("生成成功，路径: " + folder.Path);
        }

        async void OnDroid(object sender, RoutedEventArgs e)
        {
            Icons icon = (Icons)_fv.Row["icon"];
            if (icon == Icons.None)
            {
                Kit.Msg("请选择图标");
                return;
            }

            var root = await OpenFolder($"{icon}_android");

            // 旧图标
            //var folder = await root.CreateFolderAsync("drawable", CreationCollisionOption.OpenIfExists);
            //await SaveIcon(640, 1136, 326, icon, "back.png", folder);
            //await SaveIcon(72, 72, 58, icon, "icon.png", folder);
            //await SaveIcon(50, 50, 40, icon, "logo.png", folder);

            //folder = await root.CreateFolderAsync("drawable-hdpi", CreationCollisionOption.OpenIfExists);
            //await SaveIcon(72, 72, 58, icon, "icon.png", folder);

            //folder = await root.CreateFolderAsync("drawable-xhdpi", CreationCollisionOption.OpenIfExists);
            //await SaveIcon(96, 96, 76, icon, "icon.png", folder);

            //folder = await root.CreateFolderAsync("drawable-xxhdpi", CreationCollisionOption.OpenIfExists);
            //await SaveIcon(144, 144, 115, icon, "icon.png", folder);

            _bd.Background = Res.主蓝;
            var folder = await root.CreateFolderAsync("drawable", CreationCollisionOption.OpenIfExists);
            await SaveIcon(640, 1136, 326, icon, "back.png", folder);
            await SaveIcon(50, 50, 40, icon, "logo.png", folder);

            folder = await root.CreateFolderAsync("mipmap-hdpi", CreationCollisionOption.OpenIfExists);
            await SaveIcon(72, 72, 58, icon, "ic_launcher.png", folder);
            await SaveIcon(72, 72, 58, icon, "ic_launcher_round.png", folder);
            _bd.Background = null;
            await SaveIcon(162, 162, 80, icon, "ic_launcher_foreground.png", folder);

            folder = await root.CreateFolderAsync("mipmap-mdpi", CreationCollisionOption.OpenIfExists);
            _bd.Background = Res.主蓝;
            await SaveIcon(48, 48, 38, icon, "ic_launcher.png", folder);
            await SaveIcon(48, 48, 38, icon, "ic_launcher_round.png", folder);
            _bd.Background = null;
            await SaveIcon(108, 108, 54, icon, "ic_launcher_foreground.png", folder);

            folder = await root.CreateFolderAsync("mipmap-xhdpi", CreationCollisionOption.OpenIfExists);
            _bd.Background = Res.主蓝;
            await SaveIcon(96, 96, 76, icon, "ic_launcher.png", folder);
            await SaveIcon(96, 96, 76, icon, "ic_launcher_round.png", folder);
            _bd.Background = null;
            await SaveIcon(216, 216, 108, icon, "ic_launcher_foreground.png", folder);

            folder = await root.CreateFolderAsync("mipmap-xxhdpi", CreationCollisionOption.OpenIfExists);
            _bd.Background = Res.主蓝;
            await SaveIcon(144, 144, 96, icon, "ic_launcher.png", folder);
            await SaveIcon(144, 144, 96, icon, "ic_launcher_round.png", folder);
            _bd.Background = null;
            await SaveIcon(324, 324, 162, icon, "ic_launcher_foreground.png", folder);

            folder = await root.CreateFolderAsync("mipmap-xxxhdpi", CreationCollisionOption.OpenIfExists);
            _bd.Background = Res.主蓝;
            await SaveIcon(192, 192, 130, icon, "ic_launcher.png", folder);
            await SaveIcon(192, 192, 130, icon, "ic_launcher_round.png", folder);
            _bd.Background = null;
            await SaveIcon(432, 432, 216, icon, "ic_launcher_foreground.png", folder);

            // 应用商店上架图标
            _bd.Background = Res.主蓝;
            await SaveIcon(216, 216, 152, icon, "华为.png", root);
            await SaveIcon(512, 512, 358, icon, "小米.png", root);

            Kit.Msg("生成成功，路径: " + folder.Path);
        }

        async void OnUwp(object sender, RoutedEventArgs e)
        {
            Icons icon = (Icons)_fv.Row["icon"];
            if (icon == Icons.None)
            {
                Kit.Msg("请选择图标");
                return;
            }

            _bd.Background = null;
            var folder = await OpenFolder($"{icon}_uwp");
            await SaveIcon(1240, 600, 480, icon, "SplashScreen.scale-200.png", folder);
            await SaveIcon(620, 300, 240, icon, "Wide310x150Logo.scale-200.png", folder);
            await SaveIcon(300, 300, 240, icon, "Square150x150Logo.scale-200.png", folder);
            await SaveIcon(88, 88, 70, icon, "Square44x44Logo.scale-200.png", folder);
            await SaveIcon(24, 24, 19, icon, "Square44x44Logo.targetsize-24_altform-unplated.png", folder);
            await SaveIcon(50, 50, 40, icon, "logo.png", folder);
            Kit.Msg("生成成功，路径: " + folder.Path);
        }

        async Task<StorageFolder> OpenFolder(string p_name)
        {
            try
            {
                return await KnownFolders.PicturesLibrary.CreateFolderAsync(p_name, CreationCollisionOption.OpenIfExists);
            }
            catch
            {
                Kit.Error("无访问图片库权限！");
                return null;
            }
        }

        async Task<StorageFile> SaveIcon(
            double p_width,
            double p_height,
            double p_fontSize,
            Icons p_icon,
            string p_fileName,
            StorageFolder p_folder)
        {
            _bd.Width = p_width;
            _bd.Height = p_height;
            _tb.FontSize = p_fontSize;
            _tb.Text = Res.GetIconChar(p_icon);

            RenderTargetBitmap bmp = new RenderTargetBitmap();
            await bmp.RenderAsync(_bd);
            var pixelBuffer = await bmp.GetPixelsAsync();
            StorageFile saveFile = null;
            try
            {
                saveFile = await p_folder.CreateFileAsync(p_fileName, CreationCollisionOption.ReplaceExisting);
            }
            catch
            {
                Kit.Error("创建图片文件失败！");
                return null;
            }

            using (var fileStream = await saveFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                var di = DisplayInformation.GetForCurrentView();
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, fileStream);
                float dpi = di.LogicalDpi;
                encoder.SetPixelData(
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Straight,
                    (uint)bmp.PixelWidth,
                    (uint)bmp.PixelHeight,
                    dpi,
                    dpi,
                    pixelBuffer.ToArray());
                await encoder.FlushAsync();
            }
            return saveFile;
        }

    }
}