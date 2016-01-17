using System;

namespace reexjungle.xcal.domain.extensions
{
    public static class Parsers
    {
        public static TEnum ParseEnumParameter<TEnum>(this string @this)
            where TEnum: struct
        {

            TEnum @enum;
            var tokens = @this.Split(new []{'='}, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length < 2) throw new FormatException("Invalid Format");
            if (Enum.TryParse(tokens[1], true, out @enum)) return @enum;
            throw new FormatException("Invalid Format");
        }

        public static void TryParseEnumParameter<TEnum>(this string @this,out TEnum value)
            where TEnum : struct
        {
            TEnum @enum;
            var tokens = @this.Split(new []{'='}, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length < 2)  @enum = default(TEnum);
            if (!Enum.TryParse(tokens[1], true, out @enum)) @enum = default(TEnum);
            value = @enum;
        }
    }
}
