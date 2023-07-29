using System;
using DLS.Game.Utilities;
using Unity.Mathematics;
using UnityEngine;

namespace DLS.Core.Data_Persistence
{
    [Serializable]
    public class SavedGameObject : IEquatable<SavedGameObject>
    {
        public string currentLevel;
        public SerializableGuid currentLevelID;
        public SerializableGuid prefabID;
        public SerializableGuid instanceID;
        public string objectName;
        public TransformData transformData;

        public SavedGameObject()
        {
            currentLevel = string.Empty;
            prefabID = new SerializableGuid(Guid.Empty);
            currentLevelID = new SerializableGuid(Guid.Empty);
            instanceID = new SerializableGuid(Guid.NewGuid());
            objectName = string.Empty;
            transformData = new TransformData(Vector3.zero, quaternion.identity, Vector3.one, new SerializableGuid(Guid.Empty));
        }

        public bool Equals(SavedGameObject other)
        {
            if (other == null) return false;
            return currentLevel == other.currentLevel &&
                   currentLevelID == other.currentLevelID && 
                   prefabID.Equals(other.prefabID) &&
                   objectName == other.objectName &&
                   instanceID == other.instanceID &&
                   transformData.Equals(other.transformData); // Assumes TransformData has an appropriate Equals method
        }

        public override bool Equals(object obj)
        {
            if (obj is SavedGameObject otherGameObject)
            {
                return Equals(otherGameObject);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}