using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;

namespace Metamesh
{
    [CustomEditor(typeof(MetameshImporter))]
    sealed class MetameshImporterEditor : ScriptedImporterEditor
    {
        SerializedProperty _shape;
        SerializedProperty _plane;
        SerializedProperty _box;

        public override void OnEnable()
        {
            base.OnEnable();
            _shape = serializedObject.FindProperty("_shape");
            _plane = serializedObject.FindProperty("_plane");
            _box   = serializedObject.FindProperty("_box");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_shape);

            switch ((Shape)_shape.enumValueIndex)
            {
                case Shape.Plane: EditorGUILayout.PropertyField(_plane); break;
                case Shape.Box  : EditorGUILayout.PropertyField(_box);   break;
            }

            serializedObject.ApplyModifiedProperties();
            ApplyRevertGUI();
        }

    }
}
