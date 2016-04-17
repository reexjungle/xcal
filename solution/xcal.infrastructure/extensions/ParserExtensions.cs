using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sprache;

namespace reexjungle.xcal.infrastructure.extensions
{
    public static class ParserExtensions
    {
        public static Parser<T> Escaped<T>(this Parser<T> parser, Parser<T> escapeParser )
        {
            return from escape in escapeParser
                   from p in parser
                   select p;
        }

        public static Parser<char> CalEscaped(this Parser<char> parser)
        {
            return parser.Escaped(Parse.Char('\\'));
        }
    }
}
