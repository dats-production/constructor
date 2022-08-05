using System.Collections.Generic;
using System.Linq;
using Constructor;
using Constructor.DataStorage;
using Constructor.Details;
using Michsky.UI.ModernUIPack;
using Services.LocalizationService;
using TMPro;
using UI.Factories;
using UI.Models;
using UI.Views.Details;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Views
{
    public class DetailInfoView : MonoBehaviour
    {
        [SerializeField] private TMP_Text detailName;
        [SerializeField] private TMP_Text rarity;
        [SerializeField] private TMP_Text layerName;
        [SerializeField] private TMP_InputField nameInputField;
        [SerializeField] private TMP_InputField rarityInputField;
        [SerializeField] private Button removeButton;
        [SerializeField] private ModalWindowManager modalWindowManager;

        private Detail detail;
        private DetailViewerFactory detailViewerFactory;
        private IDetailViewer currentDetailViewer;
        private readonly List<IDetailViewer> chosenDetailViewers = new ();
        private ILocalizationService localizationService;

        [Inject]
        public void Construct(IDataStorage dataStorage, 
            DetailViewerFactory detailViewerFactory, ILocalizationService localizationService)
        {
            removeButton.OnClickAsObservable().Subscribe(x => Remove()).AddTo(removeButton);
        
            nameInputField.onEndEdit
                .AsObservable()
                .Where(x => x != null)
                .Subscribe(SetName)
                .AddTo(nameInputField);
            nameInputField.onSelect
                .AsObservable()
                .Subscribe(x => nameInputField.text = detail.Name.Value)
                .AddTo(nameInputField);
        
            rarityInputField.onSelect
                .AsObservable()
                .Subscribe(x => rarityInputField.text = detail.Rarity.Value.ToString())
                .AddTo(rarityInputField);
            rarityInputField.onEndEdit
                .AsObservable()
                .Where(x => x != null)
                .Subscribe(SetRarity)
                .AddTo(rarityInputField);
            
            this.detailViewerFactory = detailViewerFactory;
            this.localizationService = localizationService;
        }

        public void ShowDetail(Detail detail, Layer layer)
        {
            this.detail = detail;
            detail.Name.Subscribe((x) => detailName.text = x).AddTo(this);
            rarity.text = $"{detail.Rarity}%";
            layerName.text = $"{localizationService.Localize("Layer")}: {layer.Name}";

            if (detail is FbxDetail)
                foreach (var detailViewer in chosenDetailViewers.OfType<ColorDetailViewer>())
                    ((IDetailViewer) detailViewer).Hide();

            currentDetailViewer = detailViewerFactory.Create(detail);
            if(!chosenDetailViewers.Contains(currentDetailViewer)) chosenDetailViewers.Add(currentDetailViewer);
            currentDetailViewer.Show();
        }
        
        public void Hide()
        {
            detailName.text = "";
            rarity.text = "";
            layerName.text = "";
            foreach (var chosenDetailViewer in chosenDetailViewers)
                chosenDetailViewer.Hide();
            chosenDetailViewers.Clear();
        }

        private void SetName(string name)
        {
            detail.Name.Value = name;
        }

        private void SetRarity(string rarityText)
        {
            if(float.TryParse(rarityText, out var rarity))
            {
                this.rarity.text = $"{rarity}%";
                detail.Rarity.Value = rarity;
            }
            else
                modalWindowManager.OpenWindow();
        }

        private void Remove() { }
    }
}
