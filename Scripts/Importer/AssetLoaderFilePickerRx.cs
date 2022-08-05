using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Extensions;
using Services;
using TriLibCore;
using TriLibCore.General;
using TriLibCore.Mappers;
using TriLibCore.SFB;
using TriLibCore.Utils;
using UniRx;
using UnityEngine;
using Zenject;

namespace Importer
{
    public class AssetLoaderFilePickerRx : MonoBehaviour
    {
        public Subject<bool> OnLoadStartedSubjectSubject => onLoadStartedSubject;
        public Subject<AssetLoaderContext> OnLoadSubjectSubject => onLoadSubject;
        public Subject<AssetLoaderContext> OnMaterialsLoadSubjectSubject => onMaterialsLoadSubject;
        public Subject<float> OnProgressSubject => onProgressSubject;
        public Subject<IContextualizedError> OnErrorSubjectSubject => onErrorSubject;

        private readonly Subject<bool> onLoadStartedSubject = new Subject<bool>();
        private readonly Subject<AssetLoaderContext> onLoadSubject = new Subject<AssetLoaderContext>();
        private readonly Subject<AssetLoaderContext> onMaterialsLoadSubject = new Subject<AssetLoaderContext>();
        private readonly Subject<float> onProgressSubject = new Subject<float>();
        private readonly Subject<IContextualizedError> onErrorSubject = new Subject<IContextualizedError>();

        private AssetLoaderOptions assetLoaderOptions;
        private Action<AssetLoaderContext> onLoad;
        private Action<AssetLoaderContext> onMaterialsLoad;
        private Action<AssetLoaderContext, float> onProgress;
        private Action<IContextualizedError> onError;
        private string modelExtension;
        private GameObject wrapperGameObject;
        private CurrentTask currentTask;
        private IFileBrowser fileBrowser;
        private Avatar avatar;
        private string cashedPath;
        private CancellationToken currentCancellationToken;
        
        [Inject]
        public void Construct(IFileBrowser fileBrowser, Avatar avatar)
        {
            this.fileBrowser = fileBrowser;
            this.avatar = avatar;
        }

        public void LoadModelsFromFolderPickerAsync(string layerName, GameObject wrapperGameObject, CancellationToken cancellationToken)
        {
            this.wrapperGameObject = wrapperGameObject;
            currentCancellationToken = cancellationToken;
            try
            {
                fileBrowser.OpenFolderPanelAsync($"Select folder with: {layerName}", cashedPath, true, OnFbxModelsWithStreamSelected);
            }
            catch (Exception)
            {
                Dispatcher.InvokeAsync(DestroyMe);
                throw;
            }
        }

        private void OnError(IContextualizedError error) => onErrorSubject.OnNext(error);

        private void OnProgress(AssetLoaderContext ctx, float progress)
        {
            currentTask.SetProgress(ctx.Filename, progress);
            onProgressSubject.OnNext(currentTask.TotalProgress);
        }

        private void OnMaterialsLoad(AssetLoaderContext ctx)
        {
            onMaterialsLoadSubject.OnNext(ctx);
        }

        private void OnLoad(AssetLoaderContext ctx)
        {
            onLoadSubject.OnNext(ctx);
        }

        private void HandleFileLoading()
        {
            DoHandleFileLoading().Forget();
        }

