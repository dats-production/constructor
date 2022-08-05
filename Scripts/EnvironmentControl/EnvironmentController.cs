using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace EnvironmentControl
{
    public class EnvironmentController : MonoBehaviour
    {
        [SerializeField] private List<EnvironmentPreset> presets = new List<EnvironmentPreset>();
        [SerializeField] private Material shadowReceiver;
        [SerializeField] private int currentPreset;
        [SerializeField] private Material emissiveEnvironmentMaterialLeft;
        [SerializeField] private Material emissiveEnvironmentMaterialRight;
        [SerializeField] private Light lightSource;
        [SerializeField] private Light lightSourceShadows;
        [SerializeField] private Light lightSourceAdditional;
        [SerializeField] private Volume postProcessing;
        
        private int prevPreset;
        private GameObject currentFloor;
      
        private static readonly int ShadowColor = Shader.PropertyToID("_ShadowColor");
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");



        private void Update()
        {
            if (prevPreset != currentPreset)
            {
                ApplyPreset();
            }
        }

        public void SelectPreset(string backgroundName)
        {
            for (int i = 0; i < presets.Count; i++)
            {
                currentPreset = i;
                if (string.Equals(presets[i].backgroundName, backgroundName, StringComparison.CurrentCultureIgnoreCase))
                {
                    break;
                }
            }
        }
        
        public void ApplyPreset()
        {
            prevPreset = currentPreset;
            var current = presets[currentPreset];
            
            lightSource.intensity = current.lightIntensity;
            lightSource.colorTemperature = current.lightTemperature;
            lightSourceShadows.shadowStrength = current.shadowIntensity;
            lightSource.transform.rotation = Quaternion.Euler(current.lightRotation);
            lightSourceShadows.intensity = current.lightShadowsIntensity;
            lightSourceShadows.transform.rotation = Quaternion.Euler(current.shadowsRotation);
            
            lightSource.color = current.lightFilter;
            lightSourceShadows.color = current.lightFilterShadows;
            shadowReceiver.SetColor(ShadowColor, current.shadowColor);
            emissiveEnvironmentMaterialLeft.SetColor(EmissionColor, current.environmentEmissionColorLeft);
            emissiveEnvironmentMaterialRight.SetColor(EmissionColor, current.environmentEmissionColorRight);
            lightSourceAdditional.gameObject.SetActive(current.useAdditionalLight);

            if (postProcessing.sharedProfile.TryGet(typeof(ShadowsMidtonesHighlights), out ShadowsMidtonesHighlights shadowsMidtonesHighlights))
            {
                shadowsMidtonesHighlights.active = current.useShadowEnhancer;
            }
            if (currentFloor != null)
            {
                DestroyImmediate(currentFloor);
            }

            if (current.floorPrefab != null) currentFloor = Instantiate(current.floorPrefab, transform);
        }
    }
}
