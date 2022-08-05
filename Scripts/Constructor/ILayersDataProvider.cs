using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Constructor
{
    public interface ILayersDataProvider
    {
        UniTask<IReadOnlyList<Layer>> GetLayers();
        UniTask ExportSheets(List<Layer> layers);
    }
}