using System;
using Constructor.DataStorage;
using Constructor.Details;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace UI.Views.Sheet
{
    public class EditCharacterInfoRowView : MonoBehaviour
    {
        [SerializeField] private TMP_Text rowNumber;
        [SerializeField] private TMP_Text layerName;
        [SerializeField] private TMP_Text oldDetailName;
        [SerializeField] private TMP_Text newDetailName;
        [SerializeField] private TMP_Text oldDetailRarityText;
        [SerializeField] private TMP_Text newDetailRarityText;
        [SerializeField] private TMP_Text oldDetailRarityDifferenceText;
        [SerializeField] private TMP_Text newDetailRarityDifferenceText;
        [SerializeField] private Image oldDetailArrow;
        [SerializeField] private Image newDetailArrow;

        private Color increaseColor = Color.green;
        private Color decreaseColor = Color.red;
        private Quaternion increaseArrowRotation = Quaternion.Euler(new Vector3(0, 0, 180));
        private Quaternion decreaseArrowRotation = Quaternion.Euler(new Vector3(0, 0, 0));

        public void SetRowData(int rowNumber, string layerName, Detail oldDetail, Detail newDetail,  IDataStorage oldDataStorage, IDataStorage temporalDataStorage)
        {
            this.rowNumber.text = rowNumber.ToString();
            this.layerName.text = layerName;
            oldDetailName.text = oldDetail.Name.Value;
            newDetailName.text = newDetail.Name.Value;

            var oldDetailRarity = temporalDataStorage.GetDetailActualRarity(oldDetail);
            var newDetailRarity = temporalDataStorage.GetDetailActualRarity(newDetail);
            oldDetailRarityText.text = Math.Round(oldDetailRarity, 3) + "%";
            newDetailRarityText.text = Math.Round(newDetailRarity, 3) + "%";

            var oldDetailRarityDifference =  oldDetailRarity - oldDataStorage.GetDetailActualRarity(oldDetail);
            var newDetailRarityDifference = newDetailRarity - oldDataStorage.GetDetailActualRarity(newDetail);

            SetDifferenceView(oldDetailRarityDifference, oldDetailRarityDifferenceText, oldDetailArrow);
            SetDifferenceView(newDetailRarityDifference, newDetailRarityDifferenceText, newDetailArrow);
        }

        private void SetDifferenceView(double rarityDifference, TMP_Text rarityDifferenceText, Image arrow)
        {
            switch (rarityDifference)
            {
                case > 0:
                    rarityDifferenceText.color = increaseColor;
                    arrow.color = increaseColor;
                    arrow.rectTransform.rotation = increaseArrowRotation;
                    arrow.gameObject.SetActive(true);
                    break;
                case < 0:
                    rarityDifferenceText.color = decreaseColor;
                    arrow.color = decreaseColor;
                    arrow.rectTransform.rotation = decreaseArrowRotation;
                    arrow.gameObject.SetActive(true);
                    break;
                default:
                    arrow.gameObject.SetActive(false);
                    break;
            }
            rarityDifferenceText.text = Math.Round(rarityDifference, 3) + "%";
        }
    }
}
