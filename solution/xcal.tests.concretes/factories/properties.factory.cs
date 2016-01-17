using System;
using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;
using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.models;
using reexjungle.xcal.tests.contracts.factories;
using reexjungle.xmisc.infrastructure.contracts;

namespace reexjungle.xcal.tests.concretes.factories
{
    public class PropertiesFactory : IPropertiesFactory
    {
        private readonly IValuesFactory valuesFactory;
        private readonly IParametersFactory parametersFactory;
        private readonly IKeyGenerator<Guid> keyGenerator;
        private readonly RandomGenerator rndGenerator;

        private readonly List<string> usernames = new List<string>
            {
            "King", 
            "Palmer",  
            "Quintin",  
            "Alexandra",  
            "Anglea",  
            "Cira",  
            "Zola",  
            "Mirella",  
            "Samuel",  
            "Elane",  
            "Stephan",  
            "Guillermo",  
            "Suellen",  
            "Shona",  
            "Melda",  
            "Tess",  
            "Tammara",  
            "Sherie",  
            "Nettie",  
            "Maggie",  
            "Merrilee",  
            "Pauletta",  
            "Stephen",  
            "Keith",  
            "Lonna",  
            "Raleigh",  
            "Burma",  
            "Delilah ", 
            "Treasa ", 
            "Myles",
              "Caesar",
              "Koba",
              "Cornelia",
              "Blue Eyes",
              "Grey",
              "Ash",
              "James Bond",
              "Jason Bourne",
              "Jack Bauer"
            };

        private readonly List<string> resources = new List<string>
            {
              "example.mp3",
              "example.wav",
              "example.mpeg",
              "example.txt",
              "examlpe.xlsx",
              "example.docx",
              "example.ical",
              "example.jpeg",
              "example.png"
            };


        private readonly List<string> categories = new List<string>
            {
                "DRESS",
                "VACATION",
                "OFFICE",
                "APPOINTMENT",
                "MEETING",
                "SICK"
            }; 

        public PropertiesFactory(IKeyGenerator<Guid> keyGenerator, IValuesFactory valuesFactory, IParametersFactory parametersFactory)
        {
            if (keyGenerator == null) throw new ArgumentNullException("keyGenerator");
            if (valuesFactory == null) throw new ArgumentNullException("valuesFactory");
            if (parametersFactory == null) throw new ArgumentNullException("parametersFactory");

            this.keyGenerator = keyGenerator;
            this.valuesFactory = valuesFactory;
            this.parametersFactory = parametersFactory;

            rndGenerator = new RandomGenerator();
        }

        public ATTACH_BINARY CreateAttachBinary()
        {
            return CreateAttachBinaries(1).First();
        }

        public IEnumerable<ATTACH_BINARY> CreateAttachBinaries(int quantity)
        {
            return Builder<ATTACH_BINARY>.CreateListOfSize(quantity)
                .All()
                .With(x => x.Id = keyGenerator.GetNext())
                .And(x => x.Content = valuesFactory.CreateBinary())
                .And(x => x.FormatType = parametersFactory.CreateFormatType())
                .Build();
        }

        public ATTACH_URI CreateAttachtUri()
        {
            return CreateAttachUris(1).First();
        }

        public IEnumerable<ATTACH_URI> CreateAttachUris(int quantity)
        {
            return Builder<ATTACH_URI>.CreateListOfSize(quantity)
                .All()
                .With(x => x.Id = keyGenerator.GetNext())
                .And(x => x.Content = valuesFactory.CreateUri())
                .And(x => x.FormatType = parametersFactory.CreateFormatType())
                .Build();
        }

        public ATTENDEE CreateAttendee()
        {
            return CreateAttendees(1).First();
        }

