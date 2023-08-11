
#总体
wasm版功能基本能运行，但目前编译慢、启动下载慢、交互响应慢，用户体验差，不推荐使用。


# 版本说明
## Release 4.2.x
### 变更
* 导出模型库，将ExportToModel放入model.json
* 服务地址拆分成独立配置文件url.json

### 功能
* .admin页面增加版本号
* 增加客户端版本号
* win版app增加自动更新功能

### Bug修改
* 库初始化异常时恢复按钮状态
* 优化数据库初始化向导，简单易懂
* boot服务启动bug，启动过程拆分成独立
* 确认对话框、错误对话框不支持非UI线程显示

## Release 4.2.0
### 变更
* 权衡利弊，以key获取sql语句的方式调整为调用存储过程，key=存储过程名，调用IDataAccess的程序无需调整，需要将cm_sql表的sql语句移植到存储过程
* 升级服务端引用包
* 升级客户端sqlite相关包
* 因支持4种数据库，命名特点各不相同，为实现自动生成实体的属性符合命名规范，基础的表名、字段名都采用小写，单词间以下划线分隔，生成的属性名符合Pascal规则，无下划线，建议业务表和字段都采用中文
* IDataAccess增加创建IEntityWriter，调整DomainSvc中的写法
* 服务启动时不再缓存所有库表结构
* 生成Entity代码3个文件，tbl.cs sql.cs .cs
* 生成DomainSvc类2个文件，sql.cs .cs
* service.json将单体模式和初始化库合并，cm配置增加导出模型库设置
* 序列名称：表名_字段名，4库不同的取序列值方法，时间类型统一精确到秒

### 功能
* 增加EntityX.GetCount方法
* .admin页增加初始化数据库功能
* 增加PostgreSql支持
* postgresql必须设置CommandBehavior.KeyInfo才能查询到列结构信息，含列结构信息时pg最慢！
* 增加获取当前服务默认数据库的类型、键名
* 初始化库增加demo库
* TableCol记录所属TableSchema
* VS扩展实体类选择表时增加“表名不包括前缀”选项
* oracle插入更新时bool类型自动转换
* postgresql分页查询功能，根据id查询数据不能为string
* Win中停靠的弹出面板可拖拽调整尺寸
* 等待对话框
* Entity 反序列化时对于byte short类型的列根据属性类型自动转 enum 类型
* 增加调用存储过程返回Table的demo
* 支持视图生成Enity
* 增加视图实体的增删改demo，mysql sqlserver能保存的列最多，oracle只可保存主键列所属表的列，pg视图不能保存数据
* 系统日志增加复制select语句的功能，方便调试查询语句
* 增加Table 作为 ITreeData时父节点id的列名

### Bug修改
* Win中停靠在四周的Tab在固定、停靠切换时丢失弹出面板的尺寸
* 解决Excel控件中公式不再重新计算bug

## Release 4.1.1
### 变更
* Cols不再继承KeyedCollection，支持重复列id
* IFvCell移除IsHorStretch、添加ColSpan，FormPanel重写布局算法
* 将LobKit登录用户相关回调放在Kit中
* 修改global.json中数据源配置，控制是否可导出到模型
* 初始Sqlite库、Execute、BatchExec调为异步
* 多数据库sql语句参数前缀不同
* 不同数据库类型的分页语句
* QueryFv FuzzySearch 只负责查询内容，查询sql的动态生成放在QueryClause，也可自定义生成sql
* 同步调整VS扩展中的框架模板
* where true 或 where false 除mysql外都不支持
* 表名采用库里原始大小写，比较时大小写不敏感，oracle生成的sql在表名字段名加引号
* cm服务中导出模型的配置放在model.json
* 单体服务支持每个服务连接不同库
* rpc调用添加服务名，涉及所有类型rpc
* 服务端缓存sql、根据key查询sql支持三种库类型
* VS扩展的服务项目支持连接不同库
* 三种数据库的序列问题


