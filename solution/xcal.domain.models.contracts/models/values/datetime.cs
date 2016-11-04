namespace reexjungle.xcal.core.domain.contracts.models.values
{
    public interface IDATE_TIME : IDATE, ITIME
    {
    }

    public interface IDATE_TIME<out TDATE_TIME> where TDATE_TIME : IDATE_TIME
    {
        TDATE_TIME AddDays(double value);

        TDATE_TIME AddWeeks(int value);

        TDATE_TIME AddMonths(int value);

        TDATE_TIME AddYears(int value);

        WEEKDAY GetWeekday();

        TDATE_TIME AddSeconds(double value);

        TDATE_TIME AddMinutes(double value);

        TDATE_TIME AddHours(double value);
    }
}
