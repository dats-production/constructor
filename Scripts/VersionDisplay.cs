using Cysharp.Threading.Tasks;
using Services.LocalizationService;
using TMPro;
using UnityEngine;
using Zenject;

public class VersionDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;
    private ILocalizationService localizationService;

    [Inject]
    public void Construct(ILocalizationService localizationService)
    {
        this.localizationService = localizationService;
    }

    private async void Start()
    {
        await UniTask.DelayFrame(10);
        textMeshProUGUI.text = $"{localizationService.Localize("Version")}: {Application.version}";           
    }
}
