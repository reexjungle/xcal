Not another xCal again!
==============================
You may be forgiven if you only thought of xCal as the [XML](http://en.wikipedia.org/wiki/XML) representation of the [iCalendar](http://en.wikipedia.org/wiki/ICalendar) format([RFC 6321](http://tools.ietf.org/html/rfc6321)). However, there is *another* xCal, which implements the [iCalendar](http://en.wikipedia.org/wiki/ICalendar) specification ([RFC 5545](http://tools.ietf.org/html/rfc5545)) and in addition provides a web service interface for calendar data to be distributed under various content types such as [CSV](http://en.wikipedia.org/wiki/Comma-separated_values), [JSON](http://en.wikipedia.org/wiki/JSON), [JSV](http://mono.servicestack.net/docs/text-serializers/jsv-format), iCalendar format and of course XML. This *so-called* xCal is a project started by [Emmanuel Ngwane](https://twitter.com/ngwanemk) (*after his many frustrated attempts to find a free open-source calendar web service*) with the support and involvement of the community. 


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
### I am a devloper, How do I obtain a fresh copy of the latest xCal code?

here are 2 quick ways to receive the code:

* [Download as ZIP file from the master repository](https://github.com/reexmonkey/xcal/archive/master.zip)
* Clone the master repository: https://github.com/reexmonkey/xcal.git


### Got the source code already - what next?

xCal is written entirely in C# and in case your favorite [IDE](http://en.wikipedia.org/wiki/Integrated_development_environment) is [Microsoft Visual Studio](http://www.visualstudio.com/), then the setup is guaranteed to be *pain-free* (*yeah you do not need pain-killers at all*). All, you need to do is to look for the solution file **xcal.sln** in the downloaded package, double-click it and *voila* - all the projects are automatically loaded. Project dependencies of the projects are in the form of [NuGet](https://www.nuget.org/) packages, which are automatically downloaded during first project build.   

However, you may not be a Visual Studio enthusiast - perhaps a hardcore programmer who types on a basic text editor and uses command line tools to build the code or an expert user of another third-party .NET IDE. Well, in that case you may have to manually manage the project folders and files but lo! do not fear it is not a daunting task either - the following illustration hepls provide an overview of the logical layout of the directories (*see the forward slashes?*) and project files(*all that ends with .csproj...*).

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

### xCal project is setup - further configuration needed?
Speaking from the heart, this project is aimed primarily at developers and from experience no killjoy (*except broken code*) is worse than wasting a devloper's time with extra configurations before getting down to core work. As such, the configuration of the project has been reduced to minimal. That is, the library dependencies have been taken care of by Nuget packages. Nevertheless, the usual configuration culprit (data sources) need to be added by hand. 

The [xCal Wiki](https://github.com/reexmonkey/xcal/wiki) contains more information on the project, web service, its architecture, dependencies, code examples and more...


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
