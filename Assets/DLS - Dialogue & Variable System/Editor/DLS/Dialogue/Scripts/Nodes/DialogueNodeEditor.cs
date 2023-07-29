using DLS.Core;
using DLS.Dialogue;
using UnityEngine;
using XNodeEditor;
using UnityEditor;

namespace DLS.Dialogue.Editor
{
    /// <summary>
    /// Custom editor for the DialogueNode.
    /// </summary>
    [CustomNodeEditor(typeof(DialogueNode))]
    public class DialogueNodeEditor : NodeEditor
    {
        /// <summary>
        /// Override of the OnBodyGUI method to draw the custom GUI for the DialogueNode.
        /// </summary>
        public override void OnBodyGUI()
        {
            serializedObject.Update();

            var segment = serializedObject.targetObject as DialogueNode;
            if (segment != null)
            {
                NodeEditorGUILayout.PortField(segment.GetPort("input"));
                NodeEditorGUILayout.PortField(segment.GetPort("exit"));

                GUILayout.Label("Variables");
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("variables"), GUIContent.none);

                // Radio buttons for actor choice
                bool newUseSourceActor = GUILayout.Toggle(segment.UseSourceActor, "Use Source Actor");
                if (newUseSourceActor != segment.UseSourceActor)
                {
                    segment.UseSourceActor = newUseSourceActor;
                    if (segment.UseSourceActor) // If the button was just selected
                    {
                        segment.UseTargetActor = false;
                        segment.Manual = false;
                    }
                }

                bool newUseTargetActor = GUILayout.Toggle(segment.UseTargetActor, "Use Target Actor");
                if (newUseTargetActor != segment.UseTargetActor)
                {
                    segment.UseTargetActor = newUseTargetActor;
                    if (segment.UseTargetActor) // If the button was just selected
                    {
                        segment.UseSourceActor = false;
                        segment.Manual = false;
                    }
                }

                bool newManual = GUILayout.Toggle(segment.Manual, "Manual");
                if (newManual != segment.Manual)
                {
                    segment.Manual = newManual;
                    if (segment.Manual) // If the button was just selected
                    {
                        segment.UseSourceActor = false;
                        segment.UseTargetActor = false;
                    }
                }

                if (segment.Manual)
                {
                    GUILayout.Label("Display Actor Name");
                    NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("actorName"), GUIContent.none);

                    GUILayout.Label("Actor Portrait");
                    NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("sprite"), GUIContent.none);
                }
                
                GUILayout.Label("Dialogue Text");

                // Use EditorGUILayout.TextArea to create an expandable text area
                var dialogueTextProp = serializedObject.FindProperty("dialogueText");

                // Define a GUIStyle that forces text to wrap
                GUIStyle textAreaStyle = new GUIStyle(EditorStyles.textArea);
                textAreaStyle.wordWrap = true;

                dialogueTextProp.stringValue = EditorGUILayout.TextArea(dialogueTextProp.stringValue, textAreaStyle); // Removed the GUILayout.Height directive to let the TextArea auto-size
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
