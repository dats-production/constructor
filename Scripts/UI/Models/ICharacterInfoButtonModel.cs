using System;
using Constructor;
using UniRx;

namespace UI.Models
{
    public interface ICharacterInfoButtonModel : IUIViewModel
    {
        public IReactiveProperty<string> Name { get; set; }
        IObservable<IButtonModel> Button { get; }
    }

    public class CharacterInfoButtonModel : ICharacterInfoButtonModel
    {
        public UIViewType Type => UIViewType.CharacterInfoButton;
        public IReactiveProperty<string> Name { get; set; } = new ReactiveProperty<string>();

        private readonly ICharacter character;
        private readonly ICharacterInfoModel characterInfoModel;

        public CharacterInfoButtonModel(ICharacter character, ICharacterInfoModel characterInfoModel)
        {
            this.character = character;
            this.characterInfoModel = characterInfoModel;
            character.Name.Subscribe((x) => Name.Value = x);
        }

        public IObservable<IButtonModel> Button =>
            Observable.Create<IButtonModel>(observer =>
            {
                var disposable = new CompositeDisposable();
                var model = new ButtonModel();
                model.AddTo(disposable);
                model.Click.Subscribe(_ => characterInfoModel.Show(character)).AddTo(disposable);
                observer.OnNext(model);
                return disposable;
            });
    }
}