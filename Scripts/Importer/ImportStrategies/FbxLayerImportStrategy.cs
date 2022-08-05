using System.Threading;
using Constructor;
using Constructor.DataStorage;
using Cysharp.Threading.Tasks;
using TriLibCore;
using UI.Models;
using UI.Views;
using UniRx;
using UnityEngine;
using Zenject;

namespace Importer.ImportStrategies
{
    public class FbxLayerImportStrategy : ILayerImportStrategy
    {
        private AssetLoaderFilePickerRx assetLoader;
        private NotificationView notificationView;
        private IDataStorage dataStorage;
        private MaterialPreprocessor materialPreprocessor;
        private IUIBlocker uiBlocker;
        
        [Inject]
        public void Construct(AssetLoaderFilePickerRx assetLoader,
            NotificationView notificationView,
            IDataStorage dataStorage,
            MaterialPreprocessor materialPreprocessor,
            IUIBlocker uiBlocker)
        {
            this.assetLoader = assetLoader;
            this.notificationView = notificationView;
            this.dataStorage = dataStorage;
            this.materialPreprocessor = materialPreprocessor;
            this.uiBlocker = uiBlocker;
        }
        
        public async UniTask Import(Layer layer, Transform layerContainer, CancellationTokenSource cancellationTokenSource)
        {
            uiBlocker.Show(assetLoader.OnProgressSubject, cancellationTokenSource.Cancel);
            var loadCompleted = false;
            var disposable = assetLoader.OnProgressSubject.Subscribe(p => loadCompleted = Mathf.Approximately(1, p));
            assetLoader.LoadModelsFromFolderPickerAsync(layer.Name, layerContainer.gameObject, cancellationTokenSource.Token);
            await UniTask.WaitUntil(() => loadCompleted, cancellationToken: cancellationTokenSource.Token);
            disposable.Dispose();

            if (layer.Details.Count < layerContainer.transform.childCount)
                notificationView.AddNotification($"You have loaded more .fbx models ({layerContainer.transform.childCount}) " +
                                                 $"then Layer \"{layer.Name}\" should include ({layer.Details.Count})", LogType.Warning);
            
            var processorData = new MaterialPreprocessorData { emissionColor = Color.white };
            for (var i = 0; i < layer.Details.Count; i++) 
            {
                if (cancellationTokenSource.IsCancellationRequested) return;
                if (i < layerContainer.transform.childCount)
                {
                    var detailGameObjectContainer = layerContainer.GetChild(i).gameObject;
                    materialPreprocessor.Preprocess(
                        detailGameObjectContainer.GetComponentInChildren<SkinnedMeshRenderer>().materials[0], processorData);
                    
                    var detailRenderer = detailGameObjectContainer.GetComponentInChildren<SkinnedMeshRenderer>();
                    var assetUnloader = detailGameObjectContainer.GetComponentInChildren<AssetUnloader>();
                    dataStorage.ChangeDetail(layer, i, new FBXDetailBody
                        {
                            modelContainer = detailGameObjectContainer,
                            modelGameObject = detailRenderer.gameObject,
                            texturesForDetail = assetUnloader.GetBaseColorList()
                        }
                    );
                }
                else
                {
                    dataStorage.ChangeDetail(layer, i, new FBXDetailBody());
                }
            }
            await UniTask.DelayFrame(1, cancellationToken: cancellationTokenSource.Token);
        }
    }
}