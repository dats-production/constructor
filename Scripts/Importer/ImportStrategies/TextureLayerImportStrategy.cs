using System.Threading;
using Constructor;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Importer.ImportStrategies
{
    public class TextureLayerImportStrategy : ILayerImportStrategy
    {
        public async UniTask Import(Layer layer, Transform layersContainer, CancellationTokenSource cancellationTokenSource)
        {
            await UniTask.DelayFrame(1, cancellationToken: cancellationTokenSource.Token);
        }
    }
}