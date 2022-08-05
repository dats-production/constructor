using System.Collections.Generic;
using Constructor.Details;

namespace Constructor.Validator.Validations
{
    public class DetailNameValidation : ILayerValidation
    {
        private List<string> notPassedDetailNames;
        
        public bool Validate(Layer layer, out List<Detail> notPassedValidationDetails)
        {
            notPassedValidationDetails = new List<Detail>();
            notPassedDetailNames = new List<string>();
            foreach (var comparedDetail in layer.Details)
            {
                foreach (var detailToCompare in layer.Details)
                {
                    if (comparedDetail == detailToCompare) continue;
                    if (!comparedDetail.Equals(detailToCompare)) continue;
                    if(!notPassedDetailNames.Contains(detailToCompare.Name.Value)) 
                        notPassedDetailNames.Add(detailToCompare.Name.Value);
                    notPassedValidationDetails.Add(comparedDetail);
                    notPassedValidationDetails.Add(detailToCompare);
                }
            }
            return notPassedValidationDetails.Count == 0;
        }

        public string GetDescription()
        {
            return $"Identical detail names \"{string.Join(", ", notPassedDetailNames)}\" are detected! Please change some of them.";
        }
    }
}