### 功能
* 增加更新模型菜单项
* 增加全局Dt.Base.SelectionMode别名
* Sqlite模型文件支持：多个数据库、多种数据库
* 增加可配置的空服务
* 升级uno4.9，Wasm支持UIElement.ProtectedCursor MediaPlayerElement
* Lv增加Defer()
* Lv增加Where
* Fv增加Defer()

### Bug修改
* 文件管理中的FolderPage因获取权限异步，造成初次加载时空引用
* Excel的表格的标题下拉箭头点击后报错，无法弹出过滤、排序对话框
* Excel打印功能升级WinUI后报错
* 涉及元素光标的ProtectedCursor通过InputSystemCursor.Create赋值时锁死，比如：ColHeaderCell，观察
* wasm使用new Rect()布局时无法隐藏元素，统一用Rect(-1, -1, 0, 0)


## Release 4.0.1
### 变更
* 流程定义中删除FormType ListType字段，使用类型别名
* 优化Entity的Hook
* Entity增加OnInit方法用来统一添加当前实体的所有回调方法，主要包括三类回调：保存前、删除前、Cell.Val值变化前
* 服务端生成Entity方法
* 修改客户端和服务端的DataProvider调用Entity的保存前、删除前回调方法
* 修改服务端生成实体类及扩展部分的方法
* VS扩展用到表名列表和表结构信息都采用实时获取方式，不使用服务端缓存
* 增加demo服务，用于业务样例
* 删除Row.AttachHook，重新生成Entity文件放在Domain下
* Entity增加GetByID，调整服务端生成Entity的方法
* 增加EntityEx类，添加静态查询方法，重新生成项目中的*.Designer.cs文件
* 增加EntityEx类，添加静态保存方法
* 增加EntityEx类，添加静态删除法，重新生成项目中的*.Designer.cs文件
* 增加UnitOfWork类，添加保存删除方法
* Entity增加静态方法DelByID，重新生成*.Designer.cs文件
* 服务端删除EntityAccess和DataProvider，增加IDataProvider接口，为支持多种库类型准备
* 修改总体架构图，增加客户端Domain部分
* VS扩展工具类的命名空间默认到一级目录模块名
* VS扩展工具单实体框架调整
* VS扩展工具单实体框架生成自定义查询面板
* 客户端增加本地事件的定义、发布、处理，和服务端用法相同
* 替换DataProvider<TSvc>中Save Delete方法，移除DataProvider<>，调整到EntityWriter
* 实体类后缀 Obj -> X
* EntityEx.DelByID支持直接删除，不经过校验领域事件等
* CellUI -> LvCall, MidVal -> FvCall, IMidVal -> IFvCall
* 因Mv内菜单无法绑定，将Mv合并到Tab，对于自定义Mv需要修改两处：Mv -> Tab, Win中xaml删除自定义Mv的外套Tab
* 登录过程移至客户端
* 领域服务用静态方式替换单例模式，增加样例
* DtControl的虚方法OnControlLoaded修改为OnFirstLoaded，方便理解
* Entity增加OnSaved OnDeleted回调
* Entity的GetByID GetByKey不再涉及缓存，GetFromCacheFirst专门用于优先从缓存读取
* 原通过DeleteBySvc SaveBySvc处理UserObj的缓存和领域事件移植客户端
* Rbac基于角色的访问控制
* 客户端缓存数据：可访问的菜单，具有的权限，数据版本号用到时比较、更新
* 文件 发布 报表管理 参数定义 用户设置 选项管理功能
* 开始菜单项提示信息由各视图自行处理


