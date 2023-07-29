using DLS.Game.GameStates;
using DLS.Game.Managers;
using DLS.Game.Utilities;
using UnityEditor;
using UnityEngine;

namespace DLS.Game.Scripts.Editor
{
    [CustomEditor(typeof(GameManager))]
    public class GameManagerEditor : UnityEditor.Editor
    {
        private SerializedProperty currentStateProperty;
        private SerializedProperty initialStateProperty;
        private SerializedProperty allStatesProperty;

        private bool isEnteringState;
        private GameState previousState;

        private void OnEnable()
        {
            currentStateProperty = serializedObject.FindProperty("CurrentState");
            initialStateProperty = serializedObject.FindProperty("InitialState");
            allStatesProperty = serializedObject.FindProperty("AllStates");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var manager = (GameManager)target;

            // Show current state dropdown
            var states = GetDerivedStates();
            int currentStateIndex = GetCurrentStateIndex(states, manager.CurrentState);
            int newStateIndex = EditorGUILayout.Popup("Current State", currentStateIndex, states);
            var newStateType = GetDerivedType(states[newStateIndex]);

            if (newStateType != manager.CurrentState?.GetType())
            {
                if (!isEnteringState)
                {
                    isEnteringState = true;
                    manager.SetState(Resources.Load("States/"+newStateType.Name) as GameState);
                    isEnteringState = false;
                }
            }

            EditorGUILayout.PropertyField(currentStateProperty);

            EditorGUILayout.Space();

            // Show initial state dropdown
            int initialStateIndex = GetCurrentStateIndex(states, manager.InitialState);
            int newInitialStateIndex = EditorGUILayout.Popup("Initial State", initialStateIndex, states);
            var newInitialStateType = GetDerivedType(states[newInitialStateIndex]);

            if (newInitialStateType != manager.InitialState?.GetType())
            {
                if (!isEnteringState)
                {
                    isEnteringState = true;
                    manager.InitialState = Resources.Load("States/"+newInitialStateType.Name) as GameState;
                    isEnteringState = false;
                }
            }

            EditorGUILayout.PropertyField(initialStateProperty);

            EditorGUILayout.PropertyField(allStatesProperty);

            serializedObject.ApplyModifiedProperties();
        }

        private string[] GetDerivedStates()
        {
            var derivedTypes = ReflectionUtility.GetDerivedTypes(typeof(GameState));
            var derivedStates = new string[derivedTypes.Length];

            for (int i = 0; i < derivedTypes.Length; i++)
            {
                derivedStates[i] = derivedTypes[i].Name;
            }

            return derivedStates;
        }

        private int GetCurrentStateIndex(string[] states, GameState currentState)
        {
            for (int i = 0; i < states.Length; i++)
            {
                if (currentState != null && currentState.GetType().Name == states[i])
                {
                    return i;
                }
            }

            return 0;
        }

        private System.Type GetDerivedType(string stateName)
        {
            var derivedTypes = ReflectionUtility.GetDerivedTypes(typeof(GameState));

            for (int i = 0; i < derivedTypes.Length; i++)
            {
                if (derivedTypes[i].Name == stateName)
                {
                    return derivedTypes[i];
                }
            }

            return typeof(GameState);
        }
    }
}

