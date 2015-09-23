using reexjungle.xcal.domain.models;
using System.Collections.Generic;

namespace reexjungle.xcal.tests.contracts.factories
{
    public interface IPropertiesFactory
    {
        ATTACH_BINARY CreateAttachBinary();

        IEnumerable<ATTACH_BINARY> CreateAttachBinaries(int quantity);

        ATTACH_URI CreateAttachtUri();

        IEnumerable<ATTACH_URI> CreateAttachUris(int quantity);

        ATTENDEE CreateAttendee();

        IEnumerable<ATTENDEE> CreateAttendees(int quantity);

        CATEGORIES CreateCategories();

        IEnumerable<CATEGORIES> CreateCategoriesList(int quantity);

        COMMENT CreateComment();

        IEnumerable<COMMENT> CreateComments(int quantity);

        CONTACT CreateContact();

        IEnumerable<CONTACT> CreateContacts(int quantity);

        SUMMARY CreateSummary();

        IEnumerable<SUMMARY> CreateSummaries(int quantity);

        LOCATION CreateLocation();

        IEnumerable<LOCATION> CreateLocations(int quantity);

        DESCRIPTION CreateDescription();

        IEnumerable<DESCRIPTION> CreateDescriptions(int quantity);

        GEO CreateGeo();

        IEnumerable<GEO> CreateGeoList(int quantity);

        RESOURCES CreateResources();

        IEnumerable<RESOURCES> CreateResourcesList(int quantity);

        FREEBUSY_INFO CreateFreebusyInfo();

        IEnumerable<FREEBUSY_INFO> CreateFreebusyInfos(int quantity);

        TZNAME CreateTimeZoneName();

        IEnumerable<TZNAME> CreateTimeZoneNames(int quantity);

        ORGANIZER CreateOrganizer();

        IEnumerable<ORGANIZER> CreateOrganizers(int quantity);

        RECURRENCE_ID CreateRecurrenceId();

        IEnumerable<RECURRENCE_ID> CreateRecurrenceIds(int quantity);

        RELATEDTO CreateRelatedto();

        IEnumerable<RELATEDTO> CreateRelatedtos(int quantity);

        EXDATE CreateExceptionDate();

        IEnumerable<EXDATE> CreateExceptionDates(int quantity);

        RDATE CreateRecurrenceDate();

        IEnumerable<RDATE> CreateRecurrenceDates(int quantity);

        TRIGGER CreateTrigger();

        IEnumerable<TRIGGER> CreateTriggers(int quantity);

        STATCODE CreateStatcode();

        IEnumerable<STATCODE> CreateStatcodes(int quantity);

        REQUEST_STATUS CreateRequestStatus();

        IEnumerable<REQUEST_STATUS> CreateRequestStatuses(int quantity);
    }
}