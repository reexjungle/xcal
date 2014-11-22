What is xCal?
============
The xCal project is an **open-source** project that basically does 2 things:

1. Implements the [Internet Calendaring and Scheduling Core Object Specification (RFC 5545)](http://tools.ietf.org/html/rfc5545) such that calendaring and scheduling information (events, to-dos, journals and free/busy) can be represented and exchanged in the [iCalendar](http://en.wikipedia.org/wiki/ICalendar) data format independently of calendar service or protocol.

2. Develops a sofware service library to facilitate the setup and hosting of [REST](http://en.wikipedia.org/wiki/Representational_state_transfer)-ful calendar services, in which operations to access and edit distributed calendar data are exposed by means of an [API](http://en.wikipedia.org/wiki/Application_programming_interface).

Why xCal?
=========
First of all, xCal is **free** - yes  *'free' as in 'free air' and not 'free' as in 'free beer'*. 

Secondly, xCal allows you **to host and maintain your calendar data on your own server** - that is, you have total control over your calendar information and thus in no way is your private data shared with third parties.

Thirdly, you shall enjoy distributing calendar information through the multiple data formats xCal currently supports: 
* [iCalendar/vCalendar](http://en.wikipedia.org/wiki/ICalendar) 
* [CSV](http://en.wikipedia.org/wiki/Comma-separated_values)
* [MessagePack](http://msgpack.org/)
* [JSON](http://en.wikipedia.org/wiki/JSON)
* [JSV](http://mono.servicestack.net/docs/text-serializers/jsv-format) 
* [XML](http://en.wikipedia.org/wiki/XML)

Fourthly, xCal aids you not to *reinvent the wheel*. Instead of creating your own custom calendaring and scheduling system, xCal readily provides you an implementation of the internet standard (RFC 5545), as well as frees up the potential development time you could have wasted, so that you can concentrate on other important tasks at hand.

Finally, you gain the awesome [benefits][1] which xCal as an open software provides. These include low costs, quality improvement through continuous community input, business agility and mitigation of business risks. 

Table of Contents
=================
1. [Get Started](https://github.com/reexmonkey/xcal/#get-started)
2. [Dependencies](https://github.com/reexmonkey/xcal/#dependencies)
3. [Contributing](https://github.com/reexmonkey/xcal/#contributing)
4. [Community](https://github.com/reexmonkey/xcal/#community)
5. [Documentation](https://github.com/reexmonkey/xcal/#documentation)
6. [Versioning](https://github.com/reexmonkey/xcal/#versioning)
7. [Contact](https://github.com/reexmonkey/xcal/#contact)
8. [License](https://github.com/reexmonkey/xcal/#license)


Get Started 
===========
To get started on the xCal, you might want to choose one of the following options:

1. Preview a [demo][2] of xCal web serivices. 
2. Download the master repository [ZIP][3] file from GitHub.
3. [Clone][4] the master repository on desktop using the Github application. 

### Contents of Download Package

The download package contains source codes and project files of xCal. It is also worth mentioning here that xCal is a .NET project and thus source files are logically associated to respective project files (\*.csproj), which in turn are linked to a single solution file (\*.sln) as illustrated below: 

```
solution/
├── application/
|   ├── xcal.application.server.web.dev1.csproj
|   ├── xcal.application.server.web.dev2.csproj
|   ├── xcal.application.server.web.prod1.csproj
|   ├── xcal.application.server.web.prod2.csproj
|   ├── xcal.application.server.web.local.csproj
├── crosscut/
|   ├── crosscut.operations.concretes.csproj
|   ├── crosscut.operations.contracts.csproj
|   ├── crosscut.security.concretes.csproj
├── domain/
|   ├── xcal.domain.csproj
├── foundation/
|   ├── foundation.essential.concretes.csproj
|   ├── foundation.essential.contracts.csproj
├── infrastructure/
|   ├── infrastructure.essential.concretes.csproj
|   ├── infrastructure.essential.contracts.csproj
├── service/
|   ├── xcal.service.clients.concretes.csproj
|   ├── xcal.service.formats.concretes.csproj
|   ├── xcal.service.interfaces.concretes.csproj
|   ├── xcal.service.interfaces.contracts.csproj
|   ├── xcal.service.repositories.concretes.csproj
|   ├── xcal.service.repositories.contracts.csproj
|   ├── xcal.service.validators.concretes.csproj
├── technical/
|   ├── technical.data.concretes.csproj
|   ├── technical.data.contracts.csproj
├── tests/
|   ├── xcal.applications.server.web.local.csproj

```

### Development Tools for xCal

xCal is written entirely in C# - therefore it is recommended to use a specialized IDE such as [Microsoft Visual Studio][5] whose editor, debugger and compiler features facilitate rapid application development. As a matter of fact, Microsoft now  offers [Visual Studio Community 2013][6] at no cost to an unlimited number of users developing open source projects. In any case, if Visual Studio does not suit your tastes, please feel free to use other third-party tools. 

*For the remainder of this Get Started section and for the sake of simplicity, it would be assumed you use Visual Studio 2013 for software development.*

### Setting up xCal
Many would agree that the setup process of a software project should be rendered as less painful as possible. This often implies minimal configuration and non-complex set up procedures. Consequently, setting up xCal has been simplified to a **one-click** process.

##### Setup from downloaded zipped source file
1. Extract source code to desired location
2. Open the *solution* folder and search for the \*.sln file
3. Double-click the file -> Visual Studio starts up and loads the entire sloution.


##### Setup through cloning in GitHub desktop application 
1. Open locally-installed instance of Github desktop application.
2. Chose to clone source and select local storage location.
3. Open the *solution* folder and search for the \*.sln file.
4. Double-click the file -> Visual Studio starts up and loads the entire solution.

### Building xCal
Building xCal is also a straightforward process - you do not have to worry about the dependencies because these are automatically resolved during build process. However, you may need a running internet connection on the first build, in order to enable Visual Studio download the necessary [NuGet](https://www.nuget.org/) Packages. 

To build xCal, please do perform the following steps:
1. Select the root of the Solution Explorer Tree
2. Right-click and choose *Rebuild Solution* from context menu

### Running xCal
After a successful build, it is time to enjoy the highly anticipated moment i.e. seeing xCal run! Well, a few more things need to be done before reaching this desired stage. This involves installing the necessary backend servers (if not yet on the host computer) and configuring them. Depending on the selected application server (there are 4 in the download package), you have the choice of installing a MySQL server (relational database) or a Redis server (a NoSQL datastore). it is recommended for beginners to choose the local application server (*xcal.application.server.web.local*), whose default data backend is MySQL server and perform the following steps (assuming you have opened xCal in Visual Studio).

##### Visual Studio Configuration
1. Select the project **xcal.application.server.web.local** under the logical application folder.
2. Right-click on the project and choose **Set as Startup Project**
3. Right-click on the project again and choose **Properties**
4. In the **Properties** dialog, go to **Settings** and check the username and password of the *mysql_server* setting.

##### MySQL Configzration
1. Start up MySQL Workbench  as **root** user (if not yet started).
2. Choose the server instance **Local Instance MySQLxx**
3. Under the **Management** section click on **Users and Privileges**
4. Add a new user account **local** and use the same username and password obtained from Step 4. of the Visual Studio configuration.
5. In the **Administrative Roles** tab check that all **Roles**, as well as **Global Privileges** are ticked.
6. Apply changes.

Go back to Visual Studio and trigger the **Run** button -> voila! xCal is up and running :)

### Deploying xCal


###### For further information on the project, web service, its architecture, dependencies, code examples and more, please do not hesitate to visit the [xCal Wiki](https://github.com/reexmonkey/xcal/wiki) 


Dependencies
============

Contributing
============


Community
==========


Documentation
=============


Versioning
==========
For better release management and backward commpability, the assemblies of xCal are maintained under the [Semantic Versioning](http://semver.org/) guidelines. Moreover, the branching of the source code follows the *Development and Release Isolation Strategy* of the [Version Control Guide](http://vsarbranchingguide.codeplex.com/releases) proposed by Microsoft Visual Studio ALM Rangers.

Contact
========
* https://github.com/reexmonkey
* https://twitter.com/ngwanemk


License
=======
Copyright (c) 2014, Emmanuel Ngwane. All rights reserved.
xCal source code is released under the [BSD](https://github.com/reexmonkey/xcal/blob/master/LICENSE) License. Its documentation is released under the [Creative Commons](https://github.com/reexmonkey/xcal/blob/master/docs/LICENSE) license.

[1]: http://www.computerworld.com/article/2486991/app-development-4-reasons-companies-say-yes-to-open-source.html?page=1 "4 reasons companies say yes to open source"
[2]: https://reexux.com/xcal/dev1/metadata "Live Demo of xCal Web Services"
[3]: https://github.com/reexmonkey/xcal/archive/master.zip "Download xCal as ZIP file"
[4]: github-windows://openRepo/https://github.com/reexmonkey/xcal "Clone the master repository"
[5]: http://www.visualstudio.com/ "Visual Studio"
[6]: http://www.visualstudio.com/en-us/visual-studio-community-vs.aspx "Visual Studio Community 2013"
