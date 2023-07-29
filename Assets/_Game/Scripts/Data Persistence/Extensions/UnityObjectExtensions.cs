using System;
using DLS.Game.Utilities;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DLS.Core.Data_Persistence.Extensions
{
    public static class UnityObjectExtensions
    {
        public static IID GetObjectIID(this Object obj)
        {
            if (obj == null) return null;
            IID identifiableObject = obj.GetComponent<IID>();
            return identifiableObject;
        }
        
        public static SerializableGuid GetObjectPrefabID(this Object obj)
        {
            if (obj == null) return null;
            IID identifiableObject = obj.GetComponent<IID>();
            if (identifiableObject == null) return null;
            return identifiableObject != null ? identifiableObject.PrefabID : new SerializableGuid(Guid.Empty);
        }
        
        public static SerializableGuid GetObjectInstanceID(this Object obj)
        {
            if (obj == null) return null;
            IID identifiableObject = obj.GetComponent<IID>();
            if (identifiableObject == null) return null;
            return identifiableObject.ID.Guid.Equals(Guid.Empty)
                ? identifiableObject.ID = new SerializableGuid(Guid.NewGuid())
                : identifiableObject.ID;
        }
        
        public static SerializableGuid GetObjectLevelID(this Object obj)
        {
            if (obj == null) return null;
            IID identifiableObject = obj.GetComponent<IID>();
            if (identifiableObject == null) return null;
            return identifiableObject.LevelID.Guid.Equals(Guid.Empty)
                ? identifiableObject.LevelID = new SerializableGuid(Guid.NewGuid())
                : identifiableObject.LevelID;
        }
        
        public static string GetObjectName(this Object obj)
        {
            if (obj == null) return null;
            IID identifiableObject = obj.GetComponent<IID>();
            if (identifiableObject == null) return null;
            return identifiableObject != null ? identifiableObject.ObjectName : obj.name;
        }
        
        public static SerializableGuid SetObjectPrefabID(this Object obj, SerializableGuid prefabID)
        {
            if (obj == null) return null;
            IID identifiableObject = obj.GetComponent<IID>();
            if (identifiableObject == null) return null;
            return identifiableObject != null ? identifiableObject.PrefabID = prefabID : new SerializableGuid(Guid.Empty);
        }
        
        public static SerializableGuid SetObjectInstanceID(this Object obj, SerializableGuid instanceID)
        {
            if (obj == null) return null;
            IID identifiableObject = obj.GetComponent<IID>();
            if (identifiableObject == null) return null;
            return identifiableObject.ID = instanceID;
        }
        
        public static SerializableGuid SetObjectLevelID(this Object obj, SerializableGuid levelID)
        {
            if (obj == null) return null;
            IID identifiableObject = obj.GetComponent<IID>();
            if (identifiableObject == null) return null;
            return identifiableObject.LevelID = levelID;
        }
        
        public static string SetObjectName(this Object obj, string name)
        {
            if (obj == null) return null;
            IID identifiableObject = obj.GetComponent<IID>();
            if (identifiableObject == null) return null;
            return identifiableObject != null ? identifiableObject.ObjectName = name : obj.name;
        }
    }
}