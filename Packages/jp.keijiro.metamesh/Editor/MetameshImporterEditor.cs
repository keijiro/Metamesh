using UnityEngine;
using UnityEditor;
#if UNITY_2020_2_OR_NEWER
using UnityEditor.AssetImporters;
#else
using UnityEditor.Experimental.AssetImporters;
#endif

namespace Metamesh
{
    [CustomEditor(typeof(MetameshImporter))]
    sealed class MetameshImporterEditor : ScriptedImporterEditor
    {
        SerializedProperty _shape;
        SerializedProperty _plane;
        SerializedProperty _box;
        SerializedProperty _sphere;
        SerializedProperty _icosphere;
        SerializedProperty _cylinder;
        SerializedProperty _roundedBox;

        public override void OnEnable()
        {
            base.OnEnable();
            _shape      = serializedObject.FindProperty("_shape");
            _plane      = serializedObject.FindProperty("_plane");
            _box        = serializedObject.FindProperty("_box");
            _sphere     = serializedObject.FindProperty("_sphere");
            _icosphere  = serializedObject.FindProperty("_icosphere");
            _cylinder   = serializedObject.FindProperty("_cylinder");
            _roundedBox = serializedObject.FindProperty("_roundedBox");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_shape);

            switch ((Shape)_shape.enumValueIndex)
            {
                case Shape.Plane     : EditorGUILayout.PropertyField(_plane);      break;
                case Shape.Box       : EditorGUILayout.PropertyField(_box);        break;
                case Shape.Sphere    : EditorGUILayout.PropertyField(_sphere);     break;
                case Shape.Icosphere : EditorGUILayout.PropertyField(_icosphere);  break;
                case Shape.Cylinder  : EditorGUILayout.PropertyField(_cylinder);   break;
                case Shape.RoundedBox: EditorGUILayout.PropertyField(_roundedBox); break;
            }

            serializedObject.ApplyModifiedProperties();
            ApplyRevertGUI();
        }

        [MenuItem("Assets/Create/Metamesh")]
        public static void CreateNewAsset()
          => ProjectWindowUtil.CreateAssetWithContent
               ("New Metamesh.metamesh", "");
    }
}
