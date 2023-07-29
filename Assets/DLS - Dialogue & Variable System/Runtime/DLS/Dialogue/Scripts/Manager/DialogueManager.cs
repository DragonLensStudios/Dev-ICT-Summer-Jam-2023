using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DLS.Dialogue
{
    /// <summary>
    /// Manages the dialogues and their selection in the dialogue system.
    /// </summary>
    [CreateAssetMenu(fileName = "New Dialogue Manager", menuName = "DLS/Dialogue/Dialogue Manager", order = 25)]
    public class DialogueManager : ScriptableObject
    {
        [SerializeField] protected string currentReferenceState;

        [SerializeField] protected DialogueInteraction currentInteraction;

        [SerializeField] protected List<DialogueInteraction> interactions = new();

        [SerializeField] protected bool useRandomDialogueSelectionByWeight = false;

        /// <summary>
        /// Gets or sets the current reference state.
        /// </summary>
        public string CurrentReferenceState
        {
            get => currentReferenceState;
            set => currentReferenceState = value;
        }

        /// <summary>
        /// Gets or sets the current interaction.
        /// </summary>
        public DialogueInteraction CurrentInteraction
        {
            get => currentInteraction;
            set => currentInteraction = value;
        }

        /// <summary>
        /// Gets or sets the list of dialogue interactions.
        /// </summary>
        public List<DialogueInteraction> Interactions
        {
            get => interactions;
            set => interactions = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use random dialogue selection by weight.
        /// </summary>
        public bool UseRandomDialogueSelectionByWeight
        {
            get => useRandomDialogueSelectionByWeight;
            set => useRandomDialogueSelectionByWeight = value;
        }

        /// <summary>
        /// Sets the current dialogue interaction based on the current reference state and available interactions.
        /// </summary>
        private bool SetCurrentDialogue()
        {
            var matchingInteractions = interactions
                .FindAll(x => x.ReferencedState.Equals(currentReferenceState) && !x.DialogueCompleted)
                .OrderByDescending(x => x.InteractionWeight).ToList();

            if (matchingInteractions.Count > 1)
            {
                int totalWeight = 0;

                if (useRandomDialogueSelectionByWeight)
                {
                    for (int i = 0; i < matchingInteractions.Count; i++)
                    {
                        totalWeight += matchingInteractions[i].InteractionWeight;
                    }

                    int randomValue = Random.Range(0, totalWeight);

                    for (int i = 0; i < matchingInteractions.Count; i++)
                    {
                        if (randomValue < matchingInteractions[i].InteractionWeight)
                        {
                            currentInteraction = matchingInteractions[i];
                            return true;
                        }

                        randomValue -= matchingInteractions[i].InteractionWeight;
                    }
                }
                else
                {
                    for (int i = 0; i < matchingInteractions.Count; i++)
                    {
                        currentInteraction = matchingInteractions[i];
                        return true;
                    }
                }

                if (currentInteraction == null) return false;
            }
            else
            {
                for (var i = 0; i < interactions.Count; i++)
                {
                    var interaction = interactions[i];

                    if (interaction.ReferencedState.Equals(currentReferenceState) && !interaction.DialogueCompleted)
                    {
                        currentInteraction = interaction;
                        return true;
                    }
                    else
                    {
                        currentInteraction = interactions.FirstOrDefault(x =>
                            x.ReferencedState.Equals(string.Empty) && interaction.RepeatableDialogue);
                    }
                }
                if (currentInteraction == null) return false;
            }

            return true;
        }

        /// <summary>
        /// Starts the dialogue by setting the current dialogue and marking it as completed.
        /// </summary>
        public bool StartDialogue()
        {
            if (SetCurrentDialogue())
            {
                if (currentInteraction == null || currentInteraction.DialogueCompleted && !currentInteraction.RepeatableDialogue)
                {
                    return false;
                }
                currentInteraction.StartDialogue();
                currentInteraction.DialogueCompleted = true;
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}