using UnityEngine;

namespace Constructor.Details
{
    public class BackgroundDetail : Detail
    {
        public Texture2D background;

        public BackgroundDetail(Texture2D background, string name, float rarity, string layerName) : base(name, rarity, layerName)
        {
            this.background = background;
        }
        
        public override bool HasBody() => background != null;
    }
}
