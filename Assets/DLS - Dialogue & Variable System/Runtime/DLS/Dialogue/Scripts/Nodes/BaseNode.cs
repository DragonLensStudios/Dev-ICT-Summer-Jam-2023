using DLS.Core;
using DLS.Game.Enums;
using DLS.Game.Messages;
using DLS.Utilities;
using UnityEngine;
using UnityEngine.Scripting;
using XNode;

namespace DLS.Dialogue
{
    /// <summary>
    /// This abstract class is the base node for all other nodes in the dialogue graph.
    /// </summary>
    [Preserve]
    public abstract class BaseNode : Node
    {
        [SerializeField] protected VariablesObject variables;
        protected GameObject sourceGameobject;
        protected GameObject targetGameobject;
        protected IActorData sourceActor;
        protected IActorData targetActor;
        

        /// <summary>
        /// Gets or sets the variables object associated with this node.
        /// </summary>
        public VariablesObject Variables { get => variables; set => variables = value; }

        /// <summary>
        /// Gets or sets the source GameObject associated with this node.
        /// </summary>
        public GameObject SourceGameobject { get => sourceGameobject; set => sourceGameobject = value; }

        /// <summary>
        /// Gets or sets the target GameObject associated with this node.
        /// </summary>
        public GameObject TargetGameobject {get => targetGameobject; set => targetGameobject = value; }

        /// <summary>
        /// Gets or sets the source actor data associated with this node.
        /// </summary>
        public IActorData SourceActor {get => sourceActor; set => sourceActor = value; }

        /// <summary>
        /// Gets or sets the target actor data associated with this node.
        /// </summary>
        public IActorData TargetActor { get=> targetActor; set => targetActor = value; }


        /// <summary>
        /// Returns the string representation of the node.
        /// </summary>
        /// <returns>The string representation of the node.</returns>
        public virtual string GetString()
        {
            return null;
        }

        /// <summary>
        /// Returns the sprite associated with the node.
        /// </summary>
        /// <returns>The sprite associated with the node.</returns>
        public virtual Sprite GetSprite()
        {
            return null;
        }

        /// <summary>
        /// Returns the type of the node as a string.
        /// </summary>
        /// <returns>The type of the node as a string.</returns>
        public virtual string GetNodeType()
        {
            return GetType().Name;
        }

        /// <summary>
        /// Assigns the method <see cref="OnDialogueInteractAction"/> to the <see cref="ActorController.OnDialogueInteractAction"/> event.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            MessageSystem.MessageManager.RegisterForChannel<DialogueInteractMessage>(MessageChannels.Dialogue, OnDialogueInteractAction);
        }

        /// <summary>
        /// Unassigns the method <see cref="OnDialogueInteractAction"/> from the <see cref="ActorController.OnDialogueInteractAction"/> event.
        /// </summary>
        protected virtual void OnDisable()
        {
            MessageSystem.MessageManager.UnregisterForChannel<DialogueInteractMessage>(MessageChannels.Dialogue, OnDialogueInteractAction);
        }
        
        /// <summary>
        /// Assigns the source and target GameObjects and actor data for
        /// </summary>
        /// <param name="source">The source GameObject.</param>
        /// <param name="target">The target GameObject.</param>
        protected virtual void OnDialogueInteractAction(MessageSystem.IMessageEnvelope messageEnvelope)
        {
            if(messageEnvelope.Message<DialogueInteractMessage>() == null) return;;
            var sourceActor = messageEnvelope.Message<DialogueInteractMessage>().Value.Source.GetComponent<IActorData>();
            var targetActor = messageEnvelope.Message<DialogueInteractMessage>().Value.Target.GetComponent<IActorData>();

            if (sourceActor != null)
            {
                this.sourceActor = sourceActor;
            }

            if (targetActor != null)
            {
                this.targetActor = targetActor;
            }

            sourceGameobject = messageEnvelope.Message<DialogueInteractMessage>().Value.Source;
            targetGameobject = messageEnvelope.Message<DialogueInteractMessage>().Value.Target;
        }
    }
}