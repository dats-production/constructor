using Constructor;
using Constructor.DataStorage;
using TMPro;
using UI.Models;
using UniRx;
using UnityEngine;
using Zenject;

namespace UI.Views
{
    public class CharacterSearchView : MonoBehaviour
    {
        [SerializeField] private TMP_InputField searchInputField;
        
        private ReactiveCollection<ICharacter> filteredCharacters;
        private IDataStorage dataStorage;
        private ICharactersCollectionInfoModel charactersCollectionInfoView ;
        
        [Inject]
        public void Construct(IDataStorage dataStorage, ICharactersCollectionInfoModel charactersCollectionInfoView)
        {
            searchInputField.onValueChanged
                .AsObservable()
                .Where(x => x is {Length: >= 3})
                .Subscribe(SetFilter)
                .AddTo(searchInputField);
            
            searchInputField.onValueChanged
                .AsObservable()
                .Where(x => x == string.Empty)
                .Subscribe(ResetFilter)
                .AddTo(searchInputField);

            this.dataStorage = dataStorage;
            this.charactersCollectionInfoView = charactersCollectionInfoView;
        }

        private void SetFilter(string inputText)
        {
            filteredCharacters = new ReactiveCollection<ICharacter>();
            foreach (var character in dataStorage.Characters)
            {
                if(character.Name.Value.Contains(inputText))
                    filteredCharacters.Add(character);
                
                foreach (var (key, value) in character.Details)
                {
                    if (value.Name.Value != inputText) continue;
                    if (filteredCharacters.Contains(character)) continue;
                    filteredCharacters.Add(character);
                }
            }
            charactersCollectionInfoView.Clear();
            charactersCollectionInfoView.Open(filteredCharacters);
        }

        private void ResetFilter(string inputText)
        {
            charactersCollectionInfoView.Clear();
            charactersCollectionInfoView.Open(dataStorage.Characters);
        }
    }
}