        public IEnumerable<ATTENDEE> CreateAttendees(int quantity)
        {
            return Builder<ATTENDEE>.CreateListOfSize(quantity)
                .All()
                .With(x => x.Id = keyGenerator.GetNext())
                .And(x => x.CN = Pick<string>.RandomItemFrom(usernames))
                .And(x => x.Address = valuesFactory.CreateEmail(x.CN))
                .And(x => x.CalendarUserType = Pick<CUTYPE>.RandomItemFrom(new[]
                {
                    CUTYPE.GROUP,
                    CUTYPE.INDIVIDUAL,
                    CUTYPE.RESOURCE,
                    CUTYPE.ROOM
                }))
                .And(x => x.Directory = parametersFactory.CreateDirectory())
                .And(x => x.Member = parametersFactory.CreateMember(usernames, rndGenerator.Next(1, quantity)))
                .And(x => x.Language = parametersFactory.CreateLanguage())
                .And(x => x.Participation = Pick<PARTSTAT>.RandomItemFrom(new[]
                {
                    PARTSTAT.ACCEPTED,
                    PARTSTAT.COMPLETED,
                    PARTSTAT.DECLINED,
                    PARTSTAT.DELEGATED,
                    PARTSTAT.IN_PROGRESS,
                    PARTSTAT.NEEDS_ACTION,
                    PARTSTAT.TENTATIVE
                }))
                .And(x => x.Role = Pick<ROLE>.RandomItemFrom(new[]
                {
                    ROLE.CHAIR,
                    ROLE.NON_PARTICIPANT,
                    ROLE.OPT_PARTICIPANT,
                    ROLE.REQ_PARTICIPANT
                }))
                .And(x => x.Rsvp = Pick<BOOLEAN>.RandomItemFrom(new[]
                {
                    BOOLEAN.FALSE,
                    BOOLEAN.TRUE
                }))
                .And(x => x.Delegatee = parametersFactory.CreateDelegatee(usernames, rndGenerator.Next(1, quantity)))
                .And(x => x.Delegator = parametersFactory.CreateDelegator(usernames, rndGenerator.Next(1, quantity)))
                .And(x => x.SentBy = parametersFactory.CreateSentBy(Pick<string>.RandomItemFrom(usernames)))
                .Build();
        }

        public CATEGORIES CreateCategories()
        {
            return CreateCategoriesList(1).First();
        }

        public IEnumerable<CATEGORIES> CreateCategoriesList(int quantity)
        {
            return Builder<CATEGORIES>.CreateListOfSize(quantity)
                .All()
                .With(x => x.Id = keyGenerator.GetNext())
                .And(x => x.Values = categories)
                .And(x => x.Language = parametersFactory.CreateLanguage())
                .Build();
        }

        public COMMENT CreateComment()
        {
            return CreateComments(1).First();
        }

        public IEnumerable<COMMENT> CreateComments(int quantity)
        {
            return Builder<COMMENT>.CreateListOfSize(quantity)
                .All()
                .With(x => x.Id = keyGenerator.GetNext())
                .And(x => x.Text = rndGenerator.Phrase(30))
                .And(x => x.AlternativeText = parametersFactory.CreateAlternativeTextRepresentation())
                .And(x => x.Language = parametersFactory.CreateLanguage())
                .Build();
        }

        public CONTACT CreateContact()
        {
            return CreateContacts(1).First();
        }

        public IEnumerable<CONTACT> CreateContacts(int quantity)
        {
            return Builder<CONTACT>.CreateListOfSize(quantity)
                .All()
                .With(x => x.Id = keyGenerator.GetNext())
                .And(x => x.Text = rndGenerator.Phrase(20))
                .And(x => x.AlternativeText = parametersFactory.CreateAlternativeTextRepresentation())
                .And(x => x.Language = parametersFactory.CreateLanguage())
                .Build();
        }

        public SUMMARY CreateSummary()
        {
            return CreateSummaries(1).First();
        }

        public IEnumerable<SUMMARY> CreateSummaries(int quantity)
        {
            return Builder<SUMMARY>.CreateListOfSize(quantity)
                .All()
                .With(x => x.Id = keyGenerator.GetNext())
                .And(x => x.Text = rndGenerator.Phrase(20))
                .And(x => x.AlternativeText = parametersFactory.CreateAlternativeTextRepresentation())
                .And(x => x.Language = parametersFactory.CreateLanguage())
                .Build();
        }

        public LOCATION CreateLocation()
        {
            return CreateLocations(1).First();
        }

        public IEnumerable<LOCATION> CreateLocations(int quantity)
        {
            return Builder<LOCATION>.CreateListOfSize(quantity)
                .All()
                .With(x => x.Id = keyGenerator.GetNext())
                .And(x => x.Text = rndGenerator.Phrase(20))
                .And(x => x.AlternativeText = parametersFactory.CreateAlternativeTextRepresentation())
                .And(x => x.Language = parametersFactory.CreateLanguage())
                .Build();
        }

