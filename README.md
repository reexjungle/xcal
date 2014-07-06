xCal
====

xCal is a system that implements the [iCalendar](http://en.wikipedia.org/wiki/ICalendar) format as specified by the internet protocol [RFC 5545](http://tools.ietf.org/html/rfc5545). Moreover xCal also provides a calendar web services supporting various request and content content types such as as xml, csv, json, jsv and icalendar. xCal is created by [Emmanuel Ngwane](https://twitter.com/ngwanemk) with the support and involvement of the community.

 

Table of Contents
=================
1. [A Brief History](https://github.com/reexmonkey/xcal/#a-brief-history) 
1. [Getting Started](https://github.com/reexmonkey/xcal/#getting-started)
2. [Contributing](https://github.com/reexmonkey/xcal/#contributing)
3. [Community](https://github.com/reexmonkey/xcal/#community)
4. [Documentation](https://github.com/reexmonkey/xcal/#documentation)
5. [Versioning](https://github.com/reexmonkey/xcal/#versioning)
6. [Contact](https://github.com/reexmonkey/xcal/#contact)
7. [License](https://github.com/reexmonkey/xcal/#license)


A Brief History
===============
Once I was interested in building a reservation-based software that involved booking of resources and event planning. Thinking it would not be a difficult task, I dived straight into the design of the system. However in mid-design, I figured out that the "simple notion" of assigning events to calendar days, weeks and months was far more complex than I had previously thought, especially when recurrent events were involved. After a short background research I stumbled on the iCalendar format and the responsible internet protocol [RFC 5545](http://tools.ietf.org/html/rfc5545)... but c'mon developers are lazy and against conventional stereotypes, we do not like "reinventing the wheel" in critical projects, when one of the "good fellas" have created something for us. So I went on a search for a calendar system - not just a client or monolithic system; rather a calendar web service, to which I could query for iCalendar objects and receive appropriate responses. 

My findings were quite interesting but not helpful:

1. First of all, I stumbled upon [Google Calendar API](https://developers.google.com/google-apps/calendar/), which is well documented and has live demo on its usage. However, it has a few limitations which did not match to my taste, such as no total control on my calendar data, since it is hosted on Google Servers thus bringing trust issues. Also the number of queries were limited per day (not good if my reservation system needed to make more queries than permitted. Thirdly the API did not reflect much of RFC 5545 specifications.
2. Then I found [Oracle Calendar Application Developer's Guide](http://docs.oracle.com/cd/B13866_04/calendar.904/b10893/wssoap.htm), which provides a documentation on the [Calendar Web Services Toolkit]() - a close implementation of the iCalendar protocol (*although it is actually an implementation of the older RFC 2245 and 2426 specifications*) that uses SOAP for message excchange between the calendar server, which arguably arguably are less performant than "better" formats like JSON and REST


Getting Started
===============



Contributing
============


Community
==========

Documentation
=============

Versioning
==========


Contact
========
* https://github.com/reexmonkey/
* https://twitter.com/ngwanemk


License
=======
Copyright (c) 2014, Emmanuel Ngwane
All rights reserved.
xCal code is released under the [BSD](https://github.com/reexmonkey/xcal/blob/master/LICENSE) License.
