using Constructor;
using UniRx;

namespace UI.Models
{
    public interface ICharacterInfoModel
    {
        IReadOnlyReactiveProperty<ICharacter> CurrentSelection { get; }
        void Show(ICharacter character);
        void ShowCharactersCount(int count);
        void Clear();
    }
}