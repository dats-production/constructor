using Michsky.UI.ModernUIPack;
using TMPro;
using UnityEngine;

namespace UI.Views
{
    public class LayerItemView : MonoBehaviour
    {
        [SerializeField] private TMP_Text name;
        [SerializeField] private TMP_Text detailsCount;
        [SerializeField] private CustomDropdown dropdown;
        [SerializeField] private TooltipContent warningTooltipContent;
    
        public TMP_Text Name
        {
            get => name;
            set => name = value;
        }

        public TMP_Text DetailsCount
        {
            get => detailsCount;
            set => detailsCount = value;
        }

        public CustomDropdown Dropdown
        {
            get => dropdown;
            set => dropdown = value;
        }

        public TooltipContent WarningTooltipContent
        {
            get => warningTooltipContent;
            set => warningTooltipContent = value;
        }
    }
}