        public DESCRIPTION CreateDescription()
        {
            return CreateDescriptions(1).First();
        }

        public IEnumerable<DESCRIPTION> CreateDescriptions(int quantity)
        {
            return Builder<DESCRIPTION>.CreateListOfSize(quantity)
                .All()
                .With(x => x.Id = keyGenerator.GetNext())
                .And(x => x.Text = rndGenerator.Phrase(20))
                .And(x => x.AlternativeText = parametersFactory.CreateAlternativeTextRepresentation())
                .And(x => x.Language = parametersFactory.CreateLanguage())
                .Build();
        }

        public GEO CreateGeo()
        {
            return CreateGeoList(1).First();
        }

        public IEnumerable<GEO> CreateGeoList(int quantity)
        {
            return Builder<GEO>.CreateListOfSize(quantity)
                .All()
                .With(x => x.Id = keyGenerator.GetNext())
                .And(x => x.Latitude = rndGenerator.Next(-90f, 90f))
                .And(x => x.Longitude = rndGenerator.Next(-180f, 180f))
                .Build();
        }

        public RESOURCES CreateResources()
        {
            return CreateResourcesList(1).First();
        }

        public IEnumerable<RESOURCES> CreateResourcesList(int quantity)
        {
            return Builder<RESOURCES>.CreateListOfSize(quantity)
                .All()
                .With(x => x.Id = keyGenerator.GetNext())
                .And(x => x.Values = resources)
                .And(x => x.Language = parametersFactory.CreateLanguage())
                .Build();
        }

        public FREEBUSY CreateFreebusyInfo()
        {
            return CreateFreebusyInfos(1).First();
        }

        public IEnumerable<FREEBUSY> CreateFreebusyInfos(int quantity)
        {
            return Builder<FREEBUSY>.CreateListOfSize(quantity)
                .All()
                .With(x => x.Id = keyGenerator.GetNext())
                .And(x => x.Type = Pick<FBTYPE>.RandomItemFrom(new []
                {
                    FBTYPE.BUSY, 
                    FBTYPE.BUSY_TENTATIVE, 
                    FBTYPE.BUSY_UNAVAILABLE, 
                    FBTYPE.FREE, 
                    FBTYPE.NONE, 
                }))
                .And(x => x.Periods = valuesFactory.CreatePeriods(rndGenerator.Next(1, quantity)).ToList())
                .Build();
        }

        public TZNAME CreateTimeZoneName()
        {
            return CreateTimeZoneNames(1).First();
        }

        public IEnumerable<TZNAME> CreateTimeZoneNames(int quantity)
        {
            return Builder<TZNAME>.CreateListOfSize(quantity)
                .All()
                .With(x => x.Id = keyGenerator.GetNext())
                .And(x => x.Text = rndGenerator.Phrase(25))
                .And(x => x.Language = parametersFactory.CreateLanguage())
                .Build();
        }

        public ORGANIZER CreateOrganizer()
        {
            return CreateOrganizers(1).First();
        }

        public IEnumerable<ORGANIZER> CreateOrganizers(int quantity)
        {
            return Builder<ORGANIZER>.CreateListOfSize(quantity)
                .All()
                .With(x => x.Id = keyGenerator.GetNext())
                .And(x => x.CN = Pick<string>.RandomItemFrom(usernames))
                .And(x => x.Address = valuesFactory.CreateEmail(x.CN))
                .And(x => x.Directory = parametersFactory.CreateDirectory())
                .And(x => x.Language = parametersFactory.CreateLanguage())
                .Build();
        }

        public RECURRENCE_ID CreateRecurrenceId()
        {
            return CreateRecurrenceIds(1).First();
        }

        public IEnumerable<RECURRENCE_ID> CreateRecurrenceIds(int quantity)
        {
            return Builder<RECURRENCE_ID>.CreateListOfSize(quantity)
                .All()
                .With(x => x.Id = keyGenerator.GetNext())
                .And(x => x.Range = RANGE.THISANDFUTURE)
                .And(x => x.TimeZoneId = parametersFactory.CreateTimeZoneId())
                .And(x => x.Value = valuesFactory.CreateDateTime())
                .Build();
        }

        public RELATEDTO CreateRelatedto()
        {
            return CreateRelatedtos(1).First();
        }

