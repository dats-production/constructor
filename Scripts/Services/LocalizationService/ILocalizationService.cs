using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.Extensions;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace Services.LocalizationService
{
    public interface ILocalizationService
    {
        string Localize(string tableEntryReference);
        void SetStringVariable(string variableName, string variableValue);
    }
    
    public class LocalizationService : ILocalizationService
    {
        public string Localize(string tableEntryReference)
        {
            string localizedString = null;
            var asyncOperationHandle = LocalizationSettings.StringDatabase
                .GetLocalizedStringAsync("UI Text", tableEntryReference);
            
            if (asyncOperationHandle.IsDone)
                localizedString = asyncOperationHandle.Result;
            else
                asyncOperationHandle.Completed += (asyncOperationHandle) => localizedString = asyncOperationHandle.Result;
            
            return localizedString;
        }

        public void SetStringVariable(string variableName, string variableValue)
        {
            var source = LocalizationSettings.StringDatabase.SmartFormatter.GetSourceExtension<PersistentVariablesSource>();
            var stringVariable = source["global"][variableName] as StringVariable;

            stringVariable.Value = variableValue;
        }
    }
}