using System.Threading;
using Constructor;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Importer.ImportStrategies
{
    public interface ILayerImportStrategy
    {
        public UniTask Import(Layer layer, Transform layersContainer, CancellationTokenSource cancellationTokenSource);
    }
}