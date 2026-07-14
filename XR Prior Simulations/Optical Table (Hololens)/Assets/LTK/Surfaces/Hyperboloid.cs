using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LightTK
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Hyperboloid", menuName = "LTK/Create Hyperboloid", order = 1)]
    public class Hyperboloid : AbstractSurface
    {
        public float minimum
        {
            get { return surface.minimum.z; }
            set { surface.minimum.z = value; }
        }

        public float maximum
        {
            get { return surface.maximum.z; }
            set { surface.maximum.z = value; }
        }

        public float seperation
        {
            get { return Mathf.Sqrt(-surface.equation.p); }
            set { surface.equation.p = -(value * value); }
        }

        public Vector2 scale
        {
            get { return new Vector2(1f / surface.equation.j, 1f / surface.equation.k); }
            set { surface.equation.j = 1f / value.x; surface.equation.k = 1f / value.y; }
        }

        public Hyperboloid(float minimum = float.NegativeInfinity, float maximum = float.PositiveInfinity)
        {
            surface = new Surface()
            {
                equation = Equation.Hyperboloid,
                minimum = Vector3.negativeInfinity,
                maximum = Vector3.positiveInfinity,
                settings = RefractionEquation.crownGlass
            };
            this.minimum = minimum;
            this.maximum = maximum;
        }

#if UNITY_EDITOR
        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Paraboloid Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Bounds", EditorStyles.boldLabel);
            minimum = EditorGUILayout.FloatField("Minimum", minimum);
            maximum = EditorGUILayout.FloatField("Maximum", maximum);
            surface.radial = EditorGUILayout.FloatField("Radius", surface.radial);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Properties", EditorStyles.boldLabel);
            scale = EditorGUILayout.Vector2Field("Scale", scale);
            seperation = EditorGUILayout.FloatField("Seperation", seperation);
        }
#endif
    }
}