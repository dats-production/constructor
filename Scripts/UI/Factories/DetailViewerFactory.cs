using System.Collections.Generic;
using Constructor.Details;
using EnvironmentControl;
using Importer;
using UI.Models;
using UI.Views;
using UI.Views.Details;
using Zenject;

namespace UI.Factories
{
    public class DetailViewerFactory : PlaceholderFactory<Detail, IDetailViewer>
    {
        private FbxDetailViewer fbxDetailViewer;
        private ColorDetailViewer colorDetailViewer;
        private TextureDetailViewer textureDetailViewer;
        private BackgroundDetailViewer backgroundDetailViewer;
        
        private ModelPreview modelPreview;
        private DiContainer diContainer;
        private FbxDetail currentlyShownFbxDetail;
        private MaterialSettings materialSettings;
        private EnvironmentController environmentController;

        [Inject]
        public DetailViewerFactory(ModelPreview modelPreview, MaterialSettings materialSettings,
            DiContainer diContainer, EnvironmentController environmentController)
        {
            this.modelPreview = modelPreview;
            this.diContainer = diContainer;
            this.materialSettings = materialSettings;
            this.environmentController = environmentController;
        }
        
        public override IDetailViewer Create(Detail detail)
        {
            switch (detail)
            {
                case FbxDetail fbxDetail:
                    if (fbxDetailViewer == null)
                    {
                        fbxDetailViewer = new FbxDetailViewer(fbxDetail, modelPreview);
                    }
                    else
                    {
                        fbxDetailViewer.SetDetail(fbxDetail);
                    }

                    currentlyShownFbxDetail = fbxDetail;
                    colorDetailViewer?.SetPreviewObj(currentlyShownFbxDetail);
                    textureDetailViewer?.SetPreviewObj(currentlyShownFbxDetail);
                    return fbxDetailViewer;
                case ColorDetail colorDetail:
                    if (colorDetailViewer == null)
                    {
                        var args = new List<TypeValuePair>();
                        args.Add(new TypeValuePair(typeof(ColorDetail), colorDetail));
                        args.Add(new TypeValuePair(typeof(ModelPreview), modelPreview));
                        args.Add(new TypeValuePair(typeof(FbxDetail), currentlyShownFbxDetail));
                        colorDetailViewer = diContainer.InstantiateExplicit<ColorDetailViewer>(args);
                    }
                    else
                    {
                        colorDetailViewer.SetPreviewObj(currentlyShownFbxDetail);
                        colorDetailViewer.SetDetail(detail);
                    }

                    return colorDetailViewer;
                case TextureDetail textureDetail:
                    if (textureDetailViewer == null)
                    {
                        var args = new List<TypeValuePair>();
                        args.Add(new TypeValuePair(typeof(TextureDetail), textureDetail));
                        args.Add(new TypeValuePair(typeof(ModelPreview), modelPreview));
                        args.Add(new TypeValuePair(typeof(FbxDetail), currentlyShownFbxDetail));
                        textureDetailViewer = diContainer.InstantiateExplicit<TextureDetailViewer>(args);
                    }
                    else
                    {
                        textureDetailViewer.SetPreviewObj(currentlyShownFbxDetail);
                        textureDetailViewer.SetDetail(detail);
                    }

                    return textureDetailViewer;
                case BackgroundDetail backgroundDetail:
                    if (backgroundDetailViewer == null)
                    {
                        backgroundDetailViewer = new BackgroundDetailViewer(materialSettings.skyboxMaterial, backgroundDetail);
                    }
                    else
                    {
                        backgroundDetailViewer.SetDetail(backgroundDetail);
                    }
                    environmentController.SelectPreset(backgroundDetail.Name.Value);
                    return backgroundDetailViewer;
                default:
                    if (fbxDetailViewer == null)
                    {
                        fbxDetailViewer = new FbxDetailViewer(detail, modelPreview);
                    }
                    else
                    {
                        fbxDetailViewer.SetDetail(detail);
                    }

                    return fbxDetailViewer;
            }
        }
    }
}