        public IEnumerable<RELATEDTO> CreateRelatedtos(int quantity)
        {
            return Builder<RELATEDTO>.CreateListOfSize(quantity)
                .All()
                .With(x => x.Id = keyGenerator.GetNext())
                .And(x => x.RelationshipType = Pick<RELTYPE>.RandomItemFrom(new []
                {
                    RELTYPE.CHILD, 
                    RELTYPE.PARENT, 
                    RELTYPE.SIBLING
                }))
                .Build();
        }

        public EXDATE CreateExceptionDate()
        {
            return CreateExceptionDates(1).First();
        }

        public IEnumerable<EXDATE> CreateExceptionDates(int quantity)
        {
            return Builder<EXDATE>.CreateListOfSize(quantity)
                .All()
                .With(x => x.Id = keyGenerator.GetNext())
                .And(x => x.DateTimes = valuesFactory.CreateDateTimes(rndGenerator.Next(0, quantity)).ToList())
                .And(x => x.TimeZoneId = parametersFactory.CreateTimeZoneId())
                .Build();
        }

        public RDATE CreateRecurrenceDate()
        {
            return CreateRecurrenceDates(1).First();
        }

        public IEnumerable<RDATE> CreateRecurrenceDates(int quantity)
        {
            return Builder<RDATE>.CreateListOfSize(quantity)
                .All()
                .With(x => x.Id = keyGenerator.GetNext())
                .And(x => x.Periods = valuesFactory.CreatePeriods(rndGenerator.Next(0, quantity)).ToList())
                .And(x => x.DateTimes = valuesFactory.CreateDateTimes(rndGenerator.Next(0, quantity)).ToList())
                .And(x => x.TimeZoneId = parametersFactory.CreateTimeZoneId())
                .And(x => x.ValueType == Pick<VALUE>.RandomItemFrom(new[]
                {
                    VALUE.DATE, 
                    VALUE.DATE_TIME, 
                }))
                .Build();
        }

        public TRIGGER CreateTrigger()
        {
            return CreateTriggers(1).First();
        }

        public IEnumerable<TRIGGER> CreateTriggers(int quantity)
        {
            return Builder<TRIGGER>.CreateListOfSize(quantity)
                .All()
                .With(x => x.Id = keyGenerator.GetNext())
                .And(x => x.DateTime = valuesFactory.CreateDateTime())
                .And(x => x.Duration = valuesFactory.CreateDuration())
                .And(x => x.Related = Pick<RELATED>.RandomItemFrom(new []
                {
                    RELATED.START, 
                    RELATED.END
                }))
                .And(x => x.ValueType == Pick<VALUE>.RandomItemFrom(new[]
                {
                    VALUE.DATE_TIME, 
                    VALUE.DURATION, 
                }))
                .Build();
        }

        public STATCODE CreateStatcode()
        {
            return CreateStatcodes(1).First();
        }

        public IEnumerable<STATCODE> CreateStatcodes(int quantity)
        {
            return Builder<STATCODE>.CreateListOfSize(quantity)
                .All()
                .And(x => x.L1 =  rndGenerator.Next(1u, 5u))
                .And(x => x.L2 = rndGenerator.Next(1u, 5u))
                .And(x => x.L3 = rndGenerator.Next(1u, 5u))
                .Build();
        }

        public REQUEST_STATUS CreateRequestStatus()
        {
            return CreateRequestStatuses(1).First();
        }

        public IEnumerable<REQUEST_STATUS> CreateRequestStatuses(int quantity)
        {
            return Builder<REQUEST_STATUS>.CreateListOfSize(quantity)
                .All()
                .With(x => x.Id = keyGenerator.GetNext())
                .And(x => x.Code = CreateStatcode())
                .And(x => x.Description = rndGenerator.Phrase(30))
                .And(x => x.ExceptionData = rndGenerator.Phrase(30))
                .And(x => x.Language = parametersFactory.CreateLanguage())
                .Build();
        }

        public URL CreateUrl()
        {
            return new URL
            {
                Uri = valuesFactory.CreateUri()
            };
        }

        public IEnumerable<URL> CreateUrls(int quantity)
        {
            var urls = new List<URL>();
            for (var i = 0; i < quantity; i++)
            {
                urls.Add(CreateUrl());
            }
            return urls;
        }
    }
}