### 功能
* VS扩展增加批量生成实体类功能
* 增加业务样例项目
* Col无Title时显示ID
* 增加查询面板QueryFv，FvCell支持比较符
* Row增加To<T>方法，和任意Entity类型转换，共用_cells
* 生成的实体类增加和外部共用Cells的构造方法
* 增加虚拟实体，并在EntityEx和UnitOfWork中处理
* BuildToos增加Dt.Core.DtDictionaryResource().Merge()
* 增加EntityX<T>，将Designer的静态方法放入泛型中
* 增加 IEntityAccess IEntityWriter 接口
* UnitOfWork 改 EntityWriter私有类
* 两端增加 DomainSvc 领域服务基类，BaseApi 改为 DomainSvc
* EntityEx的普通实体方法：增删改及批量，及EntityX.DelByID
* 实体领域事件样例
* 虚拟实体的增删改查样例
* 父子实体的增删改查样例
* 实体缓存样例
* 生成的实体类增加自定义服务名
* sqlite的增删改查样例
* 系统日志增加复制和除此清空功能
* sqlite的增删改查记录系统日志
* 系统日志增加输出附加属性
* 测试sqlite本地库涉及的改动，修改bug
* 服务端实体的增删改查样例
* 增加支持窗口xaml的精简写法
* 升级uno4.7.37，完善Tab相关的文档
* VS扩展工具单机版项目模板bug
* 添加单实体及虚拟实体框架模板、样例
* 虚拟实体的内部实体Cell值变化时统一内部实体的OnChanging回调
* 虚拟实体和内部实体IsChanged的状态同步
* Tab区域内导航增加切换Tab和返回首页的功能
* VS扩展工具一对多模板
* 多对多模板及样例
* EntityX的Query Page First支持完整sql查询
* 完善文档
* 多Tab在Dlg中显示


### Bug修改
* Fv格标题提示被截断的长文本
* Mv中Menu的项无法绑定，如保存按钮的IsEnabled，以合并到Tab



## Release 3.3.1
### 变更
* 将CList的SqlKey,Enum,Option合并到Ex属性
* OmMenu cm_menu移除SvcName，视图类通过静态方法GetMenuTip提供提醒数字

### 功能
* Lv增加内置过滤
* 

### Bug修改
* 



## Release 3.3.0
### 变更
* 解决代理服务DataProvider<TSvc>类型小写的警告，调整为大写内联类型
* SqliteProvider<TDbName>库名类型调整为大写内联类型

### 功能
* 升级.net7.0
* 升级服务端、客户端引用包

### Bug修改
* android平台过时API警告
* iOS平台过时API警告
* 解决升级.net7.0后wasm生存错误
* 后台任务启动构造Stub时App为null



## Release 3.2.4
### Bug修改
* Lv自定义单元格UI重复绘制
* NavList递归触发嵌套子窗口Closing事件
* Win的Main或Pane的子项类型错误时不抛异常，提示错误，winappsdk 1.2已处理异常但没升级
* 继承Win的子类或任意Control子类若没有对应的xaml，放可视树不显示，会异常退出，和uwp早期版本一样
* Excel切换显示网格_excel.ActiveSheet.ShowGridLine无效

### 功能
* 优化NavList
* 增加Lv行样式动态切换样例
* 增加Lv单元格UI动态切换样例，完善Lv文档
* 增加自定义格取值赋值过程
* RptView工具栏菜单
* 报表组的报表列表采用NavList
* 从开始菜单打开报表窗口和报表组窗口
* 报表查询基类及查询对话框
* 报表增加工具栏菜单和上下文菜单的控制



## Release 3.1.0
### 变更
* Lv自定义行列UI的调整：
  移除CellEx静态类自定义行列UI的方式
  增加 Lv.ItemStyle 回调方法自定义行样式
  移除 Col.UI Dot.UI 属性
  增加 Col.Call Dot.Call 属性，自定义单元格UI的方法名，多个方法名用逗号隔开，形如：Def.Icon,Def.小灰

### 功能
* 增加Stub.Reboot方法支持切换Stub新实例重启
* Dt.BuildTools 增加对wasm发布时清理无用文件，处理PWA缓存文件重复的问题
* 类型别名字典中每个别名对应一个类型列表
* 后台功能确认：文件 通讯录



## Release 3.0.1 
### 变更
* 将使用标准服务的管理功能全部拆分迁移到Dt.Mgr，Dt.Core Dt.Base与标准服务无任何耦合
* 增加Dt.BuildTools，自动生成Stub的字典代码
* sqlite数据查询转异步方法

