using System.Linq;
using Constructor;
using Constructor.Details;
using EnvironmentControl;
using Extensions;
using UnityEngine;
using Zenject;

namespace Importer
{
    public class CharacterViewer
    {
        private GameObject characterBasePrefab;
        private GameObject skeleton;
        private Transform charactersContainer;
        private MaterialSettings materialImportSettings;
        private EnvironmentController environmentController;
        
        private static readonly int FrontTex = Shader.PropertyToID("_FrontTex");
        private static readonly int BackTex = Shader.PropertyToID("_BackTex");
        private static readonly int LeftTex = Shader.PropertyToID("_LeftTex");
        private static readonly int RightTex = Shader.PropertyToID("_RightTex");
        private static readonly int UpTex = Shader.PropertyToID("_UpTex");
        private static readonly int DownTex = Shader.PropertyToID("_DownTex");
        private static readonly int BaseMap = Shader.PropertyToID("_BaseMap");
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

        [Inject]
        public CharacterViewer(GameObject characterBasePrefab, MaterialSettings materialImportSettings, EnvironmentController environmentController)
        {
            this.characterBasePrefab = characterBasePrefab;
            this.materialImportSettings = materialImportSettings;
            this.environmentController = environmentController;
            
            charactersContainer = new GameObject("Characters Container").transform;
            charactersContainer.position = charactersContainer.position.WithY(-2.4f);//character size offset
            charactersContainer.Rotate(Vector3.left, 10);
        }

        public void ClearContainer()
        {
            foreach (Transform child in charactersContainer)
            {
                Object.Destroy(child.gameObject);
            }
            
            var skyboxMaterial = materialImportSettings.skyboxMaterial;
            skyboxMaterial.SetTexture(FrontTex, Texture2D.whiteTexture);
            skyboxMaterial.SetTexture(BackTex, Texture2D.whiteTexture);
            skyboxMaterial.SetTexture(LeftTex, Texture2D.whiteTexture);
            skyboxMaterial.SetTexture(RightTex, Texture2D.whiteTexture);
            skyboxMaterial.SetTexture(UpTex, Texture2D.whiteTexture);
            skyboxMaterial.SetTexture(DownTex, Texture2D.whiteTexture);
        }
        
        public Transform AssembleCharacter(ICharacter character)
        {
            ClearContainer();
            
            skeleton = GameObject.Instantiate(characterBasePrefab, charactersContainer);
            skeleton.name = character.Name.Value;

            var textureName = character.Details.FirstOrDefault(d => d.Value is TextureDetail).Value.Name.Value;
            var emissionColor = ((ColorDetail)(character.Details.FirstOrDefault(d => d.Value is ColorDetail).Value)).Color.Value;
            var backgroundDetail =
                ((BackgroundDetail)character.Details.FirstOrDefault(d => d.Value is BackgroundDetail).Value);
            
            foreach (var detail in character.Details.Values)
            {
                if (detail is FbxDetail fbxDetail && fbxDetail.modelGameObject != null)
                {
                    var detailCopy = GameObject.Instantiate(fbxDetail.modelGameObject);
                    var boneAssigner = detailCopy.AddComponent<BoneAssigner>();
                    var renderer = detailCopy.GetComponentInChildren<Renderer>();
                    renderer.material.SetTexture(
                        BaseMap, 
                        fbxDetail.baseColors.Find(t => t.name == textureName));
                    renderer.material.SetColor(EmissionColor, emissionColor * materialImportSettings.emissionValue);
                    renderer.material.EnableKeyword("_EMISSION");
                    renderer.material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                    boneAssigner.AssignToBones(skeleton.transform);
                    detailCopy.SetActive(true);
                }
            }

            var skyboxMaterial = materialImportSettings.skyboxMaterial;
            skyboxMaterial.SetTexture(FrontTex, backgroundDetail.background);
            skyboxMaterial.SetTexture(BackTex, backgroundDetail.background);
            skyboxMaterial.SetTexture(LeftTex, backgroundDetail.background);
            skyboxMaterial.SetTexture(RightTex, backgroundDetail.background);
            skyboxMaterial.SetTexture(UpTex, backgroundDetail.background);
            skyboxMaterial.SetTexture(DownTex, backgroundDetail.background);
            environmentController.SelectPreset(backgroundDetail.Name.Value);
            
            skeleton.transform.Rotate(Vector3.up, 180);
            skeleton.transform.localScale = new Vector3(2.4f, 2.4f, 2.4f);
            return skeleton.transform;
        }

        public void TogglePauseAnimation(bool isPaused)
        {
            var animator = skeleton.GetComponent<Animator>();
            animator.Rebind();
            animator.speed = isPaused ? 0 : 1;
        }
    }
}
