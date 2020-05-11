using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;

namespace MetaMesh
{
    [CustomEditor(typeof(MetaMeshImporter))]
    sealed class MetaMeshImporterEditor : ScriptedImporterEditor
    {
        SerializedProperty _shape;
        SerializedProperty _quad;
        SerializedProperty _box;

        public override void OnEnable()
        {
            base.OnEnable();
            _shape = serializedObject.FindProperty("_shape");
            _quad = serializedObject.FindProperty("_quad");
            _box = serializedObject.FindProperty("_box");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_shape);

            switch ((Shape)_shape.enumValueIndex)
            {
                case Shape.Quad: EditorGUILayout.PropertyField(_quad); break;
                case Shape.Box : EditorGUILayout.PropertyField(_box);  break;
            }

            serializedObject.ApplyModifiedProperties();
            ApplyRevertGUI();
        }

    }
}
