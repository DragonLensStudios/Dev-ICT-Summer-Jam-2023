using UnityEngine;
using XNode;

namespace DLS.Dialogue
{
    /// <summary>
    /// Represents a dialogue node in a dialogue graph.
    /// </summary>
    public class DialogueNode : BaseNode
    {
        [Input] public Connection input;
        [Output] public Connection exit;
        [SerializeField] protected string actorName;
        [SerializeField, TextArea] protected string dialogueText;
        [SerializeField] protected Sprite sprite;
        [SerializeField] protected bool useSourceActor;
        [SerializeField] protected bool useTargetActor;
        [SerializeField] protected bool manual;

        /// <summary>
        /// Gets or sets the name of the actor associated with this dialogue node.
        /// </summary>
        public string ActorName { get => actorName; set => actorName = value; }

        /// <summary>
        /// Gets or sets the dialogue text of this node.
        /// </summary>
        public string DialogueText { get => dialogueText; set => dialogueText = value; }

        /// <summary>
        /// Gets or sets the sprite associated with this dialogue node.
        /// </summary>
        public Sprite Sprite { get => sprite; set => sprite = value; }
        
        /// <summary>
        /// Gets or sets the use source actor data associated with this node.
        /// </summary>
        public bool UseSourceActor { get => useSourceActor; set => useSourceActor = value; }

        /// <summary>
        /// Gets or sets the use target actor data associated with this node.
        /// </summary>
        public bool UseTargetActor { get => useTargetActor; set => useTargetActor = value; }
        
        /// <summary>
        /// Gets or sets the use manual data associated with this node.
        /// </summary>
        public bool Manual { get => manual; set=> manual = value; }

        /// <summary>
        /// Returns the value of the specified output port.
        /// </summary>
        /// <param name="port">The output port to retrieve the value from.</param>
        /// <returns>The value of the output port.</returns>
        public override object GetValue(NodePort port)
        {
            return null;
        }

        /// <summary>
        /// Returns the sprite associated with this dialogue node.
        /// </summary>
        /// <returns>The sprite of the dialogue node.</returns>
        public override Sprite GetSprite()
        {
            return sprite;
        }
    }
}