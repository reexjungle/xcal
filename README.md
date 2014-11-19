What is xCal?
============
The xCal project is an **open-source** project that basically does 2 things:

1. Implements the [Internet Calendaring and Scheduling Core Object Specification (RFC 5545)](http://tools.ietf.org/html/rfc5545) such that calendaring and scheduling information (events, to-dos, journals and free/busy) can be represented and exchanged in the [iCalendar](http://en.wikipedia.org/wiki/ICalendar) data format independently of calendar service or protocol.

2. Develops a sofware service library to facilitate the setup and hosting of [REST](http://en.wikipedia.org/wiki/Representational_state_transfer)-ful calendar services, in which operations to access and edit distributed calendar data are exposed by means of a calendar API.

Why xCal?
=========
First of all it is free - yes **free** as in *free air* and not as in *free beer* ditto *with no strings attached*. 

Secondly, it allows you **to host and maintain your calendar data on your own server** - that is, it respects your privacy by giving you total control over your data and thus eliminates the risk of sharing your data with unkown third parties.

Thirdly, xCal also supports (in addition to iCalendar) various calendar data representations: 
* [CSV](http://en.wikipedia.org/wiki/Comma-separated_values)
* [MessagePack](http://msgpack.org/)
* [JSON](http://en.wikipedia.org/wiki/JSON)
* [JSV](http://mono.servicestack.net/docs/text-serializers/jsv-format) 
* [XML](http://en.wikipedia.org/wiki/XML)

Fourthly, as a developer, you do not need to *reinvent the wheel* when tasked with incorporating a time-based system (e.g. reservation/booking systems) in your applications. xCal takes this burden off your shoulders by reducing the overhead to simple calls from your client application (though the API) to the calendar web server, thereby allowing you to concentrate on the goals of your application.  

Finally since xCal is open-source, you gain the awesome [benefits][1] of using open source software, which include low-cost (*zero-cost in this case*), quality improvement through continuous community input, business agility and mitigation of business risks. 

Table of Contents
=================
1. [Get Started](https://github.com/reexmonkey/xcal/#get-started)
2. [Dependencies](https://github.com/reexmonkey/xcal/#contributing)
3. [Contributing](https://github.com/reexmonkey/xcal/#contributing)
4. [Community](https://github.com/reexmonkey/xcal/#community)
5. [Documentation](https://github.com/reexmonkey/xcal/#documentation)
6. [Versioning](https://github.com/reexmonkey/xcal/#versioning)
7. [Contact](https://github.com/reexmonkey/xcal/#contact)
8. [License](https://github.com/reexmonkey/xcal/#license)


Get Started 
===========
To get started on the xCal, you might want to choose one of the following options:

1. Preview a [demo][2] of a running xCal server. 
2. Download the master repository [ZIP][3] file from GitHub.
3. [Clone][4] the master repository on desktop using the Github application. 

### Contents of Download Package

The download package contains source codes and versioning files of xCal. It is also worth mentioning here that xCal is a .NET project and thus source files are logically associated to respective project files (\*.csproj), which in turn are linked to a single solution file (\*.sln) as illustrated below: 

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

xCal is written entirely in C# and therefore it is recommended to use a specialized IDE such as [Microsoft Visual Studio][5] whose editor, debugger and compiler features facilitate the maintenance, compiling and testing of the source files. As a matter of fact, Microsoft now  offers [Visual Studio Community 2013][6] at no cost to an unlimited number of users developing open source projects. However if Visual Studio does not suit your tastes, please feel free to use other third-party text editors and development tools. 

*For the remainder of this Get Started section, it would be assumed for the sake of simplicity, that you are using visual studio as the main development tool.*

### Setting up xCal
Many would agree that the setup process of a software project should be as less painful as possible. This implies minimal configurations or setup with a click. Following, the setup of xCal is fairly straight-forward as described below depending on which download opton you chose above.

#### Source Code downloaded as ZIP file
1. Extract source code to desired location
2. Open the *solution* folder and search for the \*.sln file
3. Double-click the file and Visual Studio starts up and loads the entire project tree.


#### Source Code Cloned directly in GitHub desktop application 
1. Local Github desktop application starts up when the clone [link][4] is clicked.
2. Chose location where code should be cloned.
3. Open the *solution* folder and search for the \*.sln file.
4. Double-click the file and Visual Studio starts up and loads the entire project tree.


### Building xCal
1. Select the root of the Solution Explorer Tree
2. Right-click and choose *Rebuild Solution* from context menu

#### *Notes on Building xCal*
* You need a running internet connection on first build for Visual Studio to automatically download Nuget Packages for dependencies.

### Running xCal
1. Select the *xcal.application.server.web.local* as start project.
2. Run the project -> voila! xCal starts running!

#### *Notes on Running xCal*
* All dependencies should be correctly-configured for xCal to run successfully (More on [Dependencies][])

### Deploying xCal


### Done with project setup - any further configuration?
Speaking from the heart, there is no greater killjoy (*perhaps with the exception of bugging exceptions*) than an initial waste of precious development time with mindless extra configurations during project setup. As such, the configuration of xCal is kept to the minimum. This means, no need to fret about maintaining external library dependencies, which happen to be [NuGet](https://www.nuget.org/) packages that are automatically and happily resolved on the first project build. 

Nevertheless, xCal also depends on backend data management systems for logging and data storage purposes. In fact, xCal uses the [Service Stack V3](https://github.com/ServiceStackV3/ServiceStackV3) lightweight [Ormlite](https://github.com/ServiceStack/ServiceStack.OrmLite/tree/v3) and [Redis](https://github.com/ServiceStack/ServiceStack.Redis/tree/v3) libraries to respectively support either [relational database management systems](http://en.wikipedia.org/wiki/Relational_database_management_system) (*we all know [MySQL](http://www.mysql.com/)- don't we?*) or the respectable [Redis](http://redis.io/) NoSQL database. In particular, the backend systems must pre-exist on the host machine and consequently access to them must be spcified in the application settings file, as well as configuration file of [Nlog](http://nlog-project.org/); one of the logging providers used by xCal. 


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
[4]: https://github.com/reexmonkey/xcal.git "Clone the master repository"
[5]: http://www.visualstudio.com/ "Visual Studio"
[6]: http://www.visualstudio.com/en-us/visual-studio-community-vs.aspx "Visual Studio Community 2013"
