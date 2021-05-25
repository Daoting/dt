搬运工扩展包主要有三方面功能：
1. 项目中添加项时的代码模板，有不同窗口类型、用户控件等
2. 扩展xaml编辑器的上下文菜单，可插入搬运工常用控件的xaml，比手写方便
3. 扩展代码编辑器的上下文菜单，可插入常用的代码块


项模板(ItemTemplate)在独立的项目中定义，在source.extension.vsixmanifest中作为Assets

扩展上下文菜单在 DtPackage.vsct 文件中定义，文件为xml格式，定义顺序：
1. 定义 Symbols\GuidSymbol 命令组的guid；
2. 定义命令组内所有命令的 IDSymbol 值；
3. 定义分组及分组菜单 Commands\Groups\Group , Commands\Menus\Menu 
4. 定义菜单项 Commands\Buttons\Button

Group Menu Button之间的关系：
Group是抽象的分组，虚拟节点无对应UI，父节点一般为菜单项，可以是已定义的VS原始菜单，也可以为自定义的Menu，子节点一般为Button
Menu为子菜单项，父子节点都是分组，与Button无直接关系
Button为实际命令的UI，通过设置父分组确定位置