<h1 align=center>
 <img align=center src="https://raw.githubusercontent.com/Daoting/dt/master/logo.png" width="64" />
</h1>

## 一套让您能够快速构建业务系统的框架
[![GitHub Stars](https://img.shields.io/github/stars/daoting/dt?label=github%20stars)](https://github.com/daoting/dt/stargazers/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/dt.client.svg)](https://www.nuget.org/packages/dt.client)

# 概述
框架全名“数据的搬运工(Data Transfer)”，简称搬运工，缩写dt。

涉及技术：.NET、UWP/WinUI、C# + Xaml、[Uno Platform](https://github.com/unoplatform/uno) 、云原生。

搬运工由客户端和服务端两部分组成，是一套用于快速构建业务系统的基础技术实现和规范集，本身与具体业务无关。

* 客户端部分通过[Uno Platform](https://github.com/unoplatform/uno)支持在Windows、 浏览器(WebAssembly)、手机上跨平台运行，包括一系列可复用的前端控件和各种基础功能模块；

* 服务端部分全面采用.NET云原生开发，提供4个通用的微服务，利用这些预定义的内容可直接快速的开发出符合常规要求的业务系统；

* 同时，两部分都支持独立使用，也可根据规范在不同层面、不同粒度下扩展框架，从而满足特定的业务需求。

# 客户端
客户端样例已上传到各平台商店，请搜索【搬运工】或使用以下链接进行安装：
* [微软商店](https://www.microsoft.com/store/productId/9NBLGGH4QJ52)
* [苹果商店](https://apps.apple.com/cn/app/%E6%90%AC%E8%BF%90%E5%B7%A5%E6%A0%B7%E4%BE%8B/id1591859126)
* [华为商店](https://appstore.huawei.com/app/C104883437)

客户端在界面布局上有个小的创新，内置两种界面模式：windows模式和phone模式，iOS和Android上是Phone模式；windows和wasm上支持两种模式的自动切换，当应用界面的实际宽度足够时按照windows模式显示，宽度较小时自动切换到Phone模式，两种模式的可视树根节点完全不同。

https://user-images.githubusercontent.com/29876815/145145784-f0ef05f3-2144-4769-83ab-dda76eb404d7.mp4

这样既能发挥windows上复杂布局时各区域之间的联动性

https://user-images.githubusercontent.com/29876815/145151944-549bec31-b599-4d2d-8051-072babde692f.mp4

也能保证phone模式时的正确导航

https://user-images.githubusercontent.com/29876815/145152462-30b51172-01bd-4dc9-b978-2523a0f82f5d.mp4

客户端的完整功能请参考《[搬运工客户端手册](https://github.com/Daoting/dt/blob/master/Doc/%E6%90%AC%E8%BF%90%E5%B7%A5%E5%AE%A2%E6%88%B7%E7%AB%AF%E6%89%8B%E5%86%8C.docx)》

