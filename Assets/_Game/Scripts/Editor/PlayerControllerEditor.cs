using DLS.Game.PLayers;
using UnityEditor;
using UnityEngine;

namespace DLS.Game.Scripts.Editor
{
    [CustomEditor(typeof(PlayerController))]
    public class PlayerControllerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // Draw the default inspector
            DrawDefaultInspector();

            // Get a reference to the PlayerController
            PlayerController playerController = (PlayerController)target;

            // Add a button to the inspector
            if (GUILayout.Button("New Guid"))
            {
                // When the button is clicked, assign a new Guid
                playerController.ID = new(System.Guid.NewGuid());
            }
        }
    }

}