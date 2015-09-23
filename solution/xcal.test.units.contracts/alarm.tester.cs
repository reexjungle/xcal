using reexjungle.xcal.domain.models;
using System.Collections.Generic;

namespace reexjungle.xcal.test.units.contracts
{
    public interface IAlarmTester
    {
        IEnumerable<AUDIO_ALARM> GenerateAudioAlarmsOfSize(int n);

        IEnumerable<DISPLAY_ALARM> GenerateDisplayAlarmsOfSize(int n);

        IEnumerable<EMAIL_ALARM> GenerateEmailAlarmsOfSize(int n);
    }
}