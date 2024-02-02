﻿#if ANDROID
#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Android.Content;
using Android.Database;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Webkit;
using Java.IO;
using System;
using System.IO;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// Android I/O utility functions
    /// </summary>
    public class IOUtil
    {
        /// <summary>
        /// Tries to find a file system path for given Uri. Note that this isn't always possible,
        /// since the content referenced by the Uri may not be stored on a file system, but is
        /// returned by the responsible app by using a ContentProvider. In this case, the method
        /// returns null, and access to the content is only possible by opening a stream.
        /// </summary>
        /// <param name="context">Android context to access content resolver</param>
        /// <param name="uri">Uri to use</param>
        /// <returns>full file system path, or null</returns>
        public static string GetPath(Context context, Android.Net.Uri uri)
        {
            // DocumentProvider
            if (DocumentsContract.IsDocumentUri(context, uri))
            {
                // ExternalStorageProvider
                if (IsExternalStorageDocument(uri))
                {
                    var docId = DocumentsContract.GetDocumentId(uri);
                    string[] split = docId.Split(':');
                    var type = split[0];

                    if ("primary".Equals(type, StringComparison.OrdinalIgnoreCase))
                    {
                        return context.GetExternalFilesDir(null) + "/" + split[1];
                    }

                    // TODO handle non-primary volumes
                }
                else if (IsDownloadsDocument(uri))
                {
                    // android 11 使用原来的方法已无法获取，采用先获取文件名，再合成路径
                    string fileName = GetFileName(context, uri);
                    if (!string.IsNullOrEmpty(fileName))
                        return System.IO.Path.Combine(GetDownloadsPath(), fileName);

                    // DownloadsProvider
                    string id = DocumentsContract.GetDocumentId(uri);

                    if (!string.IsNullOrEmpty(id) && id.StartsWith("raw:"))
                    {
                        return id.Substring(4);
                    }

                    string[] contentUriPrefixesToTry = new string[]
                    {
                        "content://downloads/public_downloads",
                        "content://downloads/my_downloads",
                        "content://downloads/all_downloads",
                    };

                    foreach (string contentUriPrefix in contentUriPrefixesToTry)
                    {
                        Android.Net.Uri contentUri = ContentUris.WithAppendedId(
                            Android.Net.Uri.Parse(contentUriPrefix), long.Parse(id));

                        try
                        {
                            var path = GetDataColumn(context, contentUri, null, null);
                            if (path != null)
                            {
                                return path;
                            }
                        }
                        catch (Exception)
                        {
                            // ignore exception; path can't be retrieved using ContentResolver
                        }
                    }
                }
                else if (IsMediaDocument(uri))
                {
                    // MediaProvider
                    var docId = DocumentsContract.GetDocumentId(uri);
                    string[] split = docId.Split(':');
                    var type = split[0];

                    Android.Net.Uri contentUri = null;
                    if ("image".Equals(type))
                    {
                        contentUri = MediaStore.Images.Media.ExternalContentUri;
                    }
                    else if ("video".Equals(type))
                    {
                        contentUri = MediaStore.Video.Media.ExternalContentUri;
                    }
                    else if ("audio".Equals(type))
                    {
                        contentUri = MediaStore.Audio.Media.ExternalContentUri;
                    }

                    var selection = "_id=?";
                    var selectionArgs = new string[]
                    {
                        split[1]
                    };

                    return GetDataColumn(context, contentUri, selection, selectionArgs);
                }
            }

            // MediaStore (and general)
            if (IsMediaStore(uri.Scheme))
            {
                    // 判断是否是google相册图片
                if (IsGooglePhotosUri(uri))
                    return uri.LastPathSegment;

                    // 判断是否是Google相册图片
                if (IsGooglePlayPhotosUri(uri))
                    return GetImageUrlWithAuthority(context, uri);

                // 其他类似于media这样的图片，和android4.4以下获取图片path方法类似
                return GetDataColumn(context, uri, null, null);
            }
            
            if ("file".Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                return uri.Path;
            }

            return null;
        }

        /// <summary>
        /// Checks if the scheme part of the URL matches the content:// scheme
        /// </summary>
        /// <param name="scheme">scheme part of URL</param>
        /// <returns>true when it matches, false when not</returns>
        public static bool IsMediaStore(string scheme)
        {
            return scheme.StartsWith("content");
        }

        /// <summary>
        /// Returns the "data" column of an Uri from the content resolver.
        /// </summary>
        /// <param name="context">Android context to access content resolver</param>
        /// <param name="uri">content Uri</param>
        /// <param name="selection">selection 'where' clause, or null</param>
        /// <param name="selectionArgs">selection arguments, or null</param>
        /// <returns>data column text, or null when query contained no data column</returns>
        public static string GetDataColumn(Context context, Android.Net.Uri uri, string selection, string[] selectionArgs)
        {
            ICursor cursor = null;
            // v29 弃用
            //string column = MediaStore.Files.FileColumns.Data;
            string column = "_data";
            string[] projection = { column };

            try
            {
                cursor = context.ContentResolver.Query(uri, projection, selection, selectionArgs, null);
                if (cursor != null && cursor.MoveToFirst())
                {
                    int column_index = cursor.GetColumnIndex(column);
                    if (column_index == -1)
                    {
                        return null;
                    }

                    string path = cursor.GetString(column_index);

                    // When the path has no root (i.e. is relative), better return null so that
                    // the content uri is used and the file contents can be read
                    if (path != null && !System.IO.Path.IsPathRooted(path))
                    {
                        return null;
                    }

                    return path;
                }
            }
            finally
            {
                if (cursor != null)
                {
                    cursor.Close();
                }
            }

            return null;
        }

        public static bool IsGooglePhotosUri(Android.Net.Uri uri)
        {
            return "com.google.android.apps.photos.content".Equals(uri.Authority);
        }

        public static bool IsGooglePlayPhotosUri(Android.Net.Uri uri)
        {
            return "com.google.android.apps.photos.contentprovider".Equals(uri.Authority);
        }

        public static string GetImageUrlWithAuthority(Context context, Android.Net.Uri uri)
        {
            if (uri.Authority != null)
            {
                Stream stream = null;
                try
                {
                    stream = context.ContentResolver.OpenInputStream(uri);
                    Bitmap bmp = BitmapFactory.DecodeStream(stream);

                    MemoryStream ms = new MemoryStream();
                    bmp.Compress(Bitmap.CompressFormat.Jpeg, 100, ms);
#pragma warning disable CA1422 // 类型或成员已过时
                    string path = MediaStore.Images.Media.InsertImage(context.ContentResolver, bmp, null, null);
#pragma warning restore CA1422 // 类型或成员已过时
                    return GetDataColumn(context, Android.Net.Uri.Parse(path), null, null);
                }
                catch { }
                finally
                {
                    if (stream != null)
                        stream.Close();
                }
            }
            return null;
        }

        public static string GetFileName(Context context, Android.Net.Uri uri)
        {
            var mimeType = context.ContentResolver.GetType(uri);
            string filename = null;
            if (mimeType == null)
            {
                Java.IO.File file = new Java.IO.File(uri.ToString());
                filename = file.Name;
            }
            else
            {
                ICursor cursor = null;
                try
                {
                    cursor = context.ContentResolver.Query(uri, null, null, null);
                    if (cursor != null && cursor.MoveToFirst())
                    {
                        int nameIndex = cursor.GetColumnIndex("_display_name");
                        if (nameIndex > -1)
                            filename = cursor.GetString(nameIndex);
                    }
                }
                finally
                {
                    if (cursor != null)
                        cursor.Close();
                }
            }
            return filename;
        }

        /// <summary>
        /// Returns if the given Uri is an ExternalStorageProvider Uri
        /// </summary>
        /// <param name="uri">the Uri to check</param>
        /// <returns>whether the Uri authority is an ExternalStorageProvider</returns>
        public static bool IsExternalStorageDocument(Android.Net.Uri uri)
        {
            return "com.android.externalstorage.documents".Equals(uri.Authority);
        }

        /// <summary>
        /// Returns if the given Uri is a DownloadsProvider Uri
        /// </summary>
        /// <param name="uri">the Uri to check</param>
        /// <returns>whether the Uri authority is a DownloadsProvider</returns>
        public static bool IsDownloadsDocument(Android.Net.Uri uri)
        {
            return "com.android.providers.downloads.documents".Equals(uri.Authority);
        }

        /// <summary>
        /// Returns if the given Uri is a MediaProvider Uri
        /// </summary>
        /// <param name="uri">the Uri to check</param>
        /// <returns>whether the Uri authority is a MediaProvider</returns>
        public static bool IsMediaDocument(Android.Net.Uri uri)
        {
            return "com.android.providers.media.documents".Equals(uri.Authority);
        }

        /// <summary>
        /// Returns MIME type for given Url
        /// </summary>
        /// <param name="url">Url to check</param>
        /// <returns>MIME type, or null when none can be determined</returns>
        public static string GetMimeType(string url)
        {
            string type = null;
            var extension = MimeTypeMap.GetFileExtensionFromUrl(url);

            if (extension != null)
            {
                type = MimeTypeMap.Singleton.GetMimeTypeFromExtension(extension);
            }

            return type;
        }

        /// <summary>
        /// 公共下载目录
        /// </summary>
        /// <returns></returns>
        public static string GetDownloadsPath()
        {
            return System.IO.Path.Combine(GetRootPath(), Android.OS.Environment.DirectoryDownloads);
        }

        /// <summary>
        /// 公共照片目录
        /// </summary>
        /// <returns></returns>
        public static string GetPicturesPath()
        {
            return System.IO.Path.Combine(GetRootPath(), Android.OS.Environment.DirectoryDcim);
        }

        /// <summary>
        /// 公共视频目录
        /// </summary>
        /// <returns></returns>
        public static string GetMoviesPath()
        {
            return System.IO.Path.Combine(GetRootPath(), Android.OS.Environment.DirectoryMovies);
        }

        /// <summary>
        /// 公共音频目录
        /// </summary>
        /// <returns></returns>
        public static string GetMusicPath()
        {
            return System.IO.Path.Combine(GetRootPath(), Android.OS.Environment.DirectoryMusic);
        }

        public static string GetRootPath()
        {
            var root = Android.App.Application.Context.GetExternalFilesDir(null).AbsolutePath;
            int index = root.IndexOf("/Android/");
            if (index > 0)
                return root.Substring(0, index);
            return root;
        }
    }
}
#endif