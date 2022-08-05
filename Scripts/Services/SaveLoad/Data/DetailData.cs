using System;

namespace Services.SaveLoad.Data
{
    [Serializable]
    public class DetailData
    {
        public string Name;
        public float Rarity;
        public string Type;

        public DetailData(string name, float rarity, string type)
        {
            Name = name;
            Rarity = rarity;
            Type = type;
        }
    }
}