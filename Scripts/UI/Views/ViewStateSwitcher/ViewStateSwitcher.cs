using System;
using UnityEngine;

namespace UI.Views
{
    public class ViewStateSwitcher : MonoBehaviour
    {
        [SerializeField] private ViewStateBase[] states;

        private string _currentState;

        public void SwitchState(Enum state)
        {
            SwitchState(state.ToString());
        }

        private void SwitchState(string stateName)
        {
            foreach (var state in states)
            {
                state.Apply(stateName);
            }

            _currentState = stateName;
        }
    }
}