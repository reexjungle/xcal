using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using reexmonkey.xcal.domain.contracts;

namespace reexmonkey.xcal.service.plugins.validators.contracts
{
    public interface IAltrepValidator
    {
        bool Valid(IALTREP value);
    }

    public interface ILanguageValidator
    {
        bool Valid(ILANGUAGE value);
    }
}
