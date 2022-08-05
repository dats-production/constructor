using UnityEngine;

namespace Extensions
{
    public static class Vector3Extensions {
        
        public static Vector3 WithX(this Vector3 vector, float value) => new Vector3(value, vector.y, vector.z);
        
        public static Vector3 WithY(this Vector3 vector, float value) => new Vector3(vector.x, value, vector.z);
        
        public static Vector3 WithZ(this Vector3 vector, float value) => new Vector3(vector.x, vector.y, value);
        
        public static Vector3 IncreaseX(this Vector3 vector, float value) => new Vector3(vector.x + value, vector.y, vector.z);
        
        public static Vector3 IncreaseY(this Vector3 vector, float value) => new Vector3(vector.x, vector.y + value, vector.z);
        
        public static Vector3 IncreaseZ(this Vector3 vector, float value) => new Vector3(vector.x, vector.y, vector.z + value);
        
        public static Vector3 ClampX(this Vector3 vector, float minValue, float maxValue) => new Vector3(Mathf.Clamp(vector.x, minValue, maxValue), vector.y, vector.z);
        
        public static Vector3 ClampY(this Vector3 vector, float minValue, float maxValue) => new Vector3(vector.x, Mathf.Clamp(vector.y, minValue, maxValue), vector.z);
        
        public static Vector3 ClampZ(this Vector3 vector, float minValue, float maxValue) => new Vector3(vector.x, vector.y, Mathf.Clamp(vector.z, minValue, maxValue));
    }
}
