What is xCal?
============
While the answer to this question is fairly straightforward, it is also troublesome; considering the fact that the term ***xCal*** may also ambiguously refer to the XML representation of the [iCalendar](http://en.wikipedia.org/wiki/ICalendar) format ([RFC 6321](http://tools.ietf.org/html/rfc6321)) or other calendar-unrelated applications. 

However, without much fuss, *this* particular xCal is an open source project that basically does 2 things:

1. Implements the [Internet Calendaring and Scheduling Core Object Specification](http://en.wikipedia.org/wiki/ICalendar) ([RFC 5545](http://tools.ietf.org/html/rfc5545))

2. Provides [REST](http://en.wikipedia.org/wiki/Representational_state_transfer)-compliant web services for the above implementation, by which calendar data is communicated under different popular data formats including [CSV](http://en.wikipedia.org/wiki/Comma-separated_values), [MessagePack](http://msgpack.org/), [JSON](http://en.wikipedia.org/wiki/JSON), [JSV](http://mono.servicestack.net/docs/text-serializers/jsv-format) and of course [XML](http://en.wikipedia.org/wiki/XML).


Why xCal?
========
Yeah - *why reinvent the wheel*, when existing calendar-based applications such as [Google Calendar](https://www.google.com/calendar), [Yahoo Calendar] (https://calendar.yahoo.com/), Apple's [Calender (formerly iCal)](https://www.apple.com/icloud/#ccm) and [Microsoft Outlook](http://products.office.com/en-us/outlook/email-and-calendar-software-microsoft-outlook) are popular and often used to schedule and exchange calendaring information? Furthermore, why build web services when existing cloud services exist to synchronize user calendar data with networked devices? Also, isn't it an overkill to provide web services, when there is already a standardized extension to WebDAv - [CalDav](http://en.wikipedia.org/wiki/CalDAV) ([RFC 4791](http://tools.ietf.org/html/rfc4791)), which allows for the accessibility and sharing of calendar data on remote servers?  




Table of Contents
=================
1. [First things first](https://github.com/reexmonkey/xcal/#first-things-first)
2. [Contributing](https://github.com/reexmonkey/xcal/#contributing)
3. [Community](https://github.com/reexmonkey/xcal/#community)
4. [Documentation](https://github.com/reexmonkey/xcal/#documentation)
5. [Versioning](https://github.com/reexmonkey/xcal/#versioning)
6. [Contact](https://github.com/reexmonkey/xcal/#contact)
7. [License](https://github.com/reexmonkey/xcal/#license)


First things first 
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
