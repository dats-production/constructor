using System.Threading;
using Constructor;
using Constructor.DataStorage;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Importer.ImportStrategies
{
    public class ColorLayerImportStrategy : ILayerImportStrategy
    {
        private IDataStorage dataStorage;

        [Inject]
        public void Construct(IDataStorage dataStorage)
        {
            this.dataStorage = dataStorage;
        }

        public async UniTask Import(Layer layer, Transform layerContainer, CancellationTokenSource cancellationTokenSource)
        {
            const string characterModelLayer = "CharacterModel";

            var sampleGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sampleGameObject.SetActive(false);
            sampleGameObject.name = "Sample Color Cube";
            sampleGameObject.layer = LayerMask.NameToLayer(characterModelLayer);
            sampleGameObject.transform.SetParent(layerContainer);

            for (var i = 0; i < layer.Details.Count; i++)
            {
                if (cancellationTokenSource.IsCancellationRequested) return;
                   dataStorage.ChangeDetail(layer, i, new ColorDetailBody { modelContainer = sampleGameObject });
            }

            await UniTask.DelayFrame(1, cancellationToken: cancellationTokenSource.Token);
        }
    }
}