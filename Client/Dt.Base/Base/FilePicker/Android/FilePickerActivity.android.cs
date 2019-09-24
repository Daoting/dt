#if ANDROID
#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// Activity that is shown in order to start Android file picking using ActionGetContent
    /// intent.
    /// </summary>
    [Activity(ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    [Preserve(AllMembers = true)]
    public class FilePickerActivity : Activity
    {
        /// <summary>
        /// Intent Extra constant to pass list of allowed types to FilePicker activity.
        /// </summary>
        public const string ExtraAllowedTypes = "EXTRA_ALLOWED_TYPES";

        /// <summary>
        /// This variable gets passed when the request for the permission to access storage
        /// gets send and then gets again read whne the request gets answered.
        /// </summary>
        const int RequestStorage = 1;

        /// <summary>
        /// Android context to be used for opening file picker
        /// </summary>
        Context _context;

        /// <summary>
        /// Called when activity is about to be created; immediately starts file picker intent
        /// when permission is available, otherwise requests permission on API level >= 23 or
        /// throws an error if the API level is below.
        /// </summary>
        /// <param name="savedInstanceState">saved instance state; unused</param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _context = Application.Context;

            if (_context.PackageManager.CheckPermission(
                Manifest.Permission.ReadExternalStorage,
                _context.PackageName) == Permission.Granted)
            {
                StartPicker();
            }
            else
            {
                if ((int)Build.VERSION.SdkInt >= 23)
                {
                    RequestPermissions(new string[] { Manifest.Permission.ReadExternalStorage }, RequestStorage);
                }
                else
                {
                    throw new InvalidOperationException(
                        "Android permission READ_EXTERNAL_STORAGE is missing and API level lower than 23, so it can't be requested");
                }
            }
        }

        /// <summary>
        /// Receives the answer from the dialog that asks for the READ_EXTERNAL_STORAGE permission
        /// and starts the FilePicker if it's granted or otherwise closes this activity.
        /// </summary>
        /// <param name="requestCode">requestCode; shows us that the dialog we requested is responsible for this answer</param>
        /// <param name="permissions">permissions; unused</param>
        /// <param name="grantResults">grantResults; contains the result of the dialog to request the permission</param>
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            if (requestCode == RequestStorage)
            {
                if (grantResults.Length > 0 &&
                    grantResults[0] == Permission.Granted)
                {
                    StartPicker();
                }
                else
                {
                    OnFilePicked(null);
                    Finish();
                }
            }
        }

        /// <summary>
        /// Sends an intent to start the FilePicker
        /// </summary>
        void StartPicker()
        {
            var intent = new Intent(Intent.ActionGetContent);

            intent.SetType("*/*");

            string[] allowedTypes = Intent
                .GetStringArrayExtra(ExtraAllowedTypes)?
                .Where(o => !string.IsNullOrEmpty(o) && o.Contains("/")).ToArray();
            if (allowedTypes != null && allowedTypes.Any())
            {
                intent.PutExtra(Intent.ExtraMimeTypes, allowedTypes);
            }

            // 多选
            if (Intent.GetBooleanExtra(Intent.ExtraAllowMultiple, false))
                intent.PutExtra(Intent.ExtraAllowMultiple, true);

            intent.AddCategory(Intent.CategoryOpenable);
            try
            {
                StartActivityForResult(Intent.CreateChooser(intent, "选择文件"), 0);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        }

        /// <summary>
        /// Called when activity started with StartActivityForResult() returns.
        /// </summary>
        /// <param name="requestCode">request code used in StartActivityForResult()</param>
        /// <param name="resultCode">result code</param>
        /// <param name="data">intent data from file picking</param>
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode != Result.Ok || data == null)
            {
                // Notify user file picking was cancelled.
                OnFilePicked(null);
                Finish();
                return;
            }

            try
            {
                List<FileData> ls = new List<FileData>();
                if (data.Data != null)
                {
                    // 单选
                    var uri = data.Data;
                    var filePath = IOUtil.GetPath(_context, uri);
                    if (string.IsNullOrEmpty(filePath))
                        filePath = IOUtil.IsMediaStore(uri.Scheme) ? uri.ToString() : uri.Path;
                    var fileName = GetFileName(_context, uri);
                    ls.Add(new FileData(filePath, fileName));
                }
                else if (data.ClipData != null)
                {
                    // 多选
                    for (int i = 0; i < data.ClipData.ItemCount; i++)
                    {
                        var uri = data.ClipData.GetItemAt(i).Uri;
                        var filePath = IOUtil.GetPath(_context, uri);
                        var fileName = GetFileName(_context, uri);
                        ls.Add(new FileData(filePath, fileName));
                    }
                }
                else
                    throw new Exception("File picking returned no valid data");

                OnFilePicked(ls);
            }
            catch (Exception readEx)
            {
                System.Diagnostics.Debug.Write(readEx);
                OnFilePicked(null);
            }
            finally
            {
                Finish();
            }
        }

        /// <summary>
        /// Retrieves file name part from given Uri
        /// </summary>
        /// <param name="context">Android context to access content resolver</param>
        /// <param name="uri">Uri to get filename for</param>
        /// <returns>file name part</returns>
        string GetFileName(Context context, Android.Net.Uri uri)
        {
            string[] projection = { MediaStore.MediaColumns.DisplayName };

            var resolver = context.ContentResolver;
            var name = string.Empty;
            var metaCursor = resolver.Query(uri, projection, null, null, null);

            if (metaCursor != null)
            {
                try
                {
                    if (metaCursor.MoveToFirst())
                    {
                        name = metaCursor.GetString(0);
                    }
                }
                finally
                {
                    metaCursor.Close();
                }
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                return name;
            }
            else
            {
                return System.IO.Path.GetFileName(WebUtility.UrlDecode(uri.ToString()));
            }
        }

        internal static event EventHandler<List<FileData>> FilePicked;

        static void OnFilePicked(List<FileData> args)
        {
            FilePicked?.Invoke(null, args);
        }
    }
}
#endif