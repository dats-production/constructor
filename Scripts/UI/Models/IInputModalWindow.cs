using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace UI.Models
{
    public interface IInputModalWindow
    {
        UniTask<string> Show(string windowTitle, string enteredText = "");
        void Hide();
        void ToggleWarningTooltip(List<string> validationFailDescriptions);
    }
}