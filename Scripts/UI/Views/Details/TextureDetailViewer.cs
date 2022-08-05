using System.Linq;
using Constructor.DataStorage;
using Constructor.Details;
using Services.LocalizationService;
using UI.Models;
using UnityEngine;
using Zenject;

namespace UI.Views.Details
{
    public class TextureDetailViewer : IDetailViewer, IApplyDetailViewerToObj
    {
        private TextureDetail textureDetail;
        private ModelPreview modelPreview;
        private FbxDetail currentlyShownDetail;
        private IDataStorage dataStorage;
        private static readonly int BaseMap = Shader.PropertyToID("_BaseMap");
        private ILocalizationService localizationService;

        [Inject]
        private void Construct(IDataStorage dataStorage, ILocalizationService localizationService)
        {
            this.localizationService = localizationService;
            this.dataStorage = dataStorage;
        }
        
        public TextureDetailViewer(Detail detail, ModelPreview modelPreview, FbxDetail currentlyShownDetail)
        {
            this.currentlyShownDetail = currentlyShownDetail;
            this.modelPreview = modelPreview;
            SetDetail(detail);
        }
        
        public void Show()
        {
            currentlyShownDetail ??=
                ((FbxDetail) dataStorage.Layers.First(l => l.Details.Count > 0 && l.Details[0] is FbxDetail).Details[0]);
            
            if (currentlyShownDetail.modelContainerObject == null)
            {
                Debug.LogError(localizationService.Localize("You have not selected any models to display the color scheme."));
                return;
            }
            modelPreview.SetModelTransform(currentlyShownDetail.modelContainerObject.transform);

            SetTexture(currentlyShownDetail.modelContainerObject);
        }

        public void Hide() { }

        public void SetDetail(Detail detail)
        {
            Hide();
            textureDetail = (TextureDetail)detail;
        }

        public void SetPreviewObj(FbxDetail fbxDetail)
        {
            currentlyShownDetail = fbxDetail;
            SetTexture(currentlyShownDetail.modelContainerObject);
        }
        
        private void SetTexture(GameObject detailGo)
        {
            var detailRenderer = detailGo.GetComponentInChildren<Renderer>();
            detailRenderer.material.SetTexture(
                BaseMap,
                currentlyShownDetail.baseColors.Find(t => t.name == textureDetail.Name.Value));
        }
    }
}
