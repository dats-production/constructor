using Constructor;
using UI.Views.Sheet;
using UnityEngine;
using Zenject;

namespace UI.Views
{
    public class CharacterInfoSheetView : MonoBehaviour
    {
        [SerializeField] private GameObject rowPrefab;
        private DiContainer diContainer;

        [Inject]
        public void Construct(DiContainer diContainer)
        {
            this.diContainer = diContainer;
        }
        
        public void SetSheet(ICharacter character)
        {
            ClearSheet();
            
            var rowNumber = 1;
            foreach (var (layerName, detail) in character.Details)
            {
                var row = diContainer.InstantiatePrefab(rowPrefab, transform);
                var rowView = row.GetComponent<CharacterInfoRowView>();
                rowView.SetRowData(rowNumber, layerName, detail);
                rowNumber++;
            }
        }

        public void ClearSheet()
        {
            for (int i = 1; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }            
        }
    }
}