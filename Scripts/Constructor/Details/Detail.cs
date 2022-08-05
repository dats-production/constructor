using System;
using UniRx;

namespace Constructor.Details
{
    public abstract class Detail : IComparable<Detail>
    {
        public IReactiveProperty<string> Name { get; set; } = new ReactiveProperty<string>();
        public IReactiveProperty<float> Rarity { get; set; } = new ReactiveProperty<float>();
        public string layerName;

        public Detail(string name, float rarity, string layerName)
        {
            Name.Value = name;
            Rarity.Value = rarity;
            this.layerName = layerName;
        }

        public int CompareTo(Detail other)
        {
            var nameComparison = String.Compare(Name.Value.Split(' ')[0], other.Name.Value.Split(' ')[0],
                StringComparison.Ordinal);
            
            if (nameComparison == 0)
            {
                var num1 = int.Parse(Name.Value.Split(' ')[1]);
                var num2 = int.Parse(other.Name.Value.Split(' ')[1]);
                return num1 > num2 ? 1 : -1;
            }

            return nameComparison;
        }

        public abstract bool HasBody();
    }
}
