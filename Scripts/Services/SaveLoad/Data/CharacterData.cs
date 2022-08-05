using System;
using System.Collections.Generic;

namespace Services.SaveLoad.Data
{
    [Serializable]
    public class CharacterData
    {
        public string Name;
        public CharacterDetailsData Details;

        public CharacterData(string name, Dictionary<string, DetailData> details)
        {
            Name = name;
            Details = new CharacterDetailsData(details);
        }
    }
}