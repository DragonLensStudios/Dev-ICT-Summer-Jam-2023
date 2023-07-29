using XNode;

namespace DLS.Dialogue
{
    /// <summary>
    /// Represents a node that serves as an exit point in the dialogue graph with no loop back to the start.
    /// </summary>
    public class ExitNode_NoLoop_toStart : BaseNode
    {
        [Input] public Connection entry; // Input connection for the node.
        [Output] public Connection exit; // Output connection for the node.

        /// <summary>
        /// Retrieves the value of the node.
        /// </summary>
        /// <param name="port">The port of the node.</param>
        /// <returns>Always returns null.</returns>
        public override object GetValue(NodePort port)
        {
            return null;
        }
    }
}