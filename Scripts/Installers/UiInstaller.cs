using System.Collections.Generic;
using Configs;
using Constructor;
using Constructor.Details;
using Constructor.Validator;
using Constructor.Validator.Validations;
using EnvironmentControl;
using UI;
using UI.Factories;
using UI.Models;
using UI.Navigation;
using UI.Views;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class UiInstaller : MonoInstaller
    {
        [SerializeField] private UINavigator uiNavigator;
        [SerializeField] private GenerateCharactersButton generateCharactersButton;
        [SerializeField] private CharactersCollectionInfoView charactersCollectionInfo;
        [SerializeField] private CharacterInfoView characterInfo;
        [SerializeField] private SaveLayersButton saveLayersButton;
        [SerializeField] private UpdateLayersButton updateLayersButton;
        [SerializeField] private LoadLayersButton loadLayersButton;
        [SerializeField] private SaveCharactersButton saveCharactersButton;
        [SerializeField] private CollectionStatisticsButton collectionStatisticsButton;
        [SerializeField] private UIViewConfig uiViewConfig;
        [SerializeField] private RenderTexture characterRenderTexture;
        [SerializeField] private EnvironmentController environmentController;
        [SerializeField] private TopMenu topMenu;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<UiBlocker>().FromComponentsInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<InputModalWindow>().FromComponentsInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<ModalWindow>().FromComponentsInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<LayerListView>().FromComponentsInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<NotificationView>().FromComponentsInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<CollectionStatisticsView>().FromComponentsInHierarchy().AsSingle();
            Container.BindInterfacesTo<UINavigator>().FromInstance(uiNavigator).AsSingle();
            Container.Bind<GenerateCharactersButton>().FromInstance(generateCharactersButton).AsSingle();
            Container.BindInterfacesTo<CharactersCollectionInfoView>().FromInstance(charactersCollectionInfo).AsSingle();
            Container.BindInterfacesTo<CharacterInfoView>().FromInstance(characterInfo).AsSingle();
            Container.BindFactory<Detail, IDetailViewer, DetailViewerFactory>().FromFactory<DetailViewerFactory>();
            Container.Bind<SaveLayersButton>().FromInstance(saveLayersButton).AsSingle();
            Container.Bind<UpdateLayersButton>().FromInstance(updateLayersButton).AsSingle();
            Container.Bind<LoadLayersButton>().FromInstance(loadLayersButton).AsSingle();
            Container.Bind<SaveCharactersButton>().FromInstance(saveCharactersButton).AsSingle();
            Container.Bind<CollectionStatisticsButton>().FromInstance(collectionStatisticsButton).AsSingle();
            Container.BindFactory<List<IValidation<Layer, List<Detail>>>, Validator<Layer, List<Detail>>, ValidatorFactory<Layer, List<Detail>>>()
                .FromFactory<ValidatorFactory<Layer, List<Detail>>>();
            Container.BindFactory<List<IValidation<string, string>>, Validator<string, string>, ValidatorFactory<string, string>>()
                .FromFactory<ValidatorFactory<string, string>>();
            Container.BindInterfacesAndSelfTo<GeneratedCharactersCountInputValidation>().AsSingle();
            Container.BindInterfacesAndSelfTo<ExportedCollectionCountInputValidation>().AsSingle();
            Container.BindInterfacesAndSelfTo<DetailNameValidation>().AsSingle();
            Container.BindInterfacesAndSelfTo<CharacterNameValidation>().AsSingle();
            Container.BindInterfacesAndSelfTo<ParseIntValidation>().AsSingle();
            Container.BindInterfacesAndSelfTo<BodyValidation>().AsSingle();
            Container.BindInterfacesAndSelfTo<RarityValidation>().AsSingle();
            Container.Bind<DetailInfoView>().FromComponentsInHierarchy().AsSingle();
            Container.Bind<ModelPreview>().FromComponentsInHierarchy().AsSingle();
            Container.BindInterfacesTo<UIViewConfig>().FromScriptableObject(uiViewConfig).AsSingle();
            Container.Bind<RenderTexture>().FromInstance(characterRenderTexture).AsSingle();
            Container.Bind<EnvironmentController>().FromInstance(environmentController).AsSingle();
            Container.BindInterfacesTo<TopMenu>().FromInstance(topMenu).AsSingle();
        }
    }
}