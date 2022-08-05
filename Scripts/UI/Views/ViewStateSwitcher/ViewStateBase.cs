using UnityEngine;

namespace UI.Views
{
    public abstract class ViewStateBase : MonoBehaviour
    {
        public abstract void Apply(string stateName);
    }

    public class ViewState<T> : ViewStateBase where T : ViewStatesHolderBase
    {
        [SerializeField] private T[] states;

        public override void Apply(string stateName)
        {
            foreach (var state in states)
            {
                state.Apply(stateName);
            }
        }
    }
}