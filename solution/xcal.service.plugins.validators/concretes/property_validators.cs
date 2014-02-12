using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.FluentValidation;
using reexmonkey.xcal.domain.contracts;
using reexmonkey.xcal.domain.models;
using reexmonkey.xcal.service.plugins.validators.contracts;

namespace reexmonkey.xcal.service.plugins.validators.concretes
{

    public class CommentValidator: AbstractValidator<COMMENT>
    {
        public IAltrepValidator AltrepValidator {get; set;}
        public ILanguageValidator LanguageValidator {get; set;}

        public CommentValidator() : base()
        {
            RuleFor(x => x.Text).NotNull().When( x => x != null);
            RuleFor(x => x.AlternativeText).Must(x => AltrepValidator.Valid(x)).When(x => x.AlternativeText != null);
            RuleFor(x => x.Language).Must(x => LanguageValidator.Valid(x)).When (x => x.Language != null);
        }
    }
}
