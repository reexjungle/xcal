using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceStack.ServiceClient.Web;
using reexmonkey.foundation.essentials.contracts;
using reexmonkey.foundation.essentials.concretes;
using reexmonkey.infrastructure.operations.concretes;
using reexmonkey.xcal.domain.contracts;
using reexmonkey.xcal.domain.models;
using reexmonkey.xcal.domain.operations;


namespace reexmonkey.xcal.application.server.web.dev.test
{
    [TestClass]
    public class EventServiceTests
    {
        [TestMethod]
        public void PublishMinimalEvent()
        {
            var sclient = new JsonServiceClient(Properties.Settings.Default.test_server);
            var published = sclient.Post<VCALENDAR>(new PublishEvent
            {
                ProductId = new FPIKeyGenerator<string>()
                {
                    Owner = Properties.Settings.Default.fpiOwner,
                    LanguageId = Properties.Settings.Default.fpiLanguageId,
                    Description = Properties.Settings.Default.fpiDescription,
                    Authority = Properties.Settings.Default.fpiAuthority
                }.GetNextKey(),

                Events = new List<VEVENT> 
                    {
                        new VEVENT
                        {
                            Uid = new GuidKeyGenerator().GetNextKey(),
                            Organizer = new ORGANIZER
                            {
                                CN = "Emmanuel Ngwane",
                                Address = new URI("ngwanemk@gmail.com"),
                                Language = new LANGUAGE("en")
                            },
                            Location = new LOCATION
                            {
                                Text = "Düsseldorf",
                                Language = new LANGUAGE("de", "DE")
                            },

                            Summary = new SUMMARY("Test Meeting"),
                            Description = new DESCRIPTION("A test meeting for freaks"),
                            Start = new DATE_TIME(new DateTime(2014, 6, 15, 16, 07, 01, 0, DateTimeKind.Utc)),
                            End = new DATE_TIME(new DateTime(2014, 6, 15, 18, 03, 08, 0, DateTimeKind.Utc)),
                            Status = STATUS.CONFIRMED,
                            Transparency = TRANSP.TRANSPARENT,
                            Classification = CLASS.PUBLIC
                        }
                    
                    },
                TimeZones = null
            });

            Assert.AreNotEqual(published, null);
            Assert.AreEqual(published.Method, METHOD.PUBLISH);

            var pevent = published.Components[0] as VEVENT;
            Assert.AreNotEqual(pevent, null);
            Assert.AreEqual(pevent.Start.ToString(), "20140615T160701Z");
            Assert.AreEqual(pevent.Duration.ToString(), "PT1H56M7S");
            
        }
    }
}
