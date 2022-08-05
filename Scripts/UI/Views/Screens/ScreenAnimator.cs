using UnityEngine;

namespace UI.Views.Screens
{
    public class ScreenAnimator : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        private const string OpenAnimation = "Demo Window In";
        private const string CloseAnimation = "Demo Window Out";

        public void Open()
        {
            animator.Play(OpenAnimation);
        }

        public void Close()
        {
            animator.Play(CloseAnimation);
        }
    }
}