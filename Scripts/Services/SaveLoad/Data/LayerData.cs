using System;
using System.Collections.Generic;

namespace Services.SaveLoad.Data
{
    [Serializable]
    public class LayerData
    {
        public string Name;
        public List<DetailData> Details;

        public LayerData(string name, List<DetailData> details)
        {
            Name = name;
            Details = details;
        }
    }
}