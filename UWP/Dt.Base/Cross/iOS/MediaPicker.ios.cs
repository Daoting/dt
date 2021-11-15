#if IOS
#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-29 创建
******************************************************************************/
#endregion

#region 引用命名
using AVFoundation;
using Dt.Core;
using Foundation;
using GMImagePicker;
using Photos;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using UIKit;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// IOS版照片、视频选择
    /// </summary>
    public class MediaPickerForIOS
    {
        TaskCompletionSource<List<FileData>> _tcs;
        readonly NSFileManager _fileManager = new NSFileManager();

        public Task<List<FileData>> PickFiles(bool p_allowMultiple, PHAssetMediaType[] p_mediaType)
        {
            // 更多用法参见下文 ShowGMImagePicker
            var picker = new GMImagePickerController
            {
                AllowsMultipleSelection = p_allowMultiple,
                MediaTypes = p_mediaType,

                Title = "选择",
                CustomDoneButtonTitle = "完成",
                CustomCancelButtonTitle = "取消",
            };

            picker.CustomSmartCollections = new[]
            {
                PHAssetCollectionSubtype.SmartAlbumUserLibrary,
                PHAssetCollectionSubtype.AlbumRegular
            };

            picker.FinishedPickingAssets += FinishedPickingAssets;
            picker.Canceled += OnPickerCanceled;

            var window = UIApplication.SharedApplication.KeyWindow;
            var vc = window.RootViewController;
            while (vc.PresentedViewController != null)
            {
                vc = vc.PresentedViewController;
            }
            vc.PresentViewController(picker, true, null);

            _tcs = new TaskCompletionSource<List<FileData>>();
            return _tcs.Task;
        }

        async void FinishedPickingAssets(object sender, MultiAssetEventArgs args)
        {
            var picker = (GMImagePickerController)sender;
            picker.FinishedPickingAssets -= FinishedPickingAssets;
            picker.Canceled -= OnPickerCanceled;

            // synchronous: NO.异步。
            // a.deliveryMode: Opportunistic: 会返回多张图片
            //  1). ResizeMode.None: 先返回低清的缩略图，再返回原图大小
            //  2). ResizeMode.Fast: 先返回低清的缩略图，再返回的原图会使用targetSize来最优解码图片，获得的图片大小可能比targetSize大
            //  3). ResizeMode.Exact: 先返回低清的缩略图，再返回的原图会使用targetSize的高质量图

            // b.deliveryMode: HighQualityFormat: 只会返回一张高清图片
            //  1). ResizeMode.None: 返回的是原图大小
            //  2). ResizeMode.Fast: 当原图是压缩图时，会使用targetSize来最优解码图片，获得的图片大小可能比targetSize大
            //  3). ResizeMode.Exact: 解压和Fast一样，但是返回的是指定targetSize的高质量图

            // c.deliveryMode: FastFormat: 只会返回一张图片，并且可能是低清图
            //  1). ResizeMode.None: 返回一张低清图
            //  2). ResizeMode.Fast: 返回一张低清图
            //  3). ResizeMode.Exact: 返回一张低清图
            var options = new PHImageRequestOptions
            {
                NetworkAccessAllowed = true,
                Synchronous = false,
                DeliveryMode = PHImageRequestOptionsDeliveryMode.HighQualityFormat,
                ResizeMode = PHImageRequestOptionsResizeMode.Exact,
            };

            var tcs = new TaskCompletionSource<object>();
            bool completed = false;
            List<FileData> result = new List<FileData>();
            for (var i = 0; i < args.Assets.Length; i++)
            {
                var asset = args.Assets[i];
                switch (asset.MediaType)
                {
                    case PHAssetMediaType.Video:
                        // 未测
                        PHImageManager.DefaultManager.RequestImageForAsset(
                            asset,
                            new SizeF(FileData.ThumbSize, FileData.ThumbSize),
                            PHImageContentMode.AspectFit,
                            options,
                            async (img, info) =>
                            {
                                // 获取路径，麻烦
                                TaskCompletionSource<NSUrl> tcsUrl = new TaskCompletionSource<NSUrl>();
                                var vOptions = new PHVideoRequestOptions
                                {
                                    NetworkAccessAllowed = true,
                                    Version = PHVideoRequestOptionsVersion.Original,
                                    DeliveryMode = PHVideoRequestOptionsDeliveryMode.FastFormat,
                                };
                                PHImageManager.DefaultManager.RequestAvAsset(
                                    asset,
                                    vOptions,
                                    (avAsset, audioMix, vInfo) =>
                                    {
                                        if (avAsset is AVUrlAsset avUrl)
                                            tcsUrl.TrySetResult(avUrl.Url);
                                        else
                                            tcsUrl.TrySetResult(null);
                                    });
                                NSUrl url = await tcsUrl.Task;

                                // 生成文件描述和缩略图
                                var fd = ParseUrl(url);
                                fd.Desc = $"{asset.PixelWidth} x {asset.PixelHeight} ({fd.Ext.TrimStart('.')})";
                                fd.ThumbPath = Path.Combine(Kit.CachePath, Kit.NewGuid + "-t.jpg");
                                img.AsJPEG().Save(fd.ThumbPath, true);
                                result.Add(fd);

                                if (args.Assets.Length == result.Count && !completed)
                                {
                                    completed = true;
                                    tcs.TrySetResult(null);
                                }
                            });

                        break;
                    default:
                        PHImageManager.DefaultManager.RequestImageForAsset(
                            asset,
                            new SizeF(FileData.ThumbSize, FileData.ThumbSize),
                            PHImageContentMode.AspectFit,
                            options,
                            async (img, info) =>
                            {
                                // 获取路径，麻烦
                                TaskCompletionSource<NSUrl> tcsUrl = new TaskCompletionSource<NSUrl>();
                                asset.RequestContentEditingInput(new PHContentEditingInputRequestOptions(), (input, _) =>
                                {
                                    tcsUrl.TrySetResult(input.FullSizeImageUrl);
                                });
                                NSUrl url = await tcsUrl.Task;

                                // 生成文件描述和缩略图
                                var fd = ParseUrl(url);
                                fd.Desc = $"{asset.PixelWidth} x {asset.PixelHeight} ({fd.Ext.TrimStart('.')})";
                                if (asset.PixelWidth > FileData.ThumbSize || asset.PixelHeight > FileData.ThumbSize)
                                {
                                    fd.ThumbPath = Path.Combine(Kit.CachePath, Kit.NewGuid + "-t.jpg");
                                    img.AsJPEG().Save(fd.ThumbPath, true);
                                }
                                result.Add(fd);

                                if (args.Assets.Length == result.Count && !completed)
                                {
                                    completed = true;
                                    tcs.TrySetResult(null);
                                }
                            });

                        break;
                }
            }

            // 用临时tcs等待比直接在回调方法中用_tcs速度大幅提高！！！
            await tcs.Task;
            _tcs.TrySetResult(result);
        }

        void OnPickerCanceled(object sender, EventArgs e)
        {
            var picker = (GMImagePickerController)sender;
            picker.FinishedPickingAssets -= FinishedPickingAssets;
            picker.Canceled -= OnPickerCanceled;

            _tcs.SetResult(null);
        }

        FileData ParseUrl(NSUrl p_url)
        {
            p_url.StartAccessingSecurityScopedResource();

            var doc = new UIDocument(p_url);
            string name = doc.LocalizedName;
            string path = doc.FileUrl?.Path;
            // iCloud drive can return null for LocalizedName.
            if (name == null && path != null)
                name = Path.GetFileName(path);
            var fd = new FileData(path, name, _fileManager.GetAttributes(path).Size.Value);

            p_url.StopAccessingSecurityScopedResource();
            return fd;
        }

        // 参见 https://raw.githubusercontent.com/roycornelissen/GMImagePicker.Xamarin/master/src/GMPhotoPicker.Xamarin/ViewController.cs
        //async partial void ShowGMImagePicker(NSObject sender)
        //{
        //    var picker = new GMImagePickerController
        //    {
        //        Title = "Custom Title",
        //        CustomDoneButtonTitle = "Finished",
        //        CustomCancelButtonTitle = "Nope",
        //        CustomNavigationBarPrompt = "Take a new photo or select an existing one!",
        //        ColsInPortrait = 3,
        //        ColsInLandscape = 5,
        //        MinimumInteritemSpacing = 2.0f,
        //        DisplaySelectionInfoToolbar = true,
        //        AllowsMultipleSelection = true,
        //        ShowCameraButton = true,
        //        AutoSelectCameraImages = true,
        //        AllowsEditingCameraImages = true,
        //        ModalPresentationStyle = UIModalPresentationStyle.Popover,
        //        MediaTypes = new[] { PHAssetMediaType.Image },

        //        // Other customizations to play with:
        //        //AdditionalToolbarItems = new UIBarButtonItem[] { new UIBarButtonItem(UIBarButtonSystemItem.Bookmarks), new UIBarButtonItem("Custom", UIBarButtonItemStyle.Bordered, (s, e) => { Console.WriteLine("test"); })},
        //        //GridSortOrder = SortOrder.Descending,
        //        //ConfirmSingleSelection = true,
        //        //ConfirmSingleSelectionPrompt = "Do you want to select the image you have chosen?",
        //        //PickerBackgroundColor = UIColor.Black,
        //        //PickerTextColor = UIColor.White,
        //        //ToolbarBarTintColor = UIColor.Red,
        //        //ToolbarBackgroundColor = UIColor.Yellow,
        //        //ToolbarTextColor = UIColor.White,
        //        //ToolbarTintColor = UIColor.Red,
        //        //NavigationBarBackgroundColor = UIColor.DarkGray,
        //        //NavigationBarBarTintColor = UIColor.DarkGray,
        //        //NavigationBarTextColor = UIColor.White,
        //        //NavigationBarTintColor = UIColor.Red,
        //        //CameraButtonTintColor = UIColor.Red,
        //        //PickerFontName = "Verdana",
        //        //PickerBoldFontName = "Verdana-Bold",
        //        //PickerFontNormalSize = 14.0f,
        //        //PickerFontHeaderSize = 17.0f,
        //        //PickerStatusBarStyle = UIStatusBarStyle.LightContent,
        //        //UseCustomFontForNavigationBar = true,
        //    };

        //    // You can limit which galleries are available to browse through
        //    picker.CustomSmartCollections = new[] {
        //        PHAssetCollectionSubtype.SmartAlbumUserLibrary,
        //        PHAssetCollectionSubtype.AlbumRegular
        //    };

        //    if (_preselectedAssets != null)
        //    {
        //        foreach (var asset in _preselectedAssets)
        //        {
        //            picker.SelectedAssets.Add(asset);
        //        }
        //    }

        //    // Event handling
        //    picker.FinishedPickingAssets += Picker_FinishedPickingAssets;
        //    picker.Canceled += Picker_Canceled;

        //    // Other events to implement in order to influence selection behavior:
        //    // Set EventArgs::Cancel flag to true in order to prevent the action from happening
        //    picker.ShouldDeselectAsset += (s, e) => { /* allow deselection of (mandatory) assets */ };
        //    picker.ShouldEnableAsset += (s, e) => { /* determine if a specific asset should be enabled */ };
        //    picker.ShouldHighlightAsset += (s, e) => { /* determine if a specific asset should be highlighted */ };
        //    picker.ShouldShowAsset += (s, e) => { /* determine if a specific asset should be displayed */ };
        //    picker.ShouldSelectAsset += (s, e) => { /* determine if a specific asset can be selected */ };

        //    picker.AssetSelected += (s, e) => { /* keep track of individual asset selection */ };
        //    picker.AssetDeselected += (s, e) => { /* keep track of individual asset de-selection */ };

        //    // GMImagePicker can be treated as a PopOver as well:
        //    var popPC = picker.PopoverPresentationController;
        //    popPC.PermittedArrowDirections = UIPopoverArrowDirection.Any;
        //    popPC.SourceView = gmImagePickerButton;
        //    popPC.SourceRect = gmImagePickerButton.Bounds;
        //    //popPC.BackgroundColor = UIColor.Black;

        //    await PresentViewControllerAsync(picker, true);
        //}

        /* 使用原生UIDocumentPickerViewController只支持单选
        //public Task<List<FileData>> PickFiles(bool p_allowMultiple)
        //{
        //    _imagePicker = new UIImagePickerController
        //    {
        //        SourceType = UIImagePickerControllerSourceType.PhotoLibrary,
        //        MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.PhotoLibrary)
        //    };

        //    // Set event handlers
        //    _imagePicker.FinishedPickingMedia += OnImagePickerFinishedPickingMedia;
        //    _imagePicker.Canceled += OnImagePickerCancelled;

        //    // Present UIImagePickerController;
        //    UIWindow window = UIApplication.SharedApplication.KeyWindow;
        //    var viewController = window.RootViewController;
        //    viewController.PresentModalViewController(_imagePicker, true);

        //    // Return Task object
        //    _tcs = new TaskCompletionSource<List<FileData>>();
        //    return _tcs.Task;
        //}

        //void OnImagePickerFinishedPickingMedia(object sender, UIImagePickerMediaPickedEventArgs args)
        //{
        //    UIImage image = args.EditedImage ?? args.OriginalImage;

        //    if (image != null)
        //    {
        //        // Convert UIImage to .NET Stream object
        //        NSData data = image.AsJPEG(1);
        //        Stream stream = data.AsStream();

        //        UnregisterEventHandlers();

        //        // Set the Stream as the completion of the Task
        //        _tcs.SetResult(new List<FileData> { });
        //    }
        //    else
        //    {
        //        UnregisterEventHandlers();
        //        _tcs.SetResult(null);
        //    }
        //    _imagePicker.DismissModalViewController(true);
        //}

        //void OnImagePickerCancelled(object sender, EventArgs args)
        //{
        //    UnregisterEventHandlers();
        //    _tcs.SetResult(null);
        //    _imagePicker.DismissModalViewController(true);
        //}

        //void UnregisterEventHandlers()
        //{
        //    _imagePicker.FinishedPickingMedia -= OnImagePickerFinishedPickingMedia;
        //    _imagePicker.Canceled -= OnImagePickerCancelled;
        //}
        */
    }
}
#endif
