using System;
using Constructor.Details;
using TMPro;
using UnityEngine;

namespace UI.Views.Sheet
{
    public class StatisticsCellView : MonoBehaviour
    {
        [SerializeField] private TMP_Text detailName;
        [SerializeField] private TMP_Text initialRarity;
        [SerializeField] private TMP_Text actualRarity;

        public void SetCellData(Detail detail, float actualRarity)
        {
            detailName.text = detail.Name.Value;
            initialRarity.text = detail.Rarity.Value + "%";
            this.actualRarity.text = Math.Round(actualRarity, 3) + "%";
        }
    }
}
