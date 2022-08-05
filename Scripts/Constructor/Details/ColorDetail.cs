using UniRx;
using UnityEngine;

namespace Constructor.Details
{
    public class ColorDetail : Detail
    {
        public IReactiveProperty<Color> Color { get; set; } = new ReactiveProperty<Color>();
        public GameObject sampleGameObject;

        public ColorDetail(Color color, string name, float rarity, string layerName) : base(name, rarity, layerName)
        {
            Color.Value = color;
        }

        public override bool HasBody() => true;
    }
}
