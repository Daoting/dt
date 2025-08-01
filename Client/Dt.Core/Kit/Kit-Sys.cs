﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: AtSys合并到Kit
* 日志: 2021-06-08 改名
******************************************************************************/
#endregion

#region 引用命名
using Windows.Storage;
using Windows.UI.ViewManagement;
#if WIN || ANDROID || IOS
using Microsoft.Maui.Devices;
#endif
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 系统相关管理类
    /// </summary>
    public partial class Kit
    {
        #region 系统基础
        /// <summary>
        /// 获取系统是否采用手机的UI模式
        /// </summary>
        public static bool IsPhoneUI
#if IOS || ANDROID
            => true;
#else
        { get; internal set; }
#endif

        /// <summary>
        /// 获取应用类型
        /// </summary>
        public static AppType AppType
        {
            get
            {
#if WIN
                return AppType.Windows;
#elif IOS
                return AppType.iOS;
#elif ANDROID
                return AppType.Android;
#elif WASM
                return AppType.Wasm;
#else
                return AppType.Desktop;
#endif
            }
        }

        /// <summary>
        /// 获取系统是否为触摸模式
        /// </summary>
        public static bool IsTouchMode
        {
            get
            {
#if WIN
                return UIViewSettings.GetForCurrentView().UserInteractionMode == UserInteractionMode.Touch;
#else
                return true;
#endif
            }
        }

        /// <summary>
        /// 获取系统描述信息
        /// </summary>
        public static string LocalInfo
        {
            get
            {
#if WIN || ANDROID || IOS
                return $"DevName: {DeviceInfo.Name}\nPlatform: {DeviceInfo.Platform}\nOsVer: {DeviceInfo.VersionString}\nAppType: {AppType}\nDtVer: {typeof(Kit).Assembly.GetName().Version.ToString(3)}\n";
#else
                return $"AppType: {AppType}\nDtVer: {typeof(Kit).Assembly.GetName().Version.ToString(3)}\n";
#endif
            }
        }
        #endregion

        #region 当前时间
        /// <summary>
        /// 获取服务器端当前时间，根据时差计算所得
        /// </summary>
        public static DateTime Now
        {
            get { return DateTime.Now + _timeSpan; }
            set { _timeSpan = value - DateTime.Now; }
        }

        static TimeSpan _timeSpan;
        #endregion

        #region 本地文件
        /* 在uno保存文件时只支持以下路径，形如：
         * LocalFolder
         * uwp：C:\Users\hdt\AppData\Local\Packages\4e169f82-ed49-494f-8c23-7dab11228222_dm57000t4aqw0\LocalState
         * android：/data/user/0/App.Droid/files
         * ios：/Users/usrname/Library/Developer/CoreSimulator/Devices/xxx/data/Containers/Data/Application/yyy/Library/Data
         * wasm：/local
         * 
         * RoamingFolder
         * android：/data/user/0/App.Droid/files/.config
         * 
         * SharedLocalFolder
         * android：/data/user/0/App.Droid/files/.local/share
         * 
         * TemporaryFolder 缓存路径，app关闭时删除，不可用于保存文件！
         */

        /// <summary>
        /// 本地文件的根路径
        /// uwp：C:\Users\...\LocalState
        /// android：/data/user/0/App.Droid/files
        /// ios：/Users/usrname/Library/Developer/CoreSimulator/Devices/xxx/data/Containers/Data/Application/yyy/Library/Data
        /// wasm：/local
        /// </summary>
        public static string RootPath
        {
            get { return ApplicationData.Current.LocalFolder.Path; }
        }

        /// <summary>
        /// 本地缓存文件的存放路径
        /// uwp：C:\Users\...\LocalState\.doc
        /// android：/data/user/0/App.Droid/files/.doc
        /// ios：/Users/usrname/Library/Developer/CoreSimulator/Devices/xxx/data/Containers/Data/Application/yyy/Library/Data/.doc
        /// wasm：/local/.doc
        /// </summary>
        public static string CachePath { get; } = Path.Combine(ApplicationData.Current.LocalFolder.Path, ".doc");

        /// <summary>
        /// 本地sqlite数据文件的存放路径
        /// uwp：C:\Users\...\LocalState\.data
        /// android：/data/user/0/App.Droid/files/.data
        /// ios：/Users/usrname/Library/Developer/CoreSimulator/Devices/xxx/data/Containers/Data/Application/yyy/Library/Data/.data
        /// wasm：/local/.doc
        /// </summary>
        public static string DataPath { get; } = Path.Combine(ApplicationData.Current.LocalFolder.Path, ".data");

        /// <summary>
        /// 清空所有存放在.doc路径的缓存文件
        /// </summary>
        public static void ClearCacheFiles()
        {
            try
            {
                if (Directory.Exists(CachePath))
                    Directory.Delete(CachePath, true);
                Directory.CreateDirectory(CachePath);
            }
            catch { }
        }

        /// <summary>
        /// 删除存放在.doc路径的本地文件
        /// </summary>
        /// <param name="p_fileName">文件名</param>
        public static void DeleteCacheFile(string p_fileName)
        {
            try
            {
                File.Delete(Path.Combine(CachePath, p_fileName));
            }
            catch { }
        }
        #endregion
    }
}