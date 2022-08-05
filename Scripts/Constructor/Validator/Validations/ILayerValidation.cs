using System.Collections.Generic;
using Constructor.Details;

namespace Constructor.Validator.Validations
{
    public interface ILayerValidation : IValidation<Layer, List<Detail>>
    {
        new bool Validate(Layer layer, out List<Detail> notPassedValidationDetails);
    }
}