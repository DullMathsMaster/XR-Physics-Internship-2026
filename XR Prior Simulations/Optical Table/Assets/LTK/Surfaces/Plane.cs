using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LightTK
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Plane", menuName = "LTK/Create Plane", order = 1)]
    public class Plane : AbstractSurface
    {
        public Vector2 minimum
        {
            get { return surface.minimum; }
            set { surface.minimum = new Vector3(value.x, value.y, -Mathf.Infinity); }
        }

        public Vector2 maximum
        {
            get { return surface.maximum; }
            set { surface.maximum = new Vector3(value.x, value.y, Mathf.Infinity); }
        }

        public Plane(float offset = 0)
        {
            Equation eq = new Equation()
            {
                o = 1f
            };

            surface = new Surface()
            {
                equation = eq,
                settings = RefractionEquation.crownGlass
            };
            minimum = Vector3.negativeInfinity;
            maximum = Vector3.positiveInfinity;
        }

        public Plane(Vector2 minimum, Vector2 maximum, float offset = 0)
        {
            surface = new Surface()
            {
                equation = Equation.Plane,
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
            minimum = EditorGUILayout.Vector2Field("Minimum", minimum);
            maximum = EditorGUILayout.Vector2Field("Maximum", maximum);
            surface.radial = EditorGUILayout.FloatField("Radius", surface.radial);
            EditorGUILayout.Space();
        }
#endif
    }
}