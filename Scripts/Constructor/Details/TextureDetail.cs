
namespace Constructor.Details
{
    public class TextureDetail : Detail
    {
        public TextureDetail(string name, float rarity, string layerName) : base(name, rarity, layerName)
        {
        }

        public override bool HasBody() => !string.IsNullOrEmpty(Name.Value);
    }
}
