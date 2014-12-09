using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.FluentValidation;
using ServiceStack.FluentValidation.Results;
using ServiceStack.FluentValidation.Validators;

namespace reexjungle.crosscut.operations.concretes
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

    public class PolymorphicCollectionValidator<TBase>: NoopPropertyValidator
    {
        IValidator<TBase> validator;
        Dictionary<Type, IValidator> deriveds; 

        public PolymorphicCollectionValidator(IValidator<TBase> validator = null)
        {
            this.validator = validator;
            this.deriveds = new Dictionary<Type, IValidator>();
        }

        public PolymorphicCollectionValidator<TBase> Add<TDerived>(IValidator<TDerived> validator)
            where TDerived: TBase
        {
            if (!this.deriveds.ContainsKey(typeof(TDerived))) 
                this.deriveds.Add(typeof(TDerived), validator);
            return this;
        }

        public override IEnumerable<ValidationFailure> Validate(PropertyValidatorContext context)
        {
            var collection = context.PropertyValue as IEnumerable<TBase>;
            if (collection == null) return Enumerable.Empty<ValidationFailure>();
            if (!collection.Any()) return Enumerable.Empty<ValidationFailure>();

            foreach(var item in collection)
            {
                if (!deriveds.ContainsKey(item.GetType())) continue;
                var derived = deriveds[item.GetType()];
                var collectionValidator = new ChildCollectionValidatorAdaptor(derived);
                return collectionValidator.Validate(context);
            }

            if(this.validator != null)
            {
                var baseCollectionValidator = new ChildCollectionValidatorAdaptor(this.validator);
                return baseCollectionValidator.Validate(context);
            }

            return Enumerable.Empty<ValidationFailure>();
        }
    }
}
