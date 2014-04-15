using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.FluentValidation;
using reexmonkey.foundation.essentials.concretes;
using reexmonkey.xcal.domain.contracts;
using reexmonkey.xcal.domain.models;

namespace reexmonkey.xcal.service.validators.concretes
{
    public class CalendarValidator: AbstractValidator<VCALENDAR>
    {
        public CalendarValidator()
        {
            RuleFor(x => x.ProdId).NotNull().NotEmpty();
            RuleFor(x => x.Version).NotNull().NotEmpty();
            RuleFor(x => x.Components).NotNull().NotEmpty();
        }
    }
}
