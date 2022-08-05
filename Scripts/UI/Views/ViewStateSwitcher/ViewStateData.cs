using System;
using UnityEngine;

namespace UI.Views
{
    [Serializable]
    public abstract class ViewStateData<TTarget, TValue>
    {
        [SerializeField] private string name;
        [SerializeField] private TValue value;

        public string Name => name;

        public TValue Value
        {
            get => value;
            set => this.value = value;
        }

        public abstract void Apply(TTarget target);
    }
}