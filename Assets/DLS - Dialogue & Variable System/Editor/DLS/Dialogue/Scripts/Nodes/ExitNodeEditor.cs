using DLS.Dialogue;
using XNodeEditor;

namespace DLS.Dialogue.Editor
{
    /// <summary>
    /// Custom editor for the ExitNode node.
    /// </summary>
    [CustomNodeEditor(typeof(ExitNode))]
    public class ExitNodeEditor : NodeEditor
    {
        /// <summary>
        /// Override of the OnBodyGUI method to draw the custom GUI for the ExitNode node.
        /// </summary>
        public override void OnBodyGUI()
        {
            serializedObject.Update();

            var segment = serializedObject.targetObject as ExitNode;
            if (segment != null)
            {
                NodeEditorGUILayout.PortField(segment.GetPort("entry"));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
