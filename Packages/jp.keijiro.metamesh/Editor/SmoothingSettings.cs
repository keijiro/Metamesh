using System;
using UnityEditor;
using UnityEngine;

namespace Metamesh
{

[Serializable]
public class SmoothingSettings
{
    public bool ConfigureSmoothingAngle;
    [Min(0)]
    public float SmoothingAngle;
}


[CustomPropertyDrawer(typeof(SmoothingSettings))]
public class SmoothingSettingsPropertyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var height = 0f;
        var configureSmoothingAngleProperty = GetConfigureSmoothingAngleProperty(property);
        height += EditorGUI.GetPropertyHeight(configureSmoothingAngleProperty) + EditorGUIUtility.standardVerticalSpacing;
        var configureSmoothingAngle = configureSmoothingAngleProperty.boolValue;
        if (configureSmoothingAngle)
        {
            var smoothingAngleProperty = GetSmoothingAngleProperty(property);
            height += EditorGUI.GetPropertyHeight(smoothingAngleProperty) + EditorGUIUtility.standardVerticalSpacing;
        }
        return height;
    }

    private static SerializedProperty GetConfigureSmoothingAngleProperty(SerializedProperty property)
        => property.FindPropertyRelative(nameof(SmoothingSettings.ConfigureSmoothingAngle));

    private static SerializedProperty GetSmoothingAngleProperty(SerializedProperty property)
        => property.FindPropertyRelative(nameof(SmoothingSettings.SmoothingAngle));

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var configureSmoothingAngleProperty = GetConfigureSmoothingAngleProperty(property);
        var configureSmoothingAngleRect = new Rect(position.x, position.y, position.width,
                                                   EditorGUI.GetPropertyHeight(configureSmoothingAngleProperty));
        EditorGUI.PropertyField(configureSmoothingAngleRect, configureSmoothingAngleProperty);


        var configureSmoothingAngle = configureSmoothingAngleProperty.boolValue;
        if (configureSmoothingAngle)
        {
            var smoothingAngleProperty = GetSmoothingAngleProperty(property);
            var smoothingAngleRect = new Rect(
                position.x, configureSmoothingAngleRect.yMax + EditorGUIUtility.standardVerticalSpacing, position.width,
                EditorGUI.GetPropertyHeight(smoothingAngleProperty));
            EditorGUI.PropertyField(smoothingAngleRect, smoothingAngleProperty);
        }

        EditorGUI.EndProperty();
    }
}

}
