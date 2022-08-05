using Constructor.Details;
using UI.Models;
using UnityEngine;


namespace UI.Views.Details
{
    public class BackgroundDetailViewer : IDetailViewer
    {
        private BackgroundDetail backgroundDetail;
        private Material skyboxMaterial;
        
        private static readonly int FrontTex = Shader.PropertyToID("_FrontTex");
        private static readonly int BackTex = Shader.PropertyToID("_BackTex");
        private static readonly int LeftTex = Shader.PropertyToID("_LeftTex");
        private static readonly int RightTex = Shader.PropertyToID("_RightTex");
        private static readonly int UpTex = Shader.PropertyToID("_UpTex");
        private static readonly int DownTex = Shader.PropertyToID("_DownTex");

        public BackgroundDetailViewer(Material skyboxMaterial, BackgroundDetail detail)
        {
            this.skyboxMaterial = skyboxMaterial;
            SetDetail(detail);
        }
        
        public void Show()
        {
            skyboxMaterial.SetTexture(FrontTex, backgroundDetail.background);
            skyboxMaterial.SetTexture(BackTex, backgroundDetail.background);
            skyboxMaterial.SetTexture(LeftTex, backgroundDetail.background);
            skyboxMaterial.SetTexture(RightTex, backgroundDetail.background);
            skyboxMaterial.SetTexture(UpTex, backgroundDetail.background);
            skyboxMaterial.SetTexture(DownTex, backgroundDetail.background);
        }

        public void Hide()
        {
            skyboxMaterial.SetTexture(FrontTex, Texture2D.whiteTexture);
            skyboxMaterial.SetTexture(BackTex, Texture2D.whiteTexture);
            skyboxMaterial.SetTexture(LeftTex, Texture2D.whiteTexture);
            skyboxMaterial.SetTexture(RightTex, Texture2D.whiteTexture);
            skyboxMaterial.SetTexture(UpTex, Texture2D.whiteTexture);
            skyboxMaterial.SetTexture(DownTex, Texture2D.whiteTexture);
        }

        public void SetDetail(Detail detail)
        {
            backgroundDetail = (BackgroundDetail)detail;
        }
    }
}
 