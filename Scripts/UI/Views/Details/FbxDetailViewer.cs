using Constructor.Details;
using UI.Models;
using UnityEngine;

namespace UI.Views.Details
{
    public class FbxDetailViewer : IDetailViewer
    {
        private FbxDetail fbxDetail;
        private ModelPreview modelPreview;

        public FbxDetailViewer(Detail detail, ModelPreview modelPreview)
        {
            this.modelPreview = modelPreview;
            SetDetail(detail);
        }
        
        public void Show()
        {
            if (fbxDetail.modelContainerObject == null) return;
            
            fbxDetail.modelContainerObject.transform.rotation = Quaternion.identity;
            fbxDetail.modelContainerObject.SetActive(true);
        }

        public void SetDetail(Detail detail)
        {
            Hide();
            fbxDetail = (FbxDetail)detail;
            modelPreview.SetModelTransform(fbxDetail.modelContainerObject.transform);
        }
        
        public void Hide()
        {
            fbxDetail?.modelContainerObject?.SetActive(false);
        }
    }
}
