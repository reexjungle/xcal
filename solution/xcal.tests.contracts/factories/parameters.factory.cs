using reexjungle.xcal.domain.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace reexjungle.xcal.tests.contracts.factories
{
    public interface IParametersFactory
    {
        DELEGATED_FROM CreateDelegator(IEnumerable<string> usernames, int quantity);

        IEnumerable<DELEGATED_FROM> CreateDelegators(IEnumerable<string> usernames,int quantity);

        DELEGATED_TO CreateDelegatee(IEnumerable<string> usernames, int quantity);

        IEnumerable<DELEGATED_TO> CreateDelegatees(IEnumerable<string> usernames, int quantity);

        FMTTYPE CreateFormatType();

        IEnumerable<FMTTYPE> CreateFormatTypes(int quantity);

        LANGUAGE CreateLanguage();

        IEnumerable<LANGUAGE> CreateLanguages(int quantity);

        MEMBER CreateMember(IEnumerable<string> usernames, int quantity);

        IEnumerable<MEMBER> CreateMembers(IEnumerable<string>usernames,int quantity);

        TZID CreateTimeZoneId();

        IEnumerable<TZID> CreateTimeZoneIds(int quantity);

        ALTREP CreateAlternativeTextRepresentation();

        IEnumerable<ALTREP> CreateAlternativeTextRepresentations(int quantity);

        DIR CreateDirectory();

        IEnumerable<DIR> CreateDirectories(int quantity);

        SENT_BY CreateSentBy(string username);

        IEnumerable<SENT_BY> CreateSentBys(IEnumerable<string>usernames, int quantity);
    }
}