        private async UniTask DoHandleFileLoading()
        {
            onLoadStartedSubject?.OnNext(currentTask.HasFiles());
            await UniTask.DelayFrame(1);
            if (!currentTask.HasFiles())
            {
                DestroyMe();
                return;
            }

            wrapperGameObject.transform.position = new Vector3(-100, -100, -100);//out of screen
            foreach (var modelFileWithStream in currentTask.Items)
            {
                if (currentCancellationToken.IsCancellationRequested) break;
                
                var modelFilename = modelFileWithStream.Name;
                var modelStream = modelFileWithStream.OpenStream();
                if (assetLoaderOptions == null)
                {
                    assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();
                    assetLoaderOptions.Timeout = 180;
                    assetLoaderOptions.ImportVisibility = false;
                    assetLoaderOptions.GenerateColliders = false;
                    assetLoaderOptions.ImportBlendShapes = false;
                    assetLoaderOptions.ScanForAlphaPixels = false;
                    assetLoaderOptions.AlphaMaterialMode = AlphaMaterialMode.None;
                  //  assetLoaderOptions.LoadMaterialsProgressively = true;
                    assetLoaderOptions.PivotPosition = PivotPosition.Center;
                    assetLoaderOptions.Avatar = avatar;
                    assetLoaderOptions.GenerateMipmaps = false;
                    assetLoaderOptions.TextureCompressionQuality = TextureCompressionQuality.NoCompression;
                    assetLoaderOptions.MergeVertices = false;
                }

                assetLoaderOptions.TextureMapper = ScriptableObject.CreateInstance<FilePickerTextureMapper>();
                assetLoaderOptions.ExternalDataMapper = ScriptableObject.CreateInstance<FilePickerExternalDataMapper>();
                modelExtension = modelFilename != null ? FileUtils.GetFileExtension(modelFilename, false) : null;
            
                if (modelStream != null)
                {
                    AssetLoader.LoadModelFromStream(modelStream, modelFilename, modelExtension, onLoad, onMaterialsLoad,
                        onProgress, onError, wrapperGameObject, assetLoaderOptions,
                        CustomDataHelper.CreateCustomDataDictionaryWithData(currentTask.Items), true, null, currentCancellationToken);
                    await UniTask.WaitUntil(() => Mathf.Approximately(1, currentTask.GetProgressForFile(modelFilename)));
                }
                else
                {
                    AssetLoader.LoadModelFromFile(modelFilename, onLoad, onMaterialsLoad, onProgress, onError,
                        wrapperGameObject, assetLoaderOptions,
                        CustomDataHelper.CreateCustomDataDictionaryWithData(currentTask.Items), true);
                    await UniTask.WaitUntil(() => Mathf.Approximately(1, currentTask.GetProgressForFile(modelFilename)));
                }
            }

            foreach (Transform model in wrapperGameObject.transform)
            {
                model.gameObject.SetLayerIncludeChildren(LayerMask.NameToLayer("CharacterModel"));
                model.gameObject.SetActive(false);
            }
            wrapperGameObject.transform.position = Vector3.zero;
            DestroyMe();
        }

        private void OnFbxModelsWithStreamSelected(IList<ItemWithStream> itemsWithStream)
        {
            if (itemsWithStream != null)
            {
                if (itemsWithStream.Count == 0)
                {
                    onProgressSubject.OnNext(1);//nothing to load
                    return;
                }

                cashedPath = itemsWithStream[0].Name;
                onLoad = OnLoad;
                onMaterialsLoad = OnMaterialsLoad;
                onProgress = OnProgress;
                onError = OnError;

                var items = new List<ItemWithStream>();

                if (!string.IsNullOrEmpty(itemsWithStream[0].Name))
                {
                    Directory.GetFiles(itemsWithStream[0].Name)
                        .ToList()
                        .FindAll(name =>
                            Readers.Extensions.Contains(FileUtils.GetFileExtension(name, false)))
                        .ForEach(path => items.Add(new ItemWithStream { Name = path }));
                    currentTask = new CurrentTask(items);
                    Dispatcher.InvokeAsync(HandleFileLoading);
                }
                else
                {
                    onProgressSubject.OnNext(1);//nothing to load
                }
            }
            else
            {
                DestroyMe();
            }
        }

        private void DestroyMe()
        {
            Destroy(this);
        }

        private class CurrentTask
        {
            public IList<ItemWithStream> Items { get; }

            public float TotalProgress
            {
                get
                {
                    var totalProgress = 0f;
                    foreach (var fileProgress in progresses)
                        totalProgress += fileProgress.Value;

                    return totalProgress / progresses.Count;
                }
            }
        
            private readonly Dictionary<string, float> progresses = new Dictionary<string, float>();
            public CurrentTask(IList<ItemWithStream> items)
            {
                Items = items;
                foreach (var item in items)
                {
                    progresses.Add(item.Name, 0);
                }
            }

            public bool HasFiles()
            {
                return Items != null && Items.Count > 0 && Items[0].HasData;
            }

            public void SetProgress(string filename, float progress)
            {
                progresses[filename] = progress;
            }
            
            public float GetProgressForFile(string modelFilename)
            {
                return progresses[modelFilename];
            }
        }
    }
}