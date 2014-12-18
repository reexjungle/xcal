using reexjungle.xcal.domain.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace reexjungle.xcal.test.units.contracts
{
    public interface IAlarmUnitTests : IUnitTests
    {
        IEnumerable<AUDIO_ALARM> GenerateAudioAlarmsOfSize(int n);

        IEnumerable<DISPLAY_ALARM> GenerateDisplayAlarmsOfSize(int n);

        IEnumerable<EMAIL_ALARM> GenerateEmailAlarmsOfSize(int n);
    }
}