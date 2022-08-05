using UnityEngine;

namespace Importer
{
    [CreateAssetMenu(fileName = "MaterialImportSettings", order = 0, menuName = "Settings/Material Settings")]
    public class MaterialSettings : ScriptableObject
    {
        [Range(0, 1)] public float normalValue;
        [Range(0, 1)] public float smoothnessValue;
        public float emissionValue;
        public Material skyboxMaterial;
    }
}
