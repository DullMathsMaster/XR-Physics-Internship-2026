using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using LightTK;
using UnityEngine;

//[CustomPropertyDrawer(typeof(Curve))]
//public class SurfaceEditor : PropertyDrawer
//{
//    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//    {

//    }
//}

[CustomEditor(typeof(AbstractSurface), true)]
public class SurfaceEditor : Editor
{
    AbstractSurface s;

    SerializedProperty surfaceType;


    private void OnEnable()
    {
        surfaceType = serializedObject.FindProperty("surfaceType");
        s = (AbstractSurface)target;
        EditorUtility.SetDirty(s);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Transform", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        s.position = EditorGUILayout.Vector3Field("Position", s.position);
        //Vector4 v4 = new Vector4(s.rotation.x, s.rotation.y, s.rotation.z, s.rotation.w);
        //v4 = EditorGUILayout.Vector4Field("Rotation", v4);
        //s.rotation = new Quaternion(v4.x, v4.y, v4.z, v4.w);
        s.eulerAngles = EditorGUILayout.Vector3Field("Rotation", s.eulerAngles);
        s.rotation = Quaternion.Euler(s.eulerAngles);
        Quaternion temp = s.rotation;
        s.rotation = temp;
        //EditorGUILayout.LabelField(s.rotation.x.ToString() + ", " + s.rotation.y.ToString() + ", " + s.rotation.z.ToString() + ", " + s.rotation.w.ToString());
        EditorGUILayout.Space();
        s.offset = EditorGUILayout.Vector3Field("Offset", s.offset);
        EditorGUILayout.Space();

        s.OnInspectorGUI();

        serializedObject.ApplyModifiedProperties();
    }
}