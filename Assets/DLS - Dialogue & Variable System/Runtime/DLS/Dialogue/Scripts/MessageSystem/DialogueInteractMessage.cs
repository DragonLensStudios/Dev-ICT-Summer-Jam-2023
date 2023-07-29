using UnityEngine;

namespace DLS.Dialogue
{
    public struct DialogueInteractMessage
    {
        public GameObject Source { get; }
        public GameObject Target { get; }

        public DialogueInteractMessage(GameObject source, GameObject target)
        {
            Source = source;
            Target = target;
        }
    }
}