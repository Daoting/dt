#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2015-05-13 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Globalization;
using System.Text;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// 定义来自于Dt.Base，和Res相同
    /// </summary>
    public static class BrushRes
    {
        // 全局资源字典
        readonly static ResourceDictionary _dict = Application.Current.Resources;

        #region 标准颜色画刷
        static SolidColorBrush _transparentBrush;
        static SolidColorBrush _blackBrush;
        static SolidColorBrush _blueBrush;
        static SolidColorBrush _brownBrush;
        static SolidColorBrush _cyanBrush;
        static SolidColorBrush _darkGrayBrush;
        static SolidColorBrush _grayBrush;
        static SolidColorBrush _greenBrush;
        static SolidColorBrush _lightGrayBrush;
        static SolidColorBrush _magentaBrush;
        static SolidColorBrush _orangeBrush;
        static SolidColorBrush _purpleBrush;
        static SolidColorBrush _redBrush;
        static SolidColorBrush _whiteBrush;
        static SolidColorBrush _yellowBrush;

        /// <summary>
        /// 透明色
        /// </summary>
        public static SolidColorBrush TransparentBrush
        {
            get
            {
                if (_transparentBrush == null)
                    _transparentBrush = new SolidColorBrush(Colors.Transparent);
                return _transparentBrush;
            }
        }

        /// <summary>
        /// 黑色
        /// </summary>
        public static SolidColorBrush BlackBrush
        {
            get
            {
                if (_blackBrush == null)
                    _blackBrush = new SolidColorBrush(Colors.Black);
                return _blackBrush;
            }
        }

        /// <summary>
        /// 蓝色
        /// </summary>
        public static SolidColorBrush BlueBrush
        {
            get
            {
                if (_blueBrush == null)
                    _blueBrush = new SolidColorBrush(Colors.Blue);
                return _blueBrush;
            }
        }

        /// <summary>
        /// 深褐色
        /// </summary>
        public static SolidColorBrush BrownBrush
        {
            get
            {
                if (_brownBrush == null)
                    _brownBrush = new SolidColorBrush(Colors.Brown);
                return _brownBrush;
            }
        }

        /// <summary>
        /// 青色
        /// </summary>
        public static SolidColorBrush CyanBrush
        {
            get
            {
                if (_cyanBrush == null)
                    _cyanBrush = new SolidColorBrush(Colors.Cyan);
                return _cyanBrush;
            }
        }

        /// <summary>
        /// 深灰
        /// </summary>
        public static SolidColorBrush DarkGrayBrush
        {
            get
            {
                if (_darkGrayBrush == null)
                    _darkGrayBrush = new SolidColorBrush(Colors.DarkGray);
                return _darkGrayBrush;
            }
        }

        /// <summary>
        /// 灰色
        /// </summary>
        public static SolidColorBrush GrayBrush
        {
            get
            {
                if (_grayBrush == null)
                    _grayBrush = new SolidColorBrush(Colors.Gray);
                return _grayBrush;
            }
        }

        /// <summary>
        /// 绿色
        /// </summary>
        public static SolidColorBrush GreenBrush
        {
            get
            {
                if (_greenBrush == null)
                    _greenBrush = new SolidColorBrush(Colors.Green);
                return _greenBrush;
            }
        }

        /// <summary>
        /// 浅灰
        /// </summary>
        public static SolidColorBrush LightGrayBrush
        {
            get
            {
                if (_lightGrayBrush == null)
                    _lightGrayBrush = new SolidColorBrush(Colors.LightGray);
                return _lightGrayBrush;
            }
        }

        /// <summary>
        /// 品红
        /// </summary>
        public static SolidColorBrush MagentaBrush
        {
            get
            {
                if (_magentaBrush == null)
                    _magentaBrush = new SolidColorBrush(Colors.Magenta);
                return _magentaBrush;
            }
        }

        /// <summary>
        /// 桔色
        /// </summary>
        public static SolidColorBrush OrangeBrush
        {
            get
            {
                if (_orangeBrush == null)
                    _orangeBrush = new SolidColorBrush(Colors.Orange);
                return _orangeBrush;
            }
        }

        /// <summary>
        /// 紫色
        /// </summary>
        public static SolidColorBrush PurpleBrush
        {
            get
            {
                if (_purpleBrush == null)
                    _purpleBrush = new SolidColorBrush(Colors.Purple);
                return _purpleBrush;
            }
        }

        /// <summary>
        /// 红色
        /// </summary>
        public static SolidColorBrush RedBrush
        {
            get
            {
                if (_redBrush == null)
                    _redBrush = new SolidColorBrush(Colors.Red);
                return _redBrush;
            }
        }

        /// <summary>
        /// 白色
        /// </summary>
        public static SolidColorBrush WhiteBrush
        {
            get
            {
                if (_whiteBrush == null)
                    _whiteBrush = new SolidColorBrush(Colors.White);
                return _whiteBrush;
            }
        }

        /// <summary>
        /// 黄色
        /// </summary>
        public static SolidColorBrush YellowBrush
        {
            get
            {
                if (_yellowBrush == null)
                    _yellowBrush = new SolidColorBrush(Colors.Yellow);
                return _yellowBrush;
            }
        }

        /// <summary>
        /// 根据标准颜色的名称获取画刷
        /// </summary>
        /// <param name="p_colorName">颜色的名称</param>
        /// <returns>返回画刷</returns>
        public static SolidColorBrush GetBrushByName(string p_colorName)
        {
            if (string.IsNullOrEmpty(p_colorName))
                return BlackBrush;

            switch (p_colorName.ToLower())
            {
                case "black":
                    return BlackBrush;
                case "blue":
                    return BlueBrush;
                case "brown":
                    return BrownBrush;
                case "cyan":
                    return CyanBrush;
                case "darkGray":
                    return DarkGrayBrush;
                case "gray":
                    return GrayBrush;
                case "green":
                    return GreenBrush;
                case "lightGray":
                    return LightGrayBrush;
                case "magenta":
                    return MagentaBrush;
                case "orange":
                    return OrangeBrush;
                case "purple":
                    return PurpleBrush;
                case "red":
                    return RedBrush;
                case "white":
                    return WhiteBrush;
                case "yellow":
                    return YellowBrush;

            }
            return BlackBrush;
        }
        #endregion

        #region 系统画刷
        public static SolidColorBrush 主蓝 => (SolidColorBrush)_dict["主蓝"];
        public static SolidColorBrush 默认背景 => (SolidColorBrush)_dict["默认背景"];
        public static SolidColorBrush 默认前景 => (SolidColorBrush)_dict["默认前景"];
        public static SolidColorBrush 暗遮罩 => (SolidColorBrush)_dict["暗遮罩"];
        public static SolidColorBrush 深暗遮罩 => (SolidColorBrush)_dict["深暗遮罩"];
        public static SolidColorBrush 亮遮罩 => (SolidColorBrush)_dict["亮遮罩"];
        public static SolidColorBrush 深亮遮罩 => (SolidColorBrush)_dict["深亮遮罩"];
        public static SolidColorBrush 黄遮罩 => (SolidColorBrush)_dict["黄遮罩"];
        public static SolidColorBrush 深黄遮罩 => (SolidColorBrush)_dict["深黄遮罩"];
        public static SolidColorBrush 中灰1 => (SolidColorBrush)_dict["中灰1"];
        public static SolidColorBrush 中灰2 => (SolidColorBrush)_dict["中灰2"];
        public static SolidColorBrush 浅灰1 => (SolidColorBrush)_dict["浅灰1"];
        public static SolidColorBrush 浅灰2 => (SolidColorBrush)_dict["浅灰2"];
        public static SolidColorBrush 深灰1 => (SolidColorBrush)_dict["深灰1"];
        public static SolidColorBrush 深灰2 => (SolidColorBrush)_dict["深灰2"];
        public static SolidColorBrush 中黄 => (SolidColorBrush)_dict["中黄"];
        public static SolidColorBrush 浅黄 => (SolidColorBrush)_dict["浅黄"];
        public static SolidColorBrush 中绿 => (SolidColorBrush)_dict["中绿"];
        public static SolidColorBrush 浅绿 => (SolidColorBrush)_dict["浅绿"];
        public static SolidColorBrush 湖蓝 => (SolidColorBrush)_dict["湖蓝"];
        public static SolidColorBrush 浅蓝 => (SolidColorBrush)_dict["浅蓝"];
        public static SolidColorBrush 深蓝 => (SolidColorBrush)_dict["深蓝"];
        public static SolidColorBrush 亮蓝 => (SolidColorBrush)_dict["亮蓝"];
        public static SolidColorBrush 亮红 => (SolidColorBrush)_dict["亮红"];
        #endregion
    }
}
