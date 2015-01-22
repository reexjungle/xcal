using FizzWare.NBuilder;
using reexjungle.infrastructure.concretes.operations;
using reexjungle.infrastructure.contracts;
using reexjungle.xcal.domain.contracts;
using reexjungle.xcal.domain.models;
using reexjungle.xcal.test.units.contracts;
using System.Collections.Generic;

namespace reexjungle.xcal.test.units.concretes
{
    public class PropertiesUnitTests : IPropertiesUnitTests
    {
        public IGuidKeyGenerator KeyGen { get; set; }

        public PropertiesUnitTests()
        {
            this.KeyGen = new GuidKeyGenerator();
        }

        public IEnumerable<ATTENDEE> GenerateAttendeesOfSize(int n)
        {
            return Builder<ATTENDEE>.CreateListOfSize(n)
                .All()
                .With(x => x.Id = this.KeyGen.GetNextKey())
                .And(x => x.CN = Pick<string>.RandomItemFrom(new string[] { "Caesar", "Koba", "Cornelia", "Blue Eyes", "Grey", "Ash" }))
                .And(x => x.Address = new URI(string.Format("{0}@apes.je", x.CN.Replace(" ", ".").ToLower())))
                .And(x => x.Role = Pick<ROLE>.RandomItemFrom(new List<ROLE> { ROLE.CHAIR, ROLE.NON_PARTICIPANT, ROLE.OPT_PARTICIPANT, ROLE.REQ_PARTICIPANT }))
                .And(x => x.Participation = Pick<PARTSTAT>.RandomItemFrom(new List<PARTSTAT> { PARTSTAT.ACCEPTED, PARTSTAT.COMPLETED, PARTSTAT.DECLINED, PARTSTAT.NEEDS_ACTION, PARTSTAT.TENTATIVE }))
                .And(x => x.CalendarUserType = Pick<CUTYPE>.RandomItemFrom(new List<CUTYPE> { CUTYPE.GROUP, CUTYPE.INDIVIDUAL, CUTYPE.RESOURCE, CUTYPE.ROOM }))
                .And(x => x.Language = new LANGUAGE(Pick<string>.RandomItemFrom(new List<string> { "en", "fr", "de" })))
                .Build();
        }
    }
}