using System.Collections.Generic;
using System.Linq;
using Extensions;
using Random = UnityEngine.Random;

namespace WalkerAlias
{
    public interface IWalkerAlias<out T>
    {
        T Sample();
    }

    public class WalkerAlias<T> : IWalkerAlias<T>
    {
        private readonly struct Bucket
        {
            public readonly double Threshold;
            public readonly T BottomValue;
            public readonly T TopValue;

            public Bucket(double threshold, T bottomValue, T topValue)
            {
                Threshold = threshold;
                BottomValue = bottomValue;
                TopValue = topValue;
            }
        }

        private readonly List<Bucket> buckets = new();
        private readonly IEnumerable<KeyValuePair<T, float>> values;

        public WalkerAlias(IEnumerable<KeyValuePair<T, float>> values)
        {
            this.values = values;
        }

        public T Sample()
        {
            if (buckets.Count == 0) Preprocess();
            
            var index = Random.Range(0, buckets.Count);
            var bucket = buckets[index];
            return bucket.Threshold >= Random.Range(0f, 1f)
                ? bucket.BottomValue
                : bucket.TopValue;
        }

        private void Preprocess()
        {
            var underfull = new List<int>();
            var overfull = new List<int>();

            var sortedValues = values.OrderBy(x => x.Value).Reverse().ToList();
            var cumulativeSum = sortedValues.Select(x => x.Value).Sum();
            var probabilities = sortedValues.Select(value => (double)value.Value * sortedValues.Count / cumulativeSum)
                .ToList();
            for (var i = 0; i < probabilities.Count; i++)
            {
                var probability = probabilities[i];
                switch (probability)
                {
                    case < 1:
                        underfull.Add(i);
                        break;
                    case > 1:
                        overfull.Add(i);
                        break;
                    default:
                        var value = sortedValues[i].Key;
                        buckets.Add(new Bucket(probability, value, value));
                        break;
                }
            }

            while (underfull.Any())
            {
                var underfullBucket = underfull.Pop();
                var overfullBucket = overfull.Pop();
                probabilities[overfullBucket] -= 1 - probabilities[underfullBucket];
                if (probabilities[overfullBucket] < 1)
                {
                    underfull.Add(overfullBucket);
                }
                else
                {
                    overfull.Add(overfullBucket);
                }

                buckets.Add(new Bucket(probabilities[underfullBucket], sortedValues[underfullBucket].Key,
                    sortedValues[overfullBucket].Key));

                if (overfull.Count != 0) continue;

                while (underfull.Any())
                {
                    var remainingBucket = underfull.Pop();
                    buckets.Add(new Bucket(probabilities[remainingBucket], sortedValues[remainingBucket].Key,
                        sortedValues[remainingBucket].Key));
                }
            }
        }
    }
}