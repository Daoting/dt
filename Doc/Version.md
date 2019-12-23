﻿# 版本说明

### 功能 
* 

### 变更 
*  移除Newtonsoft.Json，使用System.Text.Json 
*  AtUI合并到AtApp

### Bug修改 
* 设置窗口自启动时参数序列化问题

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