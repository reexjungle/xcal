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
        DELEGATE CreateDelegate();

        IEnumerable<DELEGATE> CreateDelegates(int quantity);

        FMTTYPE CreateFormatType();

        IEnumerable<FMTTYPE> CreateFormatTypes(int quantity);

        LANGUAGE CreateLanguage();

        IEnumerable<LANGUAGE> CreateLanguages(int quantity);

        MEMBER CreateMember();

        IEnumerable<MEMBER> CreateMembers(int quantity);

        TZID CreateTimeZoneId();

        IEnumerable<TZID> CreateTimeZoneIds(int quantity);
    }
}