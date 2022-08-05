using System;
using System.Linq;
using UnityEngine;

namespace UI.Views
{
    public abstract class ViewStatesHolderBase
    {
        public abstract void Apply(string stateName);
    }

    [Serializable]
    public class ViewStatesHolder<TTarget, TValue, TState> : ViewStatesHolderBase
        where TState : ViewStateData<TTarget, TValue>
    {
        [SerializeField] private TTarget target;
        [SerializeField] private TState[] states;

        public override void Apply(string stateName)
        {
            var state = states.FirstOrDefault(t => t.Name.Equals(stateName));
            state?.Apply(target);
        }
    }
}