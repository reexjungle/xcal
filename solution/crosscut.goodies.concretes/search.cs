using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace reexmonkey.crosscut.goodies.concretes
{
    [Flags]
    public enum DateTimeOption
    {
        time = 0x01,
        date = 0x2,
        unknown = 0x4
    }

    public enum DateTimeSearchOption
    {
        previoushour,
        previoushours,
        nexthour,
        nexthours,
        yesterday,
        previousdays,
        tomorrow,
        nextdays,
        previousweek,
        previousweeks,
        nextweek,
        nextweeks,
        previousmonth,
        previousmonths,
        nextmonth,
        nextmonths,
        previousyear,
        previousyears,
        nextyear,
        nextyears,
        none
    }

    public enum TextSearchOption
    {
        startswith,
        endswith,
        contains,
        not_contains,
        not_equals,
        equals,
        none
    }

    public static class SearchParserExtensions
    {

        public static DateTimeSearchOption ToDateFindOption(this string value)
        {
            if (value.Trim().ToLower().Equals("pasthour")) return DateTimeSearchOption.lasthour;
            else if (value.Trim().ToLower().Equals("nexthour")) return DateTimeSearchOption.nexthour;
            else if (value.Trim().ToLower().Equals("yesterday")) return DateTimeSearchOption.yesterday;
            else if (value.Trim().ToLower().Equals("tomorrow")) return DateTimeSearchOption.tomorrow;
            else if (value.Trim().ToLower().Equals("pastweek")) return DateTimeSearchOption.lastweek;
            else if (value.Trim().ToLower().Equals("nextweek")) return DateTimeSearchOption.nextweek;
            else if (value.Trim().ToLower().Equals("pastmonth")) return DateTimeSearchOption.lastmonth;
            else if (value.Trim().ToLower().Equals("nextmonth")) return DateTimeSearchOption.nextmonth;
            else if (value.Trim().ToLower().Equals("pastyear")) return DateTimeSearchOption.lastyear;
            else if (value.Trim().ToLower().Equals("pastyear")) return DateTimeSearchOption.nextyear;
            else return DateTimeSearchOption.none;
        }

        public static TextSearchOption ToTextSearchOption(this string value)
        {
            if (value.Trim().ToLower().Equals("contains")) return TextSearchOption.contains;
            else if (value.Trim().ToLower().Equals("starts")) return TextSearchOption.startswith;
            else if (value.Trim().ToLower().Equals("ends")) return TextSearchOption.endswith;
            else if (value.Trim().ToLower().Equals("tomorrow")) return TextSearchOption.equals;
            else if (value.Trim().ToLower().Equals("pastweek")) return TextSearchOption.not_contains;
            else if (value.Trim().ToLower().Equals("nextweek")) return TextSearchOption.not_equals;
            else return TextSearchOption.none;
        }


    }




}
