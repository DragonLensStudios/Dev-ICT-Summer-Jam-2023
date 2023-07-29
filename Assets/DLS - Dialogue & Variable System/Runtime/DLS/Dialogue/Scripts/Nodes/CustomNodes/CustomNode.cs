using XNode;

namespace DLS.Dialogue
{
	/// <summary>
	/// Represents a custom node in a dialogue graph.
	/// </summary>
	public abstract class CustomNode : BaseNode
	{
		[Input] public Connection input;
		[Output] public Connection exit;

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

