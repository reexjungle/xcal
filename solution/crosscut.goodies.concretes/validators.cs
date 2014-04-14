using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.FluentValidation;
using ServiceStack.FluentValidation.Results;

namespace reexmonkey.crosscut.goodies.concretes
{
    public abstract class AbstractCompositeValidator<T>: AbstractValidator<T>
    {
        List<IValidator> validators = new List<IValidator>();

        protected void RegisterBaseValidator<TBase>(IValidator<TBase> validator)
        {
            if (validator.CanValidateInstancesOfType(typeof(T))) validators.Add(validator);
            else throw new NotSupportedException(string.Format("Type {0} is not a base-class or interface implemented by {1}.", typeof(TBase).Name, typeof(T).Name));
        }

        public override ValidationResult Validate(ValidationContext<T> context)
        {

            var berrors = base.Validate(context).Errors;
            var oerrors = validators.SelectMany(x => x.Validate(context).Errors);
            var combined = berrors.Concat(oerrors);
            return new ValidationResult(combined);
        } 
    }
}