### 功能 
* VS扩展工具新建项目时增加向导
* Kit.ShowRoot 支持 View



## Release 2.8.2 
### 功能 
* PhoneUI版提示信息框增加动画
* PhoneUI模式增加切换Tab方法

### Bug修改   
* [ios]Lv TreeView 绘制死循环
* Fv中颜色选择不显示
* [ios] 工具栏菜单死循环 
* [ios] Excel迷你图、分组排序、冻结线死循环
* [ios] 报表死循环
* Lv TreeView 外部有滚动栏时布局未刷新
* wasm提示信息框动画样式调整
* Excel在uno4.5.x 后因iOS版切换Tab时内容不显示，改为Add方式



## Release 2.8.0 
### 功能 
* 升级到 WinAppSdk1.0(WinUI3.0) + .Net6.0 + 一声叹息
* 不再支持的API列表：https://docs.microsoft.com/en-us/windows/apps/desktop/modernize/desktop-to-uwp-supported-api
* 删除PhoneUI模式下窗口左上角的后退按钮
* Window.Current.CoreWindow.GetKeyState  -> InputKeyboardSource.GetKeyStateForCurrentThread
* Window.Current Window.Dispatcher  DependencyObject.Dispatcher 始终null，改用DispatcherQueue
* ProtectedCursor 加载到可视树前设置崩溃，无提示！
* FileOpenPicker, FileSavePicker, FolderPicker 增加Window句柄
* Win版Application的Suspending Resuming 事件已移除，Kit-Sys.cs
* Button Menu交互样式调整
* 全局快捷键适配WinUI
* wasm版日期选择CalendarView、DatePickerFlyout、TimePickerFlyout已实现 
* 启动时增加Kit.IsUsingDtSvc判断，GetConfig提前调用 
* Kit.Msg Kit.Warn增加动态调整信息的功能
* GMImagePicker.Xamarin不再升级，升.net6.0不兼容，合并到Dt.Cell
* NotifyItem动态自动关闭，动态回调
* 使用Serilog，完成日志的file console trace输出
* Dlg 标题和菜单平分标题栏宽度
* wasm通过StorageFile 可以保存文件
* Xamarin.Essentials 升级到 Microsoft.Maui.Essentials
* wasm不引用Microsoft.Maui.Essentials 
* [window]升级 Microsoft.Maui.Essentials 后，除无法"分享"外，其余正常
* 升级maui rc2后 Dt.Cells包的GMImagePicker 引用Xamarin.iOS 转为 Micorsoft.iOS
* Dt.Core不依赖Dt.Base中的样式 ，修改NotifyItem
* 增加uno在debug状态下的日志输出
* 移除继承Application的 BaseApp，功能合并到Stub，因uno的InitializeComponent()中使用 base.GetType().Assembly 造成莫名崩溃，5天才查出原因
* Dt.App因App名称易混改为Dt.Mgr
* Agent项目合并到Dt.Base
* Notify的UI移动到Dt.Base 
* [ios] SqliteConnection初始化异常 ，无法启动，SQLitePCLRaw 2.1.0 preview以后版本无异常
* 字体ttf文件放在win项目中，其他项目引用
* Client-Stable 改名Infra  

### Bug修改  
* Release版的Service Api生成代理类时方法无注释
* Lv的Table模式列头排序只在Tapped事件触发
* Phone模式标题栏触发右键菜单位置调整
* BackgroundTask 的TimeTriggeredTask 能注册、触发，和uwp调试方式相同
* 点击Toast自启动传参数问题
* win版Excel样例中 ChartExcel.xaml 报错，Dt.Cells.Data.Utility.GetFontWeightString()问题
* 合并WinUI库的默认样式，DefaultLogin.xaml 的 ProgressRing  正常显示
* Lv中行的上下文菜单按钮和行的ItemClick事件同时触发
* uno4.1.8 后Lv面板与外部ScrollViewer的相对距离和WinUI调整为一致 
* [iOS]Lv.Toolbar在Table模式时 MeasureOverride 死循环 
* iOS版CDate除TimePickerFlyout.TimePicked事件外，都正常
* 删除Common.props中的SingleProject ，即可移除项目中的Android 目录 

