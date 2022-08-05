using UI.Models;
using UniRx;
using UnityEngine;
using Zenject;

namespace UI.Views
{
    public class CollectionStatisticsButton : MonoBehaviour
    {
        [SerializeField] private ButtonView button;

        [Inject]
        public void Construct(CollectionStatisticsView collectionStatisticsView)
        {
            var model = new CollectionStatisticsButtonModel(collectionStatisticsView);
            model.Button.Subscribe(button.Bind).AddTo(this);
        }
    }
}