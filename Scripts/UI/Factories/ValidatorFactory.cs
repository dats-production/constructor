using System.Collections.Generic;
using Constructor.Validator;
using Constructor.Validator.Validations;
using Zenject;

namespace UI.Factories
{
    public class ValidatorFactory<T, R> : PlaceholderFactory<List<IValidation<T, R>>, Validator<T, R>>
    {
        public override Validator<T, R> Create(List<IValidation<T, R>> validations)
        {
            return new Validator<T, R>(validations);
        }
    }
}