### 服务  
* 所有服务升级到.net6
* 调整项目结构，支持合并所有微服务为单体服务
* pub合并到cm, editor放入fsm
* 调整fsm静态文件的虚拟路径为drv，避免和其他服务冲突
* cm的GetConfig增加提供服务url列表，service.json 调整后支持实时更新服务地址
* 客户端rpc采用动态服务路径
* 用Kit.Rpc<T>封装Api调用，客户端和微服务之间rpc相同
* 每个微服务Api的Rpc调用
采用独立程序集，供客户端和其他微服务调用
* 支持非内置类型序列化
* 原pub的管理功能，表名规范按cm
* 服务监听rabbitmq的队列变化事件，更新服务列表



## Release 1.9.10 
### 功能 
* 增加NavList，替换原有的MainInfoLv方式
* 为上架到商店，样例设为主页
* 通用的隐私政策及用户协议对话框

### 变更 
* 精简常用画刷的名称
* 因放商店服务端不自动注册账号
* Mv.OnInit 在Loaded事件时调用

### Bug修改  
* Lv 切换数据源、删除行后的布局bug
* CNum采用 NumberFullWidth 键盘，Number键盘无小数点
* Lv滑动时行交互背景不消失
* uwp的release版 Excel的画刷改名
* Dot中string空时不显示
* win模式Win切换主区时若保存布局后再加载布局错误



## Release 1.9.9 
### 功能 
* 实体类中增加enum的支持，包括sql查询、序列化反序列化 
* Dlg修改为独立遮罩，无遮罩时也可控制是否允许将点击事件传递到下层对话框 
* Entity的OnSaving  OnDeleting 返回值Task，不再支持void，因服务端通过EntityAccess 时异常内容无法获取
* MainInfo增加Cache属性，实现INotifyPropertyChanged
* 增加首次运行向导页功能 
* 手机横竖屏不支持UI自适应，始终为PhoneUI模式 
* Lv分组模板统计功能 
* CTip增加Click事件
* 两UI模式切换后老窗口能自启动， Stub中可设置Startup.AutoStartOnce 
* Dlg增加TargetOverlap 布局
* 增加Mv控件，能够放在Tab中支持内部导航
* 调整Dlg、ToolWindow样式 
* 增加 SearchMv 控件
* VS插件增加三种框架模板
* Lv顶部增加工具栏，放置排序筛选等
* Lv 默认筛选对话框
* android ios uwp后台服务 
* android ios uwp点击通知栏，根据参数自定义启动 

### 变更 
* Lv的上下文菜单在Phone模式默认为按钮触发
* 调整默认字体大小，15 -> 16
* 控件适配默认字体 
* 删除FvCell.ShowStar属性
* PhoneUI模式页标题20px
* Tab.Pin按钮只用作返回，Pin按钮转为右键菜单
* 删除Tab.PhoneBar 
* 删除Win.Home，增加Tab.Order控制首页显示
* 实体对象名增加Obj后缀，避免重复命名 
* 删除SearchFv 

### Bug修改 
* Dlg禁止获得焦点时调整显示层次，太乱！
* Dlg 背景遮罩放在Dlg外部 
* ChatMember.Sex 转Gender
* CText内部的TextBox的清空按钮无效，输入法
* ios wasm 中 Fv的 CText 长文本回车跳格的问题
* wasm版Chat的子Canvas不绘制的bug，uno已解决
* 两UI模式切换事件SysVisual.ViewWidth值不对
* Menu在手机大字体模式显示不全
* SysVisual.ViewWidth 采用 Window.Bounds.Width


## Release 1.9.6

