using System;
using DLS.Game.Utilities;
using UnityEngine;

namespace DLS.Core
{
    /// <summary>
    /// This interface represents the actor data.
    /// </summary>
    public interface IActorData : IDialogue
    {
        /// <summary>
        /// Indicates whether movement is disabled for the actor.
        /// </summary>
        bool IsMovementDisabled { get; set; }
    }
}