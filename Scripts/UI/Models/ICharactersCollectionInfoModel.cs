using Constructor;
using UniRx;

namespace UI.Models
{
    public interface ICharactersCollectionInfoModel
    {
        void Open(IReadOnlyReactiveCollection<ICharacter> characters);
        void Close();
        void Clear();
    }
}