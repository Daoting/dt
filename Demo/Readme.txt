搬运工样例说明


/*********************************************** sln ***********************************************/
》 项目根目录下的解决方案
Demo.sln          所有客户端项目，4个目标框架
Demo-svc.sln      只服务
Demo-win.sln      只net-windows框架
Demo-android.sln  只net-android框架
Demo-ios.sln      只net-ios框架
Demo-skia.sln     只net框架，gtk和wpf项目
Demo-wasm.sln     只net框架，wasm项目

》 为提高效率，开发时使用 Demo-win.sln ，适配不同平台时再打开对应 sln





/***************************************************************************************************/
》 完整目录
App
App\Demo.Shared  app共享项目
App\Demo.Droid   android app项目
App\Demo.iOS     iOS app项目
App\Demo.Gtk     linux app项目
App\Demo.Wpf     wpf项目
App\Demo.Wasm    wasm web项目
App\Demo.Win     win app项目

App\InitSql     样例用到的初始化数据库脚本

Demo.Base       客户端基础项目，无任何UI，包括实体、领域服务、数据访问类、接口等
Demo.Entry      含Stub、主窗口、框架UI的入口项目，不依赖任何Demo项目
Demo.UI         控件样例，不依赖任何Demo项目
Demo.Crud       管理样例，包括：db增删改查，一对一、一对多、多对多框架代码，报表、工作流基础样例
Demo.Lob        业务样例





/***************************************************************************************************/
》 项目导入 Shared.props 定义的内容，方便支持多平台

》 UnoIcon：
   Include 背景svg
   ForegroundFile 前景svg
   ForegroundScale 前景缩放比例 0 ~ 1
   AndroidForegroundScale WasmForegroundScale WindowsForegroundScale IOSForegroundScale SkiaForegroundScale 覆盖ForegroundScale
   Color 背景色
   注意：win上svg路径不可使用相对路径，否则不报错不生成，app无法注册

》 UnoSplashScreen：
   Include 图片svg
   BaseSize 用作调整大小操作的基底的大小
   Color 背景色
   Scale 缩放比例
   AndroidScale IOSScale WindowsScale SkiaScale WasmScale 平台缩放比例，覆盖Scale

》 字体生成svg：开源 https://github.com/fontforge/fontforge/releases/ 安装，以管理员运行 run_fontforge.exe
   文件 > 执行脚本 SelectWorthOutputting(); foreach Export("svg"); endloop;  > 选择“FF” > OK
   在安装目录下每个字生成一个svg文件
   Demo\Common\icon-svg.rar 为icon.ttf字库的svg文件

》 Images\*.png 文件链接方式，项目中用到的图片，作为内容生成，ms-appx:///Images/*.png





/*********************************************** Win ***********************************************/
》 注意 Properties\PublishProfiles 下的配置文件，最初调试时因配置问题造成各种变量异常！

》 点击Toast自启动传参数：
   Package.appxmanifest 增加 desktop:Extension com:Extension 两节，见注释
   处理 AppNotificationManager.Default.NotificationInvoked 事件，在当前应用接收通知，否则会另外启动新应用实例！
   AppNotificationManager..Register 注册当前应用接收通知
   
》 发布：升级net8.0后需要先把 Directory.Build.props 和 当前项目csproj文件的 TargetFrameworks 改 TargetFramework，重启
   项目右键 -> 打包和发布 -> 将应用程序和应用商店关联 -> 创建应用程序包 -> 将 msix 文件上传到商店





/********************************************** Android **********************************************/
》 项目目录说明：
Android\Resources\values\styles.xml  定义样式参数
Android\Resources\xml\file_paths.xml 共享路径配置
\MainActivity.cs 入口

》 App标题定义有两处：AndroidManifest.xml AppStub.Title

》 生成apk：
真机apk：选择 Release -> AndroidManifest.xml设置版本 -> 调整csproj文件的RuntimeIdentifier配置 -> 右键选择“发布...” -> apk文件在 bin\Release\net7.0-android\android-arm64\ 目录下
虚拟机：选择 Release -> 调整csproj文件的RuntimeIdentifier为android-x64 -> Ctrl + F5 自动打包部署到虚拟机并运行
也可用命令发布：
dotnet publish -f:net7.0-android -c:Release

》 app必须引用所有用到的程序集项目，否则绑定类型使用反射方法，影响性能，uno警告提示：
The Bindable attribute is missing and the type [T] is not known by the MetadataProvider. Reflection was used instead of the binding engine and generated static metadata. Add the Bindable attribute to prevent this message and performance issues.





/************************************************ Gtk ************************************************/
》 控件样式中必须设置FontFamily，否则中文乱码

》 打包部署 https://mp.weixin.qq.com/s/lm5opxgR94__oFSM03vUPA





