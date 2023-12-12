<div align=center>
 <img src="https://raw.githubusercontent.com/Daoting/dt/master/logo.png" width="64" />
 <h1>搬运工</h1>
</div>

## 利用 C# + XAML 进行快速业务开发的跨平台框架
[![GitHub Stars](https://img.shields.io/github/stars/daoting/dt?label=github%20stars)](https://github.com/daoting/dt/stargazers/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/dt.client.svg)](https://www.nuget.org/packages/dt.client)

## 概述
框架全名“数据的搬运工(Data Transfer)”，简称**搬运工**，缩写**dt**。

涉及技术：.NET、UWP/WinUI、C# + Xaml、[Uno Platform](https://github.com/unoplatform/uno) 、云原生。

搬运工由客户端和服务端两部分组成，是一套用于快速构建业务系统的基础技术实现和规范集，本身与具体业务无关。

* 客户端部分通过[Uno Platform](https://github.com/unoplatform/uno)支持在Windows、 浏览器(WebAssembly)、手机上跨平台运行，包括一系列可复用的前端控件和各种基础功能模块；

* 服务端部分全面采用.NET云原生开发，提供多个通用的微服务，利用这些预定义的内容可直接快速的开发出符合常规要求的业务系统；

* 同时，两部分都支持独立使用，也可根据规范在不同层面、不同粒度下扩展框架，从而满足特定的业务需求。

## 所有项目
* 使用样例　[项目](https://github.com/daoting/dt.demo)
* 服务端　[NuGet](https://www.nuget.org/packages/Dt.Service)　[项目](https://github.com/daoting/dt.service)
* 客户端　[NuGet](https://www.nuget.org/packages/Dt.Client)　[项目](https://github.com/daoting/dt.client)
* Excel Chart控件　[NuGet](https://www.nuget.org/packages/Dt.Infras)　[项目](https://github.com/daoting/dt.infras)
* Visual Studio扩展工具，包括项目模板、常用代码模板、中文代码简拼助手，[下载](https://github.com/daoting/dt/releases/latest)
* [查看详细教程](https://daotingh.gitee.io/dt-docs/docs)
* [最新Release版本](https://github.com/daoting/dt/releases/latest)

 
## 客户端
客户端样例已在各主流应用商店中上架，请在应用商店搜索【搬运工样例】或使用以下链接进行安装：
* [微软商店](https://www.microsoft.com/store/productId/9PBFQ5NHPH14)
* [WebAssembly 样例](https://x13382a571.oicp.vip/demoui/)
* [Android 安装包](https://x13382a571.oicp.vip/downloads/apk/)
* [苹果商店](https://apps.apple.com/cn/app/%E6%90%AC%E8%BF%90%E5%B7%A5%E6%A0%B7%E4%BE%8B/id1591859126)

客户端在界面布局上有个创新，内置两种界面模式：windows模式和phone模式，iOS和Android上是Phone模式；windows和wasm上支持两种模式的自动切换，当应用界面的实际宽度足够时按照windows模式显示，宽度较小时自动切换到Phone模式，两种模式的可视树根节点完全不同。

https://user-images.githubusercontent.com/29876815/145145784-f0ef05f3-2144-4769-83ab-dda76eb404d7.mp4

这样既能发挥windows上复杂布局时各区域之间的联动性

https://user-images.githubusercontent.com/29876815/145151944-549bec31-b599-4d2d-8051-072babde692f.mp4

也能保证phone模式时的正确导航

https://user-images.githubusercontent.com/29876815/145152462-30b51172-01bd-4dc9-b978-2523a0f82f5d.mp4

## 服务端
全面采用基于 .NET 的云原生应用开发。
* 所有服务既可以使用 Kubernetes 调度，也支持简单部署到IIS；
* 既支持标准的微服务部署方式，也支持将所有服务合并成单体服务的部署方式。

系统未严格按照当前常见的某种具体的服务架构进行设计，而是根据自身整体架构的特点，借鉴常见架构的思想，比如DDD模式的领域事件、充血模式、数据仓库，CQRS架构的读写分离，六边形架构的微服务API设计等，结合客户端xaml的优势，使用MVVM(Model-View-ViewModel)模式开发客户端控件。**实现客户端领域层和服务端领域层代码相同**，既可以将业务放在客户端处理也可以稍加修改放在服务端处理，完美实现技术与业务解耦、业务逻辑解耦、业务封装复用等关键功能，为开发人员提供一种易懂易用的框架风格。总体框架图：

![fm](https://daotingh.gitee.io/dt-docs/docs/2%E5%9F%BA%E7%A1%80/1%E6%80%BB%E4%BD%93%E6%9E%B6%E6%9E%84/6.png)

目前提供3个通用的微服务：
* 内核模型服务(cm)，CM是Core Model的缩写，为平台提供基础的内核模型服务
* 消息服务(msg)，平台内置的基础消息服务，支持多副本部署，所有已注册的客户端在启动时调用服务端的Register方法，该Api采用基于http2协议的ServerStream模式，即：客户端发送一个请求，服务端返回数据流响应，相当于在服务端和客户端建立了长连接，服务端可以实时向客户端推送信息
* 文件服务(fsm)，统一管理应用范围内使用的文件，在k8s中支持多副本部署，支持分卷存储文件，可通过ConfigMap将数据卷Volume挂载到指定目录，该服务只负责文件的上传下载和文件管理功能，不涉及具体的业务问题，在功能和Api上较稳定。


# [打开教程，开始使用](https://daotingh.gitee.io/dt-docs/docs)
