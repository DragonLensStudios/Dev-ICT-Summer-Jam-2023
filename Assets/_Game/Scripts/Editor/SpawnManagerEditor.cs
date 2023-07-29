using System.Collections.Generic;
using DLS.Game.Managers;
using UnityEditor;
using UnityEngine;

namespace DLS.Game.Scripts.Editor
{
  [CustomEditor(typeof(SpawnManager))]
  public class SpawnManagerEditor : UnityEditor.Editor
  {
    private SerializedObject serializedSpawnManagerObject;
    private SerializedProperty boundsGizmoColor;
    private SerializedProperty objectToSpawn;
    private SerializedProperty objectsToSpawn;
    private SerializedProperty useRandomSpawnObjects;
    private SerializedProperty amountToSpawnMin;
    private SerializedProperty amountToSpawnMax;
    private SerializedProperty useSpawningBounds;
    private SerializedProperty minBounds;
    private SerializedProperty maxBounds;
    private SerializedProperty objectDetectRadius;
    private SerializedProperty objectSpawnOffset;
    private SerializedProperty useTilemapToPopulateAvailableSpawnLocations;
    private SerializedProperty tilemapToDetectID;
    private SerializedProperty tilemapToDetectAvailableSpawnPoints;
    private SerializedProperty tilemapToSpawnInsideID;
    private SerializedProperty tilemapToSpawnInside;
    private SerializedProperty useObjectPool;
    private SerializedProperty objectPoolMinCapacity;
    private SerializedProperty objectPoolMaxCapacity;
    private SerializedProperty availableSpawnLocations;
    private SerializedProperty activeObjects;
    private SerializedProperty pool;
    private SerializedProperty SpawnObjectsQueue;

    private bool useObjectToSpawnPositionOverride;
    private Vector3 objectToSpawnPositionOverrideEditor;
    
    void OnEnable()
    {
      serializedSpawnManagerObject = new SerializedObject(target);

      boundsGizmoColor = serializedSpawnManagerObject.FindProperty("BoundsGizmoColor");
      objectToSpawn = serializedSpawnManagerObject.FindProperty("ObjectToSpawn");
      objectsToSpawn = serializedSpawnManagerObject.FindProperty("ObjectsToSpawn");
      useRandomSpawnObjects = serializedSpawnManagerObject.FindProperty("UseRandomSpawnObjects");
      amountToSpawnMin = serializedSpawnManagerObject.FindProperty("AmountToSpawnMin");
      amountToSpawnMax = serializedSpawnManagerObject.FindProperty("AmountToSpawnMax");
      useSpawningBounds = serializedSpawnManagerObject.FindProperty("UseSpawningBounds");
      minBounds = serializedSpawnManagerObject.FindProperty("MinBounds");
      maxBounds = serializedSpawnManagerObject.FindProperty("MaxBounds");
      objectDetectRadius = serializedSpawnManagerObject.FindProperty("ObjectDetectRadius");
      objectSpawnOffset = serializedSpawnManagerObject.FindProperty("ObjectSpawnOffset");
      useTilemapToPopulateAvailableSpawnLocations = serializedSpawnManagerObject.FindProperty("UseTilemapToPopulateAvailableSpawnLocations");
      tilemapToDetectID = serializedSpawnManagerObject.FindProperty("TilemapToDetectID");
      tilemapToDetectAvailableSpawnPoints = serializedSpawnManagerObject.FindProperty("TilemapToDetectAvailableSpawnPoints");
      tilemapToSpawnInsideID = serializedSpawnManagerObject.FindProperty("TilemapToSpawnInsideID");
      tilemapToSpawnInside = serializedSpawnManagerObject.FindProperty("TilemapToSpawnInside");
      useObjectPool = serializedSpawnManagerObject.FindProperty("UseObjectPool");
      objectPoolMinCapacity = serializedSpawnManagerObject.FindProperty("ObjectPoolMinCapacity");
      objectPoolMaxCapacity = serializedSpawnManagerObject.FindProperty("ObjectPoolMaxCapacity");
      availableSpawnLocations = serializedSpawnManagerObject.FindProperty("AvailableSpawnLocations");
      activeObjects = serializedSpawnManagerObject.FindProperty("ActiveObjects");
    }

    public override void OnInspectorGUI()
    {
      serializedSpawnManagerObject.Update();

      EditorGUILayout.PropertyField(objectToSpawn);
      EditorGUILayout.PropertyField(objectsToSpawn);
      EditorGUILayout.PropertyField(useRandomSpawnObjects);
      EditorGUILayout.PropertyField(amountToSpawnMin);
      EditorGUILayout.PropertyField(amountToSpawnMax);
      EditorGUILayout.PropertyField(useSpawningBounds);
      EditorGUILayout.PropertyField(boundsGizmoColor);
      EditorGUILayout.PropertyField(minBounds);
      EditorGUILayout.PropertyField(maxBounds);
      EditorGUILayout.PropertyField(objectSpawnOffset);
      EditorGUILayout.PropertyField(objectDetectRadius);
      EditorGUILayout.PropertyField(useTilemapToPopulateAvailableSpawnLocations);
      if (useTilemapToPopulateAvailableSpawnLocations.boolValue)
      {
        EditorGUILayout.PropertyField(tilemapToDetectID);
        EditorGUILayout.PropertyField(tilemapToDetectAvailableSpawnPoints);
      }

      EditorGUILayout.PropertyField(tilemapToSpawnInsideID);
      EditorGUILayout.PropertyField(tilemapToSpawnInside);
      
      serializedSpawnManagerObject.ApplyModifiedProperties();

      EditorGUILayout.PropertyField(useObjectPool);
      EditorGUILayout.PropertyField(objectPoolMinCapacity);
      EditorGUILayout.PropertyField(objectPoolMaxCapacity);
      EditorGUILayout.PropertyField(availableSpawnLocations);
      EditorGUILayout.PropertyField(activeObjects);
      
      useObjectToSpawnPositionOverride = EditorGUILayout.Toggle("Use Spawn Position Override", useObjectToSpawnPositionOverride);
      if (useObjectToSpawnPositionOverride)
      {
        objectToSpawnPositionOverrideEditor = EditorGUILayout.Vector3Field("Position to Spawn", objectToSpawnPositionOverrideEditor);
      }

      if (GUILayout.Button("Set Available Spawn Locations"))
      {
        SpawnManager spawnManager = (SpawnManager)target;
        spawnManager.AddSpawnLocations(spawnManager.AvailableSpawnLocations.ToArray());
      }
      if (GUILayout.Button("Spawn Single Object"))
      {
        SpawnManager spawnManager = (SpawnManager)target;
        if (useObjectToSpawnPositionOverride)
        {
          spawnManager.SpawnObject((GameObject)objectToSpawn.objectReferenceValue, objectToSpawnPositionOverrideEditor);
            
        }
        else
        {
          spawnManager.SpawnObject((GameObject)objectToSpawn.objectReferenceValue);
        }
      }

      if (GUILayout.Button("Spawn All Objects")) 
      {
        SpawnManager spawnManager = (SpawnManager)target;
        spawnManager.SpawnObjects();
      }

      if (GUILayout.Button("Despawn All Objects"))
      {
        SpawnManager spawnManager = (SpawnManager)target;
        spawnManager.DespawnObjects();
      }

      serializedSpawnManagerObject.ApplyModifiedProperties();
    }

  }
}