### 功能 
* 因sqlite调整重构生成存根代码
* 本地sqlite数据的备份与删除
* 添加接收分享功能 
* 本地库增加分享库文件功能
* msg服务支持同一账号多个会话
* 远程事件组播时实现等待调用结束
* 增加Dlg对话框关闭时的返回值，返回值传递给ShowAsync OnClosing OnClosed方法 和 Closing Closed事件
* wasm完成视频通话功能 

### 变更 
* 服务端导出sqlite模型只使用Microsoft.Data.Sqlite，无类型映射
* 重构sqlite客户端代码，调整为DataProvider 风格，映射类型转Entity，支持自定义sqlite库
* 服务端生成ID时去除3位标志位
* AtSys AtKit AtUser AtApp合并为Kit
* ImgKit CrossKit合并到Kit
* 服务端Glb ID等合并到Kit
* Bag EventBus Cache合并到Kit 

### Bug修改 
* Lv自动生成列时，切换数据源报无旧数据源列的警告
* ios版System.Text.Json 序列化json串时异常，将nuget包system.text.encodings.web的netstandard2.1 下的dll用2.0的替换解决
* CNum.Scale因和UIElement.Scale重名，xaml中赋值时异常，改名Decimals
* sqlite第一行含空时的列类型错误 
* 避免win内部导航时自己导航到自己的情况 
* SqliteCommandEx 可空列bug
* 3.8.6 DatePicker TimePicker已正常
* Lv的分组导航头不显示时在android上也堆在一起绘制
* 解决msg服务离线消息异常
* ios wasm注销时停止接收推送


## Release 1.9

### 功能 
* wasm的Dt.Cells包升级到.net5.0，解决wasm的图标字体库 
* 部署到iis的安装脚本、更新pfx证书、调试、fsm上传文件大小1g
* admin页面支持浏览器前进/后退、显示实时日志
* 为Serilog增加输出html功能
* admin页面历史日志功能 
* wasm版sqlite终于可用了
* wasm部署到iis，支持调试
* wasm动态获取服务器地址
* wasm中未处理异常统一到异常处理
* wasm中显示图片
* FileList图片预览
* PhoneTabs左右滑动，只支持触摸，滑动设最小阈值
* msg服务停用心跳包，ios设置5分钟超时 
* uno升级到3.6.6，确认FileOpenPicker功能
* android升到11.0，vs升到16.9.3，修改android升级后的警告

### 变更 
* 修改fsm中缩略图命名标准，完整文件名
加后缀"-t.jpg"
* 调整pub服务部署目录

### Bug修改 
* 解决sqlite的GetFieldType错误
* wasm版sqlite不支持事务
* 解决admin页面实时日志在频繁输出时漏项的bug，服务端缓存最近50条log 

## Release 1.5

### 功能 
* 增加页面导航动画
* 增加Chart
* 增加Excel
* NaviWin和PageWin功能合并到Win，移除NaviWin和PageWin
* Lv的ICellUI.UIType支持自动加载文件服务的Image
* 增加FileList, FileItem
* Fv增加链接格
* Fv增加图像格、文件格
* Lv的ICellUI.UIType 增加File类型
* Chat发送各类型文件功能
* Lv.Data类型调整为INotifyList
* Lv外部有ScrollViewer的处理
* 优化Win结构
* Fv外部有ScrollViewer的处理 
* Tv外部有ScrollViewer的处理
* Chat 撤回消息功能
* Chat 发送语音
* FileList增加录音文件
* 调整Chat 录音文件模板样式
* Chat 拍照和录视频发送
* Fsm增加直接按路径浏览的功能
* 增加Pub服务，引用Froala Editor
* Fv 增加CHtml
* CTree增加清空功能
* 增加CTip格
* 升级到 uno2.4.0, vs16.6, android Q, 福祉堂
* 各平台的未处理异常日志
* Lv增加IRowView支持
* FileList增加EnableClick 
* FileList增加 Spacing
* Msg服务增加心跳
包
* 文件管理
* 发布系统，合并到App
* 富文本格，插入图片、视频
* 工作流
* 所有服务升级到.net5.0

