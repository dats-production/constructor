using System.Collections.Generic;
using System.Linq;
using Constructor.Details;
using Services.LocalizationService;
using Zenject;

namespace Constructor.Validator.Validations
{
    public class BodyValidation : ILayerValidation
    {
        [Inject] private readonly ILocalizationService localizationService;

        public bool Validate(Layer layer, out List<Detail> notPassedValidationDetails)
        {
            notPassedValidationDetails = layer.Details.Where(detail => !detail.HasBody()).ToList();
            return layer.Details.Find(d => !d.HasBody()) == null;
        }

        public string GetDescription()
        {
            return localizationService.Localize("Fbx model is missing!");
        }
    }
}