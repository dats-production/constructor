using System.Collections.Generic;
using System.Linq;
using Constructor.Details;
using UniRx;

namespace Constructor
{
    public interface ICharacter
    {
        IReactiveProperty<string> Name { get; set; }
        IReadOnlyDictionary<string, Detail> Details { get; }
        void SetDetail(string layerName, Detail detail);
        ICharacter Copy();
    }

    public class Character : ICharacter
    {
        public IReactiveProperty<string> Name { get; set; } = new ReactiveProperty<string>();
        public IReadOnlyDictionary<string, Detail> Details => details;

        private readonly Dictionary<string, Detail> details = new();

        public Character(string name)
        {
            Name.Value = name;
        }

        public void SetDetail(string layerName, Detail detail)
        {
            details[layerName] = detail;
        }

        public ICharacter Copy()
        {
            var newCharacter = new Character(Name.Value);
            foreach (var detail in details)
            {
                newCharacter.SetDetail(detail.Key, detail.Value);
            }

            return newCharacter;
        }

        public override bool Equals(object obj)
        {
            if (obj is Character otherCharacter)
            {
                var detailsThis = details.Values.ToArray();
                var detailsOther = otherCharacter.Details.Values.ToArray();
                for (var i = 0; i < details.Count; i++)
                {
                    if (detailsThis[i] != detailsOther[i])
                        return false;
                }
                return true;
            }

            return false;
        }
        
        public override int GetHashCode()
        {
            return Details.Values.Aggregate((int)default, (current, detail) => current ^ detail.GetHashCode());
        }
    }
}