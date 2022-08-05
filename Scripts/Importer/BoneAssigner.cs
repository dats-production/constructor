using UnityEngine;

namespace Importer
{
    public class BoneAssigner : MonoBehaviour
    {
        private const string RootBoneName = "Root";
        
        public void AssignToBones(Transform newArmature)
        {
            var rend = gameObject.GetComponent<SkinnedMeshRenderer>();
            gameObject.transform.SetParent(newArmature);
            rend.updateWhenOffscreen = true;
            Transform[] bonesOfRenderer = rend.bones;
 
            rend.rootBone = newArmature.Find(RootBoneName);
 
            Transform[] childrenOfNewArmature = newArmature.GetComponentsInChildren<Transform>();
 
            for (int i = 0; i < bonesOfRenderer.Length; i++)
            for (int a = 0; a < childrenOfNewArmature.Length; a++)
                if (bonesOfRenderer[i].name == childrenOfNewArmature[a].name) {
                    bonesOfRenderer[i] = childrenOfNewArmature[a];
                    break;
                }
 
            rend.bones = bonesOfRenderer;
            Destroy(this);
        }
    }
}
