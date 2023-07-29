using DLS.Game.Enums;
using DLS.Game.Messages;
using DLS.Game.Transition;
using DLS.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace DLS.Game.Levels
{
    public class LevelTransition : MonoBehaviour
    { 
        public Level level;
        public bool useForceSpawnPosition;
        public Vector2 forceSpawnPosition;
        public Color gizmoColor = Color.green; // set a default color
        public TransitionParameters transitionParameters;
        
        private BoxCollider2D boxCollider;

        private void Awake()
        {
            boxCollider = GetComponent<BoxCollider2D>();
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player"))
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new TransitionMessage(
                    transitionParameters.transitionType, transitionParameters.slideDirection, 
                    transitionParameters.animationDurationInSeconds,transitionParameters.endEvent
                    ));
                MessageSystem.MessageManager.SendImmediate(MessageChannels.Level,
                    useForceSpawnPosition
                        ? new LevelMessage(level.ID, level.LevelName, LevelState.Loading, forceSpawnPosition)
                        : new LevelMessage(level.ID, level.LevelName, LevelState.Loading, level.PlayerSpawnPosition));
            }
        }

        // Draw a colored box for the collider bounds
        private void OnDrawGizmos()
        {
            if (boxCollider == null)
            {
                boxCollider = GetComponent<BoxCollider2D>();
            }

            if (boxCollider == null)
            {
                return;
            }

            Gizmos.color = gizmoColor;
            Gizmos.DrawCube(boxCollider.bounds.center, boxCollider.bounds.size);
        }
        
        // This method is called when transform of the GameObject changes
        private void OnTransformChildrenChanged()
        {
            // If boxCollider is null, get it
            if (boxCollider == null)
            {
                boxCollider = GetComponent<BoxCollider2D>();
            }

            // If boxCollider is still null, return
            if (boxCollider == null)
            {
                return;
            }

            // Update the size and position of the BoxCollider2D based on the GameObject's transform
            boxCollider.size = transform.localScale;
            boxCollider.offset = transform.localPosition;
        }
    }
}