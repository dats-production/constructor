using UnityEngine;

namespace EnvironmentControl
{
    [CreateAssetMenu(fileName = "EnvironmentPreset", order = 0, menuName = "Settings/Environment Preset")]

    public class EnvironmentPreset : ScriptableObject
    {
        public string backgroundName;

        public GameObject floorPrefab;
        public bool useAdditionalLight;
        public bool useShadowEnhancer;
        public Vector3 lightRotation;
        public Vector3 shadowsRotation;
        [Range(0, 100)] public float lightIntensity;
        [Range(0, 100)] public float lightShadowsIntensity;
        [Range(0, 20000)] public int lightTemperature;
        [Range(0, 1)] public float shadowIntensity;

        public Color shadowColor;
        public Color lightFilter;
        public Color lightFilterShadows;
        [ColorUsage(false, true)] public Color environmentEmissionColorLeft;
        [ColorUsage(false, true)] public Color environmentEmissionColorRight;
    }
}
