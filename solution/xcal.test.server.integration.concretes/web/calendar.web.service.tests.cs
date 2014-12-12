using GenFu;
using reexjungle.infrastructure.operations.concretes;
using reexjungle.infrastructure.operations.contracts;
using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.models;
using reexjungle.xcal.domain.operations;
using reexjungle.xcal.test.server.integration.contracts;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace reexjungle.xcal.test.server.integration.concretes
{
    public abstract class CalendarWebServicesTests : IWebServiceTests
    {
        protected string baseUri = string.Empty;

        protected GuidKeyGenerator keygen = new GuidKeyGenerator();

        protected StringFPIKeyGenerator fkeygen = null;

        protected IEnumerable<VCALENDAR> calendars = null;

        public CalendarWebServicesTests()
        {
            var config = GenFu.GenFu.Configure<StringFPIKeyGenerator>()
                .Fill(x => x.Owner, () => "reexjungle")
                .Fill(x => x.Authority, () => Authority.None)
                .Fill(x => x.Description, () => "Test iCalendar Web Services Provider")
                .Fill(x => x.LanguageId, () => "EN");

            this.fkeygen = A.New<StringFPIKeyGenerator>();
        }

        public void Initialize()
        {
            ///TODO: Tanssfer seed data generation to separate *.cs file
            //create seed data with GenFu

            GenFu.GenFu.Configure<VCALENDAR>()
                .Fill(x => x.Id, () => this.keygen.GetNextKey())
                .Fill(x => x.ProdId, () => this.fkeygen.GetNextKey())
                .Fill(x => x.Method).WithRandom(new METHOD[] { METHOD.ADD, METHOD.CANCEL, METHOD.PUBLISH, METHOD.REFRESH, METHOD.REPLY, METHOD.REQUEST, METHOD.COUNTER, METHOD.DECLINECOUNTER })
                .Fill(x => x.Calscale).WithRandom(new CALSCALE[] { CALSCALE.CHINESE, CALSCALE.GREGORIAN, CALSCALE.HEBREW, CALSCALE.INDIAN, CALSCALE.ISLAMIC, CALSCALE.JULIAN });

            this.calendars = A.ListOf<VCALENDAR>();
        }

        public void TearDown()
        {
            var client = new JsonWebServicesTests(this.baseUri).CreateServiceClient();
            client.Post(new FlushDatabase { Mode = FlushMode.soft });
        }

        [Fact]
        public void MaintainSingleCalendar()
        {
            this.TearDown();
            this.Initialize();

            var c1 = calendars.First();
            var client = new JsonWebServicesTests(this.baseUri).CreateServiceClient();

            client.Post(new AddCalendar { Calendar = c1 });
            var f1 = client.Get(new FindCalendar { CalendarId = c1.Id });
            Assert.Equal(f1.Calscale, c1.Calscale);
            Assert.Equal(f1.ProdId, c1.ProdId);
            Assert.Equal(f1.Method, c1.Method);
            Assert.Equal(f1.Version, c1.Version);
            Assert.Equal(f1.Id, c1.Id);

            c1.Method = METHOD.REQUEST;
            c1.Version = "3.0";
            c1.Calscale = CALSCALE.HEBREW;
            client.Put(new UpdateCalendar { Calendar = c1 });
            var u1 = client.Get(new FindCalendar { CalendarId = c1.Id });
            Assert.Equal(u1.Calscale, CALSCALE.HEBREW);
            Assert.Equal(u1.Version, "3.0");
            Assert.Equal(u1.Method, METHOD.REQUEST);
            Assert.Equal(u1, c1);

            client.Patch(new PatchCalendar { Scale = CALSCALE.JULIAN, CalendarId = c1.Id });
            var p1 = client.Get(new FindCalendar { CalendarId = c1.Id });
            Assert.Equal(p1.Calscale, CALSCALE.JULIAN);

            client.Delete(new DeleteCalendar { CalendarId = c1.Id });
            var d1 = client.Get(new FindCalendar { CalendarId = c1.Id });
            Assert.Equal(d1, null);
        }
    }

    public class CalendarRemoteServiceTestsDev1 : CalendarWebServicesTests
    {
        public CalendarRemoteServiceTestsDev1()
            : base()
        {
            this.baseUri = Properties.Settings.Default.remote_dev1_uri;
        }
    }

    public class CalendarRemoteServiceTestsDev2 : CalendarWebServicesTests
    {
        public CalendarRemoteServiceTestsDev2()
            : base()
        {
            this.baseUri = Properties.Settings.Default.remote_dev2_uri;
        }
    }

    public class CalendarLocalServiceTests : CalendarWebServicesTests
    {
        public CalendarLocalServiceTests()
            : base()
        {
            this.baseUri = Properties.Settings.Default.localhost_uri;
        }
    }
}