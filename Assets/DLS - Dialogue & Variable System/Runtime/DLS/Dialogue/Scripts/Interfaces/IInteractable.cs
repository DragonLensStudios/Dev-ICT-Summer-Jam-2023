using DLS.Core.Data_Persistence;
using UnityEngine;

namespace DLS.Core
{
    public interface IInteractable : IID
    {
        /// <summary>
        /// The GameObject associated with the actor.
        /// </summary>
        GameObject GameObject { get; }
        
        /// <summary>
        /// The target GameObject that the actor is interacting with.
        /// </summary>
        GameObject TargetGameObject { get; set; }
        
        /// <summary>
        /// Indicates whether the actor is currently interacting with a target.
        /// </summary>
        bool IsInteracting { get; set; }
        
        /// <summary>
        /// Allows the actor to interact with the current target GameObject.
        /// </summary>
        void Interact();
    }
}