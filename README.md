What is xCal?
============
 xCal is a **free and open source software (FOSS)** that:

1. Allows you to exchange calendaring and scheduling information (events, to-dos, journals and free/busy) in the iCalendar [iCalendar](http://en.wikipedia.org/wiki/ICalendar) data format independently of calendar service or protocol.

2. Operates on a [REST](http://en.wikipedia.org/wiki/Representational_state_transfer)-ful calendar services, in which operations to access and edit distributed calendar data are exposed by means of an [API](http://en.wikipedia.org/wiki/Application_programming_interface).

Why xCal?
=========
First of all, xCal is **free** - yes  *'free' as in 'free air'* and not *'free' as in 'free beer'* because the best things in life such as air and sunshine are free!

Secondly, xCal gives you total control over your calendar information because you are responsible for the storage and maintenance of your calendar on your own machine.

Thirdly, you enjoy distributing calendar information in multiple data formats: 
* [iCalendar](http://en.wikipedia.org/wiki/ICalendar) 
* [CSV](http://en.wikipedia.org/wiki/Comma-separated_values)
* [MessagePack](http://msgpack.org/)
* [JSON](http://en.wikipedia.org/wiki/JSON)
* [JSV](http://mono.servicestack.net/docs/text-serializers/jsv-format) 
* [XML](http://en.wikipedia.org/wiki/XML)

Fourthly for developers, xCal takes away the burdden of *reinventing the wheel*. Why should you homebrew a non-standard calendaring and scheduling system, when xCal already provides an implementation of the [Internet Calendaring and Scheduling Core Object Specification (RFC 5545)](http://tools.ietf.org/html/rfc5545)?

Finally, you gain the awesome [benefits][1] of using xCal as an open source software such as low costs, quality improvement through continuous community input, business agility and mitigation of business risks. 

Table of Contents
=================
1. [Get Started](https://github.com/reexmonkey/xcal/#get-started)
2. [Dependencies](https://github.com/reexmonkey/xcal/#dependencies)
3. [Contributing](https://github.com/reexmonkey/xcal/#contributing)
4. [Documentation](https://github.com/reexmonkey/xcal/#documentation)
5. [Versioning](https://github.com/reexmonkey/xcal/#versioning)
6. [Community](https://github.com/reexmonkey/xcal/#community)
7. [License](https://github.com/reexmonkey/xcal/#license)


Get Started 
===========
To get started on the xCal, you might want to choose any of the following options:

1. Preview a [demo][2] of xCal web serivices in operation. 
2. Download the master repository [ZIP][3] file from GitHub.
3. [Clone][4] the master repository on desktop using the Github application. 
4. Download core xCal binaries from the [Nuget Gallery](https://www.nuget.org/packages/xcal.service.interfaces.concretes/) 
5. Install core xCal service interface binaries in Visual studio e.g by means of the via the Project Mangager Console:

```
PM> Install-Package xcal.service.interfaces.concretes -Pre
```

### Source Code Structure

If you are interested in making a fork of xCal, then this section is for you. xCal is a .NET-based solution made to run on Windows and Unix-based systems (via Mono). Hence, its source files are logically arranged in similar hierarchies as in Visual Studio/Projects. That is: 

```
xcal.sln/
├── servers/
|   ├── xcal.server.web.dev1.csproj
|   ├── xcal.servers.web.dev2.csproj
|   ├── xcal.servers.web.prod1.csproj
|   ├── xcal.servers.web.local.csproj
├── crosscut/
|   ├── xcal.crosscut.concretes.csproj
|   ├── xcal.crosscut.contracts.csproj
├── domain/
|   ├── xcal.domain.csproj
├── service/
|   ├── xcal.service.clients.concretes.csproj
|   ├── xcal.service.interfaces.concretes.csproj
|   ├── xcal.service.interfaces.contracts.csproj
|   ├── xcal.service.operations.contracts.csproj
|   ├── xcal.service.plugins.formats.concretes.csproj
|   ├── xcal.service.repositories.concretes.csproj
|   ├── xcal.service.repositories.contracts.csproj
|   ├── xcal.service.validators.concretes.csproj
├── tests/
|   ├── xcal.test.concretes.csproj
|   ├── xcal.test.contracts.csproj
```

### Development Tools for xCal

xCal is written entirely in C# - therefore it is recommended to use a specialized IDE such as [Microsoft Visual Studio][5] whose editor, debugger and compiler features facilitate rapid application development. As a matter of fact, Microsoft now  offers [Visual Studio Community][6] versions at no cost to an unlimited number of users developing open source projects. In any case, if Visual Studio does not suit your tastes, please feel free to use other third-party tools e.g. [Xamarin Studio / Monodevelop](http://www.monodevelop.com/download/). 

*For the remainder of this Get Started section and for the sake of simplicity, it would be assumed you use Visual Studio 2013 for software development.*

### Setting up xCal
Many would agree that the setup process of a software project should be rendered as less painful as possible. This often implies minimal configuration and non-complex set up procedures. Consequently, setting up xCal has been simplified to a **one-click** process.

#### Setup from downloaded zipped source file
1. Extract source code to desired location
2. Open the *solution* folder and search for the \*.sln file
3. Double-click the file -> Visual Studio starts up and loads the entire sloution.


#### Setup through cloning in GitHub desktop application 
1. Open locally-installed instance of Github desktop application.
2. Chose to clone source and select local storage location.
3. Open the *solution* folder and search for the \*.sln file.
4. Double-click the file -> Visual Studio starts up and loads the entire solution.

### Building xCal
Building xCal is also a straightforward process - you do not have to worry about the dependencies because these are automatically resolved during build process. However, you may need a running internet connection on the first build, in order to enable Visual Studio download the necessary [NuGet](https://www.nuget.org/) Packages. 

To build xCal, please do perform the following steps:

1. Select the root of the Solution Explorer Tree.
2. Right-click and choose *Rebuild Solution* from context menu.

### Executing xCal
It is now time to see xCal run... although a few more steps are required before this final goal is achieved. These involve the installation and configuration of necessary backend servers (if not yet installed on the host computer). Depending on the selected application server from the download package, you have the choice of using a MySQL (relational database) or a Redis (one of the fastest NoSQL datastore) as the backend server. 

It is highly recommended for beginners to start with the local application server (*xcal.application.server.web.local*), which been configured for testing on a local machine. The default data backend of this server is MySQL, even though the user can alternatively use a Redis backend through a provided switch in the project settings. 

Assuming you are a newbie and xCal is opened in Visual Studio, please perform the following steps:
#### Visual Studio Configuration
1. Select the project **xcal.application.server.web.local** under the logical application folder.
2. Right-click on the project and choose **Set as Startup Project**
3. Right-click on the project again and choose **Properties**
4. In the **Properties** dialog, go to **Settings** and check the username and password of the *mysql_server* setting.

#### MySQL Configuration
1. Start up MySQL Workbench  as **root** user (if not yet started).
2. Choose the server instance **Local Instance MySQLxx**
3. Under the **Management** section click on **Users and Privileges**
4. Add a new user account **local** and use the same username and password obtained from Step 4. of the Visual Studio configuration.
5. In the **Administrative Roles** tab check that all **Roles**, as well as **Global Privileges** are ticked.
6. Apply changes.

#### ..and finally...
1. Go back to Visual Studio
2. Trigger the **Run** button (or press **F5**) -> *voila!* xCal is up and running :)

### Deploying xCal
If you downloaded the xCal source and setup the projects using Visual Studio, then deplyoment of the binaries should be simple with Visual Studio's **Publish** option. Of course, if you prefer to deploy xCal through other means, it is up to you to decide on the third-party deployment tools you would like to employ ;)

Dependencies
============
xCal depends on the awesome open source software libraries below, in order to be compiled, executed and tested. Furthermore, xCal integrates and extends the source code of some of these libraries for its core functionality. Each referenced library is released under its respective license.

The following development library is referenced in all of the xcal projects:
* [Semantic Git Versioning](https://www.nuget.org/packages/SemanticGit/) [(license)](https://github.com/kzu/SemanticGit/blob/master/LICENSE)

The following library is referenced in almost all of the xcal projects:
* [Service Stack V3](https://github.com/ServiceStackV3/ServiceStackV3) [(license)](https://github.com/ServiceStack/ServiceStack/blob/v3/LICENSE)

The following libraries are only referenced in the xcal test projects (*xcal.test.server.integration.**):
* [NBuilder](https://www.nuget.org/packages/NBuilder/) [(license)](http://www.gnu.org/licenses/lgpl.html)
* [xUnit](https://www.nuget.org/packages/xunit/1.9.2) as transitive reference to [xunit](https://github.com/xunit/xunit) [(license)](https://github.com/xunit/xunit/blob/master/license.txt)

The following libraries are only referenced in the xcal application projects (*xcal.application.server.web.**):
* [ELMAH on MySQL](https://www.nuget.org/packages/elmah.mysql/) as transitive reference to [Elmah](https://code.google.com/p/elmah/) [(license)](http://www.apache.org/licenses/LICENSE-2.0)
* [Nlog](https://github.com/NLog/NLog/) as transitive reference to [Nlog Project](http://nlog-project.org/) [(license)](https://raw.githubusercontent.com/NLog/NLog/master/LICENSE.txt)

Contributing
============
We are very happy that you are interested in contributing to xCal. [Contributing to the repository on GitHub](https://guides.github.com/activities/contributing-to-open-source/) is quite easy; fork the repository, make changes and send a pull request on Github.

However before any contributed code is reviewed, the contributor is required to accept the [Individual Contributor Assignment Agreement](http://goo.gl/forms/hvyoqegA6s).

To ensure high quality of code, contributors must include unit tests alongside their source code. Documentation of source code shall also be highly appreciated.

Documentation
=============
The [xCal Wiki](https://github.com/reexmonkey/xcal/wiki) provides detailed information on the xCal project, tutorials, and blogs on auxillary software engineering topics for newbies such as design principles, continuous integration and web services.

Versioning
==========
For better release management and backward commpability, the assemblies of xCal are maintained under the [Semantic Versioning](http://semver.org/) guidelines. Moreover, the branching of the source code follows the [GitFlow](http://nvie.com/posts/a-successful-git-branching-model/) branching model proposed by Vincent Driessen.

Community
==========
xCal is created by [Emmanuel Ngwane](https://github.com/reexmonkey) and maintained by [ collaborators](https://github.com/orgs/reexjungle/people) with the support and involvement of the community.

Keep track on upcoming features, development and community news:
* Chat on [**xCal Google+ Community**](https://plus.google.com/communities/105811904931972542578)
* Follow [**@xcal5545**](https://twitter.com/xcal5545) on twitter.
 

License
=======
Copyright (c) 2014, Emmanuel Ngwane and contributors. All rights reserved.
xCal source code is released under the [BSD](https://github.com/reexmonkey/xcal/blob/master/LICENSE) License. Its documentation is released under the [Creative Commons](https://github.com/reexmonkey/xcal/blob/master/docs/LICENSE) license.

[1]: http://www.computerworld.com/article/2486991/app-development-4-reasons-companies-say-yes-to-open-source.html?page=1 "4 reasons companies say yes to open source"
[2]: https://reexux.com/xcal/dev1/metadata "Live Demo of xCal Web Services"
[3]: https://github.com/reexmonkey/xcal/archive/master.zip "Download xCal as ZIP file"
[4]: github-windows://openRepo/https://github.com/reexmonkey/xcal "Clone the master repository"
[5]: http://www.visualstudio.com/ "Visual Studio"
[6]: http://www.visualstudio.com/en-us/visual-studio-community-vs.aspx "Visual Studio Community 2013"
