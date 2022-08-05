using Constructor;
using Constructor.DataStorage;
using UI.Views.Sheet;
using UnityEngine;
using Zenject;

namespace UI.Views
{
    public class EditCharacterInfoSheetView : MonoBehaviour
    {
        [SerializeField] private GameObject rowPrefab;
        private DiContainer diContainer;

        [Inject]
        public void Construct(DiContainer diContainer)
        {
            this.diContainer = diContainer;
        }
        
        public void SetSheet(ICharacter oldCharacter, IDataStorage oldDataStorage, ICharacter newCharacter,  IDataStorage temporalDataStorage)
        {
            for (int i = 1; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
            
            var rowNumber = 1;
            foreach (var (layerName, newDetail) in newCharacter.Details)
            {
                var row = diContainer.InstantiatePrefab(rowPrefab, transform);
                var rowView = row.GetComponent<EditCharacterInfoRowView>();

                foreach (var (oldLayerName, oldDetail) in oldCharacter.Details)
                {
                    if(layerName != oldLayerName) continue;
                    rowView.SetRowData(rowNumber, layerName, oldDetail, newDetail, oldDataStorage, temporalDataStorage);
                }
                rowNumber++;
            }
        }
    }
}