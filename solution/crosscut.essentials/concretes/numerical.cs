using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace reexmonkey.crosscut.essentials.concretes
{
    public static class NumericalExtensions
    {
        public static bool IsNull<TNumber>(this TNumber? number)
     where TNumber : struct, IComparable, IComparable<TNumber>
        {
            return (number == null);
        }

        public static bool IsNullOrDefault<TNumber>(this TNumber? number)
             where TNumber : struct, IComparable, IComparable<TNumber>
        {
            return (!(number != null && !(number.HasValue && number.Value.Equals(default(TNumber)))));
        }


        public static bool IsNonNull<TNumber>(this TNumber? number)
    where TNumber : struct, IComparable, IComparable<TNumber>
        {
            return (number != null && number.HasValue);
        }

        public static bool IsDefault<TNumber>(this TNumber? number)
            where TNumber : struct, IComparable, IComparable<TNumber>
        {
            return (number != null && number.HasValue && number.Value.Equals(default(TNumber)));
        }

        public static bool IsPositive<TNumber>(this TNumber? number)
    where TNumber : struct, IComparable, IComparable<TNumber>
        {
            return (number != null && number.HasValue && number.Value.CompareTo(default(TNumber)) > 0);
        }

        public static bool IsPositiveOrDefault<TNumber>(this TNumber? number)
 where TNumber : struct, IComparable, IComparable<TNumber>
        {
            return (number != null && !(number.HasValue && number.Value.CompareTo(default(TNumber)) < 0));
        }

        public static bool IsNegative<TNumber>(this TNumber? number)
 where TNumber : struct, IComparable, IComparable<TNumber>
        {
            return (number != null && number.HasValue && number.Value.CompareTo(default(TNumber)) < 0);
        }

        public static bool IsNegativeOrDefault<TNumber>(this TNumber? number)
 where TNumber : struct, IComparable, IComparable<TNumber>
        {
            return (number != null && !(number.HasValue && number.Value.CompareTo(default(TNumber)) > 0));
        }

        public static bool IsNegativeOrNullOrDefault<TNumber>(this TNumber? number)
            where TNumber : struct, IComparable, IComparable<TNumber>
        {
            return !number.IsPositive();
        }
        public static bool IsPositiveOrNullOrDefault<TNumber>(this TNumber? number)
    where TNumber : struct, IComparable, IComparable<TNumber>
        {
            return !number.IsNegative();
        }


    }
}
