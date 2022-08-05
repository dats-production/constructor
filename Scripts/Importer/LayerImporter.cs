using System;
using System.Threading;
using Constructor;
using Constructor.DataStorage;
using Cysharp.Threading.Tasks;
using Importer.ImportStrategies;
using Services.LocalizationService;
using UI.Models;
using UI.Views;
using UnityEngine;
using Zenject;

namespace Importer
{
    public class LayerImporter : MonoBehaviour
    {
        private AssetLoaderFilePickerRx assetLoader;
        private IUIBlocker uiBlocker;
        private IDataStorage dataStorage;
        private ILayersDataProvider layersDataProvider;
        private Transform layersContainer;
        private ImportStrategyFactory importStrategyFactory;
        private IModalWindow modalWindow;
        private DetailInfoView detailInfoView;
        private ILocalizationService localizationService;

        [Inject]
        public void Construct(AssetLoaderFilePickerRx assetLoader,
            IUIBlocker uiBlocker, IDataStorage dataStorage,
            DiContainer diContainer, ILayersDataProvider layersDataProvider,
            ImportStrategyFactory importStrategyFactory, IModalWindow modalWindow,
            DetailInfoView detailInfoView, ILocalizationService localizationService)
        {
            this.localizationService = localizationService;
            this.assetLoader = assetLoader;
            this.uiBlocker = uiBlocker;
            this.dataStorage = dataStorage;
            this.layersDataProvider = layersDataProvider;
            this.importStrategyFactory = importStrategyFactory;
            this.modalWindow = modalWindow;
            this.detailInfoView = detailInfoView;
            layersContainer = diContainer.CreateEmptyGameObject("Layers Container").transform;
        }

        public async void OnImportButtonClick()
        {
            if (dataStorage.Layers.Count > 0)
            {
                var windowTitle = localizationService.Localize("Warning");
                var descriptionText = localizationService.Localize("Reimport Layers Warning");                
                var isConfirmed = await modalWindow.Show(windowTitle, descriptionText);
                if (!isConfirmed) return;
                ClearLayersInformation();
                ImportLayersFromRemote().Forget();
            }
            else ImportLayersFromRemote().Forget();
        }

        private async UniTask ImportLayersFromRemote()
        {
            using var cts = new CancellationTokenSource();
            uiBlocker.Show(assetLoader.OnProgressSubject, cts.Cancel);
    
            var layers = await layersDataProvider.GetLayers();
            foreach (var layer in layers)
            {
                try
                {
                    if (cts.IsCancellationRequested) break;
                    
                    await UniTask.DelayFrame(1, cancellationToken: cts.Token);
                    await ImportLayer(layer, cts);
                    dataStorage.AddLayer(layer);
                }
                catch (OperationCanceledException)
                {
                    uiBlocker.Hide();
                    return;
                }
            }
        }
    
        private async UniTask ImportLayer(Layer layer, CancellationTokenSource cancellationTokenSource)
        {
            var layerContainer = layersContainer.Find(layer.Name);

            layerContainer ??= new GameObject(layer.Name).transform;
            layerContainer.transform.SetParent(layersContainer);
            
            if (layerContainer.childCount > 0)
                foreach (Transform childLayerContainer in layerContainer.transform)
                    Destroy(childLayerContainer.gameObject);
            
            var detailType = layer.Details[0].GetType();
            await importStrategyFactory.Create(detailType).Import(layer, layerContainer, cancellationTokenSource);
        }

        private void ClearLayersInformation()
        {
            detailInfoView.Hide();
            dataStorage.RemoveLayers();
        }
    }
}
