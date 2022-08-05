using Constructor.DataStorage;
using UI.Views.Sheet;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Views
{
    public class CollectionStatisticsView : MonoBehaviour, IInitializable
    {
        [SerializeField] private Transform sheet;
        [SerializeField] private Button closeButton;
        [SerializeField] private GameObject rowPrefab;
        private IDataStorage dataStorage;
        private DiContainer diContainer;
        private ModelPreview modelPreview;
        
        [Inject]
        public void Construct(IDataStorage dataStorage, DiContainer diContainer, ModelPreview modelPreview)
        {
            this.dataStorage = dataStorage;
            this.diContainer = diContainer;
            this.modelPreview = modelPreview;
        }
        
        public void Initialize()
        {
            closeButton.OnClickAsObservable().Subscribe(x =>
            {
                modelPreview.gameObject.SetActive(true);
                gameObject.SetActive(false);
                foreach (Transform row in sheet)
                    Destroy(row.gameObject);
                
            }).AddTo(closeButton);
        }
        
        public void SetSheet()
        {
            modelPreview.gameObject.SetActive(false);
            foreach (var layer in dataStorage.Layers)
            {
                var row = diContainer.InstantiatePrefab(rowPrefab, sheet);
                var rowView = row.GetComponent<StatisticsRowView>();
                rowView.SetRowData(layer);
            }
        }
    }
}