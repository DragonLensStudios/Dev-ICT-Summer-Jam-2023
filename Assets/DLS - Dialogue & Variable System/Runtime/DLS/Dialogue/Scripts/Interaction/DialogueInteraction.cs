using UnityEngine;

namespace DLS.Dialogue
{
    /// <summary>
    /// Represents a dialogue interaction in the dialogue system.
    /// </summary>
    [CreateAssetMenu(fileName = "DialogueInteraction.asset", menuName = "DLS/Dialogue/Dialogue Interaction", order = 20)]
    public class DialogueInteraction : ScriptableObject
    {
        [SerializeField]
        protected string dialogueId;
        [SerializeField]
        protected int interactionWeight;
        [SerializeField]
        protected string referenceState;
        [SerializeField]
        protected DialogueGraph graph;
        [SerializeField]
        protected bool dialogueCompleted;
        [SerializeField]
        protected bool repeatableDialogue;

        /// <summary>
        /// Gets or sets the ID of the dialogue.
        /// </summary>
        public string DialogueId { get => dialogueId; set => dialogueId = value; }

        /// <summary>
        /// Gets or sets the weight of the interaction.
        /// </summary>
        public int InteractionWeight { get => interactionWeight; set => interactionWeight = value; }
    
        /// <summary>
        /// Gets or sets the referenced state for the dialogue interaction.
        /// </summary>
        public string ReferencedState { get => referenceState; set => referenceState = value; }
    
        /// <summary>
        /// Gets or sets the dialogue graph associated with the interaction.
        /// </summary>
        public DialogueGraph Graph { get => graph; set => graph = value; }

        /// <summary>
        /// Gets or sets a value indicating whether the dialogue is completed.
        /// </summary>
        public bool DialogueCompleted
        {
            get => dialogueCompleted; 
        
            set
            {
                // Check to ensure that repeatable dialogues cannot be set to a 'Completed' state.
                if (!repeatableDialogue)
                {
                    dialogueCompleted = value;
                }
            }
        }
    
        /// <summary>
        /// Gets or sets a value indicating whether the dialogue is repeatable.
        /// </summary>
        public bool RepeatableDialogue { get => repeatableDialogue; set => repeatableDialogue = value; }

        /// <summary>
        /// Called when the scriptable object is enabled.
        /// </summary>
        private void OnEnable()
        {
            Application.quitting += ApplicationOnQuitting;
        }

        /// <summary>
        /// Called when the application is quitting.
        /// </summary>
        private void ApplicationOnQuitting()
        {
            dialogueCompleted = false;
        }

        /// <summary>
        /// Called when the scriptable object is disabled.
        /// </summary>
        private void OnDisable()
        {
            Application.quitting -= ApplicationOnQuitting;
        }

        /// <summary>
        /// Starts the dialogue by displaying it in the dialogue UI.
        /// </summary>
        public void StartDialogue()
        {
            DialogueUi.Instance.StartDialogue(graph);            
        }
    }
}
