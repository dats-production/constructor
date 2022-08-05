using System.Collections.Generic;
using Constructor.Details;
using Constructor.Validator.Validations;
using UnityEngine;

namespace Constructor.Validator
{
    public class Validator<T, R>
    {
        private List<IValidation<T, R>> validations;

        public Validator(List<IValidation<T, R>> validations)
        {
            this.validations = validations;
        }

        public bool Validate(T t, 
            out List<string> validationFailDescriptions, 
            out List<Detail> finalListOfValidationFailedDetails,  
            out List<string> validationFailDetailDescriptions)
        {
            return MainValidate(t, 
                out validationFailDescriptions, 
                out finalListOfValidationFailedDetails,  
                out validationFailDetailDescriptions);
        }
        
        public bool Validate(T t, out List<string> validationFailDescriptions)
        {
            return MainValidate(t, 
                out validationFailDescriptions, 
                out var finalListOfValidationFailedDetails,  
                out var validationFailDetailDescriptions);
        }

        public bool MainValidate(T t,  out List<string> validationFailDescriptions, 
            out List<Detail> allValidationFailedDetails,  
            out List<string> validationFailDetailDescriptions)
        {
            validationFailDescriptions = new List<string>();
            allValidationFailedDetails = new List<Detail>();
            validationFailDetailDescriptions = new List<string>();
            var failedValidations = new List<string>();

            foreach (var validation in validations)
            {
                if (validation.Validate(t, out R r)) continue;
                if (r is List<Detail> validationFailedDetails)
                {
                    allValidationFailedDetails.AddRange(validationFailedDetails);
                    validationFailDetailDescriptions.Add(validation.GetDescription());
                }
                
                var failedValidation = validation.GetType().ToString();
                failedValidations.Add(failedValidation);
                validationFailDescriptions.Add(validation.GetDescription());
                
                switch (t)
                {
                    case Layer layer:
                        Debug.LogWarning($"Layer {layer.Name} is not passed validation: {failedValidation}");
                        break;
                    case string inputText:
                        Debug.LogWarning($"Input text \"{inputText}\" is not passed validation: {failedValidation}");
                        break;
                }
            }
            return failedValidations.Count == 0;
        }
    }
}