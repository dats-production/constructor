using Michsky.UI.ModernUIPack;
using UniRx;
using UnityEngine;

namespace UI.Views
{
    public class HorizontalSelectorView : MonoBehaviour, IBindable<IHorizontalSelectorModel>
    {
        [SerializeField] private HorizontalSelector horizontalSelector;

        private SerialDisposable disposable = new();

        private void Awake()
        {
            disposable.AddTo(this);
        }

        public void Bind(IHorizontalSelectorModel model)
        {
            horizontalSelector.Clear();

            foreach (var item in model.Items)
            {
                horizontalSelector.CreateNewItem($"{model.LayerName} - {item}");
            }

            horizontalSelector.defaultIndex = model.OnSelect.Value;
            horizontalSelector.SetupSelector();

            disposable.Disposable =
                horizontalSelector.selectorEvent.AsObservable().Subscribe(x => model.OnSelect.Value = x);
        }
    }
}