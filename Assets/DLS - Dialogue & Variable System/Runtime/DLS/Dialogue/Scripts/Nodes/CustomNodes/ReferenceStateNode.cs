using UnityEngine;
using XNode;

namespace DLS.Dialogue
{
    /// <summary>
    /// Represents a reference state node in a dialogue graph.
    /// </summary>
    public class ReferenceStateNode : BaseNode
    {
        [Input] public Connection input;
        [Output] public Connection exitTrue;
        [SerializeField] protected string referenceState;

        /// <summary>
        /// Gets or sets the reference state associated with this node.
        /// </summary>
        public string ReferenceState { get => referenceState; set => referenceState = value; }

        /// <summary>
        /// Returns the value of the specified output port.
        /// </summary>
        /// <param name="port">The output port to retrieve the value from.</param>
        /// <returns>The value of the output port.</returns>
        public override object GetValue(NodePort port)
        {
            return null;
        }
    }
}