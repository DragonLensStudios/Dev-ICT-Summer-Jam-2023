using UnityEngine;
using XNode;

namespace DLS.Dialogue
{
    /// <summary>
    /// Represents a choice dialogue node in a dialogue graph.
    /// </summary>
    public class ChoiceDialogueNode : BaseNode
    {
        [Input] public Connection input;
        [Output(dynamicPortList = true)] public string[] Answers;
        [SerializeField] protected string actorName;
        [SerializeField] protected Sprite sprite;
        [SerializeField, TextArea] protected string dialogueText;
        [SerializeField] protected bool useSourceActor;
        [SerializeField] protected bool useTargetActor;
        [SerializeField] protected bool manual;

        /// <summary>
        /// Gets or sets the name of the actor associated with the choice dialogue.
        /// </summary>
        public string ActorName { get => actorName; set => actorName = value; }

        /// <summary>
        /// Gets or sets the sprite of the actor associated with the choice dialogue.
        /// </summary>
        public Sprite Sprite { get => sprite; set => sprite = value; }

        /// <summary>
        /// Gets or sets the dialogue text of the choice dialogue.
        /// </summary>
        public string DialogueText { get => dialogueText; set => dialogueText = value; }
        
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
        /// Returns the sprite associated with the choice dialogue node.
        /// </summary>
        /// <returns>The sprite associated with the choice dialogue node.</returns>
        public override Sprite GetSprite()
        {
            return sprite;
        }

    }
}
