using System.Linq;
using Constructor;
using Constructor.DataStorage;
using Constructor.Details;

namespace Services.SaveLoad.Data
{
    public static class DataExtensions
    {
        public static DetailData AsDetailData(this Detail detail)
        {
            return new DetailData(detail.Name.Value, detail.Rarity.Value, detail.layerName);
        }

        public static LayerData AsLayerData(this Layer layer)
        {
            var details = layer.Details.Select(x => x.AsDetailData()).ToList();
            return new LayerData(layer.Name, details);
        }

        public static CharacterData AsCharacterData(this ICharacter character)
        {
            var details = character.Details.ToDictionary(x => x.Key, x => x.Value.AsDetailData());
            return new CharacterData(character.Name.Value, details);
        }

        public static void ChangeLayersData(this LayersData data, IDataStorage dataStorage)
        {
            foreach (var layer in dataStorage.Layers)
            {
                foreach (var dataLayer in data.Layers.Where(dataLayer => dataLayer.Name == layer.Name))
                {
                    for (var i = 0; i < layer.Details.Count; i++)
                    {
                        layer.Details[i].Name.Value = dataLayer.Details[i].Name;
                        layer.Details[i].Rarity.Value = dataLayer.Details[i].Rarity;
                    }
                }
            }
        }
    }
}