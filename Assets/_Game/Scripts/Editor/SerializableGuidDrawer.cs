using System;
using DLS.Game.Utilities;
using UnityEditor;
using UnityEngine;

namespace DLS.Game.Scripts.Editor
{
    [CustomPropertyDrawer(typeof(SerializableGuid), true)]
    public class SerializableGuidDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Calculate rect for the guid string
            var guidRect = new Rect(position.x, position.y, position.width - 100, position.height);

            // Calculate rect for the button
            var buttonRect = new Rect(position.x + position.width - 95, position.y, 90, position.height);

            // Get the guidString property
            var guidStringProp = property.FindPropertyRelative("guidString");

            // Draw the guid string field
            EditorGUI.PropertyField(guidRect, guidStringProp, GUIContent.none);

            // Draw the button
            if (GUI.Button(buttonRect, "New ID"))
            {
                guidStringProp.stringValue = Guid.NewGuid().ToString();
            }

            EditorGUI.EndProperty();
        }
    }
}