using reexjungle.xcal.domain.models;
using System.Collections.Generic;

namespace reexjungle.xcal.tests.contracts.factories
{
    public interface IAlarmFactory
    {
        AUDIO_ALARM CreateAudioAlarm();

        DISPLAY_ALARM CreateDisplayAlarm();

        EMAIL_ALARM CreateEmailAlarm();

        IEnumerable<AUDIO_ALARM> CreateAudioAlarms(int quantity);

        IEnumerable<DISPLAY_ALARM> CreateDisplayAlarms(int quantity);

        IEnumerable<EMAIL_ALARM> CreateEmailAlarms(int quantity);
    }
}