using UnityEngine;

namespace Extensions
{
    public static class GameObjectExtensions
    {
        public static void SetLayerIncludeChildren(this GameObject go, int layer)
        {
            go.layer = layer;

            foreach (Transform child in go.transform)
            {
                SetLayerIncludeChildren(child.gameObject, layer);
            }
        }

        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            return gameObject.TryGetComponent<T>(out var component) ? component : gameObject.AddComponent<T>();
        }
    }
}