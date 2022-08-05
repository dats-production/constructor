using System;
using System.Collections.Generic;

namespace Services.SaveLoad.Data
{
    [Serializable]
    public class CharacterDetailsData
    {
        public List<DetailData> Values = new();

        public CharacterDetailsData(Dictionary<string, DetailData> details)
        {
            foreach (var detail in details)
            {
                Values.Add(detail.Value);
            }
        }
    }
}