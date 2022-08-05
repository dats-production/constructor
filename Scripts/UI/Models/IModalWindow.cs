using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace UI.Models
{
    public interface IModalWindow
    {
        UniTask<bool> Show(string windowTitle, string descriptionText);
        void Hide();
    }
}