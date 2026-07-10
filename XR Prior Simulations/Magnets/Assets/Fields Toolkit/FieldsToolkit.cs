using UnityEngine;

namespace FieldsToolkit
{
    public class FieldsToolkit : MonoBehaviour
    {
        internal static FieldsToolkit instance;

        public FTK.Settings settings = FTK.Settings.defaultSettings;
        public Material fieldLineMaterial;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Debug.LogWarning("An instance of FTK already exists.");
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            FieldsToolkit existing = FindFirstObjectByType<FieldsToolkit>();
            if (existing == null)
            {
                Debug.LogWarning("FTK was not found, creating a new object.");
                GameObject o = new GameObject("Fields Toolkit");
                instance = o.AddComponent<FieldsToolkit>();
            }
            else
            {
                instance = existing;
            }
        }
    }
}