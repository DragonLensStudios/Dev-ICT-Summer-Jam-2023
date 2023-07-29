using DLS.Dialogue;
using UnityEngine;
using XNodeEditor;

namespace DLS.Dialogue.Editor
{
    /// <summary>
    /// Custom editor for the ReferenceStateNode node.
    /// </summary>
    [CustomNodeEditor(typeof(ReferenceStateNode))]
    public class ReferenceStateNodeEditor : NodeEditor
    {
        /// <summary>
        /// Override of the OnBodyGUI method to draw the custom GUI for the ReferenceStateNode node.
        /// </summary>
        public override void OnBodyGUI()
        {
            serializedObject.Update();

            var segment = serializedObject.targetObject as ReferenceStateNode;
            if (segment != null)
            {
                NodeEditorGUILayout.PortField(segment.GetPort("input"));
                NodeEditorGUILayout.PortField(segment.GetPort("exitTrue"));
                GUILayout.Label("Reference State");
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("referenceState"), GUIContent.none);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