/************************************************ iOS ************************************************/
》 项目目录说明：
iOS\Assets.xcassets 图标资源，可通过控件样例的工具生成，在项目中不可见
iOS\Resources       名称必须是 Resources，否则字体无法加载
    \Assets
        \Lottie  *.json是lottie格式动画的json文件

        -- 图片文件无需指定 BundleResource，生成操作默认“无”即可，会自动 BundleResource 到 Assets目录
        \SplashScreen@x.png 启动图片，配合 LaunchScreen.storyboard 实现启动动画

    \Fonts
        \icon.ttf  自定义矢量字体库，文件链接
        \uno-fluentui-assets.ttf uno用到的字体库，文件链接
iOS\Info.plist  App配置文件
iOS\Entitlements.plist  需要权限列表
iOS\LaunchScreen.storyboard 启动动画定义
iOS目录下的资源为iOS专用
\Main.cs 入口

》 资源文件的生成操作：BundleResource 和 Content(内容) 的区别
它们的输出目录不同，参见：Dt.Shell.IOS\bin\Debug\net7.0-ios\iossimulator-x64\Dt.Shell.IOS.app
BundleResource的资源文件输出目录省略 iOS\Resources 两级目录，Content的资源文件输出到完整目录
如 iOS\Resources\Lottie\Dash.json 资源文件，
BundleResource 时路径：ms-appx:///Lottie/Dash.json
Content 时路径：       ms-appx:///iOS/Resources/Lottie/Dash.json
为了统一资源文件的路径，约定放在ms-appx:///Assets目录下，这就需要在iOS项目中资源文件放在iOS\Resources\Assets目录下

》 mac端生成后的位置：/Users/oysd/Library/Caches/Xamarin/mtbs/builds/Dt.Shell.iOS/

》 接收“分享”有两种情况实现，主要取决于发起“分享”的app的实现方式：
1. 使用UIDocumentInterationController发起的分享，接收时只需要在Info.plist中配置CFBundleDocumentTypes，并在App.xaml.cs中重写OpenUrl，如app 文件；
2. 使用UIActivityViewController发起的分享，接收时需要在独立的dll项目中实现Share Extension，iOS8.0 以后增加的方式，如 照片 邮箱；
本系统只处理第一种情况！未实现“照片”中的接收分享功能

》 发布：选择Release + iPhone + 远程设备，生成前先确认Info.plist和iOS项目文件csproj中的版本相同，重新生成iOS项目，过程比较漫长，生成成功后使用命令发布，
使用PowerShell切到iOS项目的路径下，运行：
dotnet publish -f:net7.0-ios -c:Release

》 开发、调试、发布上架的详细过程参见《搬运工客户端手册.docx》 





/************************************************ Wasm ***********************************************/
》 AppManifest.js：自定义启动页logo、背景色、浏览器标题等

》 项目文件(*.csproj)中：
   EmbeddedResource 节点可以将js文件或css文件输出到网站 package_xxx 目录下，js文件在WasmScripts下，css文件在WasmCSS下
   Content 节点的内容文件输出到 package_xxx 的对应子目录下，也可以自定义目录如：<Content Include="Path\To\My\File.txt" UnoDeploy="Root" />
   https://platform.uno/docs/articles/external/uno.wasm.bootstrap/doc/features-additional-files.html
   WasmCSS WasmScripts目录名不可修改，uno用到

》 系统使用 RequireJS 加载模块、管理依赖关系，在WasmScripts下js文件中：
   define([],function(){})：第一个参数是模块依赖，如果需要依赖别的js或者css，就在第一个参数中指定，第二个是函数，当第一个参数中的所有依赖都加载成功后，然后才会该函数会被调用来定义该模块，因此该模块应该返回一个定义了本模块的object
   require([],function(){})：参数和define相同，只是不返回当前object

》 manifest.json 为PWA的清单描述
   LinkerConfig.xml 生成包时不参加裁剪的dll列表
   pwa目录是pwa用到的图片，可在 https://www.pwabuilder.com/imageGenerator 生成

》 Dt.BuildTools 功能：
   向项目添加默认字体文件、字体css、dt.js
   Release模式生成时处理PWA相关的后续工作

》 wasm不支持直连数据库，因启动时读取配置，不使用Config.json，使用Config.js
   UnoAppManifest.displayName 在AppManifest.js中设置
   DtConfig.server 在Config.js配置，AppManifest.js每次会动态生成，无法在其设置

》 跨域请求时boot服务和所有要访问的服务都必须为https

》 发布：选择Release -> 生成wasm项目 -> 生成完整的单页面静态网站：bin\Release\net7.0\dist  -> 用boot服务承载静态网站（参见文档）
   生成时默认会为js css wasm clr格式的文件生成Brotli压缩文件(*.br)，Uno的DevServer缺少自动下载同名br文件、PWA、允许跨域访问的功能
   启用PWA网站必须为https，且跨域请求的服务都为https