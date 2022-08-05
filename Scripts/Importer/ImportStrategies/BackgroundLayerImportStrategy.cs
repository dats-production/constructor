using System.Threading;
using Constructor;
using Constructor.DataStorage;
using Cysharp.Threading.Tasks;
using TriLibCore;
using UI.Models;
using UniRx;
using UnityEngine;
using Zenject;

namespace Importer.ImportStrategies
{
    public class BackgroundLayerImportStrategy : ILayerImportStrategy
    {
        private AssetLoaderFilePickerRx assetLoader;
        private IDataStorage dataStorage;
        private IUIBlocker uiBlocker;

        [Inject]
        public void Construct(AssetLoaderFilePickerRx assetLoader,
            IDataStorage dataStorage,
            IUIBlocker uiBlocker)
        {
            this.assetLoader = assetLoader;
            this.dataStorage = dataStorage;
            this.uiBlocker = uiBlocker;
        }

        public async UniTask Import(Layer layer, Transform layersContainer, CancellationTokenSource cancellationTokenSource)
        {
            uiBlocker.Show(assetLoader.OnProgressSubject, cancellationTokenSource.Cancel);
            var loadCompleted = false;
            var disposable = assetLoader.OnProgressSubject.Subscribe(p => loadCompleted = Mathf.Approximately(1, p));
            assetLoader.LoadModelsFromFolderPickerAsync(layer.Name, layersContainer.gameObject, cancellationTokenSource.Token);
            await UniTask.WaitUntil(() => loadCompleted, cancellationToken: cancellationTokenSource.Token);
            disposable.Dispose();

            if (layersContainer.childCount == 0) return;

            var detailGameObjectContainer = layersContainer.GetChild(0).gameObject;
            var textures = detailGameObjectContainer.GetComponentInChildren<AssetUnloader>().GetBaseColorList();
            for (var i = 0; i < layer.Details.Count; i++)
            {
                dataStorage.ChangeDetail(layer, i,
                    i < textures.Count
                        ? new BackgroundDetailBody { background = textures[i] }
                        : new BackgroundDetailBody());
                
                if (cancellationTokenSource.IsCancellationRequested) return;
            }

            await UniTask.DelayFrame(1, cancellationToken: cancellationTokenSource.Token);
        }
    }
}