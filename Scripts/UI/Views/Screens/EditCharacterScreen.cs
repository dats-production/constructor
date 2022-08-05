using System.Linq;
using Constructor;
using Constructor.DataStorage;
using Importer;
using Michsky.UI.ModernUIPack;
using TMPro;
using UI.Models;
using UI.Models.Screens;
using UI.Navigation;
using UniRx;
using UnityEngine;
using Zenject;

namespace UI.Views.Screens
{
    public class EditCharacterScreen : ScreenBase<EditCharacterScreenArgs>
    {
        [SerializeField] private HorizontalSelectorCollectionView detailSelectorCollectionView;
        [SerializeField] private ButtonView saveCharacterButton;
        [SerializeField] private ButtonView discardChangesButton;
        [SerializeField] private ButtonView photoModeButton;
        [SerializeField] private TMP_Text characterInfoDisplay;
        [SerializeField] private CustomDropdown animationDropdown;
        [SerializeField] private CustomDropdown lightingDropdown;
        [SerializeField] private EditCharacterInfoSheetView editCharacterInfoSheetView;
        [SerializeField] private ViewStateSwitcher modelPreviewStateSwitcher;
        [SerializeField] private TMP_Text characterName;
        
        public override ScreenType Type => ScreenType.EditCharacter;
        
        private ICharacter characterOriginal;
        private ICharacter characterCopy;
        private readonly CompositeDisposable disposable = new();
        private IDataStorage dataStorage;
        private IDataStorage temporalDataStorage;
        private CharacterViewer characterViewer;
        private IUINavigator uiNavigator;

        [Inject]
        public void Construct(IDataStorage dataStorage, CharacterViewer characterViewer, IUINavigator uiNavigator)
        {
            this.dataStorage = dataStorage;
            this.characterViewer = characterViewer;
            this.uiNavigator = uiNavigator;
        }

        private void Awake()
        {
            disposable.AddTo(this);
        }

        public override void Open(EditCharacterScreenArgs args)
        {
            base.Open(args);
            modelPreviewStateSwitcher.SwitchState(ModelPreviewVisualState.EditCharacter);
            characterOriginal = args.Character;
            characterCopy = args.Character.Copy();
            temporalDataStorage = dataStorage.Copy();
            temporalDataStorage.ReplaceCharacter(characterCopy);
            characterName.text = args.Character.Name.Value;

            BindLayersInfoCollection();
            BindSaveCharacterButton();
            BindDiscardChangesButton();
        }

        public override void Close()
        {
            base.Close();
            disposable.Clear();
        }

        private void UpdateCharacterInfoDisplay(ICharacter newCharacter) =>
            editCharacterInfoSheetView.SetSheet(characterOriginal, dataStorage, newCharacter, temporalDataStorage);

        private void BindLayersInfoCollection()
        {
            var details = characterCopy.Details.ToDictionary(x => x.Key, x => x.Value);
            var models = details.Select(x =>
            {
                var layer = dataStorage.Layers.First(layer => layer.Name.Equals(x.Key));
                var models = layer.Details.Select(detail => detail.Name.Value).ToList();
                var selectedIndex = models.FindIndex(model => model.Equals(characterCopy.Details[x.Key].Name.Value));
                var model = new HorizontalSelectorModel(models, selectedIndex, layer.Name);
                model.AddTo(disposable);
                model.OnSelect.Subscribe(index =>
                    {
                        characterCopy.SetDetail(layer.Name, layer.Details[index]);
                        characterViewer.AssembleCharacter(characterCopy);
                        UpdateCharacterInfoDisplay(characterCopy);
                    })
                    .AddTo(disposable);
                return model;
            }).ToList();
            detailSelectorCollectionView.Bind(models);
        }

        private void BindSaveCharacterButton()
        {
            var model = new ButtonModel();
            model.AddTo(disposable);
            model.Click.Subscribe(_ =>
            {
                dataStorage.ReplaceCharacter(characterCopy);
                characterOriginal = characterCopy;
                UpdateCharacterInfoDisplay(characterCopy);
                uiNavigator.OpenCollectionPreviewScreen();
            }).AddTo(disposable);
            saveCharacterButton.Bind(model);
        }

        private void BindDiscardChangesButton()
        {
            var model = new ButtonModel();
            model.AddTo(disposable);
            model.Click.Subscribe(_ =>
            {
                characterCopy = characterOriginal;
                characterCopy = characterOriginal.Copy();
                temporalDataStorage.ReplaceCharacter(characterCopy);
                UpdateCharacterInfoDisplay(characterCopy);
                uiNavigator.OpenCollectionPreviewScreen();
            }).AddTo(disposable);
            discardChangesButton.Bind(model);
        }
    }
}