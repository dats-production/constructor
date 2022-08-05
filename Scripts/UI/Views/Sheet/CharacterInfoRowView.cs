using System;
using Constructor.DataStorage;
using Constructor.Details;
using TMPro;
using UnityEngine;
using Zenject;

namespace UI.Views.Sheet
{
    public class CharacterInfoRowView : MonoBehaviour
    {
        [SerializeField] private TMP_Text rowNumber;
        [SerializeField] private TMP_Text layerName;
        [SerializeField] private TMP_Text detailName;
        [SerializeField] private TMP_Text initialRarity;
        [SerializeField] private TMP_Text actualRarity;

        private IDataStorage dataStorage;
        
        [Inject]
        public void Construct(IDataStorage dataStorage)
        {
            this.dataStorage = dataStorage;
        }
        
        public void SetRowData(int rowNumber, string layerName, Detail detail)
        {
            this.rowNumber.text = rowNumber.ToString();
            this.layerName.text = layerName;
            detailName.text = detail.Name.Value;
            initialRarity.text = detail.Rarity.Value + "%";
            actualRarity.text = Math.Round(dataStorage.GetDetailActualRarity(detail), 3) + "%";
        }
    }
}
