using DLS.Dialogue;
using XNodeEditor;

namespace DLS.Dialogue.Editor
{
    /// <summary>
    /// Custom editor for the StartNode node.
    /// </summary>
    [CustomNodeEditor(typeof(StartNode))]
    public class StartNodeEditor : NodeEditor
    {
        /// <summary>
        /// Override of the OnBodyGUI method to draw the custom GUI for the StartNode node.
        /// </summary>
        public override void OnBodyGUI()
        {
            serializedObject.Update();

            var segment = serializedObject.targetObject as StartNode;
            if (segment != null)
            {
                NodeEditorGUILayout.PortField(segment.GetPort("exit"));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
