using System.Collections.Generic;
using UnityEngine;

namespace Constructor.Details
{
    public class FbxDetail : Detail
    {
        public GameObject modelContainerObject;
        public GameObject modelGameObject;
        public List<Texture2D> baseColors = new List<Texture2D>();

        public FbxDetail(GameObject modelContainerObject, string name, float rarity, string layerName) : base(name, rarity, layerName)
        {
            this.modelContainerObject = modelContainerObject;
        }

        public override bool HasBody() => modelContainerObject != null;
    }
}
