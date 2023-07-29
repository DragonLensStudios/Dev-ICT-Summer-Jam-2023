using System.Linq;
using DLS.Core;
using UnityEngine;

namespace DLS.Dialogue.Demo
{
    /// <summary>
    /// Represents an NPC actor in the game.
    /// </summary>
    public class NpcActor : ActorController
    {
        /// <summary>
        /// Called when a 2D collider enters the trigger.
        /// </summary>
        /// <param name="col">The collider that entered the trigger.</param>
        protected override void OnTriggerEnter2D(Collider2D col)
        {
            base.OnTriggerEnter2D(col);
            if(dialogueManager.Interactions.All(x=> x.DialogueCompleted && !x.RepeatableDialogue)) return;
            if (col.CompareTag("Player"))
            {
                DialogueUi.Instance.ShowInteractionText($"Press E to talk to {objectName}");
            }
        }

        /// <summary>
        /// Called when a 2D collider exits the trigger.
        /// </summary>
        /// <param name="col">The collider that exited the trigger.</param>
        protected override void OnTriggerExit2D(Collider2D col)
        {
            base.OnTriggerExit2D(col);
            if (col.CompareTag("Player"))
            {
                DialogueUi.Instance.HideInteractionText();
            }
        }
    }
}