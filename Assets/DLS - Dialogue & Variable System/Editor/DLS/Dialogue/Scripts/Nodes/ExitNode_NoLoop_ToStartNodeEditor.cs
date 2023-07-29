using DLS.Dialogue;
using XNodeEditor;

namespace DLS.Dialogue.Editor
{
    /// <summary>
    /// Custom editor for the ExitNode_NoLoop_toStart node.
    /// </summary>
    [CustomNodeEditor(typeof(ExitNode_NoLoop_toStart))]
    public class ExitNode_NoLoop_ToStartNodeEditor : NodeEditor
    {
        /// <summary>
        /// Override of the OnBodyGUI method to draw the custom GUI for the ExitNode_NoLoop_toStart node.
        /// </summary>
        public override void OnBodyGUI()
        {
            serializedObject.Update();

            var segment = serializedObject.targetObject as ExitNode_NoLoop_toStart;
            if (segment != null)
            {
                NodeEditorGUILayout.PortField(segment.GetPort("entry"));
                NodeEditorGUILayout.PortField(segment.GetPort("exit"));
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
