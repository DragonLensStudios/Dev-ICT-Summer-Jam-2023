using XNode;

namespace DLS.Dialogue
{
    /// <summary>
    /// Represents the starting point node in the dialogue graph.
    /// </summary>
    public class StartNode : BaseNode
    {
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