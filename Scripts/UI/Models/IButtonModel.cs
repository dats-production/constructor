using System;
using UniRx;

namespace UI.Models
{
    public interface IButtonModel
    {
        IReactiveCommand<Unit> Click { get; }
        IReactiveProperty<bool> Interactable { get; }
    }

    public class ButtonModel : IButtonModel, IDisposable
    {
        public IReactiveCommand<Unit> Click => _click;
        public IReactiveProperty<bool> Interactable => _interactable;

        private readonly ReactiveCommand _click = new();
        private readonly ReactiveProperty<bool> _interactable = new(true);

        public void Dispose()
        {
            _click?.Dispose();
            _interactable?.Dispose();
        }
    }
}