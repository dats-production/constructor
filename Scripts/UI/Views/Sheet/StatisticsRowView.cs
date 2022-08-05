using Constructor;
using Constructor.DataStorage;
using TMPro;
using UnityEngine;
using Zenject;

namespace UI.Views.Sheet
{
    public class StatisticsRowView : MonoBehaviour
    {
        [SerializeField] private GameObject cellPrefab;
        [SerializeField] private Transform cells;
        [SerializeField] private TMP_Text layerName;
        private IDataStorage dataStorage;
        private DiContainer diContainer;

        [Inject]
        public void Construct(IDataStorage dataStorage, DiContainer diContainer)
        {
            this.dataStorage = dataStorage;
            this.diContainer = diContainer;
        }

        public void SetRowData(Layer layer)
        {
            layerName.text = layer.Name;
            var totalRarity = 0f;
            foreach (var detail in layer.Details)
            {
                var cell = diContainer.InstantiatePrefab(cellPrefab, cells);
                var cellView = cell.GetComponent<StatisticsCellView>();
                var actualRarity = dataStorage.GetDetailActualRarity(detail);
                totalRarity += actualRarity;
                cellView.SetCellData(detail, actualRarity);
            }
        }
    }
}
