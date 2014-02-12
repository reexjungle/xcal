using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using reexmonkey.xcal.domain.contracts;
using reexmonkey.xcal.service.plugins.validators.contracts;

namespace reexmonkey.xcal.service.plugins.validators.concretes
{
    public class AltrepValidator : IAltrepValidator
    {
        public bool Valid(IALTREP value)
        {
            return  value != null && value.Path != null &&
                Uri.IsWellFormedUriString(value.Path, UriKind.RelativeOrAbsolute);
        }
    }

    public class LanguageValidator : ILanguageValidator
    {
        public bool Valid(ILANGUAGE value)
        {
            return value != null && value.Tag != null;
        }
    }
}
