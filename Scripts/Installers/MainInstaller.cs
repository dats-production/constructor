using System;
using Constructor.DataStorage;
using Importer;
using Importer.ImportStrategies;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Installers
{
    public class MainInstaller : MonoInstaller
    {
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private MaterialSettings materialSettings;
        [SerializeField] private GameObject characterMainPrefab;
        [SerializeField] private Avatar characterAvatar;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<AssetLoaderFilePickerRx>().FromNewComponentOn(new GameObject("AssetLoader")).AsSingle();
            Container.BindInterfacesAndSelfTo<LayerImporter>().FromNewComponentOn(gameObject).AsSingle();
            Container.BindInterfacesTo<DataStorage>().FromNew().AsSingle();
            Container.Bind<PlayerInput>().FromInstance(playerInput).AsSingle();
            Container.Bind<MaterialSettings>().FromInstance(materialSettings).AsSingle();
            Container.Bind<MaterialPreprocessor>().FromNew().AsSingle();
            Container.Bind<CharacterViewer>().FromNew().AsSingle().WithArguments(characterMainPrefab);
            Container.Bind<Avatar>().FromInstance(characterAvatar).AsSingle();
            Container.BindFactory<Type, ILayerImportStrategy, ImportStrategyFactory>().FromFactory<ImportStrategyFactory>();
        }
    }
}