### 变更 
* 移除Newtonsoft.Json，使用System.Text.Json 
* AtUI合并到AtApp
* 调整TabControl继承自Control 
* uno升级到2.2.0，Holding事件可用
* uno升级到2.4.4
* 所有Holding事件转换成RightTapped，长按效果好且能和Tapped分开
* uno升级到3.1.6

### Bug修改 
* 设置窗口自启动时参数序列化问题
* Chart饼图在android不正常
* Excel 添加/删除列时bug
* Excel 导出pdf的bug
* mono 中 The Bindable attribute is missing and the type [Dt.Base.XXX] is not known
* ServerStream模式结束时为避免关闭连接，只能服务器端Abort
* Fsm客户端上传过程中关闭时未处理 
* Fv 手机加载慢
* Lv 初次加载无法滚动到底部，增加AutoScrollBottom 属性
* Lv在分页数据源顶部插入虚拟模式时，uwp出现无刷新现象，暂无使用场景
* Msg同一设备可能多次注册
* Fv初始隐藏Cell时的bug
* 窗口切换主区样例出错

*  MediaPlayerElement 在android无法播放音频，bug已提交uno

## Release 1.0 

### 功能
* 客户端框架结构
    - 页面窗口，继承自PageWin，基础内容控件，包括标题、图标、工具栏和页面内容；手机上承载在PhonePage内进行导航 
    - 普通窗口 ，继承自Win，窗口在PC上分上下左右和主区五个区域，由Tab承载内容，拖动时自动停靠；手机上自适应为可导航多页面
    - 导航窗口 ，停靠式窗口包含多个子Tab，手机上在Tab之间实现页面导航，将Tab承载在PhonePage内 
    - 对话框 ，模拟传统对话框，PC上显示在窗口上层，可拖动、调整大小、自动关闭等，手机上承载在PhonePage内 
    - 提示信息，提供两个级别的提示信息（普通、警告），在对话框上层显示，可自动关闭，最多可显示一个操作按钮 
    - 浮动面板 ，显示在最上层的面板容器，内部使用Popup实现，始终有遮罩
* 客户端数据控件
    -  表单Fv ，表单由单元格组成，单元格包括列名和编辑器，自动布局，支持自定义行数和内容，可作为独立的布局面板使用
    -  列表Lv ，支持三类视图表格、列表、磁贴，两种数据源，三种选择模式，定制分组，上下文菜单
    -  树Tv ，支持动态加载子节点，自定义节点样式、节点内容，动态设置节点模板，上下文菜单
* 客户端基础控件
    -  菜单 ，包括普通工具栏菜单、上下文菜单，支持多层子项、选择和分组单选等功能
    -  系统监视输出，内部使用的调试输出与系统工具
    - 可停靠面板，停靠式窗口的布局面板
    - 分隔栏 ，包括水平/垂直分隔功能 
    - Tab页，TabControl控件
    - 上传下载文件 
    - 基础事件
    - 图标，内置的矢量文字，可用作图标、提示
    - 样式资源
* 客户端基础模块
    - 用户账号
    - 系统菜单
    - 系统角色
    - 基础权限
    - 系统参数 
* 服务端基础功能
    - 服务配置 
    - 服务日志
    - EventBus，一种是进程内事件LocalEventBus，一种是基于RabbitMQ实现的远程事件RemoteEventBus
    - 三种缓存，服务端全局缓存、进程内缓存、客户端缓存
    - RPC ，参照grpc基于http2协议实现4种通信模式
    - 分布式ID
    - Api授权控制
    - 依赖注入/控制反转，系统采用Autofac容器以应对复杂的注册、注入，配合Castle.Core实现拦截
    - 业务开发框架，基于DDD模式，吸取CQRS读写分离的优势
* cm服务
    - 平台内核模型服务Core Model
    - 生成客户端模型缓存
    - 用户角色权限管理
    - 登录
* msg服务
* fsm服务