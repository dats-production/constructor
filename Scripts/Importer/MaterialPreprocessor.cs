using UnityEngine;
using Zenject;

namespace Importer
{
    public class MaterialPreprocessor : IPreprocessor<Material, MaterialPreprocessorData>
    {
        private MaterialSettings materialSettings;
        private static readonly int BumpScale = Shader.PropertyToID("_BumpScale");
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
        private static readonly int Smoothness = Shader.PropertyToID("_Smoothness");

        [Inject]
        private void Construct(MaterialSettings materialSettings)
        {
            this.materialSettings = materialSettings;
        }

        public Material Preprocess(Material toProcess, MaterialPreprocessorData preprocessorData)
        {
            toProcess.SetFloat(BumpScale, materialSettings.normalValue);
            toProcess.SetColor(EmissionColor, preprocessorData.emissionColor * materialSettings.emissionValue);
            toProcess.SetFloat(Smoothness, materialSettings.smoothnessValue);
            toProcess.EnableKeyword("_EMISSION");
            toProcess.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;

            return toProcess;
        }
    }
}
