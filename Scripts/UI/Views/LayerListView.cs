using System.Collections.Generic;
using System.Linq;
using Constructor;
using Constructor.DataStorage;
using Constructor.Details;
using Constructor.Validator;
using Constructor.Validator.Validations;
using Michsky.UI.ModernUIPack;
using Services.LocalizationService;
using UI.Factories;
using UniRx;
using UnityEngine;
using Zenject;

namespace UI.Views
{
    public class LayerListView : MonoBehaviour
    {
        [SerializeField] private GameObject layerPrefab;
        [SerializeField] private Sprite warningIcon;
        
        private List<LayerItemView> layerViews = new List<LayerItemView>();
        private Validator<Layer, List<Detail>> validator;
        private DetailInfoView detailInfoView;
        private ILocalizationService localizationService;

        [Inject]
        public void Construct(IDataStorage dataStorage, ValidatorFactory<Layer, List<Detail>> validatorFactory, 
            DetailInfoView detailInfoView, DiContainer diContainer, ILocalizationService localizationService)
        {
            validator = validatorFactory.Create(new List<IValidation<Layer, List<Detail>>>()
            {
                diContainer.Instantiate<BodyValidation>(),
                diContainer.Instantiate<RarityValidation>(),
                diContainer.Instantiate<DetailNameValidation>()
            });
            
            this.detailInfoView = detailInfoView;
            this.localizationService = localizationService;
            
            dataStorage.Layers.ObserveAdd()
                .Subscribe(_ => ShowLayersData(dataStorage.Layers.ToList()))
                .AddTo(this);
        
            dataStorage.OnLayerChanged
                .BatchFrame(1, FrameCountType.EndOfFrame)
                .Subscribe(UpdateLayersData)
                .AddTo(this);
        }

        private void UpdateLayersData(IList<Layer> layers)
        {
            layers = layers.Distinct().ToList();
            foreach (var layer in layers)
            {
                var layerToUpdate = layerViews.Find(layerItemView => layerItemView.Name.text == layer.Name);
                UpdateLayerData(layerToUpdate, layer);
            }
        }
    
        private void UpdateLayerData(LayerItemView layerToUpdate, Layer layer)
        {
            layerToUpdate.Name.text = layer.Name;
            layerToUpdate.DetailsCount.text = layer.Details.Count.ToString();
        
            var dropdown = layerToUpdate.Dropdown;
            var details = layer.Details;
        
            dropdown.listParent = transform.parent.parent;
            dropdown.dropdownItems.Clear();
            dropdown.selectedItemIndex = 0;
            
            layerToUpdate.WarningTooltipContent.gameObject.SetActive(
                !validator.Validate(layer, 
                    out var validationFailDescriptions, 
                    out var validationFailedDetails, 
                    out var validationFailDetailDescriptions));

            var warningString = localizationService.Localize("WARNING");
            layerToUpdate.WarningTooltipContent.description = $"{warningString}: {string.Join($"\n{warningString}: ", validationFailDescriptions)}";
            
            for (int j = 0; j < layer.Details.Count; j++)
            {
                var detail = details[j];
                var newItem = new CustomDropdown.Item();
                newItem.itemName = detail.Name.Value;
                newItem.itemIcon = warningIcon;

                foreach (var notPassedValidationDetail in validationFailedDetails)
                {
                    if (detail != notPassedValidationDetail) continue;
                    newItem.isWarningOn = true;
                    newItem.warningDescription = $"{warningString}: {string.Join($"\n{warningString}: ", validationFailDetailDescriptions)}";
                }

                //todo передавать данные в UI manager о том, что была выбрана деталь
                //todo избавиться от зависимости на другой view
                newItem.OnItemSelection.AddListener(() =>
                {
                    if(!detailInfoView.gameObject.activeSelf)
                        detailInfoView.gameObject.SetActive(true);

                    detailInfoView.ShowDetail(detail, layer);
                });

                dropdown.AddCustomItem(newItem);
            }

            layerToUpdate.gameObject.SetActive(true);
        }
    
        private void ShowLayersData(IReadOnlyList<Layer> layers)
        {
            if (layers.Count > layerViews.Count)
            {
                var countToAdd = layers.Count - layerViews.Count;
                AddLayers(countToAdd);
            }

            ClearLayerList();

            for (var i = 0; i < layers.Count; i++)
            {
                var layer = layers[i];
                var layerView = layerViews[i];
       
                UpdateLayerData(layerView, layer);
                layerView.gameObject.SetActive(true);
            }
        }

        private void AddLayers(int countToAdd)
        {
            for (int i = 0; i < countToAdd; i++)
            {
                var layerGo = Instantiate(layerPrefab, transform);
                var layerView = layerGo.GetComponent<LayerItemView>();
                layerViews.Add(layerView);
            }        
        }

        private void ClearLayerList()
        {
            foreach (var layerView in layerViews)
                layerView.gameObject.SetActive(false);
        }
    }
}

