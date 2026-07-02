using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LightTK
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Ellipsoid", menuName = "LTK/Create Ellipsoid", order = 1)]
    public class Ellipsoid : AbstractSurface
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

        public float radius
        {
            get { return Mathf.Sqrt(-surface.equation.p); }
            set { surface.equation.p = -(value * value); }
        }

        public Vector3 scale
        {
            get { return new Vector3(Mathf.Sqrt(1f/surface.equation.j), Mathf.Sqrt(surface.equation.k), Mathf.Sqrt(surface.equation.l)); }
            set {
                surface.equation.j = 1f / (value.x * value.x);
                surface.equation.k = 1f / (value.y * value.y);
                surface.equation.l = 1f / (value.z * value.z); 
            }
        }

        public Ellipsoid(float minimum = float.NegativeInfinity, float maximum = float.PositiveInfinity)
        {
            surface = new Surface()
            {
                equation = Equation.Ellipsoid,
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
            EditorGUILayout.LabelField("Ellipsoid Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Bounds", EditorStyles.boldLabel);
            minimum = EditorGUILayout.FloatField("Minimum", minimum);
            maximum = EditorGUILayout.FloatField("Maximum", maximum);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Properties", EditorStyles.boldLabel);
            scale = EditorGUILayout.Vector3Field("Scale", scale);
            radius = EditorGUILayout.FloatField("Radius", radius);
        }
#endif
    }
}