using Michsky.UI.ModernUIPack;
using UI.Models;
using UniRx;
using UnityEngine;

namespace UI.Views
{
    public class CharacterInfoButton : MonoBehaviour, IBindable<ICharacterInfoButtonModel>
    {
        [SerializeField] private ButtonManagerBasic buttonManager;
        [SerializeField] private ButtonView button;

        public void Bind(ICharacterInfoButtonModel model)
        {
            model.Name.Subscribe(ChangeName).AddTo(this);
            model.Button.Subscribe(button.Bind).AddTo(this);
        }

        private void ChangeName(string name)
        {
            buttonManager.buttonText = name;
            buttonManager.UpdateUI();
        }
    }
}