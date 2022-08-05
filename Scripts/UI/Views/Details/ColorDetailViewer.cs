using Constructor.Details;
using Importer;
using UI.Models;
using UnityEngine;
using Zenject;

namespace UI.Views.Details
{
    public class ColorDetailViewer : IDetailViewer, IApplyDetailViewerToObj
    {
        private ColorDetail colorDetail;
        private ModelPreview modelPreview;
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
        private MaterialSettings materialSettings;
        private FbxDetail currentlyShownDetail;
        
        [Inject]
        private void Construct(MaterialSettings materialSettings)
        {
            this.materialSettings = materialSettings;
        }
        
        public ColorDetailViewer(Detail detail, ModelPreview modelPreview, FbxDetail currentlyShownDetail)
        {
            this.currentlyShownDetail = currentlyShownDetail;
            this.modelPreview = modelPreview;
            SetDetail(detail);
        }
        
        public void Show()
        {
            GameObject sample = null;
            if (currentlyShownDetail == null)
            {
                sample = colorDetail.sampleGameObject;
                sample.transform.rotation = Quaternion.identity;
                colorDetail.sampleGameObject.SetActive(true);
            }
            else
            {
                sample = currentlyShownDetail.modelGameObject;
            }

            SetColor(sample);
        }

        public void SetDetail(Detail detail)
        {
            Hide();
            colorDetail = (ColorDetail)detail;
            if (currentlyShownDetail != null)
            {
                modelPreview.SetModelTransform(currentlyShownDetail.modelContainerObject.transform);
            }
            else
            {
                modelPreview.SetModelTransform(colorDetail.sampleGameObject.transform);
            }
        }

        public void Hide()
        {
            colorDetail?.sampleGameObject?.SetActive(false);
        }

        public void SetPreviewObj(FbxDetail fbxDetail)
        {
            currentlyShownDetail = fbxDetail;
            SetColor(currentlyShownDetail == null
                ? colorDetail.sampleGameObject
                : currentlyShownDetail.modelGameObject);
        }

        private void SetColor(GameObject detailGo)
        {
            var detailRenderer = detailGo.GetComponentInChildren<Renderer>();
            detailRenderer.material.EnableKeyword("_EMISSION");
            detailRenderer.material.SetColor(EmissionColor, colorDetail.Color.Value * materialSettings.emissionValue);
        }
    }
}
