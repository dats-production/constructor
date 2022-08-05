using Configs;
using Constructor;
using Services;
using Services.ExportCollection;
using Services.GoogleSheets;
using Services.LocalizationService;
using Services.SaveLoad;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class ServicesInstaller : MonoInstaller
    {
        [SerializeField] private GoogleSheetsConfig googleSheetsConfig;

        public override void InstallBindings()
        {
            Container.BindInterfacesTo<FileBrowser>().AsSingle().NonLazy();
            Container.BindInterfacesTo<SaveLoadService>().AsSingle().NonLazy();
            Container.BindInterfacesTo<GoogleSheetsService>().AsSingle().WithArguments(googleSheetsConfig.ApplicationName, googleSheetsConfig.CredentialsPath);
            Container.BindInterfacesTo<LayersDataProvider>().AsSingle().WithArguments(googleSheetsConfig.LayersSpreadsheetId).NonLazy();
            Container.BindInterfacesTo<ExportCollectionService>().AsSingle().NonLazy();
            Container.BindInterfacesTo<LocalizationService>().AsSingle().NonLazy();
        }
    }
}