using System.Collections.Generic;
using System.Linq;
using Constructor.Details;
using Services.LocalizationService;
using UnityEngine;
using Zenject;

namespace Constructor.Validator.Validations
{
    public class RarityValidation : ILayerValidation
    {
        [Inject] private readonly ILocalizationService localizationService;
        private float raritiesSum;

        public bool Validate(Layer layer, out List<Detail> notPassedValidationDetails)
        {
            notPassedValidationDetails = default;
            raritiesSum = layer.Details.Sum(detail => detail.Rarity.Value);
            return Mathf.Approximately(100f, raritiesSum);
        }

        public string GetDescription()
        {
            localizationService.SetStringVariable("raritiesSum", raritiesSum.ToString());
            return localizationService.Localize("The sum of the layer's rarities (%) is not equal to 100%!");
        }
    }
}