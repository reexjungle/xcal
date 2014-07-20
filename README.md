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
There are 2 quick ways experience at firsthand the latest release of xCal source code:

* [Download as ZIP file from the master repository](https://github.com/reexmonkey/xcal/archive/master.zip)
* Clone the master repository: https://github.com/reexmonkey/xcal.git


As a project initially created with Visual Studio 2013 Express, the logical structure resembles closely the typical directory structure proposed by Visual Studio; albeit with minor differences. Indeed, if you are using Visual studio, it suffices to double-click on the solution file **xcal.sln** file and *voila* - you are *done!* 

Otherwise, you will find the following logical layout of directories and projects(*visual-studio-speak*) helpful:

```
solution/
|-------application
        |----------xcal.application.server.web.dev1

```


Contributing
============


Community
==========


Documentation
=============


Versioning
==========
For better release management and backward commpability, the assemblies of xCal are maintained under the [Semantic Versioning](https://github.com/twbs/bootstrap) guidelines. Moreover, the branching of the source code follows the *Development and Release Isolation Strategy* of the [Version Control Guide](http://vsarbranchingguide.codeplex.com/releases) proposed by Microsoft Visual Studio ALM Rangers.

Contact
========
* https://github.com/reexmonkey
* https://twitter.com/ngwanemk


License
=======
Copyright (c) 2014, Emmanuel Ngwane
All rights reserved.
xCal source code is released under the [BSD](https://github.com/reexmonkey/xcal/blob/master/LICENSE) License. Its documentation is released under the [Creative Commons](https://github.com/reexmonkey/xcal/blob/master/docs/LICENSE) license.
