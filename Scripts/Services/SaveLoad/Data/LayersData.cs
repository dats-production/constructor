using System;
using System.Collections.Generic;

namespace Services.SaveLoad.Data
{
    [Serializable]
    public class LayersData
    {
        public List<LayerData> Layers;

        public LayersData(List<LayerData> layers)
        {
            Layers = layers;
        }
    }
}