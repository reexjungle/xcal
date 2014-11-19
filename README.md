What is xCal?
============
The xCal project is an **open-source** project that basically does 2 things:

1. Implements the [Internet Calendaring and Scheduling Core Object Specification (RFC 5545)](http://tools.ietf.org/html/rfc5545) such that calendaring and scheduling information (events, to-dos, journals and free/busy) are represented in the [iCalendar](http://en.wikipedia.org/wiki/ICalendar) data format.

2. Develops a sofware library to facilitate the setup and hosting of calendar services, in which operations to access and edit calendar data are exposed through a calendar API.


Why xCal?
=========
First of all it is free - yeah **free** as in *free air* and not as in *free beer* i.e. *"free with no strings attached"*. 

Secondly, it allows you to host and maintain your  calendar data on your own private data - it respects your privacy by giving you total control over your data and thus eliminates the risk of sharing your data with unkown third parties.

Thirdly, as a developer, you do not need to *reinvent the wheel* when tasked with incorporating a time-based system (e.g. reservation/booking systems) in your applications. xCal takes this burden off your shoulders by reducing the overhead to simple calls from your client application (though the API) to the calendar web server, thereby allowing you to concentrate on the goals of your application.  

Fourthly, since it open-source xCal gains the same benefits as other open source software such as low-cost of usage (*indeed free in this case*), quality improvement through continuous community input, business agility and mitigation of business risks. 


Table of Contents
=================
1. [Getting Started](https://github.com/reexmonkey/xcal/#getting-started)
2. [Contributing](https://github.com/reexmonkey/xcal/#contributing)
3. [Community](https://github.com/reexmonkey/xcal/#community)
4. [Documentation](https://github.com/reexmonkey/xcal/#documentation)
5. [Versioning](https://github.com/reexmonkey/xcal/#versioning)
6. [Contact](https://github.com/reexmonkey/xcal/#contact)
7. [License](https://github.com/reexmonkey/xcal/#license)


Getting Started 
==================
### How to obtain xCal run-times 


### Nope, how to get xCal source code

Here are 2 quick ways to receive the code:

* [Download as ZIP file from the master repository](https://github.com/reexmonkey/xcal/archive/master.zip)
* Clone the master repository: https://github.com/reexmonkey/xcal.git


### Got the source code already - what next?

xCal is written entirely in C# and in case your favorite [IDE](http://en.wikipedia.org/wiki/Integrated_development_environment) is [Microsoft Visual Studio](http://www.visualstudio.com/), then the setup is guaranteed to be *pain-free* (*yeah you do not need the pain-killers*). All, you have to do is look for the solution file **xcal.sln** in the downloaded package, double-click it and *voila* - xCal project tree is automatically loaded. 

However, you may not be a Visual Studio enthusiast - perhaps a hardcore programmer who types on a basic text editor and uses command line tools to build the code or an expert user of another third-party .NET IDE. Well, in that case you may have to manually manage the project folders and files but lo! do not fear it is not a daunting task either - the following illustration hepls provide an overview of the logical layout of the directories and project files (*~~not~~ all that ends with .csproj,...*).

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

### Done with project setup - any further configuration?
Speaking from the heart, there is no greater killjoy (*perhaps with the exception of bugging exceptions*) than an initial waste of precious development time with mindless extra configurations during project setup. As such, the configuration of xCal is kept to the minimum. This means, no need to fret about maintaining external library dependencies, which happen to be [NuGet](https://www.nuget.org/) packages that are automatically and happily resolved on the first project build. 

Nevertheless, xCal also depends on backend data management systems for logging and data storage purposes. In fact, xCal uses the [Service Stack V3](https://github.com/ServiceStackV3/ServiceStackV3) lightweight [Ormlite](https://github.com/ServiceStack/ServiceStack.OrmLite/tree/v3) and [Redis](https://github.com/ServiceStack/ServiceStack.Redis/tree/v3) libraries to respectively support either [relational database management systems](http://en.wikipedia.org/wiki/Relational_database_management_system) (*we all know [MySQL](http://www.mysql.com/)- don't we?*) or the respectable [Redis](http://redis.io/) NoSQL database. In particular, the backend systems must pre-exist on the host machine and consequently access to them must be spcified in the application settings file, as well as configuration file of [Nlog](http://nlog-project.org/); one of the logging providers used by xCal. 


###### For further information on the project, web service, its architecture, dependencies, code examples and more, please do not hesitate to visit the [xCal Wiki](https://github.com/reexmonkey/xcal/wiki) 


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
