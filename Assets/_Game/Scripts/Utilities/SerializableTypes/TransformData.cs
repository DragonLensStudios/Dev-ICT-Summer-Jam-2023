using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace DLS.Game.Utilities
{
    [System.Serializable]
    public class TransformData
    {
        public Vector3 localPosition;
        public Quaternion localRotation;
        public Vector3 localScale;
        public SerializableGuid parentID;

        public TransformData(Vector3 localPosition, Quaternion localRotation, Vector3 localScale, SerializableGuid parentID)
        {
            this.localPosition = localPosition;
            this.localRotation = localRotation;
            this.localScale = localScale;
            this.parentID = parentID;
        }
    }
}