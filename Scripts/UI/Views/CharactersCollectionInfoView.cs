using System.Collections.Generic;
using Constructor;
using Constructor.DataStorage;
using UI.Models;
using UniRx;
using UnityEngine;
using Zenject;

namespace UI.Views
{
    public class CharactersCollectionInfoView : MonoBehaviour, ICharactersCollectionInfoModel
    {
        [SerializeField] private CharacterInfoButtonCollectionView collectionView;

        private readonly SerialDisposable disposable = new();
        private readonly List<ICharacterInfoButtonModel> models = new();
        private IDataStorage dataStorage;
        private ICharacterInfoModel characterInfoModel;
        private IReadOnlyReactiveCollection<Character> characters;

        [Inject]
        public void Construct(IDataStorage dataStorage, ICharacterInfoModel characterInfoModel)
        {
            this.dataStorage = dataStorage;
            this.characterInfoModel = characterInfoModel;
        }

        private void Awake()
        {
            disposable.AddTo(this);
        }

        public void Open(IReadOnlyReactiveCollection<ICharacter> characters)
        {
            foreach (var character in characters)
            {
                AddModel(character);
            }

            collectionView.Bind(models);

            disposable.Disposable = dataStorage.Characters.ObserveAdd().Subscribe(x =>
            {
                AddModel(x.Value);
                collectionView.Bind(models);
            });
        }

        public void Close()
        {
            Clear();
            characterInfoModel.Clear();
        }
        
        public void Clear()
        {
            models.Clear();
            disposable.Disposable?.Dispose();
        }

        private void AddModel(ICharacter character)
        {
            var model = new CharacterInfoButtonModel(character, characterInfoModel);
            models.Add(model);
        }
    }
}