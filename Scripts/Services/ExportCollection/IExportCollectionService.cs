using System;
using System.IO;
using System.Threading;
using Constructor.DataStorage;
using Cysharp.Threading.Tasks;
using Importer;
using Services.SaveLoad;
using Services.SaveLoad.Data;
using UI.Models;
using UniRx;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Services.ExportCollection
{
    public interface IExportCollectionService
    {
        UniTask ExportCollection(string pathName, int itemsCount);
    }

    public class ExportCollectionService : IExportCollectionService
    {
        private IDataStorage dataStorage;
        private ICharacterInfoModel characterInfoView;
        private ISaveLoadService saveLoadService;
        private RenderTexture characterRenderTexture;
        private IUIBlocker uiBlocker;
        private Camera camera;
        private CharacterViewer characterViewer;
        
        [Inject]
        public void Construct(IDataStorage dataStorage,
            ICharacterInfoModel characterInfoView,
            RenderTexture characterRenderTexture,
            ISaveLoadService saveLoadService,
            IUIBlocker uiBlocker,
            CharacterViewer characterViewer)
        {
            this.dataStorage = dataStorage;
            this.characterInfoView = characterInfoView;
            this.characterRenderTexture = characterRenderTexture;
            this.uiBlocker = uiBlocker;
            this.saveLoadService = saveLoadService;
            this.characterViewer = characterViewer;
            
            camera = GameObject.FindWithTag("DetailCamera").GetComponent<Camera>();
        }
        
        public async UniTask ExportCollection(string pathName, int imagesCount)
        {
            using var cts = new CancellationTokenSource();
            using var onProgressSubject = new Subject<float>();
            uiBlocker.Show(onProgressSubject, cts.Cancel);
            
            var tex = new Texture2D(characterRenderTexture.width, characterRenderTexture.height, TextureFormat.ARGB32, false);
            var tempRenderTexture = new RenderTexture(characterRenderTexture.width,characterRenderTexture.height, characterRenderTexture.depth, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
            
            for (var i = 0; i < imagesCount; i++)
            {
                var oldRT = RenderTexture.active;
                var character = dataStorage.Characters[i];

                characterInfoView.Show(character);
                camera.targetTexture = tempRenderTexture;
                try
                {
                    await UniTask.DelayFrame(1, cancellationToken: cts.Token);
                    characterViewer.TogglePauseAnimation(true);
                    camera.Render();
                    await UniTask.DelayFrame(1, cancellationToken: cts.Token);
                    RenderTexture.active = tempRenderTexture;
                    tex.ReadPixels(new Rect(0, 0, tempRenderTexture.width, tempRenderTexture.height), 0, 0);
                    tex.Apply();
                    await File.WriteAllBytesAsync(pathName + character.Name + ".png", tex.EncodeToPNG(), cts.Token);
                    saveLoadService.SaveCharacter(character.AsCharacterData(), pathName + character.Name + ".json");
                }
                catch (OperationCanceledException)
                {
                    Clear();
                    uiBlocker.Hide();
                    onProgressSubject.OnCompleted();
                    return;
                }

                onProgressSubject.OnNext(1f / ((float)imagesCount / (i + 1)));
                Clear();
                
                void Clear()
                {
                    camera.targetTexture = characterRenderTexture;
                    camera.Render();
                    RenderTexture.active = oldRT;
                }
            }
            characterViewer.TogglePauseAnimation(false);
            
            Object.Destroy(tex);
            Object.Destroy(tempRenderTexture);
        }